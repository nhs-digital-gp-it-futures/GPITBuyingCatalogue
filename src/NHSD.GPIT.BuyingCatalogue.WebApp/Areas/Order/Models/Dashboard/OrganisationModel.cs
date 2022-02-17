using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard
{
    public sealed class OrganisationModel : OrderingBaseModel
    {
        public OrganisationModel(Organisation organisation, ClaimsPrincipal user, IList<EntityFramework.Ordering.Models.Order> orders)
        {
            organisation.ValidateNotNull(nameof(organisation));

            BackLinkText = "Go back to homepage";
            BackLink = "/";
            Title = organisation.Name;
            OrganisationName = organisation.Name;
            OdsCode = organisation.OdsCode;
            CanActOnBehalf = user.GetSecondaryOrganisationInternalIdentifiers().Any();
            Orders = orders ?? new List<EntityFramework.Ordering.Models.Order>();
        }

        public string OrganisationName { get; set; }

        public bool CanActOnBehalf { get; set; }

        public IList<EntityFramework.Ordering.Models.Order> Orders { get; set; }

        public PageOptions Options { get; set; }
    }
}
