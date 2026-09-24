using FluentValidation;
using MediatR;
using RealtimeChat.Application.Common.Behaviors;
using RealtimeChat.Application.Features.Auths.Commands;
using RealtimeChat.Application.Features.Auths.Commands.Validators;
using RealtimeChat.Application.Features.Auths.Dtos.Response;

namespace RealtimeChat.Tests.Application;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_WithInvalidRequest_ThrowsBeforeCallingNext()
    {
        IValidator<LoginUserCommand>[] validators =
        [
            new LoginUserCommandValidator()
        ];

        ValidationBehavior<LoginUserCommand, LoginResponseDto> behavior =
            new(validators);

        bool nextWasCalled = false;

        RequestHandlerDelegate<LoginResponseDto> next = _ =>
        {
            nextWasCalled = true;
            return Task.FromResult(new LoginResponseDto());
        };

        ValidationException exception =
            await Assert.ThrowsAsync<ValidationException>(() =>
                behavior.Handle(
                    new LoginUserCommand(),
                    next,
                    CancellationToken.None));

        Assert.False(nextWasCalled);
        Assert.Contains(
            exception.Errors,
            error => error.PropertyName == nameof(LoginUserCommand.UserName));
        Assert.Contains(
            exception.Errors,
            error => error.PropertyName == nameof(LoginUserCommand.Password));
    }

    [Fact]
    public async Task Handle_WithValidRequest_CallsNext()
    {
        IValidator<LoginUserCommand>[] validators =
        [
            new LoginUserCommandValidator()
        ];

        ValidationBehavior<LoginUserCommand, LoginResponseDto> behavior =
            new(validators);

        LoginResponseDto expected = new() { UserId = 42 };
        bool nextWasCalled = false;

        RequestHandlerDelegate<LoginResponseDto> next = _ =>
        {
            nextWasCalled = true;
            return Task.FromResult(expected);
        };

        LoginResponseDto response = await behavior.Handle(
            new LoginUserCommand
            {
                UserName = "test.user",
                Password = "AnyPassword"
            },
            next,
            CancellationToken.None);

        Assert.True(nextWasCalled);
        Assert.Same(expected, response);
    }
}