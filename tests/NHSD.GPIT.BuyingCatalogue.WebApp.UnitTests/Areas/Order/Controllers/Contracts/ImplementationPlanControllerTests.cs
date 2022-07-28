using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.ImplementationPlans;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.Contracts
{
    public static class ImplementationPlanControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(ImplementationPlanController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(ImplementationPlanController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ImplementationPlanController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DefaultImplementationPlan_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            ContractFlags contract,
            CatalogueItem catalogueItem,
            ImplementationPlan defaultPlan,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IContractsService> mockContractsService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            [Frozen] Mock<IImplementationPlanService> mockImplementationPlanService,
            ImplementationPlanController controller)
        {
            var solution = order.OrderItems.First();

            contract.UseDefaultImplementationPlan = null;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(order);

            mockContractsService
                .Setup(x => x.GetContract(order.Id))
                .ReturnsAsync(contract);

            mockSolutionsService
                .Setup(x => x.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockImplementationPlanService
                .Setup(x => x.GetDefaultImplementationPlan())
                .ReturnsAsync(defaultPlan);

            var result = await controller.DefaultImplementationPlan(internalOrgId, order.CallOffId);

            mockOrderService.VerifyAll();
            mockContractsService.VerifyAll();
            mockSolutionsService.VerifyAll();
            mockImplementationPlanService.VerifyAll();

            var expected = new DefaultImplementationPlanModel
            {
                CallOffId = order.CallOffId,
                Solution = catalogueItem.Solution,
                Plan = defaultPlan,
                UseDefaultMilestones = null,
            };

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DefaultImplementationPlan_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            ContractFlags contract,
            CatalogueItem catalogueItem,
            ImplementationPlan defaultPlan,
            DefaultImplementationPlanModel model,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IContractsService> mockContractsService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            [Frozen] Mock<IImplementationPlanService> mockImplementationPlanService,
            ImplementationPlanController controller)
        {
            var solution = order.OrderItems.First();

            contract.UseDefaultImplementationPlan = null;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(order);

            mockContractsService
                .Setup(x => x.GetContract(order.Id))
                .ReturnsAsync(contract);

            mockSolutionsService
                .Setup(x => x.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockImplementationPlanService
                .Setup(x => x.GetDefaultImplementationPlan())
                .ReturnsAsync(defaultPlan);

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.DefaultImplementationPlan(internalOrgId, order.CallOffId, model);

            mockOrderService.VerifyAll();
            mockContractsService.VerifyAll();
            mockSolutionsService.VerifyAll();
            mockImplementationPlanService.VerifyAll();

            var expected = new DefaultImplementationPlanModel
            {
                CallOffId = order.CallOffId,
                Solution = catalogueItem.Solution,
                Plan = defaultPlan,
                UseDefaultMilestones = null,
            };

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DefaultImplementationPlan_UseDefaultMilestones_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            DefaultImplementationPlanModel model,
            [Frozen] Mock<IContractsService> mockContractsService,
            ImplementationPlanController controller)
        {
            mockContractsService
                .Setup(x => x.UseDefaultImplementationPlan(callOffId.Id, true))
                .Verifiable();

            model.UseDefaultMilestones = true;

            var result = await controller.DefaultImplementationPlan(internalOrgId, callOffId, model);

            mockContractsService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DefaultImplementationPlan_DoNotUseDefaultMilestones_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            DefaultImplementationPlanModel model,
            [Frozen] Mock<IContractsService> mockContractsService,
            ImplementationPlanController controller)
        {
            model.UseDefaultMilestones = false;

            mockContractsService
                .Setup(x => x.UseDefaultImplementationPlan(callOffId.Id, false))
                .Verifiable();

            var result = await controller.DefaultImplementationPlan(internalOrgId, callOffId, model);

            mockContractsService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(ImplementationPlanController.CustomImplementationPlan));
            actualResult.ControllerName.Should().Be(typeof(ImplementationPlanController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static void Get_CustomImplementationPlan_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ImplementationPlanController controller)
        {
            var result = controller.CustomImplementationPlan(internalOrgId, callOffId);

            var expected = new CustomImplementationPlanModel
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }
    }
}
