using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public sealed class DeleteOrderModel : NavBaseModel
    {
        public DeleteOrderModel()
        {
        }

        public DeleteOrderModel(Order order)
        {
            CallOffId = order.CallOffId;
        }

        public CallOffId CallOffId { get; set; }

        public string WarningText { get; set; }

        public string AdviceText { get; set; }

        public bool? SelectedOption { get; set; }

        public IList<SelectableRadioOption<bool>> AvailableOptions => new List<SelectableRadioOption<bool>>
        {
            new("Yes, I want to delete this order", true),
            new("No, I do not want to delete this order", false),
        };
    }
}
