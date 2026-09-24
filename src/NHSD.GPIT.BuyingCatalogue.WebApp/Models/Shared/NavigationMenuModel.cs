using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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
    private static readonly Func<IUrlHelper, KeyValuePair<string, string>> Home = urlHelper => new KeyValuePair<string, string>(
        "Home",
        urlHelper.Action(nameof(HomeController.Index), typeof(HomeController).ControllerName()));

    private static readonly Func<IUrlHelper, KeyValuePair<string, string>> CatalogueSolutions = urlHelper => new KeyValuePair<string, string>(
        "Catalogue solutions",
        urlHelper.Action(
            nameof(SolutionsController.Index),
            typeof(SolutionsController).ControllerName(),
            new { area = typeof(SolutionsController).AreaName() }));

    private static readonly Func<IUrlHelper, KeyValuePair<string, string>> GeneralPractice = urlHelper => new KeyValuePair<string, string>(
        "General Practice",
        urlHelper.Action(
            nameof(HomeController.GPIT),
            typeof(HomeController).ControllerName()));

    private static readonly Func<IUrlHelper, KeyValuePair<string, string>> CommunityPharmacy = urlHelper => new KeyValuePair<string, string>(
        "Community Pharmacy",
        urlHelper.Action(
            nameof(CommunityPharmacySolutionsController.Index),
            typeof(CommunityPharmacySolutionsController).ControllerName(),
            new { area = typeof(CommunityPharmacySolutionsController).AreaName() }));

    private static readonly Func<IUrlHelper, KeyValuePair<string, string>> Logout = urlHelper =>
        new KeyValuePair<string, string>(
            "Log out",
            urlHelper.Action(
                nameof(AccountController.Logout),
                typeof(AccountController).ControllerName(),
                new { area = typeof(AccountController).AreaName() }));

    private static readonly Func<IUrlHelper, KeyValuePair<string, string>> AdminHome = urlHelper => new KeyValuePair<string, string>(
        "Home",
        urlHelper.Action(
            nameof(Areas.Admin.Controllers.HomeController.Index),
            typeof(Areas.Admin.Controllers.HomeController).ControllerName(),
            new
            {
                area =
                    typeof(Areas.Admin.Controllers.HomeController)
                        .AreaName(),
            }));

    private static readonly Func<IUrlHelper, KeyValuePair<string, string>> AdminBuyerOrganisations = urlHelper => new(
        "Buyer organisations",
        urlHelper.Action(
            nameof(OrganisationsController.Index),
            typeof(OrganisationsController).ControllerName(),
            new { area = typeof(OrganisationsController).AreaName() }));

    private static readonly Func<IUrlHelper, KeyValuePair<string, string>> AdminSupplierOrganisations = urlHelper =>
        new(
            "Supplier organisations",
            urlHelper.Action(
                nameof(SuppliersController.Index),
                typeof(SuppliersController).ControllerName(),
                new { area = typeof(OrganisationsController).AreaName() }));

    private static readonly Func<IUrlHelper, KeyValuePair<string, string>> AdminCatalogueSolutions = urlHelper => new(
        "Catalogue solutions",
        urlHelper.Action(
            nameof(CatalogueSolutionsController.Index),
            typeof(CatalogueSolutionsController).ControllerName(),
            new { area = typeof(OrganisationsController).AreaName() }));

    private static readonly
        List<(Func<ClaimsPrincipal, bool> Key, Func<ClaimsPrincipal, IUrlHelper, RouteValueDictionary,
                List<KeyValuePair<string, string>>>
            Factory)> LinksFactories =
            new()
            {
                (
                    user => !user.Identity?.IsAuthenticated ?? false,
                    (_, urlHelper, _) =>
                    [
                        Home(urlHelper),
                        GeneralPractice(urlHelper),
                        CommunityPharmacy(urlHelper),
                    ]),
                (
                    user => user.IsBuyer() || user.IsAccountManager(), (user, urlHelper, routeValues) =>
                    [
                        Home(urlHelper),

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
                                new
                                {
                                    Area = typeof(DashboardController).AreaName(),
                                    internalOrgId = routeValues.TryGetValue("internalOrgId", out var orgId)
                                        ? orgId
                                        : user.GetPrimaryOrganisationInternalIdentifier(),
                                })),
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
                                new
                                {
                                    Area = typeof(CompetitionsDashboardController).AreaName(),
                                    internalOrgId = user.GetPrimaryOrganisationInternalIdentifier(),
                                })),

                        CatalogueSolutions(urlHelper),
                        Logout(urlHelper),
                    ]),
                (
                    user => user.IsFullAdmin(), (_, urlHelper, _) =>
                    [
                        AdminHome(urlHelper),
                        AdminBuyerOrganisations(urlHelper),
                        AdminSupplierOrganisations(urlHelper),
                        AdminCatalogueSolutions(urlHelper),
                        Logout(urlHelper),
                    ]),
                (
                    user => user.IsOnboarding(), (_, urlHelper, _) =>
                    [
                        AdminHome(urlHelper),
                        AdminSupplierOrganisations(urlHelper),
                        AdminCatalogueSolutions(urlHelper),
                        Logout(urlHelper),
                    ]),
                (
                    user => user.IsReadOnly(), (_, urlHelper, _) =>
                    [
                        AdminHome(urlHelper),
                        Logout(urlHelper),
                    ]),
            };

    public NavigationMenuModel(
        ClaimsPrincipal user,
        IUrlHelper urlHelper,
        RouteValueDictionary routeValues)
    {
        (_, Func<ClaimsPrincipal, IUrlHelper, RouteValueDictionary, List<KeyValuePair<string, string>>> linkFactory) =
            LinksFactories.First(x => x.Key(user));

        Links = linkFactory(user, urlHelper, routeValues);
    }

    public IList<KeyValuePair<string, string>> Links { get; }
}
