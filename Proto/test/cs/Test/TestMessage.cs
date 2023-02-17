using ProtoBuf;
using System.Collections.Generic;

namespace Moon
{
	[ProtoContract]
	[Message(TestOpcode.M0000_C2S_Test0)]
	public partial class M0000_C2S_Test0 : IMessage
	{
		[ProtoMember(1)]
		public string human { get; set; }

		[ProtoMember(2)]
		public int age { get; set; }

	}
	[ProtoContract]
	[Message(TestOpcode.M0000_S2C_Test1)]
	public partial class M0000_S2C_Test1 : IMessage
	{
		[ProtoMember(3)]
		public List<string> human { get; set; } = new List<string>();

		[ProtoMember(2)]
		public int age { get; set; }

	}
	[ProtoContract]
	[Message(TestOpcode.R0000_C2S_Test2)]
	public partial class R0000_C2S_Test2 : IRequest
	{
		[ProtoMember(1)]
		public int RpcID { get; set; }

		[ProtoMember(2)]
		public string pokeName { get; set; }

	}
	[ProtoContract]
	[Message(TestOpcode.R0000_S2C_Test2)]
	public partial class R0000_S2C_Test2 : IResponse
	{
		[ProtoMember(1)]
		public int RpcID { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string pokeName { get; set; }

		[ProtoMember(4)]
		public int level { get; set; }

	}
	[ProtoContract]
	[Message(TestOpcode.R0000_C2S_Test3)]
	public partial class R0000_C2S_Test3 : IRequest
	{
		[ProtoMember(1)]
		public int RpcID { get; set; }

		[ProtoMember(2)]
		public string pokeName { get; set; }

	}
	[ProtoContract]
	[Message(TestOpcode.R0000_S2C_Test3)]
	public partial class R0000_S2C_Test3 : IResponse
	{
		[ProtoMember(1)]
		public int RpcID { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string pokeName { get; set; }

		[ProtoMember(4)]
		public int level { get; set; }

	}
	[ProtoContract]
	[Message(TestOpcode.AM0000_C2S_Test4)]
	public partial class AM0000_C2S_Test4 : IMessage
	{
		[ProtoMember(1)]
		public string Actor { get; set; }

	}
	[ProtoContract]
	[Message(TestOpcode.AM0000_C2S_Test5)]
	public partial class AM0000_C2S_Test5 : IAvatarMessage
	{
		[ProtoMember(1)]
		public string Actor { get; set; }

	}
	[ProtoContract]
	[Message(TestOpcode.AR0000_C2S_Test5)]
	public partial class AR0000_C2S_Test5 : IAvatarRequest
	{
		[ProtoMember(1)]
		public int RpcID { get; set; }

		[ProtoMember(2)]
		public string Actor { get; set; }

	}
	[ProtoContract]
	[Message(TestOpcode.AR0000_S2C_Test5)]
	public partial class AR0000_S2C_Test5 : IAvatarResponse
	{
		[ProtoMember(1)]
		public int RpcID { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Actor { get; set; }

	}
}
