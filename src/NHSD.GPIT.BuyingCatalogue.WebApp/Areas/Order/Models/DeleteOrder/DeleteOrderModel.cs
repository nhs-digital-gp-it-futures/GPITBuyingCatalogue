using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteOrder
{
    public sealed class DeleteOrderModel : NavBaseModel
    {
        public const string Amendment = "amendment";
        public const string AmendmentAdvice = "Deleting this amendment will revert this order to the previous version.";
        public const string AmendmentNoOptionText = "No, I do not want to delete this amendment";
        public const string AmendmentYesOptionText = "Yes, I want to delete this amendment";
        public const string Order = "order";
        public const string OrderAdvice = "The order will be permanently deleted from your organisation’s dashboard.";
        public const string OrderNoOptionText = "No, I do not want to delete this order";
        public const string OrderYesOptionText = "Yes, I want to delete this order";

        private readonly IList<SelectableRadioOption<bool>> amendmentOptions = new List<SelectableRadioOption<bool>>
        {
            new(AmendmentYesOptionText, true),
            new(AmendmentNoOptionText, false),
        };

        private readonly IList<SelectableRadioOption<bool>> orderOptions = new List<SelectableRadioOption<bool>>
        {
            new(OrderYesOptionText, true),
            new(OrderNoOptionText, false),
        };

        public DeleteOrderModel()
        {
        }

        public DeleteOrderModel(EntityFramework.Ordering.Models.Order order)
        {
            CallOffId = order.CallOffId;
            IsAmendment = order.IsAmendment;
        }

        public CallOffId CallOffId { get; set; }

        public bool IsAmendment { get; set; }

        public string Advice => IsAmendment
            ? AmendmentAdvice
            : OrderAdvice;

        public string OrderType => IsAmendment
            ? Amendment
            : Order;

        public bool? SelectedOption { get; set; }

        public IList<SelectableRadioOption<bool>> AvailableOptions => IsAmendment
            ? amendmentOptions
            : orderOptions;
    }
}
