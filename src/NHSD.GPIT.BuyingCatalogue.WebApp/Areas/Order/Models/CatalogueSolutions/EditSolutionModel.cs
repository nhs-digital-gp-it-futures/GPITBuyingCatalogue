using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public class EditSolutionModel : OrderingBaseModel
    {
        public EditSolutionModel(string odsCode, string callOffId, string id, CreateOrderItemModel createOrderItemModel)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions";
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
