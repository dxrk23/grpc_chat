using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using chat.Context;
using ChatProject;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;

namespace chat
{
    public class ChatRoom
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly ConcurrentDictionary<string, KeyValuePair<int, IServerStreamWriter<Message>>> users =
            new ConcurrentDictionary<string, KeyValuePair<int, IServerStreamWriter<Message>>>();

        public ChatRoom(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void Join(Message message, IServerStreamWriter<Message> response)
        {
            var temp = new KeyValuePair<int, IServerStreamWriter<Message>>(message.Room, response);
            LoadFromDatabase(response);
            users.TryAdd(message.User, temp);
        }

        public void Remove(string name)
        {
            users.TryRemove(name, out _);
        }

        public async Task BroadcastMessageAsync(Message message)
        {
            await BroadcastMessage(message);
        }

        private async Task BroadcastMessage(Message message)
        {
            foreach (var user in users.Where(x => x.Key != message.User && x.Value.Key.Equals(message.Room)))
            {
                var item = await SendMessageToSubscriber(user, message);
                if (item != null) Remove(item?.Key);
            }
        }

        private async Task<KeyValuePair<string, KeyValuePair<int, IServerStreamWriter<Message>>>?>
            SendMessageToSubscriber(KeyValuePair<string, KeyValuePair<int, IServerStreamWriter<Message>>> user,
                Message message)
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

        public async Task ToDatabase(Message message)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var msg = new Models.Message
            {
                Username = message.User,
                UserMessage = message.Text,
                Room = message.Room
            };

            await db.Messages.AddAsync(msg);
            await db.SaveChangesAsync();
        }

        public async Task LoadFromDatabase(IServerStreamWriter<Message> response)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            var messages = db.Messages.ToArray();

            foreach (var message in messages)
            {
                var temp = new Message
                {
                    Room = message.Room,
                    Text = message.UserMessage,
                    User = message.Username
                };

                var temppair = new KeyValuePair<int, IServerStreamWriter<Message>>(temp.Room, response);
                var temptemp =
                    new KeyValuePair<string, KeyValuePair<int, IServerStreamWriter<Message>>>(temp.User, temppair);
                foreach (var user in users)
                    if (user.Value.Key == temp.Room && user.Key != temp.User)
                        SendMessageToSubscriber(temptemp, temp);
            }
        }
    }
}