using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using ComSubLayer;
using MessageTypes;
using System.Collections.Concurrent;


namespace Communicators
{
    public class TcpCommunicator : Communicator
    {
        TcpListener myListener;
        IPEndPoint myListenerEP;
        Dictionary<IPEndPoint, TcpClient> tcpClients = new Dictionary<IPEndPoint, TcpClient>();

        private ConcurrentQueue<Envelope> inQ;
        private ConcurrentQueue<Envelope> outQ;

        bool running = true;

        public TcpCommunicator(int port, ref ConcurrentQueue<Envelope> incoming, ref ConcurrentQueue<Envelope> outgoing)
        {
            myListener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            myListenerEP = (IPEndPoint) myListener.LocalEndpoint;
            inQ = incoming;
            outQ = outgoing;
            startThreads();
        }
        public TcpCommunicator(ref ConcurrentQueue<Envelope> incoming, ref ConcurrentQueue<Envelope> outgoing)
        {
            myListener = new TcpListener(new IPEndPoint(IPAddress.Any, 0));
            myListenerEP = (IPEndPoint)myListener.LocalEndpoint;
            inQ = incoming;
            outQ = outgoing;
            startThreads();
        }

        private void startThreads()
        {
            Thread sendingThread = new Thread(new ThreadStart(sendStuff));
            Thread recievingThread = new Thread(new ThreadStart(recieveStuff));
            Thread listeningThread = new Thread(new ThreadStart(Listen));
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
                        tcpClients.Add((IPEndPoint)myTcpClient.Client.RemoteEndPoint , myTcpClient); //Add the client to the dictionary. 
                    }
                    catch (Exception e)
                    {
                        throw e; //TODO: Log unable to recieved connection tcp. 
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
                throw e; //TODO: Log error. Cannot parse peer Endpoint. 
            }
            return EP;
        }

        public void closeConnection(IPEndPoint EP)
        {
            if (tcpClients[EP].Connected)
            {
                tcpClients[EP].Close();
            }
            tcpClients.Remove(EP);
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
                tcpClients.Add(env.To, aTcpClient); //Add it to our dict of clients. 
                stream = aTcpClient.GetStream(); //Get the stream to send data. 
            }

            byte[] data = env.message.encode();
            //Send data through stream. 
            try
            {
                byte[] lengthBytes = new byte[4];
                lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((UInt32)data.Length)); //Packs length in 4 bytes
                stream.Write(lengthBytes, 0, 4);
                //Write the actual message.
                stream.Write(data, 0, data.Length);
            }
            catch(InvalidOperationException e)
            {
                //We are not connected to that client anymore. 
                tcpClients.Remove(env.To); //Remove that guy from the dictionary. 
                //TODO: Log an error here. 
            }
            catch(Exception e)
            {
                throw e; //Todo Log this error. Failed sending. 
            }
        }

        //Todo test and edit this function. 
        public Envelope recieve(IPEndPoint EP)
        {
            if (!tcpClients[EP].Connected)
            {
                tcpClients[EP].Close();
                return null; //Failed to get anything connection was closed. 
            }

            var stream = tcpClients[EP].GetStream();
            bool gettingData = true;

            while (gettingData)
            {
                //Reading length
                int lenMessage = 0;
                try
                {
                    byte[] len = new byte[4];
                    stream.Read(len, 0, len.Length);
                    lenMessage = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(len, 0));
                }
                catch(Exception e)
                {
                    throw e; //TODO: log error. Couldn't recieved data. 
                }


                while(stream.Length < lenMessage)
                {
                    Thread.Sleep(100); //Wait for message to be recieved. 
                }

                //Getting data and packing Envelope. 
                byte[] data = new byte[lenMessage];
                try
                {
                    stream.Read(data, 0, lenMessage);
                    Message mess = Message.decoder(data);
                    return new Envelope(mess, myListenerEP, EP, true);
                }
                catch (Exception e)
                {
                    throw e; //TODO: log error. Couldn't recieved data. 
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
                    Envelope toSend;
                    outQ.TryDequeue(out toSend);
                    this.send(toSend);
                }
                else
                {
                    Thread.Sleep(200); //Sleeps for 200 ms. We could used an event trigger to make this more efficient. 
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
                    }
                }
                Thread.Sleep(200); //Take a 200ms snoozer. 
            }
        }
    }
}


//TODO: What happens if a connection stops in the middle of a communication? 
