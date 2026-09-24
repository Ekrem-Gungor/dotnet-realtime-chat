using DevBudy.APPLICATION.Features.Auths.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.APPLICATION.Features.Auths.Queries
{
    public class OnlineUsersQuery : IRequest<List<ConnectedUserDto>>
    {

    }
}
