using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.DataProcessing;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Contracts;

public class DataProcessingPlanModelValidator : AbstractValidator<DataProcessingPlanModel>
{
    public const string DefaultDataProcessingNullErrorMessage =
        "Select yes if you want to proceed using the supplier’s default data processing information";

    public DataProcessingPlanModelValidator()
    {
        RuleFor(m => m.UseDefaultDataProcessing)
            .NotNull()
            .WithMessage(DefaultDataProcessingNullErrorMessage);
    }
}
