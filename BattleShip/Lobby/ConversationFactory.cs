using Messages;

namespace Lobby
{
    class ConversationFactory : CommSubSys.ConversationFactory
    {
        //public ConversationFactory(SubSystem mySubSys)
        //{
        //    ManagingSubsystem = mySubSys;
        //}

        public ConversationFactory()
        {
        }

        // this needs to map an incoming message type to a new conversation type that needs to be created
        public override void Initialize()
        {
            Add(typeof(GMJoinLobbyMessage), typeof(RespondJoinLobby)); 
            Add(typeof(PlayerJoinLobbyMessage), typeof(RespondJoinLobby));
            Add(typeof(EndGameResultMessage), typeof(RespondEndGame));
        }
    }
}
