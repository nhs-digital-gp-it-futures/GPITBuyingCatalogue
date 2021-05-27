using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard
{
    public class OrganisationModel : OrderingBaseModel
    {
        private readonly IList<EntityFramework.Models.Ordering.Order> allOrders;

        public OrganisationModel(Organisation organisation, ClaimsPrincipal user, IList<EntityFramework.Models.Ordering.Order> allOrders)
        {
            organisation.ValidateNotNull(nameof(organisation));

            BackLinkText = "Go back to homepage";
            BackLink = "/";
            Title = organisation.Name;
            OrganisationName = organisation.Name;
            OdsCode = organisation.OdsCode;
            CanActOnBehalf = user.GetSecondaryOdsCodes().Any();
            this.allOrders = allOrders;
        }

        public string OrganisationName { get; set; }

        public bool CanActOnBehalf { get; set; }

        public IList<EntityFramework.Models.Ordering.Order> InCompleteOrders
        {
            get { return allOrders.Where(x => !x.IsDeleted && x.OrderStatus.Name == "Incomplete").ToList();  }
        }

        public IList<EntityFramework.Models.Ordering.Order> CompleteOrders
        {
            get { return allOrders.Where(x => !x.IsDeleted && x.OrderStatus.Name == "Complete").ToList(); }
        }
    }
}
