using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices
{
    public sealed class EditAssociatedServiceModel : NavBaseModel
    {
        public EditAssociatedServiceModel()
        {
            BackLinkText = "Go back";
        }

        public EditAssociatedServiceModel(CatalogueItem solution, CatalogueItem associatedService)
            : this()
        {
            SolutionId = solution.Id;
            SolutionName = solution.Name;
            AssociatedServiceId = associatedService.Id;
            AssociatedServiceName = associatedService.Name;
            SelectedPublicationStatus = associatedService.PublishedStatus;
            AssociatedServicePublicationStatus = associatedService.PublishedStatus;

            DetailsStatus = (!string.IsNullOrEmpty(associatedService.AssociatedService.Description)
                && !string.IsNullOrEmpty(associatedService.AssociatedService.OrderGuidance)
                && !string.IsNullOrEmpty(associatedService.Name))
                ? TaskProgress.Completed
                : TaskProgress.NotStarted;

            ListPriceStatus = associatedService.CataloguePrices.Any()
                ? TaskProgress.Completed
                : TaskProgress.NotStarted;
        }

        public CatalogueItemId SolutionId { get; init; }

        public CatalogueItemId AssociatedServiceId { get; init; }

        public string SolutionName { get; init; }

        public string AssociatedServiceName { get; init; }

        public PublicationStatus AssociatedServicePublicationStatus { get; init; }

        public PublicationStatus SelectedPublicationStatus { get; set; }

        public IReadOnlyList<SelectListItem> PublicationStatuses => AssociatedServicePublicationStatus
                .GetAvailablePublicationStatuses(CatalogueItemType.AssociatedService)
                .Select(p => new SelectListItem(p.Description(), p.EnumMemberName()))
                .ToList();

        public TaskProgress DetailsStatus { get; init; }

        public TaskProgress ListPriceStatus { get; init; }
    }
}
