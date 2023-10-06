using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices
{
    public sealed class AssociatedServicesDetailsModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string OrderGuidance { get; set; }

        public int UserId { get; set; }

        public PracticeReorganisationTypeEnum PracticeReorganisationType { get; set; }
    }
}
