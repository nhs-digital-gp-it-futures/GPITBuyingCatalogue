using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class RowViewModel
    {
        public RowViewModel(CatalogueItemCapability catalogueItemCapability)
        {
            Heading = catalogueItemCapability.Capability.Name;
            Description = catalogueItemCapability.Capability.Description;

            // This feels like it could be done better, but it works as is
            CheckEpicsUrl = $"capability/{catalogueItemCapability.Capability.Id}";
        }

        public string Heading { get; set; }

        public string Description { get; set; }

        public string CheckEpicsUrl { get; set; }
    }
}
