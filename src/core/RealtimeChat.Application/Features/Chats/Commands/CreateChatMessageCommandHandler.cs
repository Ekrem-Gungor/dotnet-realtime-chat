using MediatR;
using Microsoft.AspNetCore.Identity;
using RealtimeChat.Application.Common.Exceptions;
using RealtimeChat.Application.Events.ChatMessages;
using RealtimeChat.Application.Features.Chats.Dtos;
using RealtimeChat.Contracts.Repositories.RedisRepositories;
using RealtimeChat.Domain.CachingModels;
using RealtimeChat.Domain.Entities.Concretes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Application.Features.Chats.Commands
{
    public sealed class CreateChatMessageCommandHandler : IRequestHandler<CreateChatMessageCommand, ChatMessageDto>
    {
        private readonly IMessageRedisRepository _messageRepository;
        private readonly IMessageQuotaRepository _quotaRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IPublisher _publisher;

        public CreateChatMessageCommandHandler(IPublisher publisher, UserManager<AppUser> userManager, IMessageRedisRepository messageRepository, IMessageQuotaRepository quotaRepository)
        {
            _publisher = publisher;
            _userManager = userManager;
            _messageRepository = messageRepository;
            _quotaRepository = quotaRepository;
        }

        public async Task<ChatMessageDto> Handle(CreateChatMessageCommand request, CancellationToken cancellationToken)
        {
            AppUser senderUser = await _userManager.FindByNameAsync(request.SenderUserName) ?? throw new UnauthorizedAccessException("Authenticated user could not be found.");

            bool quotaConsumed = await _quotaRepository.TryConsumeAsync(senderUser.Id.ToString());

            if (!quotaConsumed)
                throw new MessageQuotaExceededException();

            RedisChatMessage redisMessage = new()
            {
                MessageId = Guid.NewGuid(),
                Message = request.Message,
                SenderUserName = request.SenderUserName
            };

            ChatMessageDto response = new()
            {
                Id = redisMessage.MessageId,
                SenderUserId = senderUser.Id,
                SenderUserName = redisMessage.SenderUserName,
                Message = redisMessage.Message,
                SendAt = redisMessage.CreatedAt
            };

            await _messageRepository.AddMessageAsync(redisMessage);

            await _publisher.Publish(new ChatMessageCreatedEvent(response), cancellationToken);

            return response;
        }
    }
}
