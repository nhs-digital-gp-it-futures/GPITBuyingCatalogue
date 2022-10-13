using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteOrder
{
    public sealed class DeleteOrderModel : NavBaseModel
    {
        public DeleteOrderModel()
        {
        }

        public DeleteOrderModel(EntityFramework.Ordering.Models.Order order)
        {
            CallOffId = order.CallOffId;
        }

        public CallOffId CallOffId { get; set; }

        public bool? SelectedOption { get; set; }

        public IList<SelectableRadioOption<bool>> AvailableOptions => new List<SelectableRadioOption<bool>>
        {
            new("Yes, I want to delete this order", true),
            new("No, I do not want to delete this order", false),
        };
    }
}
