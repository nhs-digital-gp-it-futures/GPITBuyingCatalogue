using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public class EditAssociatedServiceModel : OrderingBaseModel
    {
        public EditAssociatedServiceModel()
        {
        }

        public EditAssociatedServiceModel(string odsCode, string callOffId, string id, CreateOrderItemModel createOrderItemModel, bool isNewSolution)
        {
            if (isNewSolution)
            {
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
            CurrencySymbol = CurrencyCodeSigns.Code[createOrderItemModel.CurrencyCode];
            TimeUnit = createOrderItemModel.TimeUnit?.Name;
        }

        public string CallOffId { get; set; }

        public CreateOrderItemModel OrderItem { get; set; }

        public string CurrencySymbol { get; set; }

        public string TimeUnit { get; set; }

        public List<EntityFramework.Models.Ordering.TimeUnit> TimeUnits
        {
            get
            {
                return new List<EntityFramework.Models.Ordering.TimeUnit>
                {
                    EntityFramework.Models.Ordering.TimeUnit.PerMonth,
                    EntityFramework.Models.Ordering.TimeUnit.PerYear,
                };
            }
        }
    }
}
