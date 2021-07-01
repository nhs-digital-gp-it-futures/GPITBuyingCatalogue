using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public sealed class EditSolutionModel : OrderingBaseModel
    {
        public EditSolutionModel()
        {
        }

        public EditSolutionModel(string odsCode, CallOffId callOffId, CreateOrderItemModel createOrderItemModel)
        {
            if (!createOrderItemModel.IsNewSolution)
            {
                BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions";
            }
            else
            {
                if (createOrderItemModel.ProvisioningType == ProvisioningType.Declarative)
                    BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution/price/flat/declarative";
                else if (createOrderItemModel.ProvisioningType == ProvisioningType.OnDemand)
                    BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution/price/flat/ondemand";
                else
                    BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution/price/recipients/date";
            }

            BackLinkText = "Go back";
            Title = $"{createOrderItemModel.CatalogueItemName} information for {callOffId}";
            OdsCode = odsCode;
            OrderItem = createOrderItemModel;

            // TODO: Legacy appears to order based on recipient name, unless some recipients have info missing in which case they appear at the top
            OrderItem.ServiceRecipients = OrderItem.ServiceRecipients.Where(x => x.Selected).ToList();

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
                return OrderItem.ProvisioningType switch
                {
                    ProvisioningType.Declarative => "Quantity",
                    ProvisioningType.OnDemand => "Quantity " + OrderItem.TimeUnit?.Description(),
                    _ => "Practice list size",
                };
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
                return OrderItem.ProvisioningType == ProvisioningType.Patient
                    ? "What list size should I enter?"
                    : "What quantity should I enter?";
            }
        }

        public string SecondColumnDetailsContent
        {
            get
            {
                if (OrderItem.ProvisioningType == ProvisioningType.Declarative)
                    return "Enter the total amount you think you'll need for the entire duration of the order.";

                return OrderItem.ProvisioningType == ProvisioningType.OnDemand
                    ? "Estimate the quantity you think you'll need either per month or per year."
                    : "Enter the amount you wish to order. This is usually based on each Service Recipient's practice list size to help calculate an estimated price, but the figure can be changed if required.As you’re ordering per patient, we've included each practice list size if we have it. If it's not included, you'll need to add it yourself.";
            }
        }

        public void UpdateModel(CreateOrderItemModel state)
        {
            OrderItem.CallOffId = state.CallOffId;
            OrderItem.PricingUnit = state.PricingUnit;
            OrderItem.CataloguePriceTimeUnit = state.CataloguePriceTimeUnit;
            OrderItem.EstimationPeriod = state.EstimationPeriod;
            OrderItem.CurrencyCode = state.CurrencyCode;
            OrderItem.CurrencySymbol = state.CurrencySymbol;
            OrderItem.ProvisioningType = state.ProvisioningType;
        }
    }
}
