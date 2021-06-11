using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public class NewOrderItemModel : OrderingBaseModel
    {
        public NewOrderItemModel()
        {
        }

        public NewOrderItemModel(string odsCode, string callOffId, CreateOrderItemModel createOrderItemModel)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution/price/flat/declarative";
            BackLinkText = "Go back";
            Title = $"{createOrderItemModel.CatalogueItemName} information for {callOffId}";
            OdsCode = odsCode;
            CallOffId = callOffId;
            OrderItem = createOrderItemModel;
            OrderItem.ServiceRecipients = OrderItem.ServiceRecipients.Where(x => x.Checked).ToList();
        }

        public string CallOffId { get; set; }

        public CreateOrderItemModel OrderItem { get; set; }
    }
}
