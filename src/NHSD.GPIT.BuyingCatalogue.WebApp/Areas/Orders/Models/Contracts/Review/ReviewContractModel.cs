using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.Review
{
    public class ReviewContractModel : NavBaseModel
    {
        public ReviewContractModel()
        {
        }

        public ReviewContractModel(
            string internalOrgId,
            Order order,
            ImplementationPlan defaultPlan)
        {
            InternalOrgId = internalOrgId;
            CallOffId = order.CallOffId;
            Order = order;
            DefaultPlan = defaultPlan;
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public Order Order { get; set; }

        public ImplementationPlan DefaultPlan { get; set; }

        public string AdviceText => Order?.OrderStatus switch
        {
            null => string.Empty,
            OrderStatus.Completed => "This order has been confirmed and can no longer be changed.",
            _ => Order.CanComplete()
                ? "Review your order before completing it. Once the order is completed, you'll be unable to make changes."
                : "This is what's been added to your order so far. You must complete all mandatory steps before you can confirm your order.",
        };
    }
}
