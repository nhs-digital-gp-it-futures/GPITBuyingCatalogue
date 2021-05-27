namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public class SupplierModel : OrderingBaseModel
    {
        public SupplierModel(string odsCode, EntityFramework.Models.Ordering.Order order)
        {
            BackLinkText = "Go back";
            BackLink = $"/order/organisation/{odsCode}/order/{order.CallOffId}";
            Title = $"Supplier information for {order.CallOffId}";
            OdsCode = odsCode;
            Id = order.Supplier.Id;
            Name = order.Supplier.Name;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}
