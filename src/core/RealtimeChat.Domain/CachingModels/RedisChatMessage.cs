using RealtimeChat.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Domain.CachingModels
{
    public class RedisChatMessage
    {
        public RedisChatMessage()
        {
            MessageType = MessageType.Text;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid MessageId { get; set; }
        public int? SenderUserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string SenderUserName { get; set; } = string.Empty;
        public MessageType MessageType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
