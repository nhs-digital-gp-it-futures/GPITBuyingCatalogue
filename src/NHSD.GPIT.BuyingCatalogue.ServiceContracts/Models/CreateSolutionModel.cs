using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class CreateSolutionModel
    {
        public IList<FrameworkModel> Frameworks { get; set; }

        public string Name { get; set; }

        public int SupplierId { get; set; }

        public int UserId { get; set; }
    }
}
