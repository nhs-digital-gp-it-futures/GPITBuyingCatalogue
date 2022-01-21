using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Organisation
{
    public sealed class AddUserModelValidator : AbstractValidator<AddUserModel>
    {
        public AddUserModelValidator()
        {
            RuleFor(m => m.FirstName)
                .NotEmpty()
                .WithMessage("First Name Required");

            RuleFor(m => m.LastName)
                .NotEmpty()
                .WithMessage("Last Name Required");

            RuleFor(m => m.TelephoneNumber)
                .NotEmpty()
                .WithMessage("Telephone Number Required");

            RuleFor(m => m.EmailAddress)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Email Address Required")
                .EmailAddress();
        }
    }
}
