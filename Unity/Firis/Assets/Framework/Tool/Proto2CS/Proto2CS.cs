using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Firis
{
    public static class Proto2CS
    {
        public class OpcodeInfo
        {
            public string Name;
            public int Opcode;
        }

        public static string config = "config.xml";
        public static List<OpcodeInfo> opcodes = new List<OpcodeInfo>();

        public static ushort opcode = 10001;
        public static ushort offset = 10000;

        public static void Export()
        {
            ushort index = 0;

            XDocument xdoc = XDocument.Load(config);
            XElement root = xdoc.Root;

            foreach (XElement rule in root.Elements())
            {
                string proto = rule.Element("proto").Value;
                var flodersc = rule.Elements("floder").ToArray();
                string[] floders = flodersc.Select(f => f.Value).ToArray();

                FileInfo file = new FileInfo(proto);

                string fileName = file.Name.Substring(0, file.Name.LastIndexOf('.'));
                Proto2Script(proto, floders, fileName, opcode + offset * (index++));
            }
        }

        private static void Proto2Script(string proto, string[] floders, string fileName, int origin)
        {
            string protoname = proto.Split('/').Last().Split('.').First();

            int current = origin;
            StringBuilder msg = new StringBuilder();
            StringBuilder code = new StringBuilder();
            StreamReader reader = File.OpenText(proto);

            msg.AppendLine("using ProtoBuf;");
            msg.AppendLine("using System.Collections.Generic;");
            msg.AppendLine();
            msg.AppendLine($"namespace Firis");
            msg.AppendLine("{");

            code.AppendLine($"namespace Firis");
            code.AppendLine("{");
            code.AppendLine($"\tpublic static partial class {protoname}Opcode");
            code.AppendLine("\t{");

            bool isMessage = false;

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine().Trim();

                if (line.StartsWith("message"))
                {
                    isMessage = true;
                    string parentClass = "";
                    string msgName = line.Split(' ')[1];

                    var ss = line.Split("//");
                    if (ss.Length == 2)
                    {
                        parentClass = ss[1].Trim();
                    }
                    //opcodes.Add(new OpcodeInfo() { Name = msgName, Opcode = current++ });

                    code.AppendLine($"\t\tpublic const ushort {msgName} = {current++};");

                    //分布式 先注释
                    //if (parentClass == "IAvatarRequest")
                    //{
                    //    msg.AppendLine($"\t[ProtoContract]");
                    //    msg.AppendLine($"\t[Message({protoname}Opcode.{msgName})]");
                    //    msg.AppendLine($"\tpublic partial class {msgName} : IAvatarRequest");
                    //}
                    //else if (parentClass == "IAvatarResponse")
                    //{
                    //    msg.AppendLine($"\t[ProtoContract]");
                    //    msg.AppendLine($"\t[Message({protoname}Opcode.{msgName})]");
                    //    msg.AppendLine($"\tpublic partial class {msgName} : IAvatarResponse");
                    //}
                    //else if (parentClass == "IAvatarMessage")
                    //{
                    //    msg.AppendLine($"\t[ProtoContract]");
                    //    msg.AppendLine($"\t[Message({protoname}Opcode.{msgName})]");
                    //    msg.AppendLine($"\tpublic partial class {msgName} : IAvatarMessage");
                    //}
                    if (parentClass == "IRequest")
                    {
                        msg.AppendLine($"\t[ProtoContract]");
                        msg.AppendLine($"\t[Message({protoname}Opcode.{msgName})]");
                        msg.AppendLine($"\tpublic partial class {msgName} : IRequest");
                    }
                    else if (parentClass == "IResponse")
                    {
                        msg.AppendLine($"\t[ProtoContract]");
                        msg.AppendLine($"\t[Message({protoname}Opcode.{msgName})]");
                        msg.AppendLine($"\tpublic partial class {msgName} : IResponse");
                    }
                    else if (parentClass == "IMessage")
                    {
                        msg.AppendLine($"\t[ProtoContract]");
                        msg.AppendLine($"\t[Message({protoname}Opcode.{msgName})]");
                        msg.AppendLine($"\tpublic partial class {msgName} : IMessage");
                    }
                    else
                    {
                        msg.AppendLine($"\t[ProtoContract]");
                        msg.AppendLine($"\tpublic class {msgName}");
                    }

                    continue;
                }

                if (isMessage)
                {
                    if (line.StartsWith("{"))
                    {
                        msg.AppendLine("\t{");
                        continue;
                    }
                    if (line.StartsWith("}"))
                    {
                        isMessage = false;
                        msg.AppendLine("\t}");
                        continue;
                    }
                    if (line.Trim().StartsWith("//"))
                    {
                        msg.AppendLine(line);
                        continue;
                    }

                    if (line.Trim() != "" && line != "}")
                    {
                        if (line.StartsWith("repeated"))
                        {
                            var num = line.Split('=')[1].Trim().Replace(";", "");
                            var type = Convert(line.Split(' ')[1]);
                            var name = line.Split(' ')[2];
                            msg.AppendLine($"\t\t[ProtoMember({num})]");
                            msg.AppendLine($"\t\tpublic List<{type}> {name} {{ get; set; }} = new List<{type}>();");
                            msg.AppendLine();
                        }
                        else if (line.StartsWith("optional"))
                        {
                            var num = line.Split('=')[1].Trim().Replace(";", "");
                            var type = Convert(line.Split(' ')[1]);
                            var name = line.Split(' ')[2];
                            msg.AppendLine($"\t\t[ProtoMember({num})]");
                            msg.AppendLine($"\t\tpublic {type} {name} {{ get; set; }}");
                            msg.AppendLine();
                        }
                        else
                        {
                            var num = line.Split('=')[1].Trim().Replace(";", "");
                            var type = Convert(line.Split(' ')[0]);
                            var name = line.Split(' ')[1];
                            msg.AppendLine($"\t\t[ProtoMember({num})]");
                            msg.AppendLine($"\t\tpublic {type} {name} {{ get; set; }}");
                            msg.AppendLine();
                        }
                    }
                }
            }

            msg.AppendLine("}");

            code.AppendLine("\t}");
            code.AppendLine("}");

            for (int i = 0; i < floders.Length; i++)
            {
                var create = $"{floders[i]}/{fileName}";
                if (!Directory.Exists(create)) Directory.CreateDirectory(create);
                File.WriteAllText($"{floders[i]}/{fileName}Message.cs", msg.ToString());
                File.WriteAllText($"{floders[i]}/{fileName}Opcode.cs", code.ToString());
            }
        }

        private static string Convert(string type)
        {
            string typeCs = "";
            switch (type)
            {
                case "int16":
                    typeCs = "short";
                    break;
                case "int32":
                    typeCs = "int";
                    break;
                case "bytes":
                    typeCs = "byte[]";
                    break;
                case "uint32":
                    typeCs = "uint";
                    break;
                case "long":
                    typeCs = "long";
                    break;
                case "int64":
                    typeCs = "long";
                    break;
                case "uint64":
                    typeCs = "ulong";
                    break;
                case "uint16":
                    typeCs = "ushort";
                    break;
                default:
                    typeCs = type;
                    break;
            }

            return typeCs;
        }

    }
}
