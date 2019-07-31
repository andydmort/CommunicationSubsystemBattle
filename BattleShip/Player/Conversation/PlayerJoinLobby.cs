using System;
using CommSubSys;
using Messages;
using log4net;
using System.Security.Cryptography;  

namespace Player
{

    public class PlayerJoinLobby : InitiatorConversation
    {
        public PlayerAppState playerAppState; 
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PlayerJoinLobby));
        protected override Type[] ExceptedReplyType => new Type[] { typeof(AssignIdMessage) };

        protected override bool IsTcpConversation => false;

        protected override Message CreateFirstMessage()
        {
            Identifier messageNumber = new Identifier(SubSystem.ProcessID, SubSystem.GetNextSeqNumber());
            ConversationId = messageNumber;

            string name = ((PlayerAppState)SubSystem.appState).playerName;
            string publicKey = ((PlayerAppState)SubSystem.appState).publicKey;

            return new PlayerJoinLobbyMessage(name, publicKey, messageNumber, this.ConversationId);
        }

        protected override void ProcessValidResponse(Envelope env)
        {
            playerAppState = this.SubSystem.appState as PlayerAppState;
            var message = env.Message as AssignIdMessage;
            SubSystem.ProcessID = message.getId();
            Logger.Debug($"encrypted sysmetric key is {BitConverter.ToString(message.getKey())}");
            Logger.Debug($"encrypted sysmetric IV is {BitConverter.ToString(message.getIV())}");
            playerAppState.InformConnectionSuccess();

            byte[] key = playerAppState.decrypt(message.getKey());
            byte[] IV = playerAppState.decrypt(message.getIV());

            Logger.Debug($"decrypted sysmetric key is {BitConverter.ToString(key)}");
            Logger.Debug($"decrypted sysmetric IV is {BitConverter.ToString(IV)}");

            SubSystem.tcpcomm.setKeyAndIV(key, IV);
            SubSystem.udpcomm.setKeyAndIV(key, IV);

            Logger.Debug($" The Assigned process Id is {SubSystem.ProcessID}");
            playerAppState.playerID = SubSystem.ProcessID;
        }
    }
}
