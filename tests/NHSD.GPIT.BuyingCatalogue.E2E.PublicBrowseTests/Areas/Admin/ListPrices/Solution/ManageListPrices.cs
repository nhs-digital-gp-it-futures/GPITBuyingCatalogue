using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Base;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Solution
{
    public sealed class ManageListPrices : ManageListPricesBase
    {
        private static readonly CatalogueItemId SolutionId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public ManageListPrices(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionListPriceController),
                  nameof(CatalogueSolutionListPriceController.Index),
                  Parameters)
        {
        }

        protected override CatalogueItemId CatalogueItemId => SolutionId;

        protected override Type Controller => typeof(CatalogueSolutionListPriceController);

        protected override Type BacklinkController => typeof(CatalogueSolutionsController);

        protected override string BacklinkAction => nameof(CatalogueSolutionsController.ManageCatalogueSolution);
    }
}
