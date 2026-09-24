using DevBudy.DOMAIN.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.DOMAIN.CachingModels
{
    public class RedisChatMessage
    {
        public RedisChatMessage()
        {
            MessageType = MessageType.Text;
            CreateAt = DateTime.Now;
        }

        public Guid MessageId { get; set; }
        public string Message { get; set; }
        public string SenderUserName { get; set; }
        public MessageType MessageType { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
