using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels
{
    public class DetailsModel
    {
        public AspNetUser User { get; set; }

        public string UserRole { get; set; }

        public List<EntityFramework.Ordering.Models.Order> Orders { get; set; }

        public AccountStatus AccountStatus => User.Disabled
            ? AccountStatus.Inactive
            : AccountStatus.Active;
    }
}
