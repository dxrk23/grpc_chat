using System;
using ChatProject;

namespace chat.ChatEventHandlers
{
    public class ChatEventArgs : EventArgs
    {
        public ChatEventArgs(Message message)
        {
            SenderName = message.User;
            ReceivedDate = DateTime.Now;
        }

        public DateTime ReceivedDate { get; }
        public string SenderName { get; }
    }
}