using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class CreateSolutionModel
    {
        public IList<FrameworkModel> Frameworks { get; set; }

        public string Name { get; set; }

        public int SupplierId { get; set; }

        public bool IsPilotSolution { get; set; }

        public int UserId { get; set; }

        public CatalogueItemId Id { get; set; }
    }
}
