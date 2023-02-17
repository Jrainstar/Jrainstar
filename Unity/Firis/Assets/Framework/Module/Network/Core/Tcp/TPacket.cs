using System;
using System.IO;

namespace Firis
{
    public enum ParserState
    {
        PacketSize,
        PacketBody
    }

    public class TPacket
    {
        private readonly CircularBuffer recvBuffer;
        private readonly CircularBuffer sendBuffer;
        private readonly byte[] recvCache = new byte[2];
        private readonly byte[] sendCache = new byte[2];
        private int packetSize;
        private ParserState state;

        public const int PacketHeadLength = 2;

        public MemoryStream MemoryStream;

        public TPacket(CircularBuffer recvBuffer, CircularBuffer sendBuffer)
        {
            this.recvBuffer = recvBuffer;
            this.sendBuffer = sendBuffer;
        }

        public bool Parse()
        {
            while (true)
            {
                switch (this.state)
                {
                    case ParserState.PacketSize:
                        {
                            if (this.recvBuffer.Length < PacketHeadLength)
                            {
                                return false;
                            }

                            this.recvBuffer.Read(this.recvCache, 0, PacketHeadLength);

                            this.packetSize = BitConverter.ToUInt16(this.recvCache, 0);
                            if (this.packetSize < TPacket.PacketHeadLength)
                            {
                                throw new Exception($"recv packet size error, 可能是外网探测端口: {this.packetSize}");
                            }


                            this.state = ParserState.PacketBody;
                            break;
                        }
                    case ParserState.PacketBody:
                        {
                            if (this.recvBuffer.Length < this.packetSize)
                            {
                                return false;
                            }

                            MemoryStream memoryStream = new MemoryStream(this.packetSize);
                            this.recvBuffer.Read(memoryStream, this.packetSize);

                            this.MemoryStream = memoryStream;

                            memoryStream.Seek(0, SeekOrigin.Begin);

                            this.state = ParserState.PacketSize;
                            return true;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void Weave(MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            ushort messageSize = (ushort)((stream.Length - stream.Position));
            sendCache[0] = (byte)(messageSize & 0xff);
            sendCache[1] = (byte)((messageSize & 0xff00) >> 8);
            this.sendBuffer.Write(this.sendCache, 0, PacketHeadLength);
            this.sendBuffer.Write(stream.GetBuffer(), (int)stream.Position, (int)(stream.Length - stream.Position));
        }
    }
}