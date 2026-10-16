using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;

public abstract class CommunityPharmacySolutionDisplayBaseModel : SolutionDisplayBaseModel
{
    private static readonly string ControllerName = typeof(CommunityPharmacyController).ControllerName();

    protected CommunityPharmacySolutionDisplayBaseModel(
        CatalogueItem catalogueItem,
        CatalogueItemContentStatus contentStatus,
        bool isSubPage = false)
        : base(catalogueItem, contentStatus, isSubPage)
    {
        
    }

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
                    Name = "Catalogue solutions",
                    Show = true,
                    RouteData = new Dictionary<string, string> { { "area", typeof(SolutionsController).AreaName() }, },
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
                    Name = "Summary",
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
                    Name = "Additional services",
                    Show = contentStatus.ShowAdditionalServices,
                    RouteData = new Dictionary<string, string> { { "solutionId", SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(SolutionsController.AssociatedServices),
                    Controller = ControllerName,
                    Name = "Associated services",
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
