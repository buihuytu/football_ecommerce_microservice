using FluentValidation.Results;

namespace Ordering.Application.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public Dictionary<string, string[]> Errors { get; }
        public ValidationException() : base("One or more validation error(s) occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failure) : this()
        {
            Errors = failure
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }
    }
}
