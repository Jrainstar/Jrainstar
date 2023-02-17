using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firis
{
    public interface IMessageDispatcher
    {
        public void OnlyLoad(string assemblyName);
        public void OnlyLoadFile(string assemblyName);
        public void GeneLoad();

        void Load(string assemblyName);
        void LoadFile(string filePath);

        ushort GetCode(Type type);
        Type GetType(ushort opcode);

        IMessageHandler GetHandler(ushort opcode);
        IMessageHandler GetHandler(Type type);
        IMessageHandler GetHandler(IMessage message);

        MemoryStream Serialize(IMessage message, short avatarId, long sessionId);
        (IMessage, short, long) Deserialize(MemoryStream ms);
        void Dispatch(MemoryStream ms, Session session);
    }
}
