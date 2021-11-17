using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public class EditSupplierDetailsModelValidator : AbstractValidator<EditSupplierDetailsModel>
    {
        public EditSupplierDetailsModelValidator(
            IUrlValidator urlValidator)
        {
            RuleFor(m => m.SupplierWebsite)
                .IsValidUrl(urlValidator)
                .Unless(m => string.IsNullOrWhiteSpace(m.SupplierWebsite));
        }
    }
}
