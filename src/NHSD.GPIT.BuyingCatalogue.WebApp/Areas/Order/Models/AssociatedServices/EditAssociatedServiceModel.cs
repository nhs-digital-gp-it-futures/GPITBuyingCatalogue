using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public sealed class EditAssociatedServiceModel : OrderingBaseModel
    {
        public EditAssociatedServiceModel()
        {
        }

        public EditAssociatedServiceModel(string odsCode, CreateOrderItemModel state)
        {
            if (state.IsNewSolution)
            {
                if (state.SkipPriceSelection)
                    BackLink = $"/order/organisation/{odsCode}/order/{state.CallOffId}/associated-services/select/associated-service";
                else
                    BackLink = $"/order/organisation/{odsCode}/order/{state.CallOffId}/associated-services/select/associated-service/price";
            }
            else
            {
                BackLink = $"/order/organisation/{odsCode}/order/{state.CallOffId}/associated-services";
            }

            BackLinkText = "Go back";
            Title = $"{state.CatalogueItemName} associated service information for {state.CallOffId}";
            OdsCode = odsCode;
            OrderItem = state;
            TimeUnit = state.TimeUnit;
        }

        public CreateOrderItemModel OrderItem { get; set; }

        public TimeUnit? TimeUnit { get; set; }

        public List<TimeUnit> TimeUnits { get; } = Enum.GetValues<TimeUnit>().ToList();

        public void UpdateModel(CreateOrderItemModel state)
        {
            OrderItem.CallOffId = state.CallOffId;
            OrderItem.PricingUnit = state.PricingUnit;
            OrderItem.TimeUnit = state.TimeUnit;
            OrderItem.CurrencyCode = state.CurrencyCode;
            OrderItem.CurrencySymbol = state.CurrencySymbol;
            OrderItem.ProvisioningType = state.ProvisioningType;
        }
    }
}
