using DevBudy.APPLICATION.Events.ChatMessages;
using DevBudy.APPLICATION.Features.Chats.Dtos;
using DevBudy.CONTRACT.Repositories.EFRepositories;
using DevBudy.CONTRACT.Repositories.RedisRepositories;
using DevBudy.DOMAIN.CachingModels;
using DevBudy.DOMAIN.Entities.Concretes;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.APPLICATION.Features.Chats.Commands
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
            AppUser senderUser = await _userManager.FindByNameAsync(request.SenderUserName);
            var hasQuota = await _quotaRepo.GetQuotaAsync(senderUser.Id.ToString());
            if (hasQuota.ReaminingMessage > 0 && await _quotaRepo.TryConsumeAsync(senderUser.Id.ToString()))
            {
                RedisChatMessage redisChatMsg = new()
                {
                    MessageId = Guid.NewGuid(),
                    Message = request.Message,
                    SenderUserName = request.SenderUserName
                };

                ChatMessageDto chatMessageDto = new()
                {
                    Id = Guid.NewGuid(),
                    SenderUserId = senderUser.Id,
                    SenderUserName = redisChatMsg.SenderUserName,
                    Message = redisChatMsg.Message,
                    SendAt = redisChatMsg.CreateAt
                };

                long score = DateTimeOffset.Now.ToUnixTimeSeconds();

                await _msgRedisRepo.AddMessageToSortedSetAsync($"message:{chatMessageDto.Id}", redisChatMsg, score);
                await _mediator.Publish(new ChatMessageCreatedEvent(chatMessageDto));
                return "Mesaj gönderildi.";
            }

            //ChatMessage chatMessage = new()
            //{
            //    SenderID = senderUser.Id,
            //    Message = request.Message
            //};
            //await _chatMsgRepo.CreateAsync(chatMessage);

            return "Yeterli mesaj kotanız kalmamıştır.";
        }
    }
}
