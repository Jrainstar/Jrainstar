using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Jrainstar
{
    [MessageSessionHandler]
    public class C2G_EnterHandler : MessageSessionHandler<C2G_Enter>
    {
        protected override async Task Run(Session session, C2G_Enter message)
        {
            Console.WriteLine("C2G_EnterHandler---" + message.Name);
        }
    }

    [MessageSessionHandler]
    public class C2G_EnterMapHandler : MessageSessionHandler<C2G_EnterMap, G2C_EnterMap>
    {
        protected override async Task Run(Session session, C2G_EnterMap request, G2C_EnterMap response)
        {
            response.MyId = 10086;
            Console.WriteLine("MessageSessionHandler---" + response.MyId);
        }
    }
}
