using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Hosting
{
    public sealed class PublicCloudModelValidator : AbstractValidator<PublicCloudModel>
    {
        public PublicCloudModelValidator(
            IUrlValidator validator)
        {
            Include(new BaseCloudModelValidator(validator));
        }
    }
}
