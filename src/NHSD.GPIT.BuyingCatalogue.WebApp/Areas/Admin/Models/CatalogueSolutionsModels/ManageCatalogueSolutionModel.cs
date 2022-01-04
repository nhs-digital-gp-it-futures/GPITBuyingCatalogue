﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Admin;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels
{
    public sealed class ManageCatalogueSolutionModel : NavBaseModel
    {
        public ManageCatalogueSolutionModel()
        {
        }

        public ManageCatalogueSolutionModel(SolutionLoadingStatusesModel solutionLoadingStatuses, CatalogueItem solution)
        {
            SolutionId = solution.Id;
            SolutionName = solution.Name;
            SupplierName = solution.Supplier.Name;
            LastUpdated = solution.Solution.LastUpdated;
            var lastUpdatedBy = solution.Solution.LastUpdatedByUser;
            if (lastUpdatedBy is not null)
                LastUpdatedByName = $"{lastUpdatedBy.FirstName} {lastUpdatedBy.LastName}";
            SelectedPublicationStatus = solution.PublishedStatus;
            SolutionPublicationStatus = solution.PublishedStatus;

            DescriptionStatus = solutionLoadingStatuses.Description;
            FeaturesStatus = solutionLoadingStatuses.Features;
            AdditionalServicesStatus = solutionLoadingStatuses.AdditionalServices;
            AssociatedServicesStatus = solutionLoadingStatuses.AssociatedServices;
            InteroperabilityStatus = solutionLoadingStatuses.Interoperability;
            ImplementationStatus = solutionLoadingStatuses.Implementation;
            ClientApplicationTypeStatus = solutionLoadingStatuses.ClientApplicationType;
            HostingTypeStatus = solutionLoadingStatuses.HostingType;
            ListPriceStatus = solutionLoadingStatuses.ListPrice;
            CapabilitiesStatus = solutionLoadingStatuses.CapabilitiesAndEpics;
            RoadmapStatus = solutionLoadingStatuses.DevelopmentPlans;
            SupplierDetailsStatus = solutionLoadingStatuses.SupplierDetails;
            SlaStatus = solutionLoadingStatuses.ServiceLevelAgreement;
        }

        public CatalogueItemId SolutionId { get; init; }

        public string SolutionName { get; init; }

        public PublicationStatus SolutionPublicationStatus { get; init; }

        public string SupplierName { get; init; }

        public DateTime LastUpdated { get; init; }

        public string LastUpdatedByName { get; init; }

        public PublicationStatus SelectedPublicationStatus { get; set; }

        public IReadOnlyList<SelectListItem> PublicationStatuses => SolutionPublicationStatus
                .GetAvailablePublicationStatuses(CatalogueItemType.Solution)
                .Select(p => new SelectListItem(p.Description(), p.EnumMemberName()))
                .ToList();

        public TaskProgress DescriptionStatus { get; init; }

        public TaskProgress FeaturesStatus { get; init; }

        public TaskProgress ImplementationStatus { get; init; }

        public TaskProgress RoadmapStatus { get; init; }

        public TaskProgress HostingTypeStatus { get; init; }

        public TaskProgress ClientApplicationTypeStatus { get; init; }

        public TaskProgress InteroperabilityStatus { get; init; }

        public TaskProgress ListPriceStatus { get; init; }

        public TaskProgress AssociatedServicesStatus { get; init; }

        public TaskProgress AdditionalServicesStatus { get; init; }

        public TaskProgress SupplierDetailsStatus { get; init; }

        public TaskProgress SlaStatus { get; init; }

        public TaskProgress CapabilitiesStatus { get; init; }
    }
}
