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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class DashboardControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(DashboardController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(DashboardController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(DashboardController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Fact]
        public static void Get_Order_NotBuyer_ReturnsNotBuyerView()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new("organisationFunction", "Authority") },
                "mock"));

            var controller = new DashboardController(Mock.Of<IOrganisationsService>(), Mock.Of<IOrderService>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user },
                },
            };

            var result = controller.Index();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Equal("NotBuyer", ((ViewResult)result).ViewName);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_Order_Buyer_RedirectsCorrectly(
            string odsCode)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[]
                {
                    new("organisationFunction", "Buyer"),
                    new("primaryOrganisationOdsCode", odsCode),
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
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", odsCode } });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Organisation_ReturnsView(
            [Frozen] Mock<IOrganisationsService> organisationService,
            [Frozen] Mock<IOrderService> orderService,
            Organisation organisation,
            PagedList<EntityFramework.Ordering.Models.Order> orders,
            string odsCode,
            DashboardController controller)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new("organisationFunction", "Buyer") },
                "mock"));

            controller.ControllerContext =
                 new ControllerContext
                 {
                     HttpContext = new DefaultHttpContext { User = user },
                 };

            organisationService.Setup(s => s.GetOrganisationByOdsCode(odsCode)).ReturnsAsync(organisation);

            orderService.Setup(s => s.GetPagedOrders(organisation.Id, It.IsAny<PageOptions>())).ReturnsAsync(orders);

            var expected = new OrganisationModel(organisation, user, orders.Items)
            {
                Options = orders.Options,
            };

            var result = (await controller.Organisation(odsCode)).As<ViewResult>();

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
                    new("organisationFunction", "Buyer"),
                    new("primaryOrganisationOdsCode", organisations.First().OdsCode),
                    new("secondaryOrganisationOdsCode", organisations.Last().OdsCode),
                },
                "mock"));

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user },
                };

            organisationService.Setup(s => s.GetOrganisationsByOdsCodes(It.IsAny<string[]>())).ReturnsAsync(organisations);

            var expected = new SelectOrganisationModel(organisations.First().OdsCode, organisations);

            var result = (await controller.SelectOrganisation(organisations.First().OdsCode)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
            result.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectOrganisation_CorrectlyRedirects(
            string odsCode,
            SelectOrganisationModel model,
            DashboardController controller)
        {
            var actualResult = controller.SelectOrganisation(odsCode, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(DashboardController.Organisation));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(DashboardController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", model.SelectedOrganisation } });
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectOrganisation_InvalidModelState_ReturnsView(
            string errorKey,
            string errorMessage,
            string odsCode,
            SelectOrganisationModel model,
            DashboardController controller)
        {
            controller.ModelState.AddModelError(errorKey, errorMessage);

            var actualResult = controller.SelectOrganisation(odsCode, model).As<ViewResult>();

            actualResult.Should().NotBeNull();
            actualResult.ViewName.Should().BeNull();
            actualResult.ViewData.ModelState.IsValid.Should().BeFalse();
            actualResult.ViewData.ModelState.ErrorCount.Should().Be(1);
            actualResult.ViewData.ModelState.Keys.Single().Should().Be(errorKey);
            actualResult.ViewData.ModelState.Values.Single().Errors.Single().ErrorMessage.Should().Be(errorMessage);
        }
    }
}
