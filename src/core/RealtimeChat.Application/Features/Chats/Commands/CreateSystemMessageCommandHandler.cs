using RealtimeChat.Application.Events.ChatMessages;
using RealtimeChat.Application.Features.Chats.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Application.Features.Chats.Commands
{
    public class CreateSystemMessageCommandHandler : IRequestHandler<CreateSystemMessageCommand, SystemMessageDto>
    {
        private readonly IMediator _mediator;

        public CreateSystemMessageCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<SystemMessageDto> Handle(CreateSystemMessageCommand request, CancellationToken cancellationToken)
        {
            SystemMessageDto systemMessage = new()
            {
                JoinedUserName = request.JoinedUserName,
                SendAt = DateTime.UtcNow
            };

            await _mediator.Publish(new SystemMessageCreateEvent(systemMessage), cancellationToken);
            return systemMessage;
        }
    }
}
