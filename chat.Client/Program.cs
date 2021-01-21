using System;
using System.Threading;
using System.Threading.Tasks;
using ChatProject;
using Grpc.Net.Client;

namespace chat.Client
{
    internal class Program
    {
        public static CancellationToken CancellationToken { get; }

        private static async Task Main(string[] args)
        {
            var loaded = false;
            Console.Write("Please enter your name: ");
            var username = Console.ReadLine();
            Console.Write("Please enter number of the room: ");
            var room = Convert.ToInt32(Console.ReadLine());

            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new ChatService.ChatServiceClient(channel);

            using (var chat = client.Join())
            {
                _ = Task.Run(async () =>
                {
                    while (await chat.ResponseStream.MoveNext(CancellationToken))
                    {
                        var response = chat.ResponseStream.Current;
                        Console.WriteLine($"Room {response.Room} : {response.User} : {response.Text}");
                    }
                });

                await chat.RequestStream.WriteAsync(new Message
                    {Room = room, User = username, Text = $"{username} has joined the chat!"});

                string line;


                while ((line = Console.ReadLine()) != null)
                {
                    if (line.ToUpper() == "EXIT") break;
                    Console.Clear();
                    client.ToDB(new Message {Room = room, User = username, Text = line});
                    await chat.RequestStream.WriteAsync(new Message {Room = room, User = username, Text = line});
                }

                await chat.RequestStream.CompleteAsync();
            }

            Console.WriteLine("Disconnection started!");
            await channel.ShutdownAsync();
        }
    }
}