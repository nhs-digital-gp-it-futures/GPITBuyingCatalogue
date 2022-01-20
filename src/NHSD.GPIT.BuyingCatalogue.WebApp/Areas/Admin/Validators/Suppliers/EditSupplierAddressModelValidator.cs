using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Suppliers
{
    public sealed class EditSupplierAddressModelValidator : AbstractValidator<EditSupplierAddressModel>
    {
        public EditSupplierAddressModelValidator()
        {
            RuleFor(m => m.AddressLine1)
                .NotEmpty()
                .WithMessage("Enter a building or street");

            RuleFor(m => m.Town)
                .NotEmpty()
                .WithMessage("Enter a town or city");

            RuleFor(m => m.PostCode)
                .NotEmpty()
                .WithMessage("Enter a postcode");

            RuleFor(m => m.Country)
                .NotEmpty()
                .WithMessage("Enter a country");
        }
    }
}
