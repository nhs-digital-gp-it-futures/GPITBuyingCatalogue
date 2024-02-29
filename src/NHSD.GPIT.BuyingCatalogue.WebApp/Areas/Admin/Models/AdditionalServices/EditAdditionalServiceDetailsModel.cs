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
            IdDisplay = additionalServiceCatalogueItem.Id.ToString();
            Name = additionalServiceCatalogueItem.Name;
            Description = additionalServiceCatalogueItem.AdditionalService.FullDescription;
            SolutionName = catalogueItem.Name;
            CatalogueItemId = catalogueItem.Id;
            CatalogueItemName = catalogueItem.Name;
            IsEdit = true;
        }

        public string SolutionName { get; init; }

        public CatalogueItemId CatalogueItemId { get; init; }

        public string CatalogueItemName { get; init; }

        public bool IsEdit { get; set; }

        public string IdDisplay { get; set; }

        public CatalogueItemId? Id
        {
            get
            {
                try
                {
                    return CatalogueItemId.ParseExact(IdDisplay);
                }
                catch
                {
                    return default;
                }
            }
        }

        [StringLength(255)]
        public string Name { get; init; }

        [StringLength(3000)]
        public string Description { get; init; }
    }
}
