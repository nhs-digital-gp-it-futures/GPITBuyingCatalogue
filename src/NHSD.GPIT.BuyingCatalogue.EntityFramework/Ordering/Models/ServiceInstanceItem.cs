using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class ServiceInstanceItem
    {
        public int OrderId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string OdsCode { get; set; }

        public string ServiceInstanceId { get; set; }
    }
}
