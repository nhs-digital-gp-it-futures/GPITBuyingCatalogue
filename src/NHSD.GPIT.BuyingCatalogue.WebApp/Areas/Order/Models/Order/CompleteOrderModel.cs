using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public sealed class CompleteOrderModel : OrderingBaseModel
    {
        public CompleteOrderModel()
        {
        }

        public CompleteOrderModel(string odsCode, CallOffId callOffId, EntityFramework.Ordering.Models.Order order)
        {
            BackLinkText = "Go back";
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}";
            Title = $"Complete order {callOffId}?";
            OdsCode = odsCode;
            Description = order.Description;
            CallOffId = callOffId;
        }

        public string Description { get; set; }

        public CallOffId CallOffId { get; set; }
    }
}
