using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Hosting
{
    public class BaseCloudModelValidator : AbstractValidator<BaseCloudModel>
    {
        public BaseCloudModelValidator(
            IUrlValidator urlValidator)
        {
            RuleFor(m => m.Link)
                .IsValidUrl(urlValidator)
                .Unless(m => string.IsNullOrWhiteSpace(m.Link));
        }
    }
}
