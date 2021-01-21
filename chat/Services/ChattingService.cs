using System.Threading.Tasks;
using ChatProject;
using Grpc.Core;

namespace chat.Services
{
    public class ChattingService : ChatService.ChatServiceBase
    {
        private readonly ChatRoom _chatRoomService;

        public ChattingService(ChatRoom chatRoomService)
        {
            _chatRoomService = chatRoomService;
        }

        public override async Task Join(IAsyncStreamReader<Message> requestStream,
            IServerStreamWriter<Message> responseStream, ServerCallContext context)
        {
            if (!await requestStream.MoveNext()) return;

            do
            {
                _chatRoomService.Join(requestStream.Current, responseStream);
                await _chatRoomService.BroadcastMessageAsync(requestStream.Current);
            } while (await requestStream.MoveNext());

            _chatRoomService.Remove(context.Peer);
        }

        public override async Task<Message> ToDB(Message request, ServerCallContext context)
        {
            await _chatRoomService.ToDatabase(request);
            return request;
        }
    }
}