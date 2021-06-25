using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public sealed class EditAdditionalServiceModel : OrderingBaseModel
    {
        public EditAdditionalServiceModel()
        {
        }

        public EditAdditionalServiceModel(string odsCode, CallOffId callOffId, CreateOrderItemModel createOrderItemModel, bool isNewSolution)
        {
            if (!isNewSolution)
            {
                BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services";
            }
            else
            {
                if (createOrderItemModel.ProvisioningType == ProvisioningType.Declarative)
                    BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services/select/additional-service/price/flat/declarative";
                else if (createOrderItemModel.ProvisioningType == ProvisioningType.OnDemand)
                    BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services/select/additional-service/price/flat/ondemand";
                else
                    BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services/select/additional-service/price/recipients/date";
            }

            BackLinkText = "Go back";
            Title = $"{createOrderItemModel.CatalogueItemName} information for {callOffId}";
            OdsCode = odsCode;
            CallOffId = callOffId;
            OrderItem = createOrderItemModel;
            OrderItem.ServiceRecipients = OrderItem.ServiceRecipients.Where(m => m.Selected).ToList();

            // TODO: currency code comes from the catalogue price
            CurrencySymbol = "£";
        }

        public CallOffId CallOffId { get; set; }

        public CreateOrderItemModel OrderItem { get; set; }

        public string CurrencySymbol { get; set; }

        public string SecondColumnName
        {
            get
            {
                if (OrderItem.ProvisioningType == ProvisioningType.Declarative)
                    return "Quantity";

                if (OrderItem.ProvisioningType == ProvisioningType.OnDemand)
                    return "Quantity" + OrderItem.TimeUnit?.Description();

                return "Practice list size";
            }
        }

        public string SecondColumnSuffix
        {
            get
            {
                return OrderItem.ProvisioningType == ProvisioningType.OnDemand
                    ? OrderItem.TimeUnit?.Description()
                    : string.Empty;
            }
        }

        public string SecondColumnDetailsTitle
        {
            get
            {
                if (OrderItem.ProvisioningType == ProvisioningType.Patient)
                    return "What list size should I enter?";

                return "What quantity should I enter?";
            }
        }

        public string SecondColumnDetailsContent
        {
            get
            {
                if (OrderItem.ProvisioningType == ProvisioningType.Declarative)
                    return "Enter the total amount you think you'll need for the entire duration of the order.";

                if (OrderItem.ProvisioningType == ProvisioningType.OnDemand)
                    return "Estimate the quantity you think you'll need either per month or per year.";

                return "Enter the amount you wish to order. This is usually based on each Service Recipient's practice list size to help calculate an estimated price, but the figure can be changed if required.As you’re ordering per patient, we've included each practice list size if we have it. If it's not included, you'll need to add it yourself.";
            }
        }
    }
}
