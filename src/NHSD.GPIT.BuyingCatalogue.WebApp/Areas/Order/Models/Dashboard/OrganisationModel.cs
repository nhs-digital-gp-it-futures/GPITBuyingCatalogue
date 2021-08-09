using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard
{
    public sealed class OrganisationModel : OrderingBaseModel
    {
        private readonly IList<EntityFramework.Ordering.Models.Order> allOrders;

        public OrganisationModel(Organisation organisation, ClaimsPrincipal user, IList<EntityFramework.Ordering.Models.Order> allOrders)
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

        public IList<EntityFramework.Ordering.Models.Order> InCompleteOrders
        {
            get { return allOrders.Where(o => !o.IsDeleted && o.OrderStatus == OrderStatus.Incomplete).ToList(); }
        }

        public IList<EntityFramework.Ordering.Models.Order> CompleteOrders
        {
            get { return allOrders.Where(o => !o.IsDeleted && o.OrderStatus == OrderStatus.Complete).ToList(); }
        }
    }
}
