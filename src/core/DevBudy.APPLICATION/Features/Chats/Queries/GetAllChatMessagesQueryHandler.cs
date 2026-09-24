using DevBudy.APPLICATION.Features.Chats.Dtos;
using DevBudy.CONTRACT.Repositories.EFRepositories;
using DevBudy.CONTRACT.Repositories.RedisRepositories;
using DevBudy.DOMAIN.CachingModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.APPLICATION.Features.Chats.Queries
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
