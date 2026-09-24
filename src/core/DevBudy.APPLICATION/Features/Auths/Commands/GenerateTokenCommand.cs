using DevBudy.APPLICATION.Features.Auths.Dtos.Response;
using DevBudy.DOMAIN.Entities.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.APPLICATION.Features.Auths.Commands
{
    public class GenerateTokenCommand : IRequest<TokenResponseDto>
    {
        public int UserId { get; set; }
    }
}
