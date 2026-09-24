using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Application.Features.Auths.Dtos.Response
{
    public class ConnectedUserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
    }
}
