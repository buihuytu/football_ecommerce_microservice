using FluentValidation;
using MediatR;

namespace Ordering.Application.Behaviour
{
    // This will collect fluent validation and run before the request is handled
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if(_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                // This will run all the validation rules one by one and returns the validation results
                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken))
                );
                // now need to check for any failture
                var failtures = validationResults.SelectMany(e => e.Errors)
                                                  .Where(f => f != null)
                                                  .ToList();
                if (failtures.Count() != 0)
                {
                    throw new ValidationException(failtures);
                }
            }
            // On success, call the next delegate in the pipeline
            return await next();
        }
    }
}
