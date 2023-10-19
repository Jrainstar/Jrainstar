using Jrainstar;
using NLog;
using ProtoBuf;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;


Log.Logger = new DotNetLogger();

//APP.Scene.AddComponent<TimerComponent>();

//APP.Scene.AddComponent<NetTcpComponent, IPEndPoint, IMessageDispatcher>(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10001), new MessageDispatcher());
//APP.Scene.GetComponent<NetTcpComponent>().Load("Module");

APP.Scene.AddComponent<CodeType>();
APP.Scene.AddComponent<ObjectPool>();
APP.Scene.AddComponent<OpcodeType>();
APP.Scene.AddComponent<TimeComponent>();
APP.Scene.AddComponent<MessageSessionDispatcher>();
APP.Scene.AddComponent<NetTcpComponent, IPEndPoint, NetworkProtocol>(NetworkHelper.ToIPEndPoint("127.0.0.1", 12050), NetworkProtocol.TCP);

Task.Run(async () =>
{
    await Task.Delay(1000);
    var session = APP.Scene.GetComponent<NetTcpComponent>().Create(NetworkHelper.ToIPEndPoint("127.0.0.1", 12050));
    await Task.Delay(1000);
    session.Send(new C2G_Enter() { Name = "10010" });
    await Task.Delay(1000);
    var e = await session.Call(new C2G_EnterMap()) as G2C_EnterMap;
    Console.WriteLine(e.MyId);
});


//APP.Scene.AddComponent<NetKcpComponent, IPEndPoint, IMessageDispatcher>(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10001), new MessageDispatcher());
//APP.Scene.GetComponent<NetKcpComponent>().OnlyLoad("Module");
//APP.Scene.GetComponent<NetKcpComponent>().GeneLoad();

//APP.Scene.AddComponent<WebSocketComponent, List<string>, IMessageDispatcher>(new List<string> { "http://127.0.0.1:10001/AAA/" }, new MessageDispatcher());
//APP.Scene.GetComponent<WebSocketComponent>().Load("Module");


//APP.Scene.AddComponent<RoomPlayerComponent>();
//APP.Scene.AddComponent<SessionComponent>();
//APP.Scene.AddComponent<PlayerComponent>();
//APP.Scene.AddComponent<RoomComponent>();

while (true)
{
    APP.EventSystem.Update();
    Thread.Sleep(1);
}

#region 操作锁实例

//Task.Run(async () =>
//{
//    using (TimedLock.Lock(OperatedLock.Get(OperatedLockType.Location, 1)))
//    {
//        for (int i = 0; i < 3000; i++)
//        {
//            Console.WriteLine(i);
//        }
//    }
//});

//Task.Run(async () =>
//{
//    using (TimedLock.Lock(OperatedLock.Get(OperatedLockType.Location, 1)))
//    {
//        for (int i = 3000; i < 5000; i++)
//        {
//            Console.WriteLine(i);
//        }
//    }
//});

//Task.Run(async () =>
//{
//    using (TimedLock.Lock(OperatedLock.Get(OperatedLockType.Location, 1)))
//    {
//        for (int i = 5000; i < 7000; i++)
//        {
//            Console.WriteLine(i);
//        }
//    }
//});

//Console.ReadKey();
#endregion