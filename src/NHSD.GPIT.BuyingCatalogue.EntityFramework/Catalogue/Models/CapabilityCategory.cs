using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public partial class CapabilityCategory
    {
        public CapabilityCategory()
        {
            Capabilities = new HashSet<Capability>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Capability> Capabilities { get; set; }
    }
}
