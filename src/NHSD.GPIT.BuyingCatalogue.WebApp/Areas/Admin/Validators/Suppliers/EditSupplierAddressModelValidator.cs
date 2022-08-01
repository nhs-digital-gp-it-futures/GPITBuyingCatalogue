using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Suppliers
{
    public sealed class EditSupplierAddressModelValidator : AbstractValidator<EditSupplierAddressModel>
    {
        public const string AddressLine1Error = "Enter a building or street";
        public const string TownOrCityError = "Enter a town or city";
        public const string PostcodeError = "Enter a postcode";
        public const string CountryError = "Enter a postcode";

        public EditSupplierAddressModelValidator()
        {
            RuleFor(m => m.AddressLine1)
                .NotEmpty()
                .WithMessage(AddressLine1Error);

            RuleFor(m => m.Town)
                .NotEmpty()
                .WithMessage(TownOrCityError);

            RuleFor(m => m.PostCode)
                .NotEmpty()
                .WithMessage(PostcodeError);

            RuleFor(m => m.Country)
                .NotEmpty()
                .WithMessage(CountryError);
        }
    }
}
