﻿using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices
{
    public sealed class EditAssociatedServiceModel : NavBaseModel
    {
        public EditAssociatedServiceModel()
        {
        }

        public EditAssociatedServiceModel(CatalogueItem solution, CatalogueItem associatedService)
            : this()
        {
            SolutionId = solution.Id;
            SolutionName = solution.Name;
            SupplierName = solution.Supplier.Name;
            AssociatedServiceId = associatedService.Id;
            AssociatedServiceName = associatedService.Name;
            SelectedPublicationStatus = associatedService.PublishedStatus;
            AssociatedServicePublicationStatus = associatedService.PublishedStatus;

            DetailsStatus = (!string.IsNullOrEmpty(associatedService.AssociatedService.Description)
                && !string.IsNullOrEmpty(associatedService.AssociatedService.OrderGuidance)
                && !string.IsNullOrEmpty(associatedService.Name))
                ? TaskProgress.Completed
                : TaskProgress.NotStarted;

            ListPriceStatus = associatedService.CataloguePrices.Any(cp => cp.PublishedStatus == PublicationStatus.Published)
                ? TaskProgress.Completed
                : associatedService.CataloguePrices.Any()
                    ? TaskProgress.InProgress
                    : TaskProgress.NotStarted;
        }

        public EditAssociatedServiceModel(CatalogueItem solution, CatalogueItem associatedService, IList<CatalogueItem> relatedSolutions)
            : this(solution, associatedService)
        {
            RelatedSolutions = relatedSolutions;
        }

        public CatalogueItemId SolutionId { get; init; }

        public CatalogueItemId AssociatedServiceId { get; init; }

        public string SolutionName { get; init; }

        public string AssociatedServiceName { get; init; }

        public string SupplierName { get; init; }

        public PublicationStatus AssociatedServicePublicationStatus { get; set; }

        public PublicationStatus SelectedPublicationStatus { get; set; }

        public IReadOnlyList<SelectOption<string>> PublicationStatuses => AssociatedServicePublicationStatus
                .GetAvailablePublicationStatuses(CatalogueItemType.AssociatedService)
                .Select(p => new SelectOption<string>(p.Description(), p.EnumMemberName()))
                .ToList();

        public TaskProgress DetailsStatus { get; init; }

        public TaskProgress ListPriceStatus { get; init; }

        public IList<CatalogueItem> RelatedSolutions { get; set; } = new List<CatalogueItem>();
    }
}
