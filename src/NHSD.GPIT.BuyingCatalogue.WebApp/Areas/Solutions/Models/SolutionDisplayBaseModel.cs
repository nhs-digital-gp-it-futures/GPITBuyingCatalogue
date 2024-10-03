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

        private string title;
        private string caption;

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

            SetSections(contentStatus);
            SetBreadcrumb();
            SetPaginationFooter();

            ShowBackToTop = true;
            ShowSideNavigation = !IsSuspended();
            ShowPagination = !IsSuspended() && !IsSubPage;
        }

        public bool IsSubPage { get; private set; }

        public DateTime LastReviewed { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public string SolutionName { get; set; }

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

        public bool HasExpiredFrameworks => Frameworks.Any(x => x.IsExpired);

        public string FrameworkTitle() => Frameworks is not null && Frameworks.Any() && Frameworks.Count > 1
            ? "Frameworks"
            : "Framework";

        public bool IsInRemediation() => PublicationStatus == PublicationStatus.InRemediation;

        public bool IsSuspended() => PublicationStatus == PublicationStatus.Suspended;

        private void SetBreadcrumb()
        {
            BreadcrumbItems = new List<SectionModel>()
            {
                new()
                {
                    Action = nameof(HomeController.Index),
                    Controller = typeof(HomeController).ControllerName(),
                    Name = "Home",
                    Show = true,
                    RouteData = new Dictionary<string, string> { { "area", typeof(HomeController).AreaName() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.Index),
                    Controller = typeof(SolutionsController).ControllerName(),
                    Name = "Catalogue Solutions",
                    Show = true,
                    RouteData = new Dictionary<string, string> { { "area", typeof(SolutionsController).AreaName() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.Description),
                    Controller = typeof(SolutionsController).ControllerName(),
                    Name = SolutionName,
                    Show = NotFirstSection,
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
            };
        }

        private void SetSections(CatalogueItemContentStatus contentStatus)
        {
            Sections = new List<SectionModel>
            {
                new()
                {
                    Action = nameof(SolutionsController.Description),
                    Controller = ControllerName,
                    Name = KeyDescription,
                    Show = CatalogueItemContentStatus.ShowDescription,
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.Features),
                    Controller = ControllerName,
                    Name = "Features",
                    Show = contentStatus.ShowFeatures,
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.ListPrice),
                    Controller = ControllerName,
                    Name = "List price",
                    Show = CatalogueItemContentStatus.ShowListPrice,
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.Capabilities),
                    Controller = ControllerName,
                    Name = "Capabilities and Epics",
                    Show = CatalogueItemContentStatus.ShowCapabilities,
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.AdditionalServices),
                    Controller = ControllerName,
                    Name = "Additional Services",
                    Show = contentStatus.ShowAdditionalServices,
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.AssociatedServices),
                    Controller = ControllerName,
                    Name = "Associated Services",
                    Show = contentStatus.ShowAssociatedServices,
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.Implementation),
                    Controller = ControllerName,
                    Name = "Implementation",
                    Show = contentStatus.ShowImplementation,
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.SupplierDetails),
                    Controller = ControllerName,
                    Name = "Supplier details",
                    Show = CatalogueItemContentStatus.ShowSupplierDetails,
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.Interoperability),
                    Controller = ControllerName,
                    Name = nameof(SolutionsController.Interoperability),
                    Show = contentStatus.ShowInteroperability,
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.ApplicationTypes),
                    Controller = ControllerName,
                    Name = "Application type",
                    Show = CatalogueItemContentStatus.ShowApplicationsType,
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.HostingType),
                    Controller = ControllerName,
                    Name = "Hosting type",
                    Show = contentStatus.ShowHosting,
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.DataProcessingInformation),
                    Controller = ControllerName,
                    Name = "Data processing information",
                    Show = CatalogueItemContentStatus.ShowDataProcessingInformation,
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.Standards),
                    Controller = ControllerName,
                    Name = "Standards",
                    Show = CatalogueItemContentStatus.ShowStandards,
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.ServiceLevelAgreement),
                    Controller = ControllerName,
                    Name = "Service Level Agreement",
                    Show = CatalogueItemContentStatus.ShowServiceLevelAgreements,
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.DevelopmentPlans),
                    Controller = ControllerName,
                    Name = "Development plans",
                    Show = CatalogueItemContentStatus.ShowDevelopmentPlans,
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
            };
        }
    }
}
