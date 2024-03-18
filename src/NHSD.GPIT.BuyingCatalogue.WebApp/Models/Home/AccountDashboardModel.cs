using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Home
{
    public class AccountDashboardModel : NavBaseModel
    {
        public string OrganisationName { get; set; }

        public string InternalOrgId { get; set; }

        public ICollection<Order> Orders { get; set; } = Enumerable.Empty<Order>().ToList();

        public ICollection<Competition> Competitions { get; set; } = Enumerable.Empty<Competition>().ToList();

        public ICollection<Filter> Shortlists { get; init; } = Enumerable.Empty<Filter>().ToList();
    }
}
