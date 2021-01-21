using System.Threading.Tasks;
using ChatProject;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace chat.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override Task<HelloReply2> SayHello2(HelloRequest request, ServerCallContext context)
        {
            var reply = new HelloReply2();
            reply.Replies.Add(new HelloReply {Message = "Hello " + request.Name + " 1"});
            reply.Replies.Add(new HelloReply {Message = "Hello " + request.Name + " 2"});
            reply.Replies.Add(new HelloReply {Message = "Hello " + request.Name + " 3"});

            return Task.FromResult(reply);
        }
    }
}