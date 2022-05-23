using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection
{
    public static class AssociatedServicesControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AssociatedServicesController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(AssociatedServicesController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AssociatedServicesController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_AddAssociatedServices_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            AssociatedServicesController controller)
        {
            var result = controller.AddAssociatedServices(internalOrgId, callOffId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new AddAssociatedServicesModel
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_AddAssociatedServices_WithModelErrors_ReturnsExpectedResult(
            AddAssociatedServicesModel model,
            AssociatedServicesController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = controller.AddAssociatedServices(model.InternalOrgId, model.CallOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_AddAssociatedServices_YesSelected_ReturnsExpectedResult(
            AddAssociatedServicesModel model,
            AssociatedServicesController controller)
        {
            model.AdditionalServicesRequired = YesNoRadioButtonTagHelper.Yes;

            var result = controller.AddAssociatedServices(model.InternalOrgId, model.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(AssociatedServicesController.SelectAssociatedServices));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", model.InternalOrgId },
                { "callOffId", model.CallOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static void Post_AddAssociatedServices_NoSelected_ReturnsExpectedResult(
            AddAssociatedServicesModel model,
            AssociatedServicesController controller)
        {
            model.AdditionalServicesRequired = YesNoRadioButtonTagHelper.No;

            var result = controller.AddAssociatedServices(model.InternalOrgId, model.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(ReviewSolutionsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(ReviewSolutionsController.ReviewSolutions));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", model.InternalOrgId },
                { "callOffId", model.CallOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectAssociatedServices_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            mockAssociatedServicesService
                .Setup(x => x.GetPublishedAssociatedServicesForSupplier(order.SupplierId))
                .ReturnsAsync(services);

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId);

            mockOrderService.VerifyAll();
            mockAssociatedServicesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectServicesModel(order, services, CatalogueItemType.AssociatedService)
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                AssociatedServicesOnly = true,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectAssociatedServices_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            SelectServicesModel model,
            AssociatedServicesController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            mockAssociatedServicesService
                .Setup(x => x.GetPublishedAssociatedServicesForSupplier(order.SupplierId))
                .ReturnsAsync(services);

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();
            mockAssociatedServicesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectServicesModel(order, services, CatalogueItemType.AssociatedService)
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                AssociatedServicesOnly = true,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectAssociatedServices_NoSelectionMade_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectServicesModel model,
            AssociatedServicesController controller)
        {
            model.Services.ForEach(x => x.IsSelected = false);

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(ReviewSolutionsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(ReviewSolutionsController.ReviewSolutions));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectAssociatedServices_SelectionMade_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectServicesModel model,
            [Frozen] Mock<IOrderItemService> mockOrderItemService,
            AssociatedServicesController controller)
        {
            model.Services.ForEach(x => x.IsSelected = false);
            model.Services.First().IsSelected = true;

            var catalogueItemId = model.Services.First().CatalogueItemId;

            mockOrderItemService
                .Setup(x => x.AddOrderItems(internalOrgId, callOffId, new[] { catalogueItemId }))
                .Returns(Task.CompletedTask);

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId, model);

            mockOrderItemService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(ServiceRecipientsController.AddServiceRecipients));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", catalogueItemId },
            });
        }
    }
}
