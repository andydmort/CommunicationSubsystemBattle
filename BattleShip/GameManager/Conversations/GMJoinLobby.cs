using System;
using CommSubSys;
using Messages;
using log4net;

namespace GameManager
{
    public class GMJoinLobby : InitiatorConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GMJoinLobby));
        protected override Type[] ExceptedReplyType => new Type[] { typeof(AssignIdMessage) };

        protected override bool IsTcpConversation => false;

        protected override Message CreateFirstMessage()
        {
            Identifier messageNumber = new Identifier(SubSystem.ProcessID, SubSystem.GetNextSeqNumber());
            ConversationId = messageNumber;
            string publicKey = ((GMAppState)SubSystem.appState).publicKey;
            return new GMJoinLobbyMessage(publicKey, messageNumber, ConversationId,SubSystem.tcpcomm.getListenerEndPoint().Port);
        }

        protected override void ProcessValidResponse(Envelope env)
        {
            var message = env.Message as AssignIdMessage;
            SubSystem.ProcessID = message.getId();

            Console.WriteLine($"encrypted sysmetric key is {BitConverter.ToString(message.getKey())}");
            Console.WriteLine($"encrypted sysmetric IV is {BitConverter.ToString(message.getIV())}");

            byte[] key = ((GMAppState)SubSystem.appState).decrypt(message.getKey());
            byte[] IV = ((GMAppState)SubSystem.appState).decrypt(message.getIV());

            Console.WriteLine($"decrypted sysmetric key is {BitConverter.ToString(key)}");
            Console.WriteLine($"decrypted sysmetric IV is {BitConverter.ToString(IV)}");

            SubSystem.tcpcomm.setKeyAndIV(message.getKey(), message.getIV());
            SubSystem.udpcomm.setKeyAndIV(message.getKey(), message.getIV());

            Logger.Debug($" The Assigned process Id is {SubSystem.ProcessID}");
            Console.WriteLine($" The Assigned process Id is {SubSystem.ProcessID}");
        }

        protected override void ExecuteDetails()
        {
            base.ExecuteDetails();
        }

    }
}
