using Firis;
using NLog;
using ProtoBuf;
using System.Diagnostics;
using System.Net;
using System.Text;


Log.Logger = new DotNetLogger();

APP.Scene.AddComponent<TimerComponent>();

APP.Scene.AddComponent<NetTcpComponent, IPEndPoint, IMessageDispatcher>(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10001), new MessageDispatcher());
APP.Scene.GetComponent<NetTcpComponent>().Load("Module");

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
    MainThreadSynchronizationContext.Instance.Update();
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