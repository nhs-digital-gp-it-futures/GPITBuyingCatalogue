namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed class OrderProgress
    {
        public int OrderId { get; set; }

        public bool CatalogueSolutionsViewed { get; set; }

        public bool AdditionalServicesViewed { get; set; }

        public bool AssociatedServicesViewed { get; set; }

        public Order Order { get; set; }
    }
}
