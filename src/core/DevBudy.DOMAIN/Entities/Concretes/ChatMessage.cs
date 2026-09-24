using DevBudy.DOMAIN.Entities.Abstracts;
using DevBudy.DOMAIN.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.DOMAIN.Entities.Concretes
{
    public class ChatMessage : BaseEntity
    {
        public ChatMessage()
        {
            MessageType = MessageType.Text; // Default message type
        }
        public string Message { get; set; }
        public MessageType MessageType { get; set; }
        public string? MetaData { get; set; } // IP, UserAgent, etc vs. gibi metadata bilgilierini saklamak için kullanılabilir.
        public int? SenderID { get; set; }

        // Relational Properties
        public virtual AppUser Sender { get; set; }
    }
}
