using Messages;

namespace GameManager
{
    class ConversationFactory : CommSubSys.ConversationFactory
    {

        //public ConversationFactory(SubSystem mySubSys)
        //{
        //    ManagingSubsystem = mySubSys;
        //    this.Initialize();
        //}

        public ConversationFactory()
        {
            //ManagingSubsystem = mySubSys;
            //this.Initialize();
        }

        public override void Initialize()
        {
            Add(typeof(PlayerIdMessage), typeof(RespondPassOff)); 
            Add(typeof(BoardMessage),     typeof(RespondBoard)); 
            Add(typeof(ShotMessage),      typeof(RespondShot));  
        }
    }
}
