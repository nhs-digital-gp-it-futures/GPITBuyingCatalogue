using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSources;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.FundingSources
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
