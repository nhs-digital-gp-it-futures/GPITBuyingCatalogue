using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceLevelAgreements
{
    public sealed class AddSlaModel
    {
        public CatalogueItem Solution { get; set; }

        public SlaType SlaLevel { get; set; }
    }
}
