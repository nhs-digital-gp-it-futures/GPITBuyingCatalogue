using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Dashboard;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class DashboardControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(DashboardController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static void Get_Order_Buyer_RedirectsCorrectly(
            string internalOrgId)
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[]
                    {
                        new(ClaimTypes.Role, "Buyer"), new("primaryOrganisationInternalIdentifier", internalOrgId),
                    },
                    "mock"));

            var controller = new DashboardController(Mock.Of<IOrganisationsService>(), Mock.Of<IOrderService>())
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user }, },
            };

            var result = controller.Index().As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(DashboardController.Organisation));
            result.ControllerName.Should().Be(typeof(DashboardController).ControllerName());
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", internalOrgId } });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Organisation_ReturnsView(
            [Frozen] IOrganisationsService organisationService,
            [Frozen] IOrderService orderService,
            Organisation organisation,
            PagedList<EntityFramework.Ordering.Models.Order> orders,
            string internalOrgId,
            DashboardController controller)
        {
            var orderIds = orders.Items.Select(x => x.CallOffId).ToList();

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[]
                    {
                        new(ClaimTypes.Role, "Buyer"),
                        new(CatalogueClaims.PrimaryOrganisationInternalIdentifier, internalOrgId),
                    },
                    "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user },
            };

            organisationService.GetOrganisationsByInternalIdentifiers(Arg.Any<string[]>())
                .Returns(Enumerable.Empty<Organisation>().ToList());
            organisationService.GetOrganisationByInternalIdentifier(internalOrgId).Returns(organisation);

            orderService.GetPagedOrders(organisation.Id, Arg.Any<PageOptions>(), string.Empty)
                .Returns((orders, orderIds));

            var expected = new OrganisationModel(organisation, Enumerable.Empty<Organisation>(), orders.Items)
            {
                Options = orders.Options,
            };

            var result = (await controller.Organisation(internalOrgId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
            result.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockAutoData]
        public static void Post_Organisation_InvalidModel_ReturnsViewWithModel(
            string internalOrgId,
            OrganisationModel model,
            string page,
            string search,
            DashboardController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = controller.Organisation(internalOrgId, model, page, search).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().Be(model);
        }

        [Theory]
        [MockAutoData]
        public static void Post_Organisation_Valid_Redirects(
            string internalOrgId,
            OrganisationModel model,
            string page,
            string search,
            DashboardController controller)
        {
            var result = controller.Organisation(internalOrgId, model, page, search).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Organisation));
            result.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary
                    {
                        { nameof(internalOrgId), model.SelectedOrganisationId },
                        { nameof(page), page },
                        { nameof(search), search },
                    });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_FilterSearchSuggestions_ReturnsResults(
            Organisation organisation,
            string searchTerm,
            List<SearchFilterModel> searchResults,
            [Frozen] IOrganisationsService organisationsService,
            [Frozen] IOrderService orderService,
            DashboardController controller)
        {
            controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                Request = { Headers = { Referer = "http://www.test.com", }, },
            };
            var requestUri = new UriBuilder(controller.HttpContext.Request.Headers.Referer.ToString());
            var expected = searchResults.Select(
                r => new SuggestionSearchResult
                {
                    Title = r.Title,
                    Category = r.Category,
                    Url = requestUri.AppendQueryParameterToUrl("search", r.Category).ToString(),
                });

            organisationsService.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier)
                .Returns(organisation);

            orderService.GetOrdersBySearchTerm(organisation.Id, searchTerm).Returns(searchResults);

            var result = (await controller.FilterSearchSuggestions(organisation.InternalIdentifier, searchTerm))
                .As<JsonResult>();

            result.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
