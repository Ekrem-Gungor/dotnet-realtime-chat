using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.APPLICATION.Features.Chats.Dtos
{
    public class ChatMessageDto : BaseDto
    {
        public Guid Id { get; set; }
        public int? SenderUserId { get; set; }
        public string SenderUserName { get; set; }
        public string Message { get; set; }
    }
}
