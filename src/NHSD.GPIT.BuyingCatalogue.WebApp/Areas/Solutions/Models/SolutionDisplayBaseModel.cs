using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public abstract class SolutionDisplayBaseModel : SideNavigationBaseModel
    {
        private const string KeyDescription = "Summary";

        private static readonly string ControllerName = typeof(SolutionsController).ControllerName();

        private readonly bool shouldShowStandardsCategory;
        private string title;
        private string caption;

        protected SolutionDisplayBaseModel()
        {
        }

        protected SolutionDisplayBaseModel(
            CatalogueItem catalogueItem,
            CatalogueItemContentStatus contentStatus,
            SolutionSections sections,
            bool isSubPage = false)
        {
            IsSubPage = isSubPage;
            if (catalogueItem?.Solution is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            SolutionId = catalogueItem.Id;
            SolutionName = catalogueItem.Name;
            SolutionType = catalogueItem.Solution.SolutionType;
            PublicationStatus = catalogueItem.PublishedStatus;
            SolutionStandardCategory = catalogueItem.Solution.Category;

            IsPilotSolution = catalogueItem.Solution.IsPilotSolution;
            LastReviewed = catalogueItem.Solution.LastUpdated;
            Frameworks = catalogueItem.Solution.FrameworkSolutions.Select(x => x.Framework).Distinct().ToList();
            SupplierName = catalogueItem.Supplier.Name;
            if (Frameworks.Count == 0)
                throw new InvalidOperationException("A solution must have at least one framework.");

            SetSections(contentStatus, sections);
            SetBreadcrumb(catalogueItem.Solution.SolutionType);
            SetPaginationFooter();

            ShowSideNavigation = !IsSuspended();
            ShowPagination = !IsSuspended() && !IsSubPage;
        }

        protected SolutionDisplayBaseModel(
            CatalogueItem catalogueItem,
            CatalogueItemContentStatus contentStatus,
            SolutionSections solutionSections,
            bool shouldShowStandardsCategory,
            bool isSubPage = false)
            : this(catalogueItem, contentStatus, solutionSections, isSubPage)
        {
            this.shouldShowStandardsCategory = shouldShowStandardsCategory;
        }

        public bool IsSubPage { get; set; }

        public DateTime LastReviewed { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public string SolutionName { get; set; }

        public SolutionType SolutionType { get; set; }

        public SolutionCategory? SolutionStandardCategory { get; set; }

        public PublicationStatus PublicationStatus { get; }

        public bool IsPilotSolution { get; }

        public List<BuyingCatalogue.EntityFramework.Catalogue.Models.Framework> Frameworks { get; set; }

        public string SupplierName { get; }

        public override string Title
        {
            get
            {
                return IsSubPage ? title : SelectedSection;
            }

            set
            {
                title = value;
            }
        }

        public override string Caption
        {
            get
            {
                return IsSubPage ? caption : SolutionName;
            }

            set
            {
                caption = value;
            }
        }

        public bool ShouldShowStandardsCategory => SolutionStandardCategory.HasValue && shouldShowStandardsCategory;

        public bool HasExpiredFrameworks => Frameworks.Any(x => x.IsExpired);

        public string FrameworkTitle() => Frameworks is not null && Frameworks.Any() && Frameworks.Count > 1
            ? "Contracting vehicles"
            : "Contracting vehicle";

        public bool IsInRemediation() => PublicationStatus == PublicationStatus.InRemediation;

        public bool IsSuspended() => PublicationStatus == PublicationStatus.Suspended;

        private void SetBreadcrumb(SolutionType solutionType)
        {
            var breadcrumbItems = new List<SectionModel>
            {
                new()
                {
                    Action = nameof(HomeController.Index),
                    Controller = typeof(HomeController).ControllerName(),
                    Name = "Home",
                    Show = true,
                    RouteData = new Dictionary<string, string> { { "area", typeof(HomeController).AreaName() }, },
                },
            };

            if (solutionType == SolutionType.CommunityPharmacy)
            {
                breadcrumbItems.Add(new()
                {
                    Action = nameof(HomeController.CommunityPharmacy),
                    Controller = typeof(HomeController).ControllerName(),
                    Name = "Community Pharmacy",
                    Show = true,
                    RouteData = new Dictionary<string, string> { { "area", typeof(HomeController).AreaName() } },
                });
                breadcrumbItems.Add(new()
                {
                    Action = nameof(SolutionsController.Index),
                    Controller = typeof(SolutionsController).ControllerName(),
                    Name = "Catalogue solutions",
                    Show = true,
                    RouteData = new Dictionary<string, string> { { "area", typeof(SolutionsController).AreaName() }, { "solutionType", nameof(SolutionType.CommunityPharmacy) } },
                });
            }
            else
            {
                breadcrumbItems.Add(new()
                {
                    Action = nameof(HomeController.GPIT),
                    Controller = typeof(HomeController).ControllerName(),
                    Name = "General Practice",
                    Show = true,
                    RouteData = new Dictionary<string, string> { { "area", typeof(HomeController).AreaName() } },
                });
                breadcrumbItems.Add(new()
                {
                    Action = nameof(SolutionsController.Index),
                    Controller = typeof(SolutionsController).ControllerName(),
                    Name = "Catalogue solutions",
                    Show = true,
                    RouteData = new Dictionary<string, string> { { "area", typeof(SolutionsController).AreaName() } },
                });
            }

            BreadcrumbItems = breadcrumbItems;
        }

        private void SetSections(CatalogueItemContentStatus contentStatus, SolutionSections sections)
        {
            Sections = new List<SectionModel>
            {
                new()
                {
                    Action = nameof(SolutionsController.Description),
                    Controller = ControllerName,
                    Name = KeyDescription,
                    Show = ShouldShowSection(sections.Description, CatalogueItemContentStatus.ShowDescription),
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.Features),
                    Controller = ControllerName,
                    Name = "Features",
                    Show = ShouldShowSection(sections.Features, contentStatus.ShowFeatures),
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.ListPrice),
                    Controller = ControllerName,
                    Name = "List price",
                    Show = ShouldShowSection(sections.ListPrice, CatalogueItemContentStatus.ShowListPrice),
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.Capabilities),
                    Controller = ControllerName,
                    Name = "Capabilities and Epics",
                    Show = ShouldShowSection(sections.CapabilitiesAndEpics, CatalogueItemContentStatus.ShowCapabilities),
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.AdditionalServices),
                    Controller = ControllerName,
                    Name = "Additional services",
                    Show = ShouldShowSection(sections.AdditionalServices, contentStatus.ShowAdditionalServices),
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.AssociatedServices),
                    Controller = ControllerName,
                    Name = "Associated services",
                    Show = ShouldShowSection(sections.AssociatedServices, contentStatus.ShowAssociatedServices),
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.Implementation),
                    Controller = ControllerName,
                    Name = "Implementation",
                    Show = ShouldShowSection(sections.Implementation, contentStatus.ShowImplementation),
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.SupplierDetails),
                    Controller = ControllerName,
                    Name = "Supplier details",
                    Show = ShouldShowSection(sections.SupplierDetails, CatalogueItemContentStatus.ShowSupplierDetails),
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.Interoperability),
                    Controller = ControllerName,
                    Name = sections.SolutionType == SolutionType.CommunityPharmacy ? "Integrations" : nameof(SolutionsController.Interoperability),
                    Show = ShouldShowSection(sections.Integrations, contentStatus.ShowInteroperability),
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.ApplicationTypes),
                    Controller = ControllerName,
                    Name = "Application type",
                    Show = ShouldShowSection(sections.ApplicationType, CatalogueItemContentStatus.ShowApplicationsType),
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.HostingType),
                    Controller = ControllerName,
                    Name = "Hosting type",
                    Show = ShouldShowSection(sections.HostingType, contentStatus.ShowHosting),
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.DataProcessingInformation),
                    Controller = ControllerName,
                    Name = "Data processing information",
                    Show = ShouldShowSection(sections.DataProcessingInformation, CatalogueItemContentStatus.ShowDataProcessingInformation),
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.Standards),
                    Controller = ControllerName,
                    Name = "Standards",
                    Show = ShouldShowSection(sections.Standards, CatalogueItemContentStatus.ShowStandards),
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.ServiceLevelAgreement),
                    Controller = ControllerName,
                    Name = "Service Level Agreement",
                    Show = ShouldShowSection(sections.ServiceLevelAgreement, CatalogueItemContentStatus.ShowServiceLevelAgreements),
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.DevelopmentPlans),
                    Controller = ControllerName,
                    Name = "Development plans",
                    Show = ShouldShowSection(sections.DevelopmentPlans, CatalogueItemContentStatus.ShowDevelopmentPlans),
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
            };
        }

        private static bool ShouldShowSection(SolutionSectionRequirement sectionRequirement, bool catalogueItemContentStatus)
        {
            return sectionRequirement switch
            {
                SolutionSectionRequirement.Mandatory => true,
                SolutionSectionRequirement.Optional => catalogueItemContentStatus,
                _ => false,
            };
        }
    }
}
