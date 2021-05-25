using System.Linq;
using System.Security.Claims;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard
{
    public class OrganisationModel : OrderingBaseModel
    {
        public OrganisationModel(Organisation organisation, ClaimsPrincipal user)
        {
            organisation.ValidateNotNull(nameof(organisation));

            BackLinkText = "Go back to homepage";
            BackLink = "/";
            Title = organisation.Name;
            OrganisationName = organisation.Name;
            OdsCode = organisation.OdsCode;
            CanActOnBehalf = user.GetSecondaryOdsCodes().Any();
        }

        public string OrganisationName { get; set; }

        public string OdsCode { get; set; }

        public bool CanActOnBehalf { get; set; }
    }
}
