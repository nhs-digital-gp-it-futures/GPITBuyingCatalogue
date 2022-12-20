using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.Review;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.Contracts
{
    public static class ReviewContractControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(ReviewContractController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(ReviewContractController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ReviewContractController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ContractSummary_ExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            ImplementationPlan defaultPlan,
            [Frozen] Mock<IImplementationPlanService> implementationPlanService,
            [Frozen] Mock<IOrderService> orderService,
            ReviewContractController controller)
        {
            implementationPlanService
                .Setup(x => x.GetDefaultImplementationPlan())
                .ReturnsAsync(defaultPlan);

            orderService
                .Setup(s => s.GetOrderForSummary(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.ContractSummary(internalOrgId, order.CallOffId);

            implementationPlanService.VerifyAll();
            orderService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new ReviewContractModel(internalOrgId, order, defaultPlan);

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ContractSummary_CannotComplete_ExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            ImplementationPlan defaultPlan,
            [Frozen] Mock<IImplementationPlanService> implementationPlanService,
            [Frozen] Mock<IOrderService> orderService,
            ReviewContractController controller)
        {
            order.Description = null;

            implementationPlanService
                .Setup(x => x.GetDefaultImplementationPlan())
                .ReturnsAsync(defaultPlan);

            orderService
                .Setup(s => s.GetOrderForSummary(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.ContractSummary(
                internalOrgId,
                order.CallOffId,
                new ReviewContractModel(internalOrgId, order, defaultPlan));

            orderService.VerifyAll();
            implementationPlanService.VerifyAll();

            var modelState = result.Should().BeOfType<ViewResult>().Subject.ViewData.ModelState;

            modelState.ValidationState.Should().Be(ModelValidationState.Invalid);
            modelState.Keys.First().Should().Be(ReviewContractController.ErrorKey);
            modelState.Values.First().Errors.First().ErrorMessage.Should().Be(ReviewContractController.ErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ContractSummary_CanComplete_ExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            ReviewContractModel model,
            [Frozen] Mock<IOrderService> orderService,
            ReviewContractController controller)
        {
            order.Completed = null;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            orderService
                .Setup(s => s.GetOrderForSummary(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            orderService
                .Setup(x => x.CompleteOrder(order.CallOffId, internalOrgId, 1, It.IsAny<Uri>()))
                .Verifiable();

            var result = await controller.ContractSummary(internalOrgId, order.CallOffId, model);

            orderService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(OrderController.Completed));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
            });
        }
    }
}
