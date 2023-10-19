using Jrainstar;
using System;
using System.Collections.Generic;

namespace Jrainstar
{
    public class OpcodeType : Component, IAwake
    {
        // 初始化后不变，所以主线程，网络线程都可以读
        private readonly DoubleMap<Type, ushort> typeOpcode = new();

        public static OpcodeType Instance { get; private set; }

        public void Awake()
        {
            Instance = this;
            HashSet<Type> types = CodeType.Instance.GetTypes(typeof(MessageAttribute));
            foreach (Type type in types)
            {
                object[] att = type.GetCustomAttributes(typeof(MessageAttribute), false);
                if (att.Length == 0)
                {
                    continue;
                }

                MessageAttribute messageAttribute = att[0] as MessageAttribute;
                if (messageAttribute == null)
                {
                    continue;
                }

                ushort opcode = messageAttribute.Opcode;
                if (opcode != 0)
                {
                    typeOpcode.Add(type, opcode);
                }
            }
        }

        public ushort GetOpcode(Type type)
        {
            return typeOpcode.GetValueByKey(type);
        }

        public Type GetType(ushort opcode)
        {
            Type type = typeOpcode.GetKeyByValue(opcode);
            if (type == null)
            {
                throw new Exception($"OpcodeType not found type: {opcode}");
            }
            return type;
        }
    }
}