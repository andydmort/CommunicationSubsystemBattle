using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageTesting
{
    [TestClass]
    public class AssignIdMessageTest
    {
        [TestMethod]
        public void TestEncode()
        {
            short id = 1;
            // test initialization 
            var assignIdMessage = new AssignIdMessage(id);
            byte[] bytes = assignIdMessage.encode();
            Console.WriteLine("bytes" + bytes);

            // test encode method
            id = 2;
            assignIdMessage.encode(id);

                   
        }
    }
}
