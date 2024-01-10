using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public class AssociatedServicesOnlyDetails
    {
        public CatalogueItemId? SolutionId { get; set; }

        public CatalogueItem Solution { get; set; }

        public string PracticeReorganisationOdsCode { get; set; }

        public OdsOrganisation PracticeReorganisationRecipient { get; set; }

        public string PracticeReorganisationNameAndCode => $"{PracticeReorganisationRecipient?.Name ?? string.Empty} ({PracticeReorganisationOdsCode})";
    }
}
