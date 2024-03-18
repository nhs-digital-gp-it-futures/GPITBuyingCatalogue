using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

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
            Competitions = competitions;
            Shortlists = shortlists;
        }

        public string OrganisationName { get; set; }

        public string InternalOrgId { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<Competition> Competitions { get; set; }

        public ICollection<Filter> Shortlists { get; init; }
    }
}
