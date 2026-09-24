using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealtimeChat.Application.Features.Chats.Commands.Validators
{
    public sealed class CreateChatMessageCommandValidator : AbstractValidator<CreateChatMessageCommand>
    {
        public CreateChatMessageCommandValidator()
        {
            RuleFor(command => command.Message)
                .NotEmpty()
                .WithMessage("Message is required.")
                .MaximumLength(1000)
                .WithMessage("Message cannot exceed 1000 characters.");

            RuleFor(command => command.SenderUserName)
                .NotEmpty()
                .WithMessage("Authenticated username is required.")
                .MaximumLength(256)
                .WithMessage("Username cannot exceed 256 characters.");
        }
    }
}
