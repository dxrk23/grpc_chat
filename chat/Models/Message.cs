using System;
using chat.ChatEventHandlers;

namespace chat.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string UserMessage { get; set; }
        public int Room { get; set; }
        public event EventHandler<ChatEventArgs> MessageReceived;

        public void OnMessageReceived(ChatProject.Message message)
        {
            var args = new ChatEventArgs(message);
            MessageReceived?.Invoke(this, args);
        }
    }
}