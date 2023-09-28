using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public class NonPriceElementWeightsModelValidator : AbstractValidator<NonPriceElementWeightsModel>
{
    internal const string ImplementationNullError = "Enter an implementation weighting";
    internal const string InteroperabilityNullError = "Enter an interoperability weighting";
    internal const string ServiceLevelNullError = "Enter a service level weighting";
    internal const string FeaturesNullError = "Enter a features weighting";

    internal const string ImplementationDivisionError = "Implementation weighting must be in multiples of 5";
    internal const string InteroperabilityDivisionError = "Interoperability weighting must be in multiples of 5";
    internal const string ServiceLevelDivisionError = "Service level weighting must be in multiples of 5";
    internal const string FeaturesDivisionError = "Features weighting must be in multiples of 5";

    internal const string TotalsInvalidError = "Totals entered do not add up to 100% ";

    private const int DivisionFactor = 5;

    public NonPriceElementWeightsModelValidator()
    {
        RuleFor(x => x.Implementation)
            .NotNull()
            .WithMessage(ImplementationNullError)
            .MustBeDivisibleBy(DivisionFactor)
            .WithMessage(ImplementationDivisionError)
            .Must(AddUpToFinalValue)
            .WithMessage(TotalsInvalidError)
            .When(m => m.HasImplementation);

        RuleFor(x => x.Interoperability)
            .NotNull()
            .WithMessage(InteroperabilityNullError)
            .MustBeDivisibleBy(DivisionFactor)
            .WithMessage(InteroperabilityDivisionError)
            .Must(AddUpToFinalValue)
            .WithMessage(TotalsInvalidError)
            .When(m => m.HasInteroperability);

        RuleFor(x => x.ServiceLevel)
            .NotNull()
            .WithMessage(ServiceLevelNullError)
            .MustBeDivisibleBy(DivisionFactor)
            .WithMessage(ServiceLevelDivisionError)
            .Must(AddUpToFinalValue)
            .WithMessage(TotalsInvalidError)
            .When(m => m.HasServiceLevel);

        RuleFor(x => x.Features)
            .NotNull()
            .WithMessage(FeaturesNullError)
            .MustBeDivisibleBy(DivisionFactor)
            .WithMessage(FeaturesDivisionError)
            .Must(AddUpToFinalValue)
            .WithMessage(TotalsInvalidError)
            .When(m => m.HasFeatures);
    }

    private bool AddUpToFinalValue(NonPriceElementWeightsModel model, int? value)
    {
        _ = value;

        return (model.Interoperability.GetValueOrDefault()
            + model.ServiceLevel.GetValueOrDefault()
            + model.Implementation.GetValueOrDefault()
            + model.Features.GetValueOrDefault()) == 100;
    }
}
