using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.APPLICATION.Features.Chats.Commands
{
    public class CreateChatMessageCommand : IRequest<string>
    {
        public string SenderUserName { get; set; }
        public string Message { get; set; }
    }
}
