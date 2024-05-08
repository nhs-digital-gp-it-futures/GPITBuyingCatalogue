using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators
{
    public class SelectFrameworkModelValidator : AbstractValidator<SelectFrameworkModel>
    {
        internal const string FrameworkRequiredErrorMessage = "Select a framework";

        public SelectFrameworkModelValidator()
        {
            RuleFor(m => m.SelectedFrameworkId)
                .NotNull()
                .WithMessage(FrameworkRequiredErrorMessage);
        }
    }
}
