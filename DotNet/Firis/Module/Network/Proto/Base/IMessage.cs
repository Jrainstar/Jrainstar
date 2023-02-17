using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firis
{
    public class MessageAttribute : Attribute
    {
        public ushort Opcode { get; set; }
        public MessageAttribute(ushort opcode)
        {
            Opcode = opcode;
        }
    }

    public interface IMessage
    {

    }

    public interface IRequest : IMessage
    {
        int RpcID
        {
            get;
            set;
        }
    }

    public interface IResponse : IMessage
    {
        int Error
        {
            get;
            set;
        }

        int RpcID
        {
            get;
            set;
        }
    }

    public interface IAvatarMessage : IMessage
    {

    }

    public interface IAvatarRequest : IRequest
    {

    }

    public interface IAvatarResponse : IResponse
    {

    }
}
