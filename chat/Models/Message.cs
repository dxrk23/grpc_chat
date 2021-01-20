using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace chat.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string UserMessage { get; set; }
        public int Room { get; set; }
    };
}
