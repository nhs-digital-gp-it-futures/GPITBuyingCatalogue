using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard
{
    public class NewOrderDescriptionModel : OrderingBaseModel
    {
        public NewOrderDescriptionModel()
        {
        }

        public NewOrderDescriptionModel(string odsCode)
        {
            BackLinkText = "Go back";
            BackLink = $"/order/organisation/{odsCode}/order/neworder";
            Title = "Order description";
            OdsCode = odsCode;
        }

        [Required(ErrorMessage = "Enter a description")]
        [StringLength(100)]
        public string Description { get; set; }
    }
}
