using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders
{
    public class CompletedModel : NavBaseModel
    {
        public const string TitleText = "Order{0} completed";

        public const string AdviceText = "You’ve successfully completed your Call-off Order Form{0}. Make sure you carry out the listed tasks where relevant to finalise your procurement. We’ll send these tasks to the email address you’ve provided.";

        public CompletedModel(string internalOrgId, Order order)
        {
            InternalOrgId = internalOrgId;
            CallOffId = order.CallOffId;
            Order = order;
        }

        public override string Title => Order.IsAmendment
            ? string.Format(TitleText, " amendment")
            : string.Format(TitleText, string.Empty);

        public override string Advice => Order.IsAmendment
            ? string.Format(AdviceText, " amendments")
            : string.Format(AdviceText, string.Empty);

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public Order Order { get; set; }
    }
}
