using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Environments;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Tags;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Dashboard
{
    public sealed class OrganisationModel : OrderingBaseModel
    {
        public OrganisationModel(Organisation organisation, ClaimsPrincipal user, IList<Order> orders)
        {
            organisation.ValidateNotNull(nameof(organisation));

            BackLinkText = "Go back to homepage";
            BackLink = "/";
            Title = organisation.Name;
            OrganisationName = organisation.Name;
            InternalOrgId = organisation.InternalIdentifier;
            CanActOnBehalf = user.GetSecondaryOrganisationInternalIdentifiers().Any();
            Orders = orders ?? new List<Order>();
        }

        public string OrganisationName { get; set; }

        public bool CanActOnBehalf { get; set; }

        public IEnumerable<CallOffId> OrderIds { get; set; }

        public IList<Order> Orders { get; set; }

        public PageOptions Options { get; set; }

        public string LinkName(Order order)
        {
            return order.OrderStatus switch
            {
                OrderStatus.Completed when !order.AssociatedServicesOnly && !AnySubsequentRevisions(order.CallOffId) => "Amend",
                OrderStatus.Completed => "View",
                OrderStatus.InProgress => "Edit",
                _ => string.Empty,
            };
        }

        public NhsTagsTagHelper.TagColour TagColour(OrderStatus orderStatus) => orderStatus switch
        {
            OrderStatus.Completed => NhsTagsTagHelper.TagColour.Green,
            OrderStatus.InProgress => NhsTagsTagHelper.TagColour.Yellow,
            _ => throw new ArgumentOutOfRangeException(nameof(orderStatus)),
        };

        private bool AnySubsequentRevisions(CallOffId callOffId) => OrderIds.Any(x => x.OrderNumber == callOffId.OrderNumber && x.Revision > callOffId.Revision);
    }
}
