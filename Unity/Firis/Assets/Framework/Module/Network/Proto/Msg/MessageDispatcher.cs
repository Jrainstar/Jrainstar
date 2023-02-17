using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Firis
{
    public class MessageDispatcher : IMessageDispatcher
    {
        //用于传输消息进行序列化的处理
        private Dictionary<Type, ushort> Type2Opcode { get; set; } = new Dictionary<Type, ushort>();
        //用于得到反序列化消息得到对应的类
        private Dictionary<ushort, Type> Opcode2Type { get; set; } = new Dictionary<ushort, Type>();
        //用于解析消息后的(Message)方法处理
        private Dictionary<ushort, IMessageHandler> Opcode2Handler { get; set; } = new Dictionary<ushort, IMessageHandler>();

        private List<Type> messageTypes { get; set; } = new List<Type>();
        private List<Type> handlerTypes { get; set; } = new List<Type>();

        // 装载引用Dll
        public void Load(string assemblyName)
        {
            Assembly current = Assembly.Load(assemblyName);
            Type[] allTypes = current.GetTypes();

            Type[] messagTypes = allTypes.Where(type => typeof(IMessage).IsAssignableFrom(type)).ToArray();
            Type[] handleTypes = allTypes.Where(type => typeof(IMessageHandler).IsAssignableFrom(type)).ToArray();
            Type generic0 = typeof(IMessage);
            Type generic1 = typeof(IRequest);
            Type generic2 = typeof(IResponse);
            foreach (Type type in messagTypes)
            {
                if (type == generic0 || type == generic1 || type == generic2) continue;
                MessageAttribute msg = type.GetCustomAttribute<MessageAttribute>();
                if (msg == null) continue;
                Type2Opcode.Add(type, msg.Opcode);
                Opcode2Type.Add(msg.Opcode, type);
            }

            generic0 = typeof(IMessageHandler);
            generic1 = typeof(MessageHandler<>);
            generic2 = typeof(RpcMessageHandler<,>);

            foreach (var type in handleTypes)
            {
                if (type == generic0 || type == generic1 || type == generic2) continue;
                Type generic = type.BaseType.GetGenericTypeDefinition();
                if (generic != generic1 && generic != generic2) continue;
                Type handleType = type.BaseType.GenericTypeArguments[0];
                if (!Type2Opcode.ContainsKey(handleType)) continue;
                IMessageHandler handler = (IMessageHandler)Activator.CreateInstance(type);
                Console.WriteLine(handler);
                Opcode2Handler.Add(Type2Opcode[handleType], handler);
            }
        }

        public void LoadFile(string filePath)
        {
            Assembly current = Assembly.LoadFile(filePath);
            Type[] allTypes = current.GetTypes();

            Type[] messagTypes = allTypes.Where(type => typeof(IMessage).IsAssignableFrom(type)).ToArray();
            Type[] handleTypes = allTypes.Where(type => typeof(IMessageHandler).IsAssignableFrom(type)).ToArray();

            Type generic0 = typeof(IMessage);
            Type generic1 = typeof(IRequest);
            Type generic2 = typeof(IResponse);
            foreach (Type type in messagTypes)
            {
                if (type == generic0 || type == generic1 || type == generic2) continue;
                MessageAttribute msg = type.GetCustomAttribute<MessageAttribute>();
                if (msg == null) continue;
                Type2Opcode.Add(type, msg.Opcode);
                Opcode2Type.Add(msg.Opcode, type);
            }

            generic0 = typeof(IMessageHandler);
            generic1 = typeof(MessageHandler<>);
            generic2 = typeof(RpcMessageHandler<,>);

            foreach (var type in handleTypes)
            {
                if (type == generic0 || type == generic1 || type == generic2) continue;
                Type generic = type.BaseType.GetGenericTypeDefinition();
                if (generic != generic1 && generic != generic2) continue;
                Type handleType = type.BaseType.GenericTypeArguments[0];
                if (!Type2Opcode.ContainsKey(handleType)) continue;
                IMessageHandler handler = (IMessageHandler)Activator.CreateInstance(type);
                Opcode2Handler.Add(Type2Opcode[handleType], handler);
            }
        }

        public ushort GetCode(Type type)
        {
            return Type2Opcode[type];
        }

        public Type GetType(ushort opcode)
        {
            return Opcode2Type[opcode];
        }

        public IMessageHandler GetHandler(ushort opcode)
        {
            return Opcode2Handler[opcode];
        }

        public IMessageHandler GetHandler(Type type)
        {
            return Opcode2Handler[GetCode(type)];
        }

        public IMessageHandler GetHandler(IMessage message)
        {
            return Opcode2Handler[GetCode(message.GetType())];
        }

        private byte[] HeadCache { get; set; } = new byte[2];
        private byte[] AvatarIdCache { get; set; } = new byte[2];
        private byte[] SessionIdCache { get; set; } = new byte[8];

        public MemoryStream Serialize(IMessage message, short avatarId, long sessionId)
        {
            MemoryStream ms = new MemoryStream();
            var opcode = GetCode(message.GetType());

            //HeadCache[0] = (byte)(opcode & 0xff);
            //HeadCache[1] = (byte)((opcode & 0xff00) >> 8);
            HeadCache.WriteTo(0, opcode);
            //AvatarIdCache[0] = (byte)(avatarId & 0xff);
            //AvatarIdCache[1] = (byte)((avatarId & 0xff00) >> 8);

            //WriteTo(SessionIdCache, 0, sessionId);

            ms.Write(HeadCache);
            //ms.Write(AvatarIdCache);
            //ms.Write(SessionIdCache);

            ProtoBuf.Serializer.Serialize(ms, message);
            return ms;
        }


        public (IMessage, short, long) Deserialize(MemoryStream ms)
        {
            ms.Seek(0, SeekOrigin.Begin);
            var opcode = BitConverter.ToUInt16(ms.GetBuffer(), 0);
            var type = GetType(opcode);
            //var avatarId = BitConverter.ToInt16(ms.GetBuffer(), 2);
            //var sessionId = BitConverter.ToInt64(ms.GetBuffer(), 4);
            ms.Seek(2, SeekOrigin.Begin);
            //ms.Seek(12, SeekOrigin.Begin);
            var message = ProtoBuf.Serializer.Deserialize(type, ms) as IMessage;
            //ms.Dispose();
            //return (message, avatarId, sessionId);
            return (message, 0, 0);
        }

        public void Dispatch(MemoryStream ms, Session session)
        {
            var message = Deserialize(ms).Item1;
            //var avatar = Deserialize(ms).Item2;
            switch (message)
            {
                case IResponse:
                    IResponse response = (IResponse)message;
                    session.SetResult(response.RpcID, response);
                    break;
                case IRequest:
                    var req_handler = GetHandler(message);
                    req_handler.Handle(message, session);
                    break;
                case IMessage:
                    var msg_handler = GetHandler(message);
                    msg_handler.Handle(message, session);
                    break;
            }
        }

        public void OnlyLoad(string assemblyName)
        {
            Assembly current = Assembly.Load(assemblyName);
            Type[] thisTypes = current.GetTypes();

            Type[] messagTypes = thisTypes.Where(type => typeof(IMessage).IsAssignableFrom(type)).ToArray();
            Type[] handleTypes = thisTypes.Where(type => typeof(IMessageHandler).IsAssignableFrom(type)).ToArray();

            messageTypes.AddRange(messagTypes);
            handlerTypes.AddRange(handleTypes);
        }

        public void OnlyLoadFile(string filePath)
        {
            Assembly current = Assembly.LoadFile(filePath);
            Type[] allTypes = current.GetTypes();

            Type[] thisTypes = current.GetTypes();

            Type[] messagTypes = thisTypes.Where(type => typeof(IMessage).IsAssignableFrom(type)).ToArray();
            Type[] handleTypes = thisTypes.Where(type => typeof(IMessageHandler).IsAssignableFrom(type)).ToArray();

            messageTypes.AddRange(messagTypes);
            handlerTypes.AddRange(handleTypes);
        }

        public void GeneLoad()
        {
            Type generic0 = typeof(IMessage);
            Type generic1 = typeof(IRequest);
            Type generic2 = typeof(IResponse);
            foreach (Type type in messageTypes)
            {
                if (type == generic0 || type == generic1 || type == generic2) continue;
                MessageAttribute msg = type.GetCustomAttribute<MessageAttribute>();
                if (msg == null) continue;
                Type2Opcode.Add(type, msg.Opcode);
                Opcode2Type.Add(msg.Opcode, type);
            }

            generic0 = typeof(IMessageHandler);
            generic1 = typeof(MessageHandler<>);
            generic2 = typeof(RpcMessageHandler<,>);

            foreach (var type in handlerTypes)
            {
                if (type == generic0 || type == generic1 || type == generic2) continue;
                Type generic = type.BaseType.GetGenericTypeDefinition();
                if (generic != generic1 && generic != generic2) continue;
                Type handleType = type.BaseType.GenericTypeArguments[0];
                if (!Type2Opcode.ContainsKey(handleType)) continue;
                IMessageHandler handler = (IMessageHandler)Activator.CreateInstance(type);

                Opcode2Handler.Add(Type2Opcode[handleType], handler);

                Console.WriteLine(type.Name);
            }
        }
    }
}
