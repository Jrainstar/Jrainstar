﻿using System;
using System.IO;
using System.Net;

namespace Jrainstar
{
    public enum ChannelType
    {
        Connect,
        Accept,
    }

    public struct Packet
    {
        public const int MinPacketSize = 2;
        public const int OpcodeLength = 2;
        public const int ActorIdIndex = 0;
        public const int ActorIdLength = 16;

        public ushort Opcode;
        public long ActorId;
        public MemoryStream MemoryStream;
    }

    public abstract class AChannel : IDisposable
    {
        public long Id { get; set; }

        public ChannelType ChannelType { get; protected set; }

        public int Error { get; set; }


        public bool IsDisposed
        {
            get
            {
                return this.Id == 0;
            }
        }

        public abstract void Dispose();
    }
}