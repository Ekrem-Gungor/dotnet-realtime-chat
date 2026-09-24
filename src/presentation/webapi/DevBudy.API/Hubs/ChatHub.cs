using DevBudy.APPLICATION.Features.AppUsers.Commands;
using DevBudy.APPLICATION.Features.Auths.Dtos.Response;
using DevBudy.APPLICATION.Features.Auths.Queries;
using DevBudy.APPLICATION.Features.Chats.Commands;
using DevBudy.DOMAIN.Entities.Concretes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DevBudy.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMediator _mediatR;
        public ChatHub(IMediator mediator)
        {
            _mediatR = mediator;
        }

        public async Task SendMessage(string message)
        {
            string senderName = Context.User?.Identity?.Name
                ?? throw new HubException("Authenticated user name is missing.");

            await _mediatR.Send(new CreateChatMessageCommand
            {
                SenderUserName = senderName,
                Message = message
            });
        }

        public async Task Join()
        {
            string joinedName = Context.User?.Identity?.Name
                ?? throw new HubException("Authenticated user name is missing.");

            await _mediatR.Send(new CreateSystemMessageCommand
            {
                JoinedUserName = joinedName
            });
        }

        public async Task GetOnlineUsers()
        {
            List<ConnectedUserDto> onlineUsers = await _mediatR.Send(new OnlineUsersQuery());
            await Clients.All.SendAsync("ReceiveOnlineUsers", onlineUsers);
        }

        public override async Task OnConnectedAsync()
        {
            await _mediatR.Send(new SetUserOnlineStatusCommand
            {
                UserId = Convert.ToInt32(Context.UserIdentifier),
                LastLogin = DateTime.Now,
                IsOnline = true
            });
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _mediatR.Send(new SetUserOnlineStatusCommand
            {
                UserId = Convert.ToInt32(Context.UserIdentifier),
                LastLogout = DateTime.Now,
                IsOnline = false
            });
            await base.OnDisconnectedAsync(exception);
        }
    }
}
