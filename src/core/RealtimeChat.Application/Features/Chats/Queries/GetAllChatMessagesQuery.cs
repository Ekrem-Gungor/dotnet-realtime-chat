using RealtimeChat.Application.Features.Chats.Dtos;
using RealtimeChat.Domain.CachingModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Application.Features.Chats.Queries
{
    public class GetAllChatMessagesQuery : IRequest<List<RedisChatMessage>>
    {

    }
}
