using Moon.Model.Module.Network.Protobuf.Example.Test.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon
{
    public class M0000_C2S_Test0_Handler : MessageHandler<M0000_C2S_Test0>
    {
        public override async Task Process(M0000_C2S_Test0 message, Session session)
        {
            Console.WriteLine("1111====" + Thread.CurrentThread.ManagedThreadId);
            await Task.CompletedTask;
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine(message.human + "===" + message.age);
            session.Send(new M0000_S2C_Test1() { human = { "zhangsan", "lisi" }, age = 22 });
        }
    }
}
