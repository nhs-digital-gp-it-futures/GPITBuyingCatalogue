using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace BuyingCatalogueFunction.Notifications.PasswordExpiry.Interfaces;

public interface IPasswordExpiryService
{
    Task<ICollection<AspNetUser>> GetUsersNearingPasswordExpiry(DateTime today);

    Task Raise(DateTime date, AspNetUser user, EventTypeEnum eventType);
}
