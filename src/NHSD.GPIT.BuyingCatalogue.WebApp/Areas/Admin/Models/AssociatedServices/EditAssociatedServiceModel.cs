using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices
{
    public sealed class EditAssociatedServiceModel : NavBaseModel
    {
        public EditAssociatedServiceModel(CatalogueItem catalogueItem, CatalogueItem associatedService)
        {
            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/associated-services";
            BackLinkText = "Go back";
            Solution = catalogueItem;
            AssociatedService = associatedService;
        }

        public CatalogueItem Solution { get; }

        public CatalogueItem AssociatedService { get; }

        public TaskProgress DetailsStatus()
        {
            if (!string.IsNullOrEmpty(AssociatedService.AssociatedService.Description)
                && !string.IsNullOrEmpty(AssociatedService.AssociatedService.OrderGuidance)
                && !string.IsNullOrEmpty(AssociatedService.Name))
                return TaskProgress.Completed;

            return TaskProgress.NotStarted;
        }

        public TaskProgress ListPriceStatus() => AssociatedService.CataloguePrices.Any()
            ? TaskProgress.Completed
            : TaskProgress.NotStarted;
    }
}
