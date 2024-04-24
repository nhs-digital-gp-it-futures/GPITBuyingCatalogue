using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Dashboard;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class DashboardControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(DashboardController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(DashboardController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(DashboardController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_Order_Buyer_RedirectsCorrectly(
            string internalOrgId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[]
                {
                    new(ClaimTypes.Role, "Buyer"),
                    new("primaryOrganisationInternalIdentifier", internalOrgId),
                },
                "mock"));

            var controller = new DashboardController(Mock.Of<IOrganisationsService>(), Mock.Of<IOrderService>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user },
                },
            };

            var result = controller.Index().As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(DashboardController.Organisation));
            result.ControllerName.Should().Be(typeof(DashboardController).ControllerName());
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", internalOrgId } });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Organisation_ReturnsView(
            [Frozen] Mock<IOrganisationsService> organisationService,
            [Frozen] Mock<IOrderService> orderService,
            Organisation organisation,
            PagedList<EntityFramework.Ordering.Models.Order> orders,
            string internalOrgId,
            DashboardController controller)
        {
            var orderIds = orders.Items.Select(x => x.CallOffId).ToList();

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new(ClaimTypes.Role, "Buyer") },
                "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user },
            };

            organisationService
                .Setup(s => s.GetOrganisationByInternalIdentifier(internalOrgId))
                .ReturnsAsync(organisation);

            orderService
                .Setup(s => s.GetPagedOrders(organisation.Id, It.IsAny<PageOptions>(), string.Empty))
                .ReturnsAsync((orders, orderIds));

            var expected = new OrganisationModel(organisation, user, orders.Items)
            {
                Options = orders.Options,
                OrderIds = orderIds,
            };

            var result = (await controller.Organisation(internalOrgId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
            result.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectOrganisation_ReturnsView(
            [Frozen] Mock<IOrganisationsService> organisationService,
            List<Organisation> organisations,
            DashboardController controller)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[]
                {
                    new(ClaimTypes.Role, "Buyer"),
                    new("primaryOrganisationInternalIdentifier", organisations.First().InternalIdentifier),
                    new("secondaryOrganisationInternalIdentifier", organisations.Last().InternalIdentifier),
                },
                "mock"));

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user },
                };

            organisationService.Setup(s => s.GetOrganisationsByInternalIdentifiers(It.IsAny<string[]>())).ReturnsAsync(organisations);

            var expected = new SelectOrganisationModel(organisations.First().InternalIdentifier, organisations);

            var result = (await controller.SelectOrganisation(organisations.First().InternalIdentifier)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
            result.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectOrganisation_CorrectlyRedirects(
            string internalOrgId,
            SelectOrganisationModel model,
            DashboardController controller)
        {
            var actualResult = controller.SelectOrganisation(internalOrgId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(DashboardController.Organisation));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(DashboardController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", model.SelectedOrganisation } });
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectOrganisation_InvalidModelState_ReturnsView(
            string errorKey,
            string errorMessage,
            string internalOrgId,
            SelectOrganisationModel model,
            DashboardController controller)
        {
            controller.ModelState.AddModelError(errorKey, errorMessage);

            var actualResult = controller.SelectOrganisation(internalOrgId, model).As<ViewResult>();

            actualResult.Should().NotBeNull();
            actualResult.ViewName.Should().BeNull();
            actualResult.ViewData.ModelState.IsValid.Should().BeFalse();
            actualResult.ViewData.ModelState.ErrorCount.Should().Be(1);
            actualResult.ViewData.ModelState.Keys.First().Should().Be(errorKey);
            actualResult.ViewData.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be(errorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_FilterSearchSuggestions_ReturnsResults(
            Organisation organisation,
            string searchTerm,
            List<SearchFilterModel> searchResults,
            [Frozen] Mock<IOrganisationsService> organisationsService,
            [Frozen] Mock<IOrderService> orderService,
            DashboardController controller)
        {
            controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                Request =
                {
                    Headers =
                    {
                        Referer = "http://www.test.com",
                    },
                },
            };
            var requestUri = new UriBuilder(controller.HttpContext.Request.Headers.Referer.ToString());
            var expected = searchResults.Select(r => new SuggestionSearchResult
            {
                Title = r.Title,
                Category = r.Category,
                Url = requestUri.AppendQueryParameterToUrl("search", r.Category).ToString(),
            });

            organisationsService.Setup(s => s.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
                .ReturnsAsync(organisation);

            orderService.Setup(s => s.GetOrdersBySearchTerm(organisation.Id, searchTerm))
                .ReturnsAsync(searchResults);

            var result = (await controller.FilterSearchSuggestions(organisation.InternalIdentifier, searchTerm)).As<JsonResult>();

            result.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
