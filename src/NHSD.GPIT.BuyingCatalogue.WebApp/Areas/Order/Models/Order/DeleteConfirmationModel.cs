namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class DeleteConfirmationModel : OrderingBaseModel
    {
        public DeleteConfirmationModel(string odsCode, string callOffId, EntityFramework.Models.Ordering.Order order)
        {
            BackLinkText = "Go back to all orders";
            BackLink = $"/order/organisation/{odsCode}";
            Title = $"Order {callOffId} deleted";
            OdsCode = odsCode;
            Description = order.Description;
        }

        public string Description { get; set; }
    }
}
