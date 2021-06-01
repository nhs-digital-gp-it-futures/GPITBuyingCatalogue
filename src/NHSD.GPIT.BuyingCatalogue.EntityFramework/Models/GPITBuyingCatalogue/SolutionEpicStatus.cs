using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public partial class SolutionEpicStatus
    {
        public SolutionEpicStatus()
        {
            SolutionEpics = new HashSet<SolutionEpic>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsMet { get; set; }

        public virtual ICollection<SolutionEpic> SolutionEpics { get; set; }
    }
}
