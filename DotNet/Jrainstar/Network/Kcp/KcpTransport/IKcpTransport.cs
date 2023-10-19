using System.Net;

namespace Jrainstar
{
    public interface IKcpTransport : IDisposable
    {
        void Send(byte[] bytes, int index, int length, EndPoint endPoint);
        int Recv(byte[] buffer, ref EndPoint endPoint);
        int Available();
        void Update();
        void OnError(long id, int error);
    }
}


