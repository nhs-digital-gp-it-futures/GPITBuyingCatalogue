using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.Contracts
{
    public static class ImplementationPlanControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(ImplementationPlanController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(ImplementationPlanController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ImplementationPlanController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Contract contract,
            CatalogueItem catalogueItem,
            ImplementationPlan defaultPlan,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IContractsService mockContractsService,
            [Frozen] ISolutionsService mockSolutionsService,
            [Frozen] IImplementationPlanService mockImplementationPlanService,
            ImplementationPlanController controller)
        {
            var solution = order.OrderItems.First();

            contract.Order = order;
            contract.ImplementationPlan = null;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockContractsService.GetContractWithImplementationPlan(order.Id).Returns(contract);

            mockSolutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(catalogueItem);

            mockImplementationPlanService.GetDefaultImplementationPlan().Returns(defaultPlan);

            var result = await controller.Index(internalOrgId, order.CallOffId);

            var expected = new ImplementationPlanModel(defaultPlan, null, catalogueItem.Solution)
            {
                CallOffId = order.CallOffId,
                InternalOrgId = internalOrgId,
            };

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Index_ModelError_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ImplementationPlanModel model,
            ImplementationPlanController controller)
        {
            controller.ModelState.AddModelError("some-property", "some-error");

            var result = await controller.Index(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Index_ReturnsExpectedResult(
            string internalOrgId,
            ImplementationPlanModel model,
            EntityFramework.Ordering.Models.Order order,
            Contract contract,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IContractsService mockContractsService,
            ImplementationPlanController controller)
        {
            contract.Order = order;
            mockOrderService.GetOrderThin(model.CallOffId, model.InternalOrgId).Returns(new OrderWrapper(order));

            mockContractsService.GetContract(order.Id).Returns(contract);

            var result = await controller.Index(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
        }

        [Theory]
        [MockAutoData]
        public static void Get_AddMilestone_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ImplementationPlanController controller)
        {
            var result = controller.AddMilestone(internalOrgId, callOffId);

            var expected = new MilestoneModel()
            {
                CallOffId = callOffId,
                InternalOrgId = internalOrgId,
            };

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("Milestone");
            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddMilestone_ModelError_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            MilestoneModel model,
            ImplementationPlanController controller)
        {
            controller.ModelState.AddModelError("some-property", "some-error");

            var result = await controller.AddMilestone(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("Milestone");
            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddMilestone_ReturnsExpectedResult(
            string internalOrgId,
            MilestoneModel model,
            EntityFramework.Ordering.Models.Order order,
            Contract contract,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IContractsService mockContractsService,
            [Frozen] IImplementationPlanService mockImplementationPlanService,
            ImplementationPlanController controller)
        {
            contract.Order = order;
            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockContractsService.GetContract(order.Id).Returns(contract);

            mockImplementationPlanService.AddBespokeMilestone(order.Id, contract.Id, model.Name, model.PaymentTrigger).Returns(Task.CompletedTask);

            var result = await controller.AddMilestone(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(ImplementationPlanController.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditMilestone_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            ImplementationPlanMilestone milestone,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IImplementationPlanService mockImplementationPlanService,
            ImplementationPlanController controller)
        {
            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockImplementationPlanService.GetMilestone(order.Id, milestone.Id).Returns(milestone);

            var expected = new MilestoneModel(milestone, order.CallOffId, internalOrgId);

            var result = await controller.EditMilestone(internalOrgId, order.CallOffId, milestone.Id);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("Milestone");
            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditMilestone_ModelError_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            MilestoneModel model,
            ImplementationPlanController controller)
        {
            controller.ModelState.AddModelError("some-property", "some-error");

            var result = await controller.EditMilestone(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("Milestone");
            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditMilestone_ReturnsExpectedResult(
            string internalOrgId,
            MilestoneModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IImplementationPlanService mockImplementationPlanService,
            ImplementationPlanController controller)
        {
            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockImplementationPlanService.EditMilestone(order.Id, model.MilestoneId, model.Name, model.PaymentTrigger).Returns(Task.CompletedTask);

            var result = await controller.EditMilestone(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(ImplementationPlanController.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteMilestone_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            ImplementationPlanMilestone milestone,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IImplementationPlanService mockImplementationPlanService,
            ImplementationPlanController controller)
        {
            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockImplementationPlanService.GetMilestone(order.Id, milestone.Id).Returns(milestone);

            var result = await controller.DeleteMilestone(internalOrgId, order.CallOffId, milestone.Id);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeEquivalentTo(new DeleteMilestoneModel(order.CallOffId, internalOrgId, milestone), x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteMilestone_ReturnsExpectedResult(
            string internalOrgId,
            DeleteMilestoneModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IImplementationPlanService mockImplementationPlanService,
            ImplementationPlanController controller)
        {
            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockImplementationPlanService.DeleteMilestone(order.Id, model.MilestoneId).Returns(Task.CompletedTask);

            var result = await controller.DeleteMilestone(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(ImplementationPlanController.Index));
        }
    }
}
