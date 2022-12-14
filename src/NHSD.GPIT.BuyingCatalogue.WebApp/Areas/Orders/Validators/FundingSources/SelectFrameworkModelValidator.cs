using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.FundingSources
{
    public sealed class SelectFrameworkModelValidator : AbstractValidator<SelectFrameworkModel>
    {
        public const string SelectFrameworkErrorMessage = "Select a framework";

        public SelectFrameworkModelValidator()
        {
            RuleFor(sfm => sfm.SelectedFramework)
                .NotEmpty()
                .WithMessage(SelectFrameworkErrorMessage);
        }
    }
}
