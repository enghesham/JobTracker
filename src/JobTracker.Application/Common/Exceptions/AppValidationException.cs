using FluentValidation.Results;

namespace JobTracker.Application.Common.Exceptions;

public sealed class AppValidationException : Exception
{
    public AppValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures occurred.")
    {
        Errors = failures
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).Distinct().ToArray());
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
