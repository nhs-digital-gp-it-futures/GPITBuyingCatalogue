using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices
{
    public sealed class EditAdditionalServiceDetailsModel : NavBaseModel
    {
        public EditAdditionalServiceDetailsModel()
        {
        }

        public EditAdditionalServiceDetailsModel(CatalogueItem catalogueItem)
            : this()
        {
            Title = "Additional service details";
            SolutionName = catalogueItem.Name;
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
            SolutionName = catalogueItem.Name;
            CatalogueItemId = catalogueItem.Id;
            CatalogueItemName = catalogueItem.Name;
        }

        public string Title { get; init; }

        public string SolutionName { get; init; }

        public CatalogueItemId CatalogueItemId { get; init; }

        public string CatalogueItemName { get; init; }

        public CatalogueItemId? Id { get; init; }

        [StringLength(255)]
        public string Name { get; init; }

        [StringLength(1000)]
        public string Description { get; init; }
    }
}
