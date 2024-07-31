using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoreLinq.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.Requirement;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.Contracts
{
    public static class RequirementControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(RequirementController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(RequirementController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(RequirementController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_ReturnsExpectedResult(
           string internalOrgId,
           EntityFramework.Ordering.Models.Order order,
           Contract contract,
           [Frozen] IOrderService mockOrderService,
           [Frozen] IContractsService mockContractsService,
           RequirementController controller)
        {
            var solution = order.OrderItems.First();

            contract.Order = order;
            contract.ContractBilling = null;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockContractsService.GetContractWithContractBillingRequirements(order.Id).Returns(contract);

            var result = await controller.Index(internalOrgId, order.CallOffId);

            var expected = new RequirementModel(contract?.ContractBilling)
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
            RequirementModel model,
            RequirementController controller)
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
            RequirementModel model,
            EntityFramework.Ordering.Models.Order order,
            Contract contract,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IContractsService mockContractsService,
            [Frozen] IRequirementsService mockRequirementsService,
            RequirementController controller)
        {
            contract.Order = order;
            mockOrderService.GetOrderThin(model.CallOffId, model.InternalOrgId).Returns(new OrderWrapper(order));

            mockContractsService.GetContract(order.Id).Returns(contract);

            mockRequirementsService.SetRequirementComplete(order.Id, contract.Id).Returns(contract);

            var result = await controller.Index(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddRequirement_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            RequirementController controller)
        {
            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.AddRequirement(internalOrgId, callOffId);

            var expected = new RequirementDetailsModel(callOffId, internalOrgId, order.GetAssociatedServices());

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("RequirementDetails");
            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddRequirement_InvalidOrder_ReturnsNotFound(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            RequirementDetailsModel model,
            RequirementController controller)
        {
            order.OrderItems.Clear();

            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.AddRequirement(internalOrgId, callOffId, model);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddRequirement_ModelError_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            RequirementDetailsModel model,
            RequirementController controller)
        {
            order.OrderItems.Add(new OrderItem()
            {
                CatalogueItemId = model.SelectedOrderItemId,
                CatalogueItem = new CatalogueItem() { Name = "Test", Id = model.SelectedOrderItemId, CatalogueItemType = CatalogueItemType.AssociatedService, },
            });

            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            controller.ModelState.AddModelError("some-property", "some-error");

            var result = await controller.AddRequirement(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("RequirementDetails");

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddRequirement_ReturnsExpectedResult(
            string internalOrgId,
            RequirementDetailsModel model,
            EntityFramework.Ordering.Models.Order order,
            Contract contract,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IContractsService mockContractsService,
            [Frozen] IRequirementsService mockRequirementsService,
            RequirementController controller)
        {
            order.OrderItems.Add(new OrderItem()
            {
                CatalogueItemId = model.SelectedOrderItemId,
                CatalogueItem = new CatalogueItem() { Name = "Test", Id = model.SelectedOrderItemId, CatalogueItemType = CatalogueItemType.AssociatedService, },
            });
            contract.Order = order;
            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockContractsService.GetContract(order.Id).Returns(contract);

            mockRequirementsService.AddRequirement(order.Id, contract.Id, model.SelectedOrderItemId, model.Details).Returns(Task.CompletedTask);

            var result = await controller.AddRequirement(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(RequirementController.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditRequirement_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Requirement item,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IRequirementsService mockRequirementsService,
            RequirementController controller)
        {
            item.OrderItem = new OrderItem();

            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockRequirementsService.GetRequirement(order.Id, item.Id).Returns(item);

            var expected = new RequirementDetailsModel(item, order.CallOffId, internalOrgId, order.GetAssociatedServices());

            var result = await controller.EditRequirement(internalOrgId, order.CallOffId, item.Id);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("RequirementDetails");
            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditRequirement_InvalidOrder_ReturnsNotFoundResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            RequirementDetailsModel model,
            RequirementController controller)
        {
            order.OrderItems.Clear();

            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.EditRequirement(internalOrgId, order.CallOffId, model);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditRequirement_ModelError_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            RequirementDetailsModel model,
            RequirementController controller)
        {
            order.OrderItems.Add(new OrderItem()
            {
                CatalogueItemId = model.SelectedOrderItemId,
                CatalogueItem = new CatalogueItem() { Name = "Test", Id = model.SelectedOrderItemId, CatalogueItemType = CatalogueItemType.AssociatedService, },
            });

            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            controller.ModelState.AddModelError("some-property", "some-error");

            var result = await controller.EditRequirement(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("RequirementDetails");

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditRequirement_ReturnsExpectedResult(
            string internalOrgId,
            RequirementDetailsModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IRequirementsService mockRequirementsService,
            RequirementController controller)
        {
            order.OrderItems.Add(new OrderItem()
            {
                CatalogueItemId = model.SelectedOrderItemId,
                CatalogueItem = new CatalogueItem() { Name = "Test", Id = model.SelectedOrderItemId, CatalogueItemType = CatalogueItemType.AssociatedService, },
            });

            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockRequirementsService.EditRequirement(order.Id, model.ItemId, model.SelectedOrderItemId, model.Details).Returns(Task.CompletedTask);

            var result = await controller.EditRequirement(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(RequirementController.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteRequirement_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Requirement item,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IRequirementsService mockRequirementsService,
            RequirementController controller)
        {
            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockRequirementsService.GetRequirement(order.Id, item.Id).Returns(item);

            var result = await controller.DeleteRequirement(internalOrgId, order.CallOffId, item.Id);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeEquivalentTo(new DeleteRequirementModel(order.CallOffId, internalOrgId, item), x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteRequirement_ReturnsExpectedResult(
            string internalOrgId,
            DeleteRequirementModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IRequirementsService mockRequirementsService,
            RequirementController controller)
        {
            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockRequirementsService.DeleteRequirement(order.Id, model.ItemId).Returns(Task.CompletedTask);

            var result = await controller.DeleteRequirement(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(RequirementController.Index));
        }
    }
}
