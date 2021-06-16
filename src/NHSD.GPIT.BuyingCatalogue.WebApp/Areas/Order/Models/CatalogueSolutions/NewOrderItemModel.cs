using System.Linq;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
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
            if (createOrderItemModel.ProvisioningType.Equals(EntityFramework.Models.Ordering.ProvisioningType.Declarative))
                BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution/price/flat/declarative";
            else if (createOrderItemModel.ProvisioningType.Equals(EntityFramework.Models.Ordering.ProvisioningType.OnDemand))
                BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution/price/flat/ondemand";
            else
                BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution/price/recipients/date";

            BackLinkText = "Go back";
            Title = $"{createOrderItemModel.CatalogueItemName} information for {callOffId}";
            OdsCode = odsCode;
            CallOffId = callOffId;
            OrderItem = createOrderItemModel;
            OrderItem.ServiceRecipients = OrderItem.ServiceRecipients.Where(x => x.Selected).ToList();
            CurrencySymbol = CurrencyCodeSigns.Code[createOrderItemModel.CurrencyCode];
        }

        public string CallOffId { get; set; }

        public CreateOrderItemModel OrderItem { get; set; }

        public string CurrencySymbol { get; set; }
    }
}
