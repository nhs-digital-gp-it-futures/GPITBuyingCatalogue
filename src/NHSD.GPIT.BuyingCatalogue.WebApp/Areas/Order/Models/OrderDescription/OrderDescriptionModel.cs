using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderDescription
{
    public sealed class OrderDescriptionModel : OrderingBaseModel
    {
        public const string AmendmentAdviceText = "You can update the description of this order to help distinguish it from the previous version if required.";
        public const string AdviceText = "Provide a brief description of your order to help identify it from any others you’ve worked on.";
        public const string NewOrderAdviceText = AdviceText + " Once you save the description, a unique order ID will be generated.";

        public OrderDescriptionModel()
        {
        }

        public OrderDescriptionModel(string internalOrgId, string organisationName)
        {
            Title = "Order description";
            InternalOrgId = internalOrgId;
            Caption = organisationName;
            Advice = NewOrderAdviceText;
        }

        public OrderDescriptionModel(string internalOrgId, EntityFramework.Ordering.Models.Order order)
        {
            Title = "Order description";
            InternalOrgId = internalOrgId;
            Caption = $"Order {order?.CallOffId}";
            Description = order?.Description;
            Advice = (order?.IsAmendment ?? false) ? AmendmentAdviceText : AdviceText;
        }

        [StringLength(100)]
        public string Description { get; set; }

        public string Caption { get; set; }

        public string Advice { get; set; }
    }
}
