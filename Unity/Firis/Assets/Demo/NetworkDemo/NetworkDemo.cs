using Firis;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkDemo : MonoBehaviour
{
    public Session Session;

    void Start()
    {
        Log.Logger = new UnityLogger();

        var net = APP.Scene.AddComponent<NetTcpComponent, IMessageDispatcher>(new MessageDispatcher());
        net.Load("Module");
        Session = APP.Scene.GetComponent<NetTcpComponent>().Create(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10001));

        //var net = APP.Scene.AddComponent<NetKcpComponent, IMessageDispatcher>(new MessageDispatcher());
        //net.Load("Module");
        //Session = APP.Scene.GetComponent<NetKcpComponent>().Create(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10001));

        //var net = APP.Scene.AddComponent<WebSocketComponent, IMessageDispatcher>(new MessageDispatcher());
        //net.Load("Module");
        //Session = APP.Scene.GetComponent<WebSocketComponent>().Create("ws://127.0.0.1:10001/AAA/");

        StartCoroutine(StartGame());
    }

    void Update()
    {
        APP.EventSystem.Update();
        MainThreadSynchronizationContext.Instance.Update();
    }

    IEnumerator StartGame()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Session.Send(new M0000_C2S_Test0() { human = "AAA", age = 18 });
            Task.Run(async () =>
            {
                var sss = (R0000_S2C_Test2)await Session.Call(new R0000_C2S_Test2() { pokeName = "皮卡丘" });
                Debug.Log(sss.pokeName + "---" + sss.level);
            });
        }
    }

    private void OnDisable()
    {
        Session.Dispose();
    }
}
