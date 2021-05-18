using System;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class FrameworkSolution
    {
        public string FrameworkId { get; set; }

        public string SolutionId { get; set; }

        public bool IsFoundation { get; set; }

        public DateTime LastUpdated { get; set; }

        public Guid LastUpdatedBy { get; set; }

        public virtual Framework Framework { get; set; }

        public virtual Solution Solution { get; set; }
    }
}
