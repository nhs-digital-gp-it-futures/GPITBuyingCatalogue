using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsSideNavigationSection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models
{
    public abstract class YourAccountBaseModel : SideNavigationBaseModel
    {
        protected YourAccountBaseModel()
        {
            SetSections();
            SetPaginationFooter();

            ShowBackToTop = false;
            ShowSideNavigation = true;
            ShowPagination = false;
        }

        private void SetSections()
        {
            Sections = new List<NhsSideNavigationSectionModel>
            {
                new()
                {
                    Action = nameof(YourAccountController.Index),
                    Controller = typeof(YourAccountController).ControllerName(),
                    RouteData = new Dictionary<string, string> { { "area", typeof(YourAccountController).AreaName() }, },
                    Name = "Your account",
                },
                new()
                {
                    Action = nameof(AccountController.Logout),
                    Controller = typeof(AccountController).ControllerName(),
                    RouteData = new Dictionary<string, string> { { "area", typeof(YourAccountController).AreaName() }, },
                    Name = "Log out",
                },
            };
        }
    }
}
