using System;

namespace Jrainstar
{
    public interface IMessageHandler
    {
        void Handle(Session session, object message);
        Type GetMessageType();

        Type GetResponseType();
    }
}