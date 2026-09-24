using RealtimeChat.Application.Features.Chats.Dtos;
using RealtimeChat.Contracts.Repositories.EFRepositories;
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
        readonly IChatMessageRepository _chatMsgRepo;
        readonly IMessageRedisRepository _messageRedisRepo;

        public GetAllChatMessagesQueryHandler(IChatMessageRepository chatMsgRepo, IMessageRedisRepository messageRedisRepo)
        {
            _chatMsgRepo = chatMsgRepo;
            _messageRedisRepo = messageRedisRepo;
        }

        public async Task<List<RedisChatMessage>> Handle(GetAllChatMessagesQuery request, CancellationToken cancellationToken)
        {
            //List<ChatMessageDto> chatMessages = _chatMsgRepo.GetAllAsync().Result
            //    .Select(msg => new ChatMessageDto
            //    {
            //        SenderUserId = msg.SenderID,
            //        SenderUserName = msg.Sender.UserName,
            //        Message = msg.Message,
            //        SendAt = msg.CreatedDate
            //    }).ToList();

            List<RedisChatMessage> result = await _messageRedisRepo.GetMessagesFromLastMinutesAsync(30);
            return result;
        }
    }
}
