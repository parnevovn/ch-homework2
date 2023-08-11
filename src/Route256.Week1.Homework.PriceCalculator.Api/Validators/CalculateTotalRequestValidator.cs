using FluentValidation;
using Route256.Week1.Homework.PriceCalculator.Api.Requests.V1;

namespace Route256.Week1.Homework.PriceCalculator.Api.Validators;

internal sealed class CalculateTotalRequestValidator : AbstractValidator<CalculateTotalRequest>
{
    public CalculateTotalRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .LessThan(Int32.MaxValue);

        RuleFor(x => x.Distance)
            .GreaterThan(0)
            .LessThan(Int32.MaxValue);
    }
}