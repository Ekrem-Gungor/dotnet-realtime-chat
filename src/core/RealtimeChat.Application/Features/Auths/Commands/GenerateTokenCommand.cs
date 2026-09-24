using RealtimeChat.Application.Features.Auths.Dtos.Response;
using RealtimeChat.Domain.Entities.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Application.Features.Auths.Commands
{
    public class GenerateTokenCommand : IRequest<TokenResponseDto>
    {
        public int UserId { get; set; }
    }
}
