using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace BuyingCatalogueFunction.EpicsAndCapabilities.Models
{
    public class StandardCsv
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public StandardType StandardType { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }
    }
}
