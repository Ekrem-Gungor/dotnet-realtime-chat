using FluentValidation.Results;
using RealtimeChat.Application.Features.Chats.Commands;
using RealtimeChat.Application.Features.Chats.Commands.Validators;

namespace RealtimeChat.Tests.Application;

public sealed class CreateChatMessageCommandValidatorTests
{
    private readonly CreateChatMessageCommandValidator _validator = new();

    [Fact]
    public async Task Validate_WithWhitespaceMessage_ReturnsRequiredError()
    {
        CreateChatMessageCommand command = new()
        {
            SenderUserName = "chat.user",
            Message = " "
        };

        ValidationResult result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error =>
                error.PropertyName == nameof(CreateChatMessageCommand.Message)
                && error.ErrorMessage == "Message is required.");
    }

    [Fact]
    public async Task Validate_WithMessageAboveMaximumLength_ReturnsLengthError()
    {
        CreateChatMessageCommand command = new()
        {
            SenderUserName = "chat.user",
            Message = new string('a', 1001)
        };

        ValidationResult result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error =>
                error.PropertyName == nameof(CreateChatMessageCommand.Message)
                && error.ErrorMessage == "Message cannot exceed 1000 characters.");
    }

    [Fact]
    public async Task Validate_WithMessageAtMaximumLength_IsValid()
    {
        CreateChatMessageCommand command = new()
        {
            SenderUserName = "chat.user",
            Message = new string('a', 1000)
        };

        ValidationResult result = await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }
}