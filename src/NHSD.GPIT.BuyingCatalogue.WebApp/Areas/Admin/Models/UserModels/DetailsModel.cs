using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels
{
    public class DetailsModel
    {
        private string userRole;

        public AspNetUser User { get; set; }

        public string UserRole
        {
            get
            {
                var role = OrganisationFunction.FromName(userRole);
                return role != null ? role.DisplayName : string.Empty;
            }

            set
            {
                userRole = value;
            }
        }

        public List<EntityFramework.Ordering.Models.Order> Orders { get; set; }

        public AccountStatus AccountStatus => User.Disabled
            ? AccountStatus.Inactive
            : AccountStatus.Active;
    }
}
