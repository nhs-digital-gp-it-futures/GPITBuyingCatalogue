using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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
            BackLink = state.IsNewSolution
                ? $"/order/organisation/{odsCode}/order/{state.CallOffId}/associated-services/select/associated-service{(!state.SkipPriceSelection ? "/price" : string.Empty)}"
                : $"/order/organisation/{odsCode}/order/{state.CallOffId}/associated-services";

            Title = $"{state.CatalogueItemName} associated service information for {state.CallOffId}";
            OdsCode = odsCode;
            OrderItem = state;
            EstimationPeriod = state.EstimationPeriod;
        }

        public CreateOrderItemModel OrderItem { get; set; }

        public TimeUnit? EstimationPeriod { get; set; }

        public List<TimeUnit> TimeUnits { get; } = Enum.GetValues<TimeUnit>().ToList();

        public void UpdateModel(CreateOrderItemModel state)
        {
            OrderItem.CallOffId = state.CallOffId;
            OrderItem.CataloguePrice = state.CataloguePrice;
            OrderItem.EstimationPeriod = state.EstimationPeriod;
            OrderItem.CurrencySymbol = state.CurrencySymbol;
        }
    }
}
