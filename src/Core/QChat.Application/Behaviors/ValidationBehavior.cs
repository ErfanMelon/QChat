using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace QChat.Application.Behaviors;

/// <summary>
/// If validation for Command/Query fail this pipeline throws a ValidationException
/// </summary>
/// <typeparam name="TRequest">Request</typeparam>
/// <typeparam name="TResponse">Response</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
                throw new CustomValidationException(failures);
        }
        return await next();
    }
}
public class CustomValidationException : Exception
{
    public IEnumerable<ValidationFailure> Errors { get; private set; }
    public CustomValidationException(string? message) : base(message) { }
    public CustomValidationException(IEnumerable<ValidationFailure> failures) : base(BuildErrorMessage(failures))
    {
        Errors = failures;
    }
    private static string BuildErrorMessage(IEnumerable<ValidationFailure> failures)
    {
        var AllErrors = Result.Combine(failures.Select(f => Result.Failure(f.ErrorMessage)));
        return AllErrors.Error;
    }
}