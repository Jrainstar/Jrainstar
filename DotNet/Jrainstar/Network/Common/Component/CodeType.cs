using System.Collections.Generic;
using System.Reflection;
using System;

namespace Jrainstar
{
    public class CodeType : Component, IAwake,IAwake<Assembly[]>
    {
        private readonly Dictionary<string, Type> allTypes = new();
        private readonly UnOrderMultiMapSet<Type, Type> types = new();

        public static CodeType Instance { get; set; }

        public void Awake()
        {
            Awake(AppDomain.CurrentDomain.GetAssemblies());
        }

        public void Awake(Assembly[] assemblies)
        {
            Instance = this;

            Dictionary<string, Type> addTypes = AssemblyHelper.GetAssemblyTypes(assemblies);
            foreach ((string fullName, Type type) in addTypes)
            {
                allTypes[fullName] = type;

                if (type.IsAbstract)
                {
                    continue;
                }

                // 记录所有的有BaseAttribute标记的的类型
                object[] objects = type.GetCustomAttributes(typeof(BaseAttribute), true);

                foreach (object o in objects)
                {
                    types.Add(o.GetType(), type);
                }
            }
        }

        public HashSet<Type> GetTypes(Type systemAttributeType)
        {
            if (!types.ContainsKey(systemAttributeType))
            {
                return new HashSet<Type>();
            }

            return types[systemAttributeType];
        }

        public Dictionary<string, Type> GetTypes()
        {
            return allTypes;
        }

        public Type GetType(string typeName)
        {
            return allTypes[typeName];
        }


    }
}