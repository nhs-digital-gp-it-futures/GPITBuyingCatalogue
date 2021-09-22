﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public abstract class SolutionDisplayBaseModel
    {
        private const string KeyDescription = "Description";

        private static readonly string ControllerName = typeof(SolutionsController).ControllerName();
        private static readonly List<Func<CatalogueItem, bool>> ShowSectionFunctions = new()
        {
            { _ => true },
            { catalogueItem => catalogueItem.HasFeatures() },
            { catalogueItem => catalogueItem.HasCapabilities() },
            { catalogueItem => catalogueItem.HasListPrice() },
            { catalogueItem => catalogueItem.HasAdditionalServices() },
            { catalogueItem => catalogueItem.HasAssociatedServices() },
            { catalogueItem => catalogueItem.HasInteroperability() },
            { catalogueItem => catalogueItem.HasImplementationDetail() },
            { catalogueItem => catalogueItem.HasClientApplication() },
            { catalogueItem => catalogueItem.HasHosting() },
            { catalogueItem => catalogueItem.HasServiceLevelAgreement() },
            { catalogueItem => catalogueItem.HasDevelopmentPlans() },
            { catalogueItem => catalogueItem.HasSupplierDetails() },
        };

        private readonly IList<SectionModel> sections = new List<SectionModel>
        {
            new()
            {
                Action = nameof(SolutionsController.Description),
                Controller = ControllerName,
                Name = KeyDescription,
            },
            new()
            {
                Action = nameof(SolutionsController.Features),
                Controller = ControllerName,
                Name = "Features",
            },
            new()
            {
                Action = nameof(SolutionsController.Capabilities),
                Controller = ControllerName,
                Name = "Capabilities",
            },
            new()
            {
                Action = nameof(SolutionsController.ListPrice),
                Controller = ControllerName,
                Name = "List price",
            },
            new()
            {
                Action = nameof(SolutionsController.AdditionalServices),
                Controller = ControllerName,
                Name = "Additional Services",
            },
            new()
            {
                Action = nameof(SolutionsController.AssociatedServices),
                Controller = ControllerName,
                Name = "Associated Services",
            },
            new()
            {
                Action = nameof(SolutionsController.Interoperability),
                Controller = ControllerName,
                Name = nameof(SolutionsController.Interoperability),
            },
            new()
            {
                Action = nameof(SolutionsController.Implementation),
                Controller = ControllerName,
                Name = "Implementation",
            },
            new()
            {
                Action = nameof(SolutionsController.ClientApplicationTypes),
                Controller = ControllerName,
                Name = "Client application type",
            },
            new()
            {
                Action = nameof(SolutionsController.HostingType),
                Controller = ControllerName,
                Name = "Hosting type",
            },
            new()
            {
                Action = nameof(SolutionsController.Description),
                Controller = ControllerName,
                Name = "Service Level Agreement",
            },
            new()
            {
                Action = nameof(SolutionsController.Description),
                Controller = ControllerName,
                Name = "Development plans",
            },
            new()
            {
                Action = nameof(SolutionsController.SupplierDetails),
                Controller = ControllerName,
                Name = "Supplier details",
            },
        };

        protected SolutionDisplayBaseModel()
        {
        }

        protected SolutionDisplayBaseModel(Solution solution)
            : this(solution.CatalogueItem)
        {
            ClientApplication = solution.ClientApplication == null
                ? new ClientApplication()
                : JsonSerializer.Deserialize<ClientApplication>(
                    solution.ClientApplication,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            LastReviewed = solution.LastUpdated;
        }

        private SolutionDisplayBaseModel(CatalogueItem catalogueItem)
        {
            SolutionId = catalogueItem.Id;
            SolutionName = catalogueItem.Name;

            SetVisibleSections(catalogueItem);
            SetPaginationFooter();
        }

        public ClientApplication ClientApplication { get; set; }

        public abstract int Index { get; }

        public DateTime LastReviewed { get; set; }

        public PaginationFooterModel PaginationFooter { get; set; } = new();

        public virtual string Section => sections[Index].Name;

        public CatalogueItemId SolutionId { get; set; }

        public string SolutionName { get; set; }

        public virtual IList<SectionModel> GetSections()
        {
            var sectionsToShow = new List<SectionModel>(sections.Where(s => s.Show));

            // TODO: what is ID in section model? Should it be CatalogueItemId?
            sectionsToShow.ForEach(s => s.Id = SolutionId.ToString());

            if (sectionsToShow.FirstOrDefault(s => s.Name.EqualsIgnoreCase(Section)) is { } sectionModel)
                sectionModel.Selected = true;

            return sectionsToShow;
        }

        public bool NotFirstSection() => !Section.EqualsIgnoreCase(KeyDescription);

        public void SetPaginationFooter()
        {
            var sectionsToShow = new List<SectionModel>(sections.Where(s => s.Show));
            if (sectionsToShow.FirstOrDefault(s => s.Name.EqualsIgnoreCase(Section)) is not { } sectionModel)
                return;

            var index = sectionsToShow.IndexOf(sectionModel);

            PaginationFooter.Previous = index > 0 ? sectionsToShow[index - 1] : null;
            PaginationFooter.Next = index < (sectionsToShow.Count - 1) ? sectionsToShow[index + 1] : null;
        }

        public void SetShowTrue(int index) => sections[index].Show = true;

        private void SetVisibleSections(CatalogueItem solution)
        {
            for (var i = 0; i < ShowSectionFunctions.Count; i++)
            {
                if (ShowSectionFunctions[i](solution))
                    SetShowTrue(i);
            }
        }
    }
}
