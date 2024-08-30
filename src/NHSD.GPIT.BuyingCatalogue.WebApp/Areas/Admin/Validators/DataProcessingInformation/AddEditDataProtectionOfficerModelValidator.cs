using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.DataProcessingInformation;

public sealed class AddEditDataProtectionOfficerModelValidator : AbstractValidator<AddEditDataProtectionOfficerModel>
{
    internal const string NameError = "Enter the name of your Data Protection Officer";

    internal const string EmailAndPhoneEmptyError =
        "Enter a telephone number or email address for your Data Protection Officer";

    internal const string InvalidEmailFormatError =
        "Enter an email address in the correct format, like name@example.com";

    public AddEditDataProtectionOfficerModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(NameError);

        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .WithMessage(EmailAndPhoneEmptyError)
            .When(x => string.IsNullOrWhiteSpace(x.PhoneNumber), ApplyConditionTo.CurrentValidator)
            .EmailAddress()
            .WithMessage(InvalidEmailFormatError)
            .Unless(x => string.IsNullOrWhiteSpace(x.EmailAddress), ApplyConditionTo.CurrentValidator);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage(EmailAndPhoneEmptyError)
            .When(x => string.IsNullOrWhiteSpace(x.EmailAddress));
    }
}
