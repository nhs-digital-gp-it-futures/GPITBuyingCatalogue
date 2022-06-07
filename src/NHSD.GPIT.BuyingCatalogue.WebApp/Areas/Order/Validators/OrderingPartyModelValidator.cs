using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderingParty;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators
{
    public class OrderingPartyModelValidator : AbstractValidator<OrderingPartyModel>
    {
        public OrderingPartyModelValidator()
        {
            RuleFor(m => m.Contact.FirstName)
                .NotEmpty()
                .WithMessage("Enter a first name");

            RuleFor(m => m.Contact.LastName)
                .NotEmpty()
                .WithMessage("Enter a last name");

            RuleFor(m => m.Contact.TelephoneNumber)
                .NotEmpty()
                .WithMessage("Enter a telephone number");

            RuleFor(m => m.Contact.EmailAddress)
                .NotEmpty()
                .WithMessage("Enter an email address")
                .EmailAddress()
                .WithMessage("Enter an email address in the correct format, like name@example.com");
        }
    }
}
