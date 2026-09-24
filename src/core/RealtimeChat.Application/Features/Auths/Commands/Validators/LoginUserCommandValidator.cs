using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealtimeChat.Application.Features.Auths.Commands.Validators
{
    public sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(command => command.UserName)
                .NotEmpty()
                .WithMessage("Username is required.")
                .MaximumLength(256)
                .WithMessage("Username cannot exceed 256 characters.");

            RuleFor(command => command.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MaximumLength(256)
                .WithMessage("Password cannot exceed 256 characters.");
        }
    }
}
