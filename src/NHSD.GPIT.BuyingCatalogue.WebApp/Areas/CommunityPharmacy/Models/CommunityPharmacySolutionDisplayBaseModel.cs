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
    private const string SolutionIdString = "solutionId";
    private static readonly string ControllerName = typeof(CommunityPharmacyController).ControllerName();

    protected CommunityPharmacySolutionDisplayBaseModel(
        CatalogueItem catalogueItem,
        CatalogueItemContentStatus contentStatus,
        bool isSubPage = false)
        : base(catalogueItem, contentStatus, isSubPage)
    {
        SetBreadcrumb();
        SetSections();
        SetPaginationFooter();
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
                    Action = nameof(HomeController.Index),
                    Controller = typeof(HomeController).ControllerName(),
                    Name = "Community pharmacy",
                    Show = true,
                    RouteData = new Dictionary<string, string> { { "area", typeof(HomeController).AreaName() }, },
                },
                new()
                {
                    Action = nameof(CommunityPharmacyController.Solutions),
                    Controller = typeof(CommunityPharmacyController).ControllerName(),
                    Name = "Pharmacy solutions",
                    Show = true,
                    RouteData = new Dictionary<string, string> { { "area", typeof(CommunityPharmacyController).AreaName() }, },
                },
            };
        }

    private void SetSections()
        {
            Sections = new List<SectionModel>
            {
                new()
                {
                    Action = nameof(CommunityPharmacyController.Summary),
                    Controller = ControllerName,
                    Name = "Summary",
                    Show = CatalogueItemContentStatus.ShowDescription,
                    RouteData = new Dictionary<string, string> { { SolutionIdString, SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(CommunityPharmacyController.ListPrice),
                    Controller = ControllerName,
                    Name = "List price",
                    Show = CatalogueItemContentStatus.ShowListPrice,
                    RouteData = new Dictionary<string, string> { { SolutionIdString, SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(CommunityPharmacyController.Capabilities),
                    Controller = ControllerName,
                    Name = "Capabilities",
                    Show = CatalogueItemContentStatus.ShowCapabilities,
                    RouteData = new Dictionary<string, string> { { SolutionIdString, SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(CommunityPharmacyController.Standards),
                    Controller = ControllerName,
                    Name = "Standards",
                    Show = CatalogueItemContentStatus.ShowStandards,
                    RouteData = new Dictionary<string, string> { { SolutionIdString, SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(CommunityPharmacyController.Integrations),
                    Controller = ControllerName,
                    Name = "Integrations",
                    Show = true,
                    RouteData = new Dictionary<string, string> { { SolutionIdString, SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(CommunityPharmacyController.ApplicationTypes),
                    Controller = ControllerName,
                    Name = "Application type",
                    Show = CatalogueItemContentStatus.ShowApplicationsType,
                    RouteData = new Dictionary<string, string> { { SolutionIdString, SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(CommunityPharmacyController.HostingType),
                    Controller = ControllerName,
                    Name = "Hosting type",
                    Show = true,
                    RouteData = new Dictionary<string, string> { { SolutionIdString, SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(CommunityPharmacyController.DevelopmentPlans),
                    Controller = ControllerName,
                    Name = "Development plans",
                    Show = CatalogueItemContentStatus.ShowDevelopmentPlans,
                    RouteData = new Dictionary<string, string> { { SolutionIdString, SolutionId.ToString() }, },
                },
                new()
                {
                    Action = nameof(CommunityPharmacyController.SupplierDetails),
                    Controller = ControllerName,
                    Name = "Supplier contact details",
                    Show = CatalogueItemContentStatus.ShowSupplierDetails,
                    RouteData = new Dictionary<string, string> { { SolutionIdString, SolutionId.ToString() }, },
                },
            };
        }
}
