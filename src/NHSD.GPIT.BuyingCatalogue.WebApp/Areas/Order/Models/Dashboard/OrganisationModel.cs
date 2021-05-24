using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard
{
    public class OrganisationModel : OrderingBaseModel
    {
        public OrganisationModel(Organisation organisation)
        {
            organisation.ValidateNotNull(nameof(organisation));

            BackLinkText = "Go back to homepage";
            BackLink = "/";
            Title = organisation.Name;
            OrganisationName = organisation.Name;
            OdsCode = organisation.OdsCode;
        }

        public string OrganisationName { get; set; }

        public string OdsCode { get; set; }
    }
}
