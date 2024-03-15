using System;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
using NSubstitute;
using Xunit;
using HomeController = NHSD.GPIT.BuyingCatalogue.WebApp.Controllers.HomeController;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Shared;

public static class NavigationMenuModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_UnauthorizedUser_ExpectedLinks(
        IUrlHelper urlHelper)
    {
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

        var model = new NavigationMenuModel(claimsPrincipal, urlHelper);

        model.Links.Should().HaveCount(2);

        urlHelper.Received()
            .Action(
                Arg.Is<UrlActionContext>(
                    x => string.Equals(x.Action, nameof(HomeController.Index)) && string.Equals(
                        x.Controller,
                        typeof(HomeController).ControllerName())));

        urlHelper.Received()
            .Action(
                Arg.Is<UrlActionContext>(
                    x => string.Equals(x.Action, nameof(SolutionsController.Index)) && string.Equals(
                        x.Controller,
                        typeof(SolutionsController).ControllerName())));
    }

    [Theory]
    [MockAutoData]
    public static void Construct_BuyerUser_ExpectedLinks(
        string internalOrgId,
        IUrlHelper urlHelper)
    {
        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(
                        ClaimTypes.Role,
                        OrganisationFunction.Buyer.Name),
                    new Claim(Framework.Constants.CatalogueClaims.PrimaryOrganisationInternalIdentifier, internalOrgId),
                },
                "someAuthType"));

        var model = new NavigationMenuModel(claimsPrincipal, urlHelper);

        model.Links.Should().HaveCount(6);

        urlHelper.Received()
            .Action(
                Arg.Is<UrlActionContext>(
                    x => string.Equals(x.Action, nameof(HomeController.Index)) && string.Equals(
                        x.Controller,
                        typeof(HomeController).ControllerName())));

        urlHelper.Received()
            .Action(
                Arg.Is<UrlActionContext>(
                    x => string.Equals(x.Action, nameof(BuyerDashboardController.Index)) && string.Equals(
                        x.Controller,
                        typeof(BuyerDashboardController).ControllerName())));

        urlHelper.Received()
            .Action(
                Arg.Is<UrlActionContext>(
                    x => string.Equals(x.Action, nameof(DashboardController.Organisation)) && string.Equals(
                        x.Controller,
                        typeof(DashboardController).ControllerName())));

        urlHelper.Received()
            .Action(
                Arg.Is<UrlActionContext>(
                    x => string.Equals(x.Action, nameof(ManageFiltersController.Index)) && string.Equals(
                        x.Controller,
                        typeof(ManageFiltersController).ControllerName())));

        urlHelper.Received()
            .Action(
                Arg.Is<UrlActionContext>(
                    x => string.Equals(x.Action, nameof(CompetitionsDashboardController.Index)) && string.Equals(
                        x.Controller,
                        typeof(CompetitionsDashboardController).ControllerName())));

        urlHelper.Received()
            .Action(
                Arg.Is<UrlActionContext>(
                    x => string.Equals(x.Action, nameof(SolutionsController.Index)) && string.Equals(
                        x.Controller,
                        typeof(SolutionsController).ControllerName())));
    }

    [Theory]
    [MockAutoData]
    public static void Construct_AdminUser_ExpectedLinks(
        string internalOrgId,
        IUrlHelper urlHelper)
    {
        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(
                        ClaimTypes.Role,
                        OrganisationFunction.Authority.Name),
                    new Claim(Framework.Constants.CatalogueClaims.PrimaryOrganisationInternalIdentifier, internalOrgId),
                },
                "someAuthType"));

        var model = new NavigationMenuModel(claimsPrincipal, urlHelper);

        model.Links.Should().HaveCount(4);

        urlHelper.Received()
            .Action(
                Arg.Is<UrlActionContext>(
                    x => string.Equals(x.Action, nameof(HomeController.Index)) && string.Equals(
                        x.Controller,
                        typeof(HomeController).ControllerName())));

        urlHelper.Received()
            .Action(
                Arg.Is<UrlActionContext>(
                    x => string.Equals(x.Action, nameof(OrganisationsController.Index)) && string.Equals(
                        x.Controller,
                        typeof(OrganisationsController).ControllerName())));

        urlHelper.Received()
            .Action(
                Arg.Is<UrlActionContext>(
                    x => string.Equals(x.Action, nameof(SuppliersController.Index)) && string.Equals(
                        x.Controller,
                        typeof(SuppliersController).ControllerName())));

        urlHelper.Received()
            .Action(
                Arg.Is<UrlActionContext>(
                    x => string.Equals(x.Action, nameof(CatalogueSolutionsController.Index)) && string.Equals(
                        x.Controller,
                        typeof(CatalogueSolutionsController).ControllerName())));

        urlHelper.Received()
            .Action(
                Arg.Is<UrlActionContext>(
                    x => string.Equals(x.Action, nameof(AccountController.Logout)) && string.Equals(
                        x.Controller,
                        typeof(AccountController).ControllerName())));
    }

    [Theory]
    [MockAutoData]
    public static void Construct_AuthorizedMissingRole_ThrowsException(
        string internalOrgId,
        IUrlHelper urlHelper)
    {
        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(Framework.Constants.CatalogueClaims.PrimaryOrganisationInternalIdentifier, internalOrgId),
                },
                "someAuthType"));
        FluentActions.Invoking(() => new NavigationMenuModel(claimsPrincipal, urlHelper))
            .Should()
            .Throw<InvalidOperationException>();
    }
}
