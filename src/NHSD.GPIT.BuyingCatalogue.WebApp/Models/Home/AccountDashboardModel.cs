using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Home
{
    public class AccountDashboardModel : NavBaseModel
    {
        private const string Zero = "0";

        public string OrganisationName { get; set; }

        public string InternalOrgId { get; set; }

        public IList<Order> Orders { get; set; }

        public bool HasOrders => Orders is { Count: > 0 };

        public string OrdersCount => HasOrders ? Orders.Count.ToString() : Zero;

        public IList<CompetitionDashboardItem> Competitions { get; set; }

        public bool HasCompetitions => Competitions is { Count: > 0 };

        public string CompetitionsCount => HasCompetitions ? Competitions.Count.ToString() : Zero;

        public IList<Filter> Shortlists { get; init; }

        public bool HasShortlists => Shortlists is { Count: > 0 };

        public string ShortlistsCount => HasShortlists ? Shortlists.Count.ToString() : Zero;
    }
}
