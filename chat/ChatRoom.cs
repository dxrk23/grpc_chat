using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatProject;
using Grpc.Core;

namespace chat
{
    public class ChatRoom
    {
        //private readonly ConcurrentDictionary<string, IServerStreamWriter<Message>> users = new ConcurrentDictionary<string, IServerStreamWriter<Message>>();
        private readonly ConcurrentDictionary<string, KeyValuePair<int, IServerStreamWriter<Message>>> users =
            new ConcurrentDictionary<string, KeyValuePair<int, IServerStreamWriter<Message>>>();

        public void Join(Message message, IServerStreamWriter<Message> response)
        {
            KeyValuePair<int, IServerStreamWriter<Message>> temp = new KeyValuePair<int, IServerStreamWriter<Message>>(message.Room, response);
            users.TryAdd(message.User, temp);
        }

        public void Remove(string name)
        {
            users.TryRemove(name, out _);
        } 

        public async Task BroadcastMessageAsync(Message message) => await BroadcastMessage(message);
        private async Task BroadcastMessage(Message message)
        {
            foreach (var user in users.Where(x => x.Key != message.User && x.Value.Key.Equals(message.Room)))
            {
                var item = await SendMessageToSubscriber(user, message);    
                if (item != null)
                {
                    Remove(item?.Key);
                }
            }
        }

        private async Task<KeyValuePair<string, KeyValuePair<int, IServerStreamWriter<Message>>>?> SendMessageToSubscriber(KeyValuePair<string, KeyValuePair<int,IServerStreamWriter<Message>>> user, Message message)
        {
            try
            {
                await user.Value.Value.WriteAsync(message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return user;
            }
        }
    }
}