using System.Collections.Generic;
using System;
using Jrainstar;

namespace Jrainstar
{
    public class MessageSessionDispatcherInfo
    {
        public IMessageHandler IMHandler { get; }

        public MessageSessionDispatcherInfo(IMessageHandler imHandler)
        {
            IMHandler = imHandler;
        }
    }

    public class MessageSessionDispatcher : Component, IAwake
    {
        private readonly Dictionary<ushort, List<MessageSessionDispatcherInfo>> handlers = new();

        public static MessageSessionDispatcher Instance { get; private set; }

        public void Awake()
        {
            Instance = this;
            HashSet<Type> types = CodeType.Instance.GetTypes(typeof(MessageSessionHandlerAttribute));
            foreach (Type type in types)
            {
                IMessageHandler iMessageSessionHandler = Activator.CreateInstance(type) as IMessageHandler;
                if (iMessageSessionHandler == null)
                {
                    Log.Error($"message handle {type.Name} 需要继承 IMHandler");
                    continue;
                }

                object[] attrs = type.GetCustomAttributes(typeof(MessageSessionHandlerAttribute), true);

                foreach (object attr in attrs)
                {
                    MessageSessionHandlerAttribute messageSessionHandlerAttribute = attr as MessageSessionHandlerAttribute;

                    Type messageType = iMessageSessionHandler.GetMessageType();

                    ushort opcode = OpcodeType.Instance.GetOpcode(messageType);
                    if (opcode == 0)
                    {
                        Log.Error($"消息opcode为0: {messageType.Name}");
                        continue;
                    }

                    MessageSessionDispatcherInfo messageSessionDispatcherInfo = new(iMessageSessionHandler);
                    RegisterHandler(opcode, messageSessionDispatcherInfo);
                }
            }
        }

        private void RegisterHandler(ushort opcode, MessageSessionDispatcherInfo handler)
        {
            if (!handlers.ContainsKey(opcode))
            {
                handlers.Add(opcode, new List<MessageSessionDispatcherInfo>());
            }

            handlers[opcode].Add(handler);
        }

        public void Handle(Session session, object message)
        {
            List<MessageSessionDispatcherInfo> actions;
            ushort opcode = OpcodeType.Instance.GetOpcode(message.GetType());
            if (!handlers.TryGetValue(opcode, out actions))
            {
                Log.Error($"消息没有处理: {opcode} {message}");
                return;
            }

            foreach (MessageSessionDispatcherInfo ev in actions)
            {
                try
                {
                    ev.IMHandler.Handle(session, message);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
    }
}