using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

namespace BuyingCatalogueFunction.Notifications.Interfaces;

public interface IEmailPreferenceService
{
    Task<bool> ShouldTriggerForUser(EmailPreferenceType emailPreferenceType, int userId);

    Task<EmailPreferenceType> GetDefaultEmailPreference(EmailPreferenceTypeEnum eventType);
}
