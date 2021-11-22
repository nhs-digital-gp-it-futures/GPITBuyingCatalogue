using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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
            CataloguePrices = catalogueItem.CataloguePrices;
        }

        public ManageListPricesModel(CatalogueItem catalogueItem, CatalogueItemId relatedCatalogueItemId)
            : this(catalogueItem)
        {
            SolutionId = relatedCatalogueItemId;
        }

        public CatalogueItemId CatalogueItemId => catalogueItem.Id;

        public string CatalogueName => catalogueItem.Name;

        public ICollection<CataloguePrice> CataloguePrices { get; }

        // This is only used for routing for Associated and Additional services
        public CatalogueItemId? SolutionId { get; }

        public string AddLink { get; init; }

        public TaskProgress Status() =>
            CataloguePrices is not null && CataloguePrices.Any()
            ? TaskProgress.Completed
            : TaskProgress.NotStarted;
    }
}
