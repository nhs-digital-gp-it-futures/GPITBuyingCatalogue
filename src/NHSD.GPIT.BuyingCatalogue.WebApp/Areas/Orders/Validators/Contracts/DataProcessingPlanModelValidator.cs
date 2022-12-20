using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DataProcessing;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts;

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
