using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Hosting
{
    public sealed class PrivateCloudModelValidator : AbstractValidator<PrivateCloudModel>
    {
        public PrivateCloudModelValidator(
            IUrlValidator validator)
        {
            Include(new BaseCloudModelValidator(validator));

            RuleFor(m => m.HostingModel)
                .NotEmpty()
                .WithMessage("Enter data centre model information");
        }
    }
}
