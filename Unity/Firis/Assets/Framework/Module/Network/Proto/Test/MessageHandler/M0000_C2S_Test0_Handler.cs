using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firis
{
    public class M0000_C2S_Test0_Handler : MessageHandler<M0000_C2S_Test0>
    {
        public override async Task Process(M0000_C2S_Test0 message, Session session)
        {
            await Task.CompletedTask;
            Console.WriteLine(message.human + "===" + message.age);
        }
    }
}
