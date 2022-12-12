using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices
{
    public sealed class EditAdditionalServiceModel : NavBaseModel
    {
        public EditAdditionalServiceModel()
        {
        }

        public EditAdditionalServiceModel(CatalogueItem solution, CatalogueItem additionalService)
            : this()
        {
            SolutionId = solution.Id;
            SolutionName = solution.Name;
            AdditionalServiceId = additionalService.Id;
            AdditionalServiceName = additionalService.Name;
            SelectedPublicationStatus = additionalService.PublishedStatus;
            AdditionalServicePublicationStatus = additionalService.PublishedStatus;

            DetailsStatus = (!string.IsNullOrEmpty(additionalService.AdditionalService.FullDescription)
                && !string.IsNullOrEmpty(additionalService.Name))
                ? TaskProgress.Completed
                : TaskProgress.NotStarted;

            CapabilitiesStatus = additionalService.CatalogueItemCapabilities.Any()
                ? TaskProgress.Completed
                : TaskProgress.NotStarted;

            ListPriceStatus = additionalService.CataloguePrices.Any(cp => cp.PublishedStatus == PublicationStatus.Published)
                ? TaskProgress.Completed
                : additionalService.CataloguePrices.Any()
                    ? TaskProgress.InProgress
                    : TaskProgress.NotStarted;
        }

        public CatalogueItemId SolutionId { get; init; }

        public CatalogueItemId AdditionalServiceId { get; init; }

        public string SolutionName { get; init; }

        public string AdditionalServiceName { get; init; }

        public PublicationStatus AdditionalServicePublicationStatus { get; init; }

        public PublicationStatus SelectedPublicationStatus { get; set; }

        public IReadOnlyList<SelectOption<string>> PublicationStatuses => AdditionalServicePublicationStatus
            .GetAvailablePublicationStatuses(CatalogueItemType.AdditionalService)
            .Select(p => new SelectOption<string>(p.Description(), p.EnumMemberName()))
            .ToList();

        public TaskProgress DetailsStatus { get; init; }

        public TaskProgress CapabilitiesStatus { get; init; }

        public TaskProgress ListPriceStatus { get; init; }
    }
}
