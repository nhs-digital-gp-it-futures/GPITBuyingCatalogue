using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public sealed class EditAdditionalServiceModel : OrderingBaseModel
    {
        public EditAdditionalServiceModel()
        {
        }

        public EditAdditionalServiceModel(string odsCode, CallOffId callOffId, CreateOrderItemModel createOrderItemModel)
        {
            if (!createOrderItemModel.IsNewSolution)
            {
                BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services";
            }
            else
            {
                if (createOrderItemModel.CataloguePrice.ProvisioningType == ProvisioningType.Declarative)
                    BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services/select/additional-service/price/flat/declarative";
                else if (createOrderItemModel.CataloguePrice.ProvisioningType == ProvisioningType.OnDemand)
                    BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services/select/additional-service/price/flat/ondemand";
                else
                    BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services/select/additional-service/price/recipients/date";
            }

            BackLinkText = "Go back";
            Title = $"{createOrderItemModel.CatalogueItemName} information for {callOffId}";
            OdsCode = odsCode;
            OrderItem = createOrderItemModel;

            // TODO: Legacy appears to order based on recipient name, unless some recipients have info missing in which case they appear at the top
            OrderItem.ServiceRecipients = OrderItem.ServiceRecipients.Where(m => m.Selected).ToList();

            foreach (var recipient in OrderItem.ServiceRecipients.Where(r => r.Quantity is null))
                recipient.Quantity = createOrderItemModel.Quantity;

            foreach (var recipient in OrderItem.ServiceRecipients.Where(r => r.DeliveryDate is null))
                recipient.DeliveryDate = createOrderItemModel.PlannedDeliveryDate;
        }

        public CreateOrderItemModel OrderItem { get; set; }

        public string SecondColumnName
        {
            get
            {
                return OrderItem.CataloguePrice?.ProvisioningType switch
                {
                    ProvisioningType.Declarative => "Quantity",
                    ProvisioningType.OnDemand => "Quantity " + OrderItem.TimeUnitDescription,
                    _ => "Practice list size",
                };
            }
        }

        public string SecondColumnSuffix
        {
            get
            {
                return OrderItem.CataloguePrice?.ProvisioningType == ProvisioningType.OnDemand
                    ? OrderItem.TimeUnitDescription
                    : string.Empty;
            }
        }

        public string SecondColumnDetailsTitle
        {
            get
            {
                if (OrderItem.CataloguePrice?.ProvisioningType == ProvisioningType.Patient)
                    return "What list size should I enter?";

                return "What quantity should I enter?";
            }
        }

        public string SecondColumnDetailsContent
        {
            get
            {
                if (OrderItem.CataloguePrice?.ProvisioningType == ProvisioningType.Declarative)
                    return "Enter the total amount you think you'll need for the entire duration of the order.";

                if (OrderItem.CataloguePrice?.ProvisioningType == ProvisioningType.OnDemand)
                    return "Estimate the quantity you think you'll need either per month or per year.";

                return "Enter the amount you wish to order. This is usually based on each Service Recipient's practice list size to help calculate an estimated price, but the figure can be changed if required.As you’re ordering per patient, we've included each practice list size if we have it. If it's not included, you'll need to add it yourself.";
            }
        }

        public void UpdateModel(CreateOrderItemModel state)
        {
            OrderItem.CallOffId = state.CallOffId;
            OrderItem.CataloguePrice = state.CataloguePrice;
            OrderItem.EstimationPeriod = state.EstimationPeriod;
            OrderItem.CurrencySymbol = state.CurrencySymbol;
        }
    }
}
