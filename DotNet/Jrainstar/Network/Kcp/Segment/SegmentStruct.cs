using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jrainstar
{
    // KCP Segment Definition

    internal struct SegmentStruct : IDisposable
    {
        public SegmentHead SegHead;
        public uint resendts;
        public int rto;
        public uint fastack;
        public uint xmit;

        private byte[] buffer;

        private ArrayPool<byte> arrayPool;

        public bool IsNull => buffer == null;

        public int WrittenCount
        {
            get => (int)SegHead.len;
            private set => SegHead.len = (uint)value;
        }

        public Span<byte> WrittenBuffer => buffer.AsSpan(0, (int)SegHead.len);

        public Span<byte> FreeBuffer => buffer.AsSpan(WrittenCount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SegmentStruct(int size, ArrayPool<byte> arrayPool)
        {
            this.arrayPool = arrayPool;
            buffer = arrayPool.Rent(size);
            SegHead = new SegmentHead() { len = 0 };
            SegHead = default;
            resendts = default;
            rto = default;
            fastack = default;
            xmit = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Encode(Span<byte> data, ref int size)
        {
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(data), SegHead);
            size += Unsafe.SizeOf<SegmentHead>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            WrittenCount += count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            arrayPool.Return(buffer);
        }
    }
}
