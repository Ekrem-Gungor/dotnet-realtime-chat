using RealtimeChat.Application.Events.ChatMessages;
using RealtimeChat.Application.Features.Chats.Dtos;
using RealtimeChat.Contracts.Repositories.RedisRepositories;
using RealtimeChat.Domain.CachingModels;
using RealtimeChat.Domain.Entities.Concretes;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Application.Features.Chats.Commands
{
    public class CreateChatMessageCommandHandler : IRequestHandler<CreateChatMessageCommand, string>
    {
        private readonly IMessageRedisRepository _msgRedisRepo;
        private readonly IMessageQuotaRepository _quotaRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMediator _mediator;

        public CreateChatMessageCommandHandler(IMediator mediator, UserManager<AppUser> userManager, IMessageRedisRepository msgRedisRepo, IMessageQuotaRepository quotaRepo)
        {
            _userManager = userManager;
            _mediator = mediator;
            _msgRedisRepo = msgRedisRepo;
            _quotaRepo = quotaRepo;
        }

        public async Task<string> Handle(CreateChatMessageCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Message) || string.IsNullOrEmpty(request.SenderUserName))
            {
                throw new ArgumentException("Message cannot be empty.", nameof(request.Message));
            }
            AppUser senderUser = await _userManager.FindByNameAsync(request.SenderUserName)
                ?? throw new UnauthorizedAccessException("Authenticated user could not be found.");

            if (!await _quotaRepo.TryConsumeAsync(senderUser.Id.ToString()))
                return "Yeterli mesaj kotanız kalmamıştır.";

            RedisChatMessage redisChatMessage = new()
            {
                MessageId = Guid.NewGuid(),
                Message = request.Message,
                SenderUserName = request.SenderUserName
            };

            ChatMessageDto chatMessageDto = new()
            {
                Id = redisChatMessage.MessageId,
                SenderUserId = senderUser.Id,
                SenderUserName = redisChatMessage.SenderUserName,
                Message = redisChatMessage.Message,
                SendAt = redisChatMessage.CreatedAt
            };

            await _msgRedisRepo.AddMessageAsync(redisChatMessage);
            await _mediator.Publish(new ChatMessageCreatedEvent(chatMessageDto), cancellationToken);

            return "Mesaj gönderildi.";
        }
    }
}
