using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Contracts.DeliveryDates.Edit.Base;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Contracts.DeliveryDates.Edit
{
    public class EditDatesAdditionalServiceDifferentDeliveryDates : EditDates
    {
        private const int OrderId = 90023;
        private const string InternalOrgId = "IB-QWO";
        private const string ItemName = "E2E Multiple Prices Additional Service";
        private const string ItemType = "Additional Service";
        private static readonly CallOffId CallOffId = new(OrderId, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrderId), $"{OrderId}" },
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
            { nameof(CatalogueItemId), $"{CatalogueItemId}" },
            { nameof(ItemName), ItemName },
            { nameof(ItemType), ItemType },
        };

        public EditDatesAdditionalServiceDifferentDeliveryDates(LocalWebApplicationFactory factory)
            : base(factory, Parameters)
        {
        }

        protected override string BackLinkMethod => nameof(DeliveryDatesController.MatchDates);

        protected override bool DisplayEditDatesLink => false;

        protected override string OnwardMethod => nameof(DeliveryDatesController.MatchDates);
    }
}
