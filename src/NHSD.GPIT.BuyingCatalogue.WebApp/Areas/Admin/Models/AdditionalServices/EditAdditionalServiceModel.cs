using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices
{
    public sealed class EditAdditionalServiceModel : NavBaseModel
    {
        public EditAdditionalServiceModel()
        {
        }

        public EditAdditionalServiceModel(CatalogueItem catalogueItem, CatalogueItem additionalService)
        {
            Solution = catalogueItem;
            AdditionalService = additionalService;
            BackLinkText = "Go back";
            SelectedPublicationStatus = additionalService.PublishedStatus;
            PublicationStatuses = additionalService
                .PublishedStatus
                .GetAvailablePublicationStatuses(additionalService.CatalogueItemType)
                .Select(p => new SelectListItem(p.Description(), p.EnumMemberName()))
                .ToList();
        }

        public CatalogueItem Solution { get; init; }

        public CatalogueItem AdditionalService { get; init; }

        public IReadOnlyList<SelectListItem> PublicationStatuses { get; }

        public PublicationStatus SelectedPublicationStatus { get; set; }

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
