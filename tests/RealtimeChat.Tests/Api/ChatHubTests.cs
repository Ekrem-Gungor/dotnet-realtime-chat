using MediatR;
using Microsoft.AspNetCore.SignalR;
using Moq;
using RealtimeChat.Api.Hubs;
using RealtimeChat.Application.Features.AppUsers.Commands;
using RealtimeChat.Application.Features.Auths.Dtos.Response;
using RealtimeChat.Application.Features.Auths.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealtimeChat.Tests.Api
{
    public sealed class ChatHubTests
    {
        [Fact]
        public async Task OnConnectedAsync_MarksUserOnlineAndBroadcastsUsers()
        {
            HubFixture fixture = CreateFixture();

            await fixture.Hub.OnConnectedAsync();

            fixture.Mediator.Verify(
                mediator => mediator.Send(
                    It.Is<SetUserOnlineStatusCommand>(
                        command =>
                            command.UserId == 4 &&
                            command.IsOnline &&
                            command.LastLogin.HasValue &&
                            command.LastLogout == null),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            fixture.Mediator.Verify(mediator => mediator.Send(It.IsAny<OnlineUsersQuery>(), It.IsAny<CancellationToken>()), Times.Once);

            VerifyOnlineUsersBroadcast(fixture);
        }

        [Fact]
        public async Task OnDisconnectedAsync_MarksUserOfflineAndBroadcastsUsers()
        {
            HubFixture fixture = CreateFixture();

            await fixture.Hub.OnDisconnectedAsync(new InvalidOperationException("Connection closed."));

            fixture.Mediator.Verify(mediator => 
                mediator.Send(
                    It.Is<SetUserOnlineStatusCommand>(
                        command =>
                            command.UserId == 4 &&
                            !command.IsOnline &&
                            command.LastLogin == null &&
                            command.LastLogout.HasValue),
                    CancellationToken.None), Times.Once);

            fixture.Mediator.Verify(mediator => mediator.Send(It.IsAny<OnlineUsersQuery>(), CancellationToken.None), Times.Once);

            VerifyOnlineUsersBroadcast(fixture);
        }

        private static HubFixture CreateFixture()
        {
            List<ConnectedUserDto> onlineUsers = [new ConnectedUserDto
            {
                Id = 4,
                UserName = "realtime.demo",
                IsOnline = true
            }];

            Mock<IMediator> mediator = new();

            mediator.Setup(instance => instance.Send(It.IsAny<SetUserOnlineStatusCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            mediator.Setup(instance => instance.Send(It.IsAny<OnlineUsersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(onlineUsers);

            Mock<IClientProxy> clientProxy = new();

            Mock<IHubCallerClients> hubClients = new();
            hubClients.SetupGet(clients => clients.All).Returns(clientProxy.Object);

            Mock<HubCallerContext> callerContext = new();
            callerContext.SetupGet(context => context.UserIdentifier).Returns("4");

            callerContext.SetupGet(context => context.ConnectionAborted).Returns(CancellationToken.None);

            ChatHub hub = new(mediator.Object)
            {
                Context = callerContext.Object,
                Clients = hubClients.Object
            };

            return new HubFixture(hub, mediator, clientProxy, onlineUsers);
        }

        private static void VerifyOnlineUsersBroadcast(HubFixture fixture)
        {
            fixture.ClientProxy.Verify(
                proxy => proxy.SendCoreAsync(
                    ChatHubEvent.ReceiveOnlineUsers,
                    It.Is<object?[]>(
                        arguments =>
                            arguments.Length == 1 &&
                            ReferenceEquals(
                                arguments[0],
                                fixture.OnlineUsers)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private sealed record HubFixture(ChatHub Hub, Mock<IMediator> Mediator, Mock<IClientProxy> ClientProxy, List<ConnectedUserDto> OnlineUsers);
    }
}
