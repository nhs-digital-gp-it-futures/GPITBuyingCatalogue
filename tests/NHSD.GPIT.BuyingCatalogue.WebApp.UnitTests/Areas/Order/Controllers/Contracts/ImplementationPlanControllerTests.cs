using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans;
using NSubstitute;
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
        [MockInlineAutoData(true, false)]
        [MockInlineAutoData(false, true)]
        [MockInlineAutoData(false, false)]
        public static async Task Post_Index_NoImplementationPlanOrMilestones_RedirectsToChoice(
            bool implementationPlanIsNull,
            bool milestonesAreNull,
            string internalOrgId,
            ImplementationPlanModel model,
            EntityFramework.Ordering.Models.Order order,
            Contract contract,
            ImplementationPlan implementationPlan,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IContractsService mockContractsService,
            [Frozen] IImplementationPlanService mockImplementationPlanService,
            ImplementationPlanController controller)
        {
            contract.Order = order;
            implementationPlan.Milestones = milestonesAreNull ? null : [];
            contract.ImplementationPlan = implementationPlanIsNull ? null : implementationPlan;
            model.BespokePlan = null;
            mockOrderService.GetOrderThin(model.CallOffId, model.InternalOrgId).Returns(new OrderWrapper(order));
            mockContractsService.GetContractWithImplementationPlan(order.Id).Returns(contract);

            var result = await controller.Index(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(ImplementationPlanController.BespokeMilestoneChoice));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
            });
            await mockImplementationPlanService.DidNotReceive().AddImplementationPlan(order.Id, contract.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Index_WithBespokeMilestones_CompletesImplementationPlan(
            string internalOrgId,
            ImplementationPlanModel model,
            EntityFramework.Ordering.Models.Order order,
            Contract contract,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IContractsService mockContractsService,
            [Frozen] IImplementationPlanService mockImplementationPlanService,
            ImplementationPlanController controller)
        {
            contract.Order = order;
            model.BespokePlan = new ImplementationPlan();
            model.BespokePlan.Milestones.Add(new ImplementationPlanMilestone());
            mockOrderService.GetOrderThin(model.CallOffId, model.InternalOrgId).Returns(new OrderWrapper(order));
            mockContractsService.GetContractWithImplementationPlan(order.Id).Returns(contract);

            var result = await controller.Index(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
            await mockImplementationPlanService.Received().AddImplementationPlan(order.Id, contract.Id);
        }

        [Theory]
        [MockAutoData]
        public static void Get_BespokeMilestoneChoice_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ImplementationPlanController controller)
        {
            var result = controller.BespokeMilestoneChoice(internalOrgId, callOffId);

            var expected = new BespokeMilestoneChoiceModel
            {
                CallOffId = callOffId,
                InternalOrgId = internalOrgId,
            };

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_BespokeMilestoneChoice_ModelError_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            BespokeMilestoneChoiceModel model,
            ImplementationPlanController controller)
        {
            controller.ModelState.AddModelError("some-property", "some-error");

            var result = await controller.BespokeMilestoneChoice(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_BespokeMilestoneChoice_AddMilestone_RedirectsToAddMilestone(
            string internalOrgId,
            CallOffId callOffId,
            BespokeMilestoneChoiceModel model,
            ImplementationPlanController controller)
        {
            model.ShouldAddMilestone = true;

            var result = await controller.BespokeMilestoneChoice(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(ImplementationPlanController.AddMilestone));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_BespokeMilestoneChoice_NoMilestone_CompletesImplementationPlan(
            string internalOrgId,
            CallOffId callOffId,
            BespokeMilestoneChoiceModel model,
            EntityFramework.Ordering.Models.Order order,
            Contract contract,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IContractsService mockContractsService,
            [Frozen] IImplementationPlanService mockImplementationPlanService,
            ImplementationPlanController controller)
        {
            model.ShouldAddMilestone = false;
            mockOrderService.GetOrderThin(model.CallOffId, model.InternalOrgId).Returns(new OrderWrapper(order));
            mockContractsService.GetContract(order.Id).Returns(contract);

            var result = await controller.BespokeMilestoneChoice(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
            await mockImplementationPlanService.Received().AddImplementationPlan(order.Id, contract.Id);
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
