using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.EmailDomainManagement;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.EmailDomainManagement;

public class AddEmailDomainModelValidator : AbstractValidator<AddEmailDomainModel>
{
    public const string EmailDomainInvalid = "Enter an email domain in a valid format, for example, @nhs.net";
    public const string DuplicateEmailDomain = "This email domain has already been added to the allow list";
    public const string TooManyWildcards = "Only one level of subdomain can be added. Remove any extra asterisks (*)";

    private static readonly Regex EmailRegex =
        new Regex("^^@((\\*\\.)+)?([A-Za-z0-9.])+$", RegexOptions.Compiled);

    private readonly IEmailDomainService emailDomainService;

    public AddEmailDomainModelValidator(IEmailDomainService emailDomainService)
    {
        this.emailDomainService = emailDomainService;

        RuleFor(m => m.EmailDomain)
            .NotEmpty()
            .WithMessage(EmailDomainInvalid)
            .Must(m => m.StartsWith(EmailConstants.AddressCharacter))
            .WithMessage(EmailDomainInvalid)
            .Must(HaveOneWildcard)
            .WithMessage(TooManyWildcards)
            .Matches(EmailRegex)
            .WithMessage(EmailDomainInvalid)
            .Must(NotBeADuplicate)
            .WithMessage(DuplicateEmailDomain);
    }

    private static bool HaveOneWildcard(string emailDomain)
    {
        if (!emailDomain.Contains(EmailConstants.WildcardCharacter))
            return true;

        return emailDomain.Count(c => c == EmailConstants.WildcardCharacter) == 1;
    }

    private bool NotBeADuplicate(string emailDomain)
    {
        return !emailDomainService.Exists(emailDomain).GetAwaiter().GetResult();
    }
}
