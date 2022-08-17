using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.EmailDomainManagement;

public class DeleteEmailDomainModel : NavBaseModel
{
    public DeleteEmailDomainModel()
    {
    }

    public DeleteEmailDomainModel(EmailDomain emailDomain)
    {
        EmailDomain = emailDomain.Domain;
    }

    public string EmailDomain { get; set; }

    public string AdviceText => EmailDomain.Contains(EmailConstants.WildcardCharacter)
        ? $"The email domain {EmailDomain} and any subdomains related to it will be deleted from the allow list."
        : $"The email domain {EmailDomain} will be deleted from the allow list.";
}
