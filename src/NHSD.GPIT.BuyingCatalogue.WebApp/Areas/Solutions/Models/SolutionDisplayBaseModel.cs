using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public abstract class SolutionDisplayBaseModel
    {
        private const string KeyDescription = "Description";

        private static readonly string ControllerName = typeof(SolutionsController).ControllerName();

        private IList<SectionModel> sections;

        protected SolutionDisplayBaseModel()
        {
        }

        protected SolutionDisplayBaseModel(CatalogueItem catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            SolutionId = catalogueItem.Id;
            SolutionName = catalogueItem.Name;
            PublicationStatus = catalogueItem.PublishedStatus;

            ClientApplication = catalogueItem.Solution.ClientApplication == null
                ? new ClientApplication()
                : JsonDeserializer.Deserialize<ClientApplication>(catalogueItem.Solution.ClientApplication);

            LastReviewed = catalogueItem.Solution.LastUpdated;

            SetSections(catalogueItem);
            SetPaginationFooter();
        }

        public ClientApplication ClientApplication { get; set; }

        public abstract int Index { get; }

        public DateTime LastReviewed { get; set; }

        public PaginationFooterModel PaginationFooter { get; set; } = new();

        public virtual string Section => sections[Index].Name;

        public CatalogueItemId SolutionId { get; set; }

        public string SolutionName { get; set; }

        public PublicationStatus PublicationStatus { get; }

        public virtual IList<SectionModel> GetSections()
        {
            var sectionsToShow = new List<SectionModel>(sections.Where(s => s.Show));

            sectionsToShow.ForEach(s => s.SolutionId = SolutionId.ToString());

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

        public bool IsInRemediation() => PublicationStatus == PublicationStatus.InRemediation;

        public bool IsSuspended() => PublicationStatus == PublicationStatus.Suspended;

        private void SetSections(CatalogueItem catalogueItem)
        {
            sections = new List<SectionModel>
            {
                new()
                {
                    Action = nameof(SolutionsController.Description),
                    Controller = ControllerName,
                    Name = KeyDescription,
                    Show = true,
                },
                new()
                {
                    Action = nameof(SolutionsController.Features),
                    Controller = ControllerName,
                    Name = "Features",
                    Show = catalogueItem.HasFeatures(),
                },
                new()
                {
                    Action = nameof(SolutionsController.Capabilities),
                    Controller = ControllerName,
                    Name = "Capabilities",
                    Show = catalogueItem.HasCapabilities(),
                },
                new()
                {
                    Action = nameof(SolutionsController.Standards),
                    Controller = ControllerName,
                    Name = "Standards",
                    Show = catalogueItem.HasStandards(),
                },
                new()
                {
                    Action = nameof(SolutionsController.ListPrice),
                    Controller = ControllerName,
                    Name = "List price",
                    Show = catalogueItem.HasListPrice(),
                },
                new()
                {
                    Action = nameof(SolutionsController.AdditionalServices),
                    Controller = ControllerName,
                    Name = "Additional Services",
                    Show = catalogueItem.HasAdditionalServices(),
                },
                new()
                {
                    Action = nameof(SolutionsController.AssociatedServices),
                    Controller = ControllerName,
                    Name = "Associated Services",
                    Show = catalogueItem.HasAssociatedServices(),
                },
                new()
                {
                    Action = nameof(SolutionsController.Interoperability),
                    Controller = ControllerName,
                    Name = nameof(SolutionsController.Interoperability),
                    Show = catalogueItem.HasInteroperability(),
                },
                new()
                {
                    Action = nameof(SolutionsController.Implementation),
                    Controller = ControllerName,
                    Name = "Implementation",
                    Show = catalogueItem.HasImplementationDetail(),
                },
                new()
                {
                    Action = nameof(SolutionsController.ClientApplicationTypes),
                    Controller = ControllerName,
                    Name = "Client application type",
                    Show = catalogueItem.HasClientApplication(),
                },
                new()
                {
                    Action = nameof(SolutionsController.HostingType),
                    Controller = ControllerName,
                    Name = "Hosting type",
                    Show = catalogueItem.HasHosting(),
                },
                new()
                {
                    Action = nameof(SolutionsController.ServiceLevelAgreement),
                    Controller = ControllerName,
                    Name = "Service Level Agreement",
                    Show = catalogueItem.HasServiceLevelAgreement(),
                },
                new()
                {
                    Action = nameof(SolutionsController.Description),
                    Controller = ControllerName,
                    Name = "Development plans",
                    Show = catalogueItem.HasDevelopmentPlans(),
                },
                new()
                {
                    Action = nameof(SolutionsController.SupplierDetails),
                    Controller = ControllerName,
                    Name = "Supplier details",
                    Show = catalogueItem.HasSupplierDetails(),
                },
            };
        }
    }
}
