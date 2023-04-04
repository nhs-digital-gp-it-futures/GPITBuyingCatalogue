using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Quantity.Base;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Quantity
{
    public class SelectRecipientQuantityCatalogueSolution : SelectRecipientQuantity
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90006;
        private static readonly CallOffId CallOffId = new(OrderId, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrderId), $"{OrderId}" },
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
            { nameof(CatalogueItemId), $"{CatalogueItemId}" },
        };

        public SelectRecipientQuantityCatalogueSolution(LocalWebApplicationFactory factory)
            : base(factory, Parameters)
        {
        }

        protected override string PageTitle => "Quantity of Catalogue Solution - E2E With Contact With Single Price";

        protected override Type OnwardController => typeof(AssociatedServicesController);

        protected override string OnwardActionName => nameof(AssociatedServicesController.AddAssociatedServices);
    }
}
