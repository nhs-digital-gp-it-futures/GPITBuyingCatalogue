using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class Standard
    {
        public Standard()
        {
            StandardCapabilities = new HashSet<StandardCapability>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public bool RequiredForAllSolutions { get; set; }

        public ICollection<StandardCapability> StandardCapabilities { get; set; }
    }
}
