using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderingParty;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators
{
    public class OrderingPartyModelValidator : AbstractValidator<OrderingPartyModel>
    {
        public const string FirstNameErrorMessage = "Enter a first name";
        public const string LastNameErrorMessage = "Enter a last name";
        public const string PhoneNumberErrorMessage = "Enter a telephone number";
        public const string NoEmailAddressErrorMessage = "Enter an email address";
        public const string InvalidEmailAddressFormatErrorMessage = "Enter an email address in the correct format, like name@example.com";

        public OrderingPartyModelValidator()
        {
            RuleFor(m => m.Contact.FirstName)
                .NotEmpty()
                .WithMessage(FirstNameErrorMessage);

            RuleFor(m => m.Contact.LastName)
                .NotEmpty()
                .WithMessage(LastNameErrorMessage);

            RuleFor(m => m.Contact.TelephoneNumber)
                .NotEmpty()
                .WithMessage(PhoneNumberErrorMessage);

            RuleFor(m => m.Contact.EmailAddress)
                .NotEmpty()
                .WithMessage(NoEmailAddressErrorMessage)
                .EmailAddress()
                .WithMessage(InvalidEmailAddressFormatErrorMessage);
        }
    }
}
