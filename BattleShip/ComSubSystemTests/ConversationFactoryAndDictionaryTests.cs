using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommSubSys;
using Messages;
using log4net;

namespace ComSubSystemTests
{
    class ConversationFactoryDummy : ConversationFactory
    {
        public override void Initialize()
        {
            Add(typeof(PlayerJoinLobbyMessage), typeof(dummyResponderConversation));
        }
    }

    class dummyResponderConversation : ResponderConversation
    {
        protected override bool IsTcpConversation => false;

        protected override void ExecuteDetails()
        {
            throw new NotImplementedException();
        }
    }

    class dummyInitiatorConversation : InitiatorConversation
    {
        protected override Type[] ExceptedReplyType => throw new NotImplementedException();

        protected override bool IsTcpConversation => false;

        protected override Message CreateFirstMessage()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessValidResponse(Envelope env)
        {
            throw new NotImplementedException();
        }
    }



    [TestClass]
    public class ConversationFactoryAndDictionaryTests
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ConversationFactoryAndDictionaryTests));
        [TestMethod]
        public void Initializing_A_Conversation_Factory()
        {
            //Create an implimented conversation factory. 
            ConversationFactoryDummy CFD = new ConversationFactoryDummy();
            //Test added types to the ConversationFactory type mappings. 
            CFD.Initialize();

            //Create envelope for incoming testings. 
            Envelope env = new Envelope(new PlayerJoinLobbyMessage("playerName","publicKey",new Identifier(1,1), new Identifier(1,1)),null , null, false);
            // Get conversation from env. 
            Conversation createdConvo = CFD.CreateFromEnvelope(env);
            // Make sure it is not null. 
            Assert.AreNotEqual(null, createdConvo);
            //Make sure it is the right type.
            Assert.AreEqual(typeof(dummyResponderConversation), createdConvo.GetType());

            
            try
            {
                Conversation createdConvo1 = CFD.CreateFromEnvelope(new Envelope(new GMJoinLobbyMessage(),null,null,false));
                Assert.Fail();
            }
            catch(Exception e)
            {
                Logger.Debug($"Test passed: {e}");
            }


        }

        [TestMethod]
        public void Testing_Conversation_Dictionary()
        {
            //Create an implimented conversation factory. 
            ConversationFactoryDummy CFD = new ConversationFactoryDummy();
            //Test added types to the ConversationFactory type mappings. 
            CFD.Initialize();

            //Creating dummy initiator conversation
            dummyInitiatorConversation InitConvo = new dummyInitiatorConversation();
            InitConvo.ConversationId = new Identifier(1, 1);

            //Adding conversation to dictionary.
            CFD.dictionary.Add(InitConvo);

            Conversation convoTest = null;

            //Get back the Conversation
            convoTest = CFD.dictionary.GetConversation(new Identifier(1, 1));

            //Make sure they are the same. 
            Assert.AreSame(convoTest, InitConvo);

            //Remove the Conversation
            CFD.dictionary.Remove(new Identifier(1, 1));
            convoTest = null;

            //Make sure that it is now null in the dictionary.
            convoTest = CFD.dictionary.GetConversation(new Identifier(1, 1));
            Assert.IsNull(convoTest);

            // Note: When a conversation is created by the conversation factory it is not being put in the dictionary. Should it? if so it should be tested here? 

        }
    }
}
