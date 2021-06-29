using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public sealed class EditAssociatedServiceModel : OrderingBaseModel
    {
        public EditAssociatedServiceModel()
        {
        }

        public EditAssociatedServiceModel(string odsCode, CallOffId callOffId, CreateOrderItemModel createOrderItemModel)
        {
            if (createOrderItemModel.IsNewSolution)
            {
                if (createOrderItemModel.SkipAssociatedServicePrices)
                    BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/associated-services/select/associated-service/price";
                else
                    BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/associated-services/select/associated-service/price";
            }
            else
            {
                BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/associated-services";
            }

            BackLinkText = "Go back";
            Title = $"{createOrderItemModel.CatalogueItemName} associated service information for {callOffId}";
            OdsCode = odsCode;
            CallOffId = callOffId;
            OrderItem = createOrderItemModel;
            TimeUnit = createOrderItemModel.TimeUnit;
            CurrencySymbol = CurrencyCodeSigns.Code[createOrderItemModel.CurrencyCode];
        }

        public CallOffId CallOffId { get; set; }

        public CreateOrderItemModel OrderItem { get; set; }

        public string CurrencySymbol { get; set; }

        public TimeUnit? TimeUnit { get; set; }

        public List<TimeUnit> TimeUnits { get; } = Enum.GetValues<TimeUnit>().ToList();
    }
}
