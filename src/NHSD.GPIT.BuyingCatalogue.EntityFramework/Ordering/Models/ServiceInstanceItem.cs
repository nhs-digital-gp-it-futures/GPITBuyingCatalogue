namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed class ServiceInstanceItem
    {
        public int OrderId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string OdsCode { get; set; }

        public string ServiceInstanceId { get; set; }
    }
}
