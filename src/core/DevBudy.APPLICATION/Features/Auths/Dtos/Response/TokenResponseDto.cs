using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.APPLICATION.Features.Auths.Dtos.Response
{
    public class TokenResponseDto
    {
        public string Token { get; set; }
        public DateTime ExpiresDate { get; set; }
    }
}
