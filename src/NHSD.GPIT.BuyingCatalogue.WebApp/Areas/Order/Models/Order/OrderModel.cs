using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public sealed class OrderModel : OrderingBaseModel
    {
        public OrderModel(string odsCode, EntityFramework.Ordering.Models.Order order, OrderTaskList orderSections)
        {
            BackLinkText = "Go back to all orders";
            BackLink = $"/order/organisation/{odsCode}";
            OdsCode = odsCode;
            SectionStatuses = orderSections;

            if (order is null)
            {
                Title = "New order";
                TitleAdvice = "Step 1 must be completed before a summary page and ID number are created for this order.";
            }
            else
            {
                Title = $"Order {order.CallOffId}";
                CallOffId = order.CallOffId;
                TitleAdvice = "Complete the following steps to create an order";
                Description = order.Description;
            }
        }

        public CallOffId CallOffId { get; set; }

        public string Description { get; set; }

        public string DescriptionUrl { get; set; }

        public string TitleAdvice { get; set; }

        public OrderTaskList SectionStatuses { get; set; }
    }
}
