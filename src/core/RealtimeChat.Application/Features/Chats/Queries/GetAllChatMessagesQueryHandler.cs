using RealtimeChat.Contracts.Repositories.RedisRepositories;
using RealtimeChat.Domain.CachingModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealtimeChat.Application.Features.Chats.Dtos;

namespace RealtimeChat.Application.Features.Chats.Queries
{
    public class GetAllChatMessagesQueryHandler : IRequestHandler<GetAllChatMessagesQuery, List<ChatMessageDto>>
    {
        readonly IMessageRedisRepository _messageRedisRepo;

        public GetAllChatMessagesQueryHandler(IMessageRedisRepository messageRedisRepo)
        {
            _messageRedisRepo = messageRedisRepo;
        }

        public async Task<List<ChatMessageDto>> Handle(GetAllChatMessagesQuery request, CancellationToken cancellationToken)
        {
            List<RedisChatMessage> messages = await _messageRedisRepo.GetMessagesFromLastMinutesAsync(30);
            List<ChatMessageDto> messageDto = messages.Select(message => new ChatMessageDto
            {
                Id = message.MessageId,
                SenderUserId = message.SenderUserId,
                SenderUserName = message.SenderUserName,
                Message = message.Message,
                SendAt = message.CreatedAt
            }).ToList();

            return messageDto;
        }
    }
}
