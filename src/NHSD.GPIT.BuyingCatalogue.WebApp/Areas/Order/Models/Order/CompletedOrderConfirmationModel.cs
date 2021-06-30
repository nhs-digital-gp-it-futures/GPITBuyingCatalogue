using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public sealed class CompletedOrderConfirmationModel : OrderingBaseModel
    {
        public CompletedOrderConfirmationModel()
        {
        }

        public CompletedOrderConfirmationModel(string odsCode, CallOffId callOffId)
        {
            BackLinkText = "Go back to all orders";
            BackLink = $"order/organisation/{odsCode}";
            Title = $"Order {callOffId} completed";
            OdsCode = odsCode;
            CallOffId = callOffId;
        }

        public CallOffId CallOffId { get; set; }
    }
}
