using FluentValidation;
using MediatR;
using RBC.BrokeragePlatform.Application.Interfaces;

namespace RBC.BrokeragePlatform.Application
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, ICommand<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            foreach (var validator in _validators)
            {
                var validationResult = await validator.ValidateAsync(context, cancellationToken);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Where(e => e != null).ToList();
                    if (errors.Any())
                    {
                        throw new ValidationException("Validation failed:", errors);
                    }
                }
            }   

            return await next();
        }
    }
}
