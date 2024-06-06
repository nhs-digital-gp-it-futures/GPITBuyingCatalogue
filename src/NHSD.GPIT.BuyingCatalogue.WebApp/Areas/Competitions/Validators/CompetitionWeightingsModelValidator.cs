using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public class CompetitionWeightingsModelValidator : AbstractValidator<CompetitionWeightingsModel>
{
    internal const string PriceWeightingNullError = "Enter a price weighting";
    internal const string PriceWeightingRangeError = "Price weighting must be between 30% and 90%";
    internal const string PriceWeightingMultiplesError = "Price weighting must be in multiples of 5";

    internal const string NonPriceWeightingNullError = "Enter a non-price weighting";
    internal const string NonPriceWeightingRangeError = "Non-price weighting must be between 10% and 70%";
    internal const string NonPriceWeightingMultiplesError = "Non-price weighting must be in multiples of 5";

    internal const string TotalsInvalidError = "Totals entered do not add up to 100%";

    [ExcludeFromCodeCoverage(Justification = "CodeCov repeatedly identifying a drop in code coverage on Price/NonPrice totals validation")]
    public CompetitionWeightingsModelValidator()
    {
        RuleFor(x => x.Price)
            .NotNull()
            .WithMessage(PriceWeightingNullError)
            .InclusiveBetween(30, 90)
            .WithMessage(PriceWeightingRangeError)
            .MustBeDivisibleBy(5)
            .WithMessage(PriceWeightingMultiplesError)
            .Must((model, price) => price + model.NonPrice == 100)
            .WithMessage(TotalsInvalidError);

        RuleFor(x => x.NonPrice)
            .NotNull()
            .WithMessage(NonPriceWeightingNullError)
            .InclusiveBetween(10, 70)
            .WithMessage(NonPriceWeightingRangeError)
            .MustBeDivisibleBy(5)
            .WithMessage(NonPriceWeightingMultiplesError)
            .Must((model, nonPrice) => model.Price + nonPrice == 100)
            .WithMessage(TotalsInvalidError);
    }
}
