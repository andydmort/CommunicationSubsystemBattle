using System;
using System.Net;
using System.Net.Sockets;
using Messages;
using System.Threading;
using log4net;
using System.IO;
using System.Security.Cryptography;

namespace CommSubSys
{
    public class UdpCommunicator : Communicator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UdpCommunicator));

        private UdpClient myClient;
        private EnvelopeQueue inQ;
        public EnvelopeQueue outQ;

        private bool recieving = true;
        private bool running = true;

        //Threads
        Thread sendingThread;
        Thread recievingThread;

        public IPEndPoint GetEndPoint()
        {
            return (IPEndPoint) myClient.Client.LocalEndPoint;
        }

        public UdpCommunicator( EnvelopeQueue incoming, EnvelopeQueue outgoing, IPEndPoint EP)
        {
            myClient = new UdpClient(EP) { Client = { ReceiveTimeout = 100000 } }; //Timeout is set to 100 s. NOTE: shorten timeout.
            inQ = incoming;
            outQ = outgoing;

            ////Start threads here.
            //Thread sendingThread = new Thread(new ThreadStart(sendStuff));
            //Thread recievingThread = new Thread(new ThreadStart(recieveStuff));

            //sendingThread.IsBackground = false;
            //recievingThread.IsBackground = false;

            //sendingThread.Start();
            //recievingThread.Start();

        }

        public void startThreads()
        {
            //Start threads here.
            sendingThread = new Thread(new ThreadStart(sendStuff));
            recievingThread = new Thread(new ThreadStart(recieveStuff));

            sendingThread.IsBackground = false;
            recievingThread.IsBackground = false;


            Logger.Debug("Starting the UDP sending and recieving threads.");
            sendingThread.Start();
            recievingThread.Start();
        }

        //Takes stuff off outQ and send it to where it should go. 
        private void sendStuff()
        {
            while (running)
            {
                if(outQ.Count != 0)
                {
                    Envelope check;
                    if (outQ.TryPeek(out check) && !check.isTcp) //Note: This will cause the UDPcommunicator to become backed up if the TCP communicator is not taking things off this queue. 
                    {
                        Envelope toSend;
                        if(outQ.TryDequeue(out toSend)) this.send(toSend);
                    }
                    else
                    {
                        outQ.setQueueSignal(); //Sets the queue again stating that nothing was taken out. 
                        Thread.Sleep(100); //Wait a little for other communicator to pick it up. 
                    }
                }
                else
                {
                    outQ.waitIncomingEnv(); //Waits for something to enter the inQ. 
                }
            }
        }
        //Takes stuff of inQ and sends it to where it should go. 
        private void recieveStuff()
        {
            while (running)
            {
                Envelope recieved = recieve();
                inQ.Enqueue(recieved);
                inQ.setQueueSignal();
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
                Logger.Error($"Failed to Parse IPaddress {ipAddress}:{port} : {e}");
                throw e;  
            }
            return EP;
        }

        public override void send(Envelope env)
        {
            try
            {
                byte[] data = env.Message.encode();
                var result  = myClient.Send(data, data.Length, env.To); //Result is the number of bytes send. 
                Logger.Debug($"Sent data from {this.GetEndPoint()} to {env.To} of Message type {env.Message.GetType()} : {BitConverter.ToString(data)}");
            }
            catch(Exception e)
            {
                Logger.Error($"Failed to send data: {e}");
                throw e;
            }
        }
       
        //This will return null bytes if it fails. 
        public Envelope recieve()
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            //Console.WriteLine("Entering Reiceiving loop");
        
            while (recieving)
            {
                //Console.WriteLine("Top of loop");
                byte[] data = null;
                try
                {
                    Logger.Debug($"Trying to recieve data.");
                    data = myClient.Receive(ref remoteEP);
                    Logger.Debug($"Recieved data from {remoteEP} : {BitConverter.ToString(data)}");
                    
                    //Here we create the envelope from the data. 
                    return new Envelope(Message.decoder(data), remoteEP, this.GetEndPoint(), false);
                }
                catch(Exception e)
                {
                    //Logger.Error($"Failed to recieve a message: {e}");
                }
            }

            //recieving = true;
            return null; 
        }

        public void stopRecieving()
        {
            recieving = false;
        }

        public void closeCommunicator()
        {
            Logger.Debug("Stopping UDP communicator sending and recieving threads.");
            running = false;
            stopRecieving();
            myClient.Close();
        }

        public byte[] encrypt(byte[] bytes)
        {
            byte[] encrypted;
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = this.key;
                rijAlg.IV  = this.IV;

                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(bytes);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

        public byte[] decrypt(byte[] bytes)
        {
            byte[] decrypted;
            int decryptedByteCount = 0;
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = this.key;
                rijAlg.IV = this.IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        decrypted = new byte[bytes.Length];
                        decryptedByteCount = csDecrypt.Read(decrypted, 0, decrypted.Length);
                    }
                }

            }

            return decrypted;
        }
    }
}
