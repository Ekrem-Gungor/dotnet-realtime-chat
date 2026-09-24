using DevBudy.APPLICATION.Events.ChatMessages;
using DevBudy.APPLICATION.Features.Chats.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.APPLICATION.Features.Chats.Commands
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
                SendAt = DateTime.Now
            };
            // Burada loglama işlemi yapılabilir.

            await _mediator.Publish(new SystemMessageCreateEvent(systemMessage));
            return systemMessage;
        }
    }
}
