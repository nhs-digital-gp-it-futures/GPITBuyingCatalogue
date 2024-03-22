using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public class BuyerDashboardModel : NavBaseModel
    {
        public BuyerDashboardModel(
            Organisation organisation,
            ICollection<Order> orders,
            ICollection<Competition> competitions,
            ICollection<Filter> shortlists)
        {
            OrganisationName = organisation.Name;
            InternalOrgId = organisation.InternalIdentifier;
            Orders = orders;
            Competitions = competitions.Select(
                    x => new CompetitionDashboardItem(x))
                .ToList();
            Shortlists = shortlists;
        }

        public string OrganisationName { get; set; }

        public string InternalOrgId { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<CompetitionDashboardItem> Competitions { get; set; }

        public ICollection<Filter> Shortlists { get; init; }
    }
}
