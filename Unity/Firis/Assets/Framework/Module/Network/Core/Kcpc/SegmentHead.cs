namespace ET
{
    public struct SegmentHead
    {
        public uint conv;
        public byte cmd;
        public byte frg;
        public ushort wnd;
        public uint ts;
        public uint sn;
        public uint una;
        public uint len;
    }
}