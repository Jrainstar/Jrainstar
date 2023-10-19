using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrainstar
{
    [Message(OuterMessage.C2G_Enter)]
    [MemoryPackable]

    public partial class C2G_Enter : MessageObject, IMessage
    {
        public static C2G_Enter Create(bool isFromPool = true)
        {
            return !isFromPool ? new C2G_Enter() : ObjectPool.Instance.Fetch(typeof(C2G_Enter)) as C2G_Enter;
        }

        [MemoryPackOrder(0)]
        public string Name { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool) return;
            this.Name = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    // [ResponseType(nameof(G2C_EnterMap))]
    [Message(OuterMessage.C2G_EnterMap)]
    [MemoryPackable]

    public partial class C2G_EnterMap : MessageObject, IRequest
    {
        public static C2G_EnterMap Create(bool isFromPool = true)
        {
            return !isFromPool ? new C2G_EnterMap() : ObjectPool.Instance.Fetch(typeof(C2G_EnterMap)) as C2G_EnterMap;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool) return;
            this.RpcId = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    [Message(OuterMessage.G2C_EnterMap)]
    [MemoryPackable]
    public partial class G2C_EnterMap : MessageObject, IResponse
    {
        public static G2C_EnterMap Create(bool isFromPool = true)
        {
            return !isFromPool ? new G2C_EnterMap() : ObjectPool.Instance.Fetch(typeof(G2C_EnterMap)) as G2C_EnterMap;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        [MemoryPackOrder(1)]
        public int Error { get; set; }

        [MemoryPackOrder(2)]
        public string Message { get; set; }

        // 自己unitId
        [MemoryPackOrder(3)]
        public long MyId { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool) return;
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.MyId = default;

            ObjectPool.Instance.Recycle(this);
        }

    }
}
