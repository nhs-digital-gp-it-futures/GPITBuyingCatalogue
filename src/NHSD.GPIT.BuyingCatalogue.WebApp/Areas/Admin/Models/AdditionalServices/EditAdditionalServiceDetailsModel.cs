using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices
{
    public sealed class EditAdditionalServiceDetailsModel : NavBaseModel
    {
        public EditAdditionalServiceDetailsModel()
        {
            BackLinkText = "Go back";
        }

        public EditAdditionalServiceDetailsModel(CatalogueItem catalogueItem)
            : this()
        {
            Title = "Additional service details";
            SupplierName = catalogueItem.Supplier.Name;
            CatalogueItemId = catalogueItem.Id;
            CatalogueItemName = catalogueItem.Name;
        }

        public EditAdditionalServiceDetailsModel(CatalogueItem catalogueItem, CatalogueItem additionalServiceCatalogueItem)
            : this()
        {
            Title = $"{additionalServiceCatalogueItem.Name} details";
            Id = additionalServiceCatalogueItem.Id;
            Name = additionalServiceCatalogueItem.Name;
            Description = additionalServiceCatalogueItem.AdditionalService.FullDescription;
            SupplierName = catalogueItem.Supplier.Name;
            CatalogueItemId = catalogueItem.Id;
            CatalogueItemName = catalogueItem.Name;
        }

        public string Title { get; init; }

        public string SupplierName { get; init; }

        public CatalogueItemId CatalogueItemId { get; init; }

        public string CatalogueItemName { get; init; }

        public CatalogueItemId? Id { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }
    }
}
