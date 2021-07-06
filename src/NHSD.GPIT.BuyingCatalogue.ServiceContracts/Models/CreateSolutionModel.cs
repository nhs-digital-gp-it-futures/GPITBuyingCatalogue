using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class CreateSolutionModel
    {
        public FrameworkModel FrameworkModel { get; set; }

        public string Name { get; set; }

        public string SupplierId { get; set; }

        public Guid UserId { get; set; }
    }
}
