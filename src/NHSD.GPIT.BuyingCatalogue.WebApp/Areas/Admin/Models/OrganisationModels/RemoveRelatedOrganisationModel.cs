using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels
{
    public class RemoveRelatedOrganisationModel : NavBaseModel
    {
        public int OrganisationId { get; init; }

        public string OrganisationName { get; init; }

        public int RelatedOrganisationId { get; init; }

        public string RelatedOrganisationName { get; init; }
    }
}
