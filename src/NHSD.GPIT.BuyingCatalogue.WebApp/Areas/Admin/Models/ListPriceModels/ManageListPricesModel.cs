using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public sealed class ManageListPricesModel : NavBaseModel
    {
        private readonly CatalogueItem catalogueItem;

        public ManageListPricesModel()
        {
        }

        public ManageListPricesModel(CatalogueItem catalogueItem)
        {
            this.catalogueItem = catalogueItem;
            CataloguePrices = catalogueItem.CataloguePrices
                .OrderBy(cp => cp.IsLocked)
                .ThenByDescending(cp => cp.PublishedStatus)
                .ToList();
        }

        public ManageListPricesModel(CatalogueItem catalogueItem, CatalogueItemId parentCatalogueItemId)
            : this(catalogueItem)
        {
            SolutionId = parentCatalogueItemId;

            if (catalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService)
                AdditionalServiceId = catalogueItem.Id;

            if (catalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService)
                AssociatedServiceId = catalogueItem.Id;

            SolutionType = catalogueItem.CatalogueItemType.DisplayName();
        }

        public CatalogueItemId CatalogueItemId => catalogueItem.Id;

        public string CatalogueName => catalogueItem.Name;

        public ICollection<CataloguePrice> CataloguePrices { get; }

        // This is only used for routing for Associated and Additional services
        public CatalogueItemId SolutionId { get; init; }

        public CatalogueItemId? AdditionalServiceId { get; init; }

        public CatalogueItemId? AssociatedServiceId { get; init; }

        public string SolutionType { get; init; }

        public bool ShowUnpublishedPrices { get; } // Only used for the checkbox component. value is not actually used server side;

        public string AddLink { get; init; }

        public string EditPriceActionName { get; set; }

        public string EditPriceStatusActionName { get; set; }

        public string ControllerName { get; set; }

        public TaskProgress Status()
        {
            if (CataloguePrices is null)
                return TaskProgress.NotStarted;

            if (CataloguePrices.Any(cp => cp.PublishedStatus == PublicationStatus.Published))
                return TaskProgress.Completed;

            return TaskProgress.InProgress;
        }
    }
}
