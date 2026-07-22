using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace BuyingCatalogueFunction.Notifications.InactiveAccount;

public interface IInactiveAccountsService
{
    Task<ICollection<AspNetUser>> GetInactiveAccounts(DateOnly utcToday);

    Task<EmailNotification> Raise(AspNetUser user, DateOnly utcToday, EmailPreferenceType defaultEmailPreference);

    Task DispatchNotification(AspNetUser user, EmailNotification notification);
}
