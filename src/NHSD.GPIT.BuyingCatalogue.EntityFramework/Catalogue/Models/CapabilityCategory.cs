using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class CapabilityCategory
    {
        public CapabilityCategory()
        {
            Capabilities = new HashSet<Capability>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Capability> Capabilities { get; set; }
    }
}
