using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealtimeChat.Application.Common.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly IValidator<TRequest>[] _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators.ToArray();
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Length == 0)
                return await next(cancellationToken);

            ValidationContext<TRequest> context = new(request);

            ValidationResult[] results = await Task.WhenAll(_validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

            ValidationFailure[] failures = results.SelectMany(result => result.Errors).ToArray();

            if (failures.Length > 0)
                throw new ValidationException(failures);

            return await next(cancellationToken);
        }
    }
}
