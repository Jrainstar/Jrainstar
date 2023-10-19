using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Jrainstar;

namespace Jrainstar
{
    public class UdpTransport : IKcpTransport
    {
        private readonly Socket socket;

        public UdpTransport(AddressFamily addressFamily)
        {
            socket = new Socket(addressFamily, SocketType.Dgram, ProtocolType.Udp);
            NetworkHelper.SetSioUdpConnReset(socket);
        }

        public UdpTransport(IPEndPoint ipEndPoint)
        {
            socket = new Socket(ipEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                socket.SendBufferSize = Kcp.OneM * 64;
                socket.ReceiveBufferSize = Kcp.OneM * 64;
            }

            try
            {
                socket.Bind(ipEndPoint);
            }
            catch (Exception e)
            {
                throw new Exception($"bind error: {ipEndPoint}", e);
            }

            NetworkHelper.SetSioUdpConnReset(socket);
        }

        public void Send(byte[] bytes, int index, int length, EndPoint endPoint)
        {
            socket.SendTo(bytes, index, length, SocketFlags.None, endPoint);
        }

        public int Recv(byte[] buffer, ref EndPoint endPoint)
        {
            return socket.ReceiveFrom(buffer, ref endPoint);
        }

        public int Available()
        {
            return socket.Available;
        }

        public void Update()
        {
        }

        public void OnError(long id, int error)
        {
        }

        public void Dispose()
        {
            socket?.Dispose();
        }
    }
}
