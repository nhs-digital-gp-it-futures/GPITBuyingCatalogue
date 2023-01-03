using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Quantity.Base;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Quantity
{
    public class SelectRecipientQuantityAssociatedService : SelectRecipientQuantity
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 92008;
        private static readonly CallOffId CallOffId = new(OrderId, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "S-999");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrderId), $"{OrderId}" },
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
            { nameof(CatalogueItemId), $"{CatalogueItemId}" },
        };

        public SelectRecipientQuantityAssociatedService(LocalWebApplicationFactory factory)
            : base(factory, Parameters)
        {
        }

        protected override string PageTitle => "Quantity of Associated Service - E2E Single Price Added Associated Service";

        protected override Type OnwardController => typeof(ReviewSolutionsController);

        protected override string OnwardActionName => nameof(ReviewSolutionsController.ReviewSolutions);
    }
}
