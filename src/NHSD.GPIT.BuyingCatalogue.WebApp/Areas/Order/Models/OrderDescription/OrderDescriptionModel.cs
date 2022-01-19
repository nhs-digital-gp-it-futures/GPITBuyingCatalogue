using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderDescription
{
    public sealed class OrderDescriptionModel : OrderingBaseModel
    {
        public OrderDescriptionModel()
        {
        }

        public OrderDescriptionModel(string odsCode, EntityFramework.Ordering.Models.Order order)
        {
            Title = "Order description";
            OdsCode = odsCode;
            Description = order?.Description;
        }

        [StringLength(100)]
        public string Description { get; set; }
    }
}
