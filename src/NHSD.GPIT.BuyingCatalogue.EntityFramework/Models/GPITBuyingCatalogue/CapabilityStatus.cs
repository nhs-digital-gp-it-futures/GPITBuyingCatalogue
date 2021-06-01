using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public partial class CapabilityStatus
    {
        public CapabilityStatus()
        {
            Capabilities = new HashSet<Capability>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Capability> Capabilities { get; set; }
    }
}
