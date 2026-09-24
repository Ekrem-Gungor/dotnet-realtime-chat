using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Application.Features.Auths.Dtos.Response
{
    public class TokenResponseDto
    {
        public string Token { get; set; }
        public DateTime ExpiresDate { get; set; }
    }
}
