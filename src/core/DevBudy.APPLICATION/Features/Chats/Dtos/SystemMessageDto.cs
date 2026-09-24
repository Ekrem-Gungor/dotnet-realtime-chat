using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.APPLICATION.Features.Chats.Dtos
{
    public class SystemMessageDto : BaseDto
    {
        public SystemMessageDto()
        {
            SenderUserName = "System";
            Message = ", Sohbete katıldı.";
        }
        public string SenderUserName { get; }
        public string Message { get; }
        public string JoinedUserName { get; set; }
    }
}
