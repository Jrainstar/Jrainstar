using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firis
{
    public interface IMessageHandler
    {
        public abstract void Handle(IMessage message, Session session);
    }

    public abstract class MessageHandler<TMessage> : IMessageHandler where TMessage : IMessage
    {
        public abstract Task Process(TMessage message, Session session);
        public async void Handle(IMessage message, Session session)
        {
            await Process((TMessage)message, session);
        }
    }

    public abstract class RpcMessageHandler<TRequest, TResponse> : IMessageHandler where TRequest : IRequest, new() where TResponse : IResponse, new()
    {
        public abstract Task Process(TRequest request, TResponse response, Session session);
        public async void Handle(IMessage message, Session session)
        {
            //if (cache.Count == 0) cache.Enqueue(new TResponse());
            TResponse response = Activator.CreateInstance<TResponse>();
            TRequest request = (TRequest)message;

            response.RpcID = request.RpcID;

            await Process(request, response, session);
            Reply(session, response);
        }
        public void Reply(Session session, TResponse response)
        {
            session.Send(response);
        }
    }
}
