using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public class EditAssociatedServiceModel : OrderingBaseModel
    {
        public EditAssociatedServiceModel()
        {
        }

        public EditAssociatedServiceModel(string odsCode, CallOffId callOffId, CatalogueItemId id, CreateOrderItemModel createOrderItemModel, bool isNewSolution)
        {
            if (isNewSolution)
            {
                // TODO - If there is only one price for this service then the back link should go to the select associated service page
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

            // TODO: currency code comes from the catalogue price
            CurrencySymbol = "£";
        }

        public CallOffId CallOffId { get; set; }

        public CreateOrderItemModel OrderItem { get; set; }

        public string CurrencySymbol { get; set; }

        public TimeUnit? TimeUnit { get; set; }

        public List<TimeUnit> TimeUnits { get; } = Enum.GetValues<TimeUnit>().ToList();
    }
}
