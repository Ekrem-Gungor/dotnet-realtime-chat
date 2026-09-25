using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RealtimeChat.Application.Common.Exceptions;
using RealtimeChat.Application.Events.ChatMessages;
using RealtimeChat.Application.Features.Chats.Commands;
using RealtimeChat.Application.Features.Chats.Dtos;
using RealtimeChat.Contracts.Repositories.RedisRepositories;
using RealtimeChat.Domain.CachingModels;
using RealtimeChat.Domain.Entities.Concretes;
using RealtimeChat.Persistence.ContextClasses;
using RealtimeChat.Tests.Infrastructure;

namespace RealtimeChat.Tests.Application;

public sealed class CreateChatMessageCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenQuotaIsUnavailable_ThrowsWithoutPersistingMessage()
    {
        await using ServiceProvider provider =
            IdentityTestServices.CreateProvider();

        await provider
            .GetRequiredService<RealtimeChatDbContext>()
            .Database
            .EnsureCreatedAsync();

        UserManager<AppUser> userManager =
            provider.GetRequiredService<UserManager<AppUser>>();

        AppUser user = await CreateUserAsync(userManager);
        RecordingPublisher publisher = new();
        StubMessageRepository messageRepository = new();
        StubQuotaRepository quotaRepository = new(canConsume: false);

        CreateChatMessageCommandHandler handler = new(
            publisher,
            userManager,
            messageRepository,
            quotaRepository);

        await Assert.ThrowsAsync<MessageQuotaExceededException>(() =>
            handler.Handle(
                new CreateChatMessageCommand
                {
                    SenderUserName = user.UserName!,
                    Message = "Quota test"
                },
                CancellationToken.None));

        Assert.Equal(user.Id.ToString(), quotaRepository.ReceivedUserId);
        Assert.Null(messageRepository.AddedMessage);
        Assert.Null(publisher.Notification);
    }

    [Fact]
    public async Task Handle_WhenQuotaIsAvailable_ReturnsAndPublishesMessage()
    {
        await using ServiceProvider provider =
            IdentityTestServices.CreateProvider();

        await provider
            .GetRequiredService<RealtimeChatDbContext>()
            .Database
            .EnsureCreatedAsync();

        UserManager<AppUser> userManager =
            provider.GetRequiredService<UserManager<AppUser>>();

        AppUser user = await CreateUserAsync(userManager);
        RecordingPublisher publisher = new();
        StubMessageRepository messageRepository = new();
        StubQuotaRepository quotaRepository = new(canConsume: true);

        CreateChatMessageCommandHandler handler = new(
            publisher,
            userManager,
            messageRepository,
            quotaRepository);

        ChatMessageDto response = await handler.Handle(
            new CreateChatMessageCommand
            {
                SenderUserName = user.UserName!,
                Message = "Successful message"
            },
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(user.Id, response.SenderUserId);
        Assert.Equal(user.UserName, response.SenderUserName);
        Assert.Equal("Successful message", response.Message);

        Assert.NotNull(messageRepository.AddedMessage);
        Assert.Equal(response.Id, messageRepository.AddedMessage.MessageId);
        Assert.Equal(response.SenderUserId, messageRepository.AddedMessage.SenderUserId);
        Assert.Equal(response.SenderUserName, messageRepository.AddedMessage.SenderUserName);
        Assert.Equal(response.Message, messageRepository.AddedMessage.Message);
        Assert.Equal(response.SendAt, messageRepository.AddedMessage.CreatedAt);

        ChatMessageCreatedEvent notification =
            Assert.IsType<ChatMessageCreatedEvent>(publisher.Notification);

        Assert.Same(response, notification.ChatMessage);
    }

    private static async Task<AppUser> CreateUserAsync(
        UserManager<AppUser> userManager)
    {
        AppUser user = new()
        {
            UserName = "chat.user",
            Email = "chat.user@example.invalid",
            EmailConfirmed = true
        };

        IdentityResult result = await userManager.CreateAsync(user);

        Assert.True(
            result.Succeeded,
            string.Join(", ", result.Errors.Select(error => error.Description)));

        return user;
    }

    private sealed class StubQuotaRepository : IMessageQuotaRepository
    {
        private readonly bool _canConsume;

        public StubQuotaRepository(bool canConsume)
        {
            _canConsume = canConsume;
        }

        public string? ReceivedUserId { get; private set; }

        public Task<bool> TryConsumeAsync(string userId)
        {
            ReceivedUserId = userId;
            return Task.FromResult(_canConsume);
        }
    }

    private sealed class StubMessageRepository : IMessageRedisRepository
    {
        public RedisChatMessage? AddedMessage { get; private set; }

        public Task AddMessageAsync(RedisChatMessage message)
        {
            AddedMessage = message;
            return Task.CompletedTask;
        }

        public Task<List<RedisChatMessage>> GetMessagesFromLastMinutesAsync(
            int minutes)
        {
            return Task.FromResult(new List<RedisChatMessage>());
        }
    }

    private sealed class RecordingPublisher : IPublisher
    {
        public object? Notification { get; private set; }

        public Task Publish(
            object notification,
            CancellationToken cancellationToken = default)
        {
            Notification = notification;
            return Task.CompletedTask;
        }

        public Task Publish<TNotification>(
            TNotification notification,
            CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            Notification = notification;
            return Task.CompletedTask;
        }
    }
}