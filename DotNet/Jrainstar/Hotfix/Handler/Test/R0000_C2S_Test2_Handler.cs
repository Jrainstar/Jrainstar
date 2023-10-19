using Moon.Model.Module.Network.Protobuf.Example.Test.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Hotfix.Handler.Test
{
    public class R0000_C2S_Test2_Handler : RpcMessageHandler<R0000_C2S_Test2, R0000_S2C_Test2>
    {
        public override async Task Process(R0000_C2S_Test2 request, R0000_S2C_Test2 response, Session session)
        {
            await Task.CompletedTask;
            response.pokeName = "皮卡丘";
            response.level = 50;
        }
    }
}
