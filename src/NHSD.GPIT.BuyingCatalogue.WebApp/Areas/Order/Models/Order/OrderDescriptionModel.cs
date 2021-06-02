using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class OrderDescriptionModel : OrderingBaseModel
    {
        public OrderDescriptionModel()
        {
        }

        public OrderDescriptionModel(string odsCode, EntityFramework.Models.Ordering.Order order)
        {
            BackLinkText = "Go back";
            BackLink = $"/order/organisation/{odsCode}/order/{order.CallOffId}";
            Title = "Order description";
            OdsCode = odsCode;
            Description = order.Description;
        }

        [Required(ErrorMessage = "Enter a description")]
        [StringLength(100)]
        public string Description { get; set; }
    }
}
