using RealtimeChat.Contracts.Repositories.RedisRepositories;
using RealtimeChat.Domain.CachingModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Application.Features.Chats.Queries
{
    public class GetAllChatMessagesQueryHandler : IRequestHandler<GetAllChatMessagesQuery, List<RedisChatMessage>>
    {
        readonly IMessageRedisRepository _messageRedisRepo;

        public GetAllChatMessagesQueryHandler(IMessageRedisRepository messageRedisRepo)
        {
            _messageRedisRepo = messageRedisRepo;
        }

        public async Task<List<RedisChatMessage>> Handle(GetAllChatMessagesQuery request, CancellationToken cancellationToken)
        {
            List<RedisChatMessage> result = await _messageRedisRepo.GetMessagesFromLastMinutesAsync(30);
            return result;
        }
    }
}
