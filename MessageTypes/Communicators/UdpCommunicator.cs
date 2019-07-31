using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using ComSubLayer;
using MessageTypes;

using System.Threading;

namespace Communicators
{
    public class UdpCommunicator : Communicator
    {
        private UdpClient myClient;
        private IPEndPoint myEndPoint;
        private ConcurrentQueue<Envelope> inQ;
        private ConcurrentQueue<Envelope> outQ;

        private bool recieving = true;
        private bool running = true;

        public IPEndPoint GetEndPoint()
        {
            return myEndPoint;
        }

        public UdpCommunicator(ref ConcurrentQueue<Envelope> incoming, ref ConcurrentQueue<Envelope> outgoing)
        {
            myEndPoint = new IPEndPoint(IPAddress.Any, 0);
            myClient = new UdpClient(myEndPoint) { Client = { ReceiveTimeout = 100000 } }; //Timeout is set to 10 s. 
            inQ = incoming;
            outQ = outgoing;

            //Start threads here.
            Thread sendingThread = new Thread(new ThreadStart(sendStuff));
            Thread recievingThread = new Thread(new ThreadStart(recieveStuff));
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
        //Takes stuff of inQ and sends it to where it should go. 
        private void recieveStuff()
        {
            while (running)
            {
                Envelope recieved = recieve();
                inQ.Enqueue(recieved);
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

        public override void send(Envelope env)
        {
            try
            {
                byte[] data = env.message.encode();
                var result = myClient.Send(data, data.Length, env.To); //Result is the number of bytes send. 
                Console.WriteLine("Sent Data");
                //TODO: Add Log for sent data. 
            }
            catch(Exception e)
            {
                throw e; //TODO: log error. Failed to sent data. 
            }
        }
       
        //This will return null bytes if it fails. 
        public Envelope recieve()
        {
            byte[] data = null;
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            while (recieving)
            {
                try
                {
                    data = myClient.Receive(ref remoteEP);
                    Console.WriteLine("Recieved data from {0} : {1}", remoteEP, BitConverter.ToString(data)); //TODO: Log this. 
                    
                    //Here we create the envelope from the data. 
                    return new Envelope(Message.decoder(data), myEndPoint, remoteEP,false);
                }
                catch(Exception e)
                {
                    throw e; //TODO: Log error. Failed to recieve data. 
                }
            }

            recieving = true;
            return null; //TODO: Finish this function. 
        }

        public void stopRecieving()
        {
            recieving = false;
        } 
    }
}
