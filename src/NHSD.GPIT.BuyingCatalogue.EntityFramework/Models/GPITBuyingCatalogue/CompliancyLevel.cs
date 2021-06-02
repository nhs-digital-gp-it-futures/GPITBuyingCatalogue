using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public partial class CompliancyLevel
    {
        public CompliancyLevel()
        {
            Epics = new HashSet<Epic>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Epic> Epics { get; set; }
    }
}
