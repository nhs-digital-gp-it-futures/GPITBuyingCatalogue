using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices
{
    public sealed class EditAdditionalServiceModel : NavBaseModel
    {
        public EditAdditionalServiceModel(CatalogueItem catalogueItem, CatalogueItem additionalService)
        {
            Solution = catalogueItem;
            AdditionalService = additionalService;
            BackLinkText = "Go back";
        }

        public CatalogueItem Solution { get; init; }

        public CatalogueItem AdditionalService { get; init; }

        public TaskProgress DetailsStatus()
        {
            if (!string.IsNullOrEmpty(AdditionalService.AdditionalService.FullDescription)
                && !string.IsNullOrEmpty(AdditionalService.Name))
                return TaskProgress.Completed;

            return TaskProgress.NotStarted;
        }

        public TaskProgress CapabilitiesStatus() => AdditionalService.CatalogueItemCapabilities.Any()
            ? TaskProgress.Completed
            : TaskProgress.NotStarted;

        public TaskProgress ListPriceStatus() => AdditionalService.CataloguePrices.Any()
            ? TaskProgress.Completed
            : TaskProgress.NotStarted;
    }
}
