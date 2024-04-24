using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using HomeController = NHSD.GPIT.BuyingCatalogue.WebApp.Controllers.HomeController;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

public readonly struct NavigationMenuModel
{
    private static readonly
        List<(Func<ClaimsPrincipal, bool> Key, Func<ClaimsPrincipal, IUrlHelper, List<KeyValuePair<string, string>>>
            Factory)> LinksFactories =
            new()
            {
                (
                    user => !user.Identity?.IsAuthenticated ?? false,
                    (_, urlHelper) =>
                    [
                        new(
                            "Home",
                            urlHelper.Action(nameof(HomeController.Index), typeof(HomeController).ControllerName())),

                        new(
                            "Catalogue Solutions",
                            urlHelper.Action(
                                nameof(SolutionsController.Index),
                                typeof(SolutionsController).ControllerName(),
                                new { area = typeof(SolutionsController).AreaName() })),
                    ]),
                (
                    user => user.IsBuyer() || user.IsAccountManager(), (user, urlHelper) =>
                    [
                        new(
                            "Home",
                            urlHelper.Action(nameof(HomeController.Index), typeof(HomeController).ControllerName())),

                        new(
                            "Dashboard",
                            urlHelper.Action(
                                nameof(BuyerDashboardController.Index),
                                typeof(BuyerDashboardController).ControllerName(),
                                new { internalOrgId = user.GetPrimaryOrganisationInternalIdentifier() })),
                        new(
                            "Orders",
                            urlHelper.Action(
                                nameof(DashboardController.Organisation),
                                typeof(DashboardController).ControllerName(),
                                new { Area = typeof(DashboardController).AreaName(), internalOrgId = user.GetPrimaryOrganisationInternalIdentifier() })),
                        new(
                            "Shortlists",
                            urlHelper.Action(
                                nameof(ManageFiltersController.Index),
                                typeof(ManageFiltersController).ControllerName(),
                                new { Area = typeof(ManageFiltersController).AreaName() })),
                        new(
                            "Competitions",
                            urlHelper.Action(
                                nameof(CompetitionsDashboardController.Index),
                                typeof(CompetitionsDashboardController).ControllerName(),
                                new { Area = typeof(CompetitionsDashboardController).AreaName(), internalOrgId = user.GetPrimaryOrganisationInternalIdentifier() })),

                        new(
                            "Catalogue Solutions",
                            urlHelper.Action(
                                nameof(SolutionsController.Index),
                                typeof(SolutionsController).ControllerName(),
                                new { area = typeof(SolutionsController).AreaName() })),
                    ]),
                (
                    user => user.IsAdmin(), (_, urlHelper) =>
                    [
                        new(
                            "Home",
                            urlHelper.Action(
                                nameof(NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers.HomeController.Index),
                                typeof(NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers.HomeController)
                                    .ControllerName(),
                                new
                                {
                                    area =
                                        typeof(NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers.HomeController)
                                            .AreaName(),
                                })),
                        new(
                            "Buyer organisations",
                            urlHelper.Action(
                                nameof(OrganisationsController.Index),
                                typeof(OrganisationsController).ControllerName(),
                                new { area = typeof(OrganisationsController).AreaName() })),
                        new(
                            "Supplier organisations",
                            urlHelper.Action(
                                nameof(SuppliersController.Index),
                                typeof(SuppliersController).ControllerName(),
                                new { area = typeof(OrganisationsController).AreaName() })),
                        new(
                            "Catalogue Solutions",
                            urlHelper.Action(
                                nameof(CatalogueSolutionsController.Index),
                                typeof(CatalogueSolutionsController).ControllerName(),
                                new { area = typeof(OrganisationsController).AreaName() })),
                    ]),
            };

    public NavigationMenuModel(
        ClaimsPrincipal user,
        IUrlHelper urlHelper)
    {
        (_, Func<ClaimsPrincipal, IUrlHelper, List<KeyValuePair<string, string>>> linkFactory) =
            LinksFactories.First(x => x.Key(user));

        Links = linkFactory(user, urlHelper);

        if (user.Identity?.IsAuthenticated ?? false)
        {
            Links.Add(
                new(
                    "Log out",
                    urlHelper.Action(
                        nameof(AccountController.Logout),
                        typeof(AccountController).ControllerName(),
                        new { area = typeof(AccountController).AreaName() })));
        }
    }

    public IList<KeyValuePair<string, string>> Links { get; }
}
