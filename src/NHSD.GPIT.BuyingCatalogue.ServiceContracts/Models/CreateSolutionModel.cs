using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class CreateSolutionModel
    {
        public IList<FrameworkModel> Frameworks { get; set; }

        public string Name { get; set; }

        public int SupplierId { get; set; }

        public SolutionType SolutionType { get; set; }

        public bool IsPilotSolution { get; set; }

        public int UserId { get; set; }
    }
}
