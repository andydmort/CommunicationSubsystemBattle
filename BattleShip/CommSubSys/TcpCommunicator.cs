using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Messages;
using System.Collections.Concurrent;
using log4net;
using System.Security.Cryptography;
using System.IO;

namespace CommSubSys
{
    public class TcpCommunicator : Communicator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TcpCommunicator));

        TcpListener myListener;
        ConcurrentDictionary<IPEndPoint, TcpClient> tcpClients = new ConcurrentDictionary<IPEndPoint, TcpClient>(); //TODO: change to concurrent dictionary. 

        private EnvelopeQueue inQ;
        public EnvelopeQueue outQ;

        bool running = true;

        //Threads
        Thread sendingThread;
        Thread recievingThread;
        Thread listeningThread;


        public TcpCommunicator(EnvelopeQueue incoming, EnvelopeQueue outgoing, IPEndPoint EP)
        {
            myListener = new TcpListener(EP);
            inQ = incoming;
            outQ = outgoing;
            //startThreads();
        }

        public TcpCommunicator(EnvelopeQueue incoming, EnvelopeQueue outgoing, int port)
        {
            myListener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            inQ = incoming;
            outQ = outgoing;
            //startThreads();
        }

        public void startThreads()
        {
            sendingThread = new Thread(new ThreadStart(sendStuff));
            recievingThread = new Thread(new ThreadStart(recieveStuff));
            listeningThread = new Thread(new ThreadStart(Listen));

            sendingThread.IsBackground = false;
            recievingThread.IsBackground = false;
            listeningThread.IsBackground = false;

            Logger.Debug("Starting the TCP sending, recieving, and listening threads");

            myListener.Start();
            listeningThread.Start();
            sendingThread.Start();
            recievingThread.Start();
        }

        //This method may be better used on another thread. 
        private void Listen()
        {
            while (running)
            {
                if (myListener.Pending())
                {
                    try
                    {
                        TcpClient myTcpClient = myListener.AcceptTcpClient();
                        if (!tcpClients.TryAdd((IPEndPoint)myTcpClient.Client.RemoteEndPoint, myTcpClient))  //Adds the client to the dictionary.
                        {
                            Logger.Error("Failed to add tcp client to tcpClient dictionary.");
                        }
                        else
                        {
                            Logger.Debug($"Created TCP connection with {myTcpClient.Client.RemoteEndPoint}");
                            Console.WriteLine($"Created TCP connection with {myTcpClient.Client.RemoteEndPoint}");
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorFormat("Failed to create connection with remote TCP client: {0}", e);
                        throw e; 
                    }
                }
                Thread.Sleep(300); //Just a quick snoozer. 
            }
        }

        private IPEndPoint parseIpaddress(string ipAddress, int port)
        {
            IPEndPoint EP;
            try
            {
                EP = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("Failed to Parse IPaddress {0}:{1} : {2}", ipAddress, port, e);
                throw e;
            }
            return EP;
        }

        public void closeConnection(IPEndPoint EP)
        {
            TcpClient clientToClose;
            if(tcpClients.TryGetValue(EP, out clientToClose ) && clientToClose.Connected)
            {
                clientToClose.Close();
            }
            TcpClient removedClient;
            if(!tcpClients.TryRemove(EP,out removedClient))
            {
                Logger.Error("Failed to remove TCP client from tcpClient dictionary.");
            }
        }

        public override void send(Envelope env)
        {
            NetworkStream stream;
            if (tcpClients.ContainsKey(env.To))
            {
                stream = tcpClients[env.To].GetStream();
            }
            else
            {
                TcpClient aTcpClient = new TcpClient(); //Create new Client. 
                aTcpClient.Connect(env.To); //Connect to it.
                if (!tcpClients.TryAdd(env.To, aTcpClient))  //Adds the client to the dictionary of clients.
                {
                    Logger.Error("Failed to add TCP client to tcpClient dictionary");
                }
                stream = aTcpClient.GetStream(); //Get the stream to send data. 
            }

            byte[] data = env.Message.encode();
            //Send data through stream. 
            try
            {
                byte[] lengthBytes =null;
                lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int32)data.Length)); //Packs length in 4 bytes
                stream.Write(lengthBytes, 0, 4);
                //Write the actual message.
                stream.Write(data, 0, data.Length);
                Logger.Debug($"Sent data from {getListenerEndPoint()} to {env.To} : { BitConverter.ToString(lengthBytes)}");
            }
            catch (InvalidOperationException e)
            {
                //We are not connected to that client anymore. 
                Logger.Error($"Tried to send Message to TCP client that is no logger connected: {0}", e);
                this.closeConnection(env.To);
            }
            catch(Exception e)
            {
                Logger.Error($"Failed to send message: {0}", e);
                throw e; 
            }
        }

        
        public Envelope recieve(IPEndPoint EP)
        {
            if (!tcpClients[EP].Connected)
            {
                Logger.DebugFormat($"TCP connection was closed to Remote EndPoint {EP}");
                Console.WriteLine($"TCP connection was closed to Remote EndPoint { EP}");
                try
                {
                    tcpClients[EP].Connect(tcpClients[EP].Client.RemoteEndPoint as IPEndPoint);
                }
                catch(Exception e)
                {
                    Logger.Debug($"Unable to reconnect through TCP to endpoint {tcpClients[EP].Client.RemoteEndPoint as IPEndPoint}"); //Note: This will happen on the GM if the player connection goes down. They player should Also try to reconnected it. 
                    this.closeConnection(EP); //Connection is officially over. 
                    return null; //Failed to get anything connection was closed. 
                }
            }

            var stream = tcpClients[EP].GetStream();
            bool gettingData = true;
            
            //Console.WriteLine($"Trying to get data from {EP}");
            //Reading length
            int lenMessage = 0;
            try
            {
                byte[] len = new byte[4];
                if (stream.DataAvailable)
                {
                    stream.Read(len, 0, len.Length);
                }
                else
                {
                    return null;
                }
                lenMessage = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(len, 0));
            }
                catch (System.IO.IOException)
                { 
                Logger.DebugFormat($"TCP connection was closed to Remote EndPoint {EP}");
                Console.WriteLine($"TCP connection was closed to Remote EndPoint {EP}");
                //this.closeConnection(EP); //Should repeat this function and try a reconnect at the top. 
                return null;
            }
            catch(Exception e)
            {
                Logger.ErrorFormat("Failed to recieve data trough TCP from {0},EP: {1}", EP, e);
                throw e; 
            }

            //Check if there was a message. 
            if (lenMessage == 0) return null;

            //Getting data and packing Envelope. 
            byte[] data = new byte[lenMessage];

            //byte[] data = new byte[30000];
            bool pullingDataOffStream = true;
            while (pullingDataOffStream && lenMessage != 0)
            {
                //Console.WriteLine($"Top of Pulling with ep : {EP}");
                try
                {
                    stream.Read(data, 0, lenMessage);
                    Message mess = Message.decoder(data);
                    Logger.Debug($"Recieved data from {EP} : {BitConverter.ToString(data)}");
                    Console.WriteLine($"Recieved data from {EP} : {BitConverter.ToString(data)}");
                    return new Envelope(mess, EP, getListenerEndPoint(), true);
                }
                catch (System.IO.IOException)
                {
                    Logger.DebugFormat($"TCP connection was closed to Remote EndPoint {EP}");
                    Console.WriteLine($"TCP connection was closed to Remote EndPoint {EP}");
                    //this.closeConnection(EP); //Should repeat this function and try a reconnect at the top. 
                    return null;
                }
                catch (ArgumentOutOfRangeException e1)
                {

                    Logger.Debug($"Only part of tcp message stream has come through, waiting for the rest of it. {e1}");
                    Console.WriteLine($"Only part of tcp message stream has come through, waiting for the rest of it. {e1}");
                    Thread.Sleep(300);
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat($"Failed to recieve data trough TCP from {0},EP: {1}", EP, e);
                    throw e;
                }
            }
            return null;
        }

        public void sendStuff()
        {
            while (running)
            {
                if(outQ.Count != 0)
                {
                    Envelope check;
                    if (outQ.TryPeek(out check) && check.isTcp) //Note: if udp message are not being send this could block up the TCP communicator. 
                    {
                        Envelope toSend;
                        if(outQ.TryDequeue(out toSend)) this.send(toSend);
                    }
                    else
                    {
                        outQ.setQueueSignal(); //Marks send in someone else to get the message that was not meant for TCP. 
                        Thread.Sleep(300); //Wait a little for other communicator to pick it up. 
                    }
                }
                else
                {
                    outQ.waitIncomingEnv(); //Waits for something to come into the queue
                }
            }
        }
        public void recieveStuff()
        {
            while (running)
            {
                foreach(KeyValuePair<IPEndPoint, TcpClient> client in tcpClients)
                {
                    Envelope env = recieve(client.Key);
                    if(env != null)
                    {
                        inQ.Enqueue(env);
                        inQ.setQueueSignal();
                    }
                }
                Thread.Sleep(300); //Take a 200ms snoozer. //Not sure how to add a event for this. 
            }
        }

        public IPEndPoint getListenerEndPoint()
        {
            return (IPEndPoint) myListener.LocalEndpoint;
        }

        public void closeCommunicator()
        {
            foreach(KeyValuePair<IPEndPoint, TcpClient> thing in tcpClients)
            {
                //thing.Value.Close();
                closeConnection(thing.Key);
            }
            running = false;
            myListener.Stop();
        }

    }
}


//TODO: What happens if a connection stops in the middle of a communication? 
