namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed class ServiceInstanceItem
    {
        public int OrderId { get; set; }

        // TODO: should be of type CatalogueItemId
        public string CatalogueItemId { get; set; }

        public string OdsCode { get; set; }

        public string ServiceInstanceId { get; set; }
    }
}
