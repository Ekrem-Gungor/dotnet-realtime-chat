using RealtimeChat.Application.Features.AppUsers.Commands;
using RealtimeChat.Application.Features.Auths.Dtos.Response;
using RealtimeChat.Application.Features.Auths.Queries;
using RealtimeChat.Application.Features.Chats.Commands;
using RealtimeChat.Domain.Entities.Concretes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RealtimeChat.Api.Hubs.Presence;

namespace RealtimeChat.Api.Hubs
{
    [Authorize]
    public sealed class ChatHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly IUserConnectionTracker _connectionTracker;

        public ChatHub(IMediator mediator, IUserConnectionTracker connectionTracker)
        {
            _mediator = mediator;
            _connectionTracker = connectionTracker;
        }

        public async Task SendMessage(string message)
        {
            string senderName = Context.User?.Identity?.Name ?? throw new HubException("Authenticated user name is missing.");

            await _mediator.Send(new CreateChatMessageCommand
            {
                SenderUserName = senderName,
                Message = message
            },
                Context.ConnectionAborted);
        }

        public async Task Join()
        {
            string joinedName = Context.User?.Identity?.Name ?? throw new HubException("Authenticated user name is missing.");

            await _mediator.Send(new CreateSystemMessageCommand
            {
                JoinedUserName = joinedName
            },
                Context.ConnectionAborted);
        }

        public Task GetOnlineUsers()
        {
            return BroadcastOnlineUsersAsync(Context.ConnectionAborted);
        }

        public override async Task OnConnectedAsync()
        {
            int userId = GetAuthenticatedUserId();
            string connectionId = Context.ConnectionId;

            bool isFirstConnection = _connectionTracker.RegisterConnection(userId, connectionId);

            if (isFirstConnection)
            {
                try
                {
                    await _mediator.Send(new SetUserOnlineStatusCommand
                    {
                        UserId = userId,
                        LastLogin = DateTime.UtcNow,
                        IsOnline = true
                    }, Context.ConnectionAborted);
                }
                catch
                {
                    _connectionTracker.UnregisterConnection(userId, connectionId);
                    throw;
                }
            }

            await base.OnConnectedAsync();

            if (isFirstConnection)
            {
                await BroadcastOnlineUsersAsync(Context.ConnectionAborted);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            int userId = GetAuthenticatedUserId();
            string connectionId = Context.ConnectionId;

            bool isLastConnection = _connectionTracker.UnregisterConnection(userId, connectionId);

            try
            {
                if (isLastConnection)
                {
                    await _mediator.Send(new SetUserOnlineStatusCommand
                    {
                        UserId = userId,
                        LastLogout = DateTime.UtcNow,
                        IsOnline = false
                    }, CancellationToken.None);

                    await BroadcastOnlineUsersAsync(CancellationToken.None);
                }
            }
            finally
            {
                await base.OnDisconnectedAsync(exception);
            }
        }

        private int GetAuthenticatedUserId()
        {
            if (int.TryParse(Context.UserIdentifier, out int userId))
            {
                return userId;
            }

            throw new HubException("Authenticated user identifier is missing.");
        }

        private async Task BroadcastOnlineUsersAsync(CancellationToken cancellationToken)
        {
            List<ConnectedUserDto> onlineUsers = await _mediator.Send(new OnlineUsersQuery(), cancellationToken);

            await Clients.All.SendAsync(
                ChatHubEvent.ReceiveOnlineUsers,
                onlineUsers,
                cancellationToken);
        }
    }
}
