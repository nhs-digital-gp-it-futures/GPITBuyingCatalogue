using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public abstract class SolutionDisplayBaseModel
    {
        private const string KeyDescription = "Summary";

        private static readonly string ControllerName = typeof(SolutionsController).ControllerName();

        private IList<SectionModel> sections;

        protected SolutionDisplayBaseModel()
        {
        }

        protected SolutionDisplayBaseModel(
            CatalogueItem catalogueItem,
            CatalogueItemContentStatus contentStatus,
            bool isSubPage = false)
        {
            IsSubPage = isSubPage;
            if (catalogueItem?.Solution is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            SolutionId = catalogueItem.Id;
            SolutionName = catalogueItem.Name;
            PublicationStatus = catalogueItem.PublishedStatus;

            IsPilotSolution = catalogueItem.Solution.IsPilotSolution;
            LastReviewed = catalogueItem.Solution.LastUpdated;
            Frameworks = catalogueItem.Solution.FrameworkSolutions.Select(x => x.Framework).Distinct().ToList();
            SupplierName = catalogueItem.Supplier.Name;
            IsFoundation = catalogueItem.Solution.FrameworkSolutions.Any(fs => fs.IsFoundation).ToYesNo();

            SetSections(contentStatus);
            SetPaginationFooter();
        }

        public bool IsSubPage { get; private set; }

        public abstract int Index { get; }

        public DateTime LastReviewed { get; set; }

        public PaginationFooterModel PaginationFooter { get; set; } = new();

        public virtual string Section => sections[Index].Name;

        public CatalogueItemId SolutionId { get; set; }

        public string SolutionName { get; set; }

        public PublicationStatus PublicationStatus { get; }

        public bool IsPilotSolution { get; }

        public List<BuyingCatalogue.EntityFramework.Catalogue.Models.Framework> Frameworks { get; set; }

        public string SupplierName { get; }

        public string IsFoundation { get; }

        public string Title { set; get; }

        public bool HasExpiredFrameworks => Frameworks.Any(x => x.IsExpired);

        public string FrameworkTitle() => Frameworks is not null && Frameworks.Any() && Frameworks.Count > 1
            ? "Frameworks"
            : "Framework";

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

        private void SetSections(CatalogueItemContentStatus contentStatus)
        {
            sections = new List<SectionModel>
            {
                new()
                {
                    Action = nameof(SolutionsController.Description),
                    Controller = ControllerName,
                    Name = KeyDescription,
                    Show = CatalogueItemContentStatus.ShowDescription,
                },
                new()
                {
                    Action = nameof(SolutionsController.Features),
                    Controller = ControllerName,
                    Name = "Features",
                    Show = contentStatus.ShowFeatures,
                },
                new()
                {
                    Action = nameof(SolutionsController.ListPrice),
                    Controller = ControllerName,
                    Name = "List price",
                    Show = CatalogueItemContentStatus.ShowListPrice,
                },
                new()
                {
                    Action = nameof(SolutionsController.Capabilities),
                    Controller = ControllerName,
                    Name = "Capabilities and Epics",
                    Show = CatalogueItemContentStatus.ShowCapabilities,
                },
                new()
                {
                    Action = nameof(SolutionsController.AdditionalServices),
                    Controller = ControllerName,
                    Name = "Additional Services",
                    Show = contentStatus.ShowAdditionalServices,
                },
                new()
                {
                    Action = nameof(SolutionsController.AssociatedServices),
                    Controller = ControllerName,
                    Name = "Associated Services",
                    Show = contentStatus.ShowAssociatedServices,
                },
                new()
                {
                    Action = nameof(SolutionsController.Implementation),
                    Controller = ControllerName,
                    Name = "Implementation",
                    Show = contentStatus.ShowImplementation,
                },
                new()
                {
                    Action = nameof(SolutionsController.SupplierDetails),
                    Controller = ControllerName,
                    Name = "Supplier details",
                    Show = CatalogueItemContentStatus.ShowSupplierDetails,
                },
                new()
                {
                    Action = nameof(SolutionsController.Interoperability),
                    Controller = ControllerName,
                    Name = nameof(SolutionsController.Interoperability),
                    Show = contentStatus.ShowInteroperability,
                },
                new()
                {
                    Action = nameof(SolutionsController.ApplicationTypes),
                    Controller = ControllerName,
                    Name = "Application type",
                    Show = CatalogueItemContentStatus.ShowApplicationsType,
                },
                new()
                {
                    Action = nameof(SolutionsController.HostingType),
                    Controller = ControllerName,
                    Name = "Hosting type",
                    Show = contentStatus.ShowHosting,
                },
                new()
                {
                    Action = nameof(SolutionsController.Standards),
                    Controller = ControllerName,
                    Name = "Standards",
                    Show = CatalogueItemContentStatus.ShowStandards,
                },
                new()
                {
                    Action = nameof(SolutionsController.ServiceLevelAgreement),
                    Controller = ControllerName,
                    Name = "Service Level Agreement",
                    Show = CatalogueItemContentStatus.ShowServiceLevelAgreements,
                },
                new()
                {
                    Action = nameof(SolutionsController.DevelopmentPlans),
                    Controller = ControllerName,
                    Name = "Development plans",
                    Show = CatalogueItemContentStatus.ShowDevelopmentPlans,
                },
            };
        }
    }
}
