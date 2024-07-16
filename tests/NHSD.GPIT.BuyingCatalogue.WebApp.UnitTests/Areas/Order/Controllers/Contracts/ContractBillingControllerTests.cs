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
using MoreLinq.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ContractBilling;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.Contracts
{
    public static class ContractBillingControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(ContractBillingController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(ContractBillingController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ContractBillingController).GetConstructors();

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
           ContractBillingController controller)
        {
            var solution = order.OrderItems.First();

            contract.Order = order;
            contract.ContractBilling = null;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockContractsService.GetContractWithContractBilling(order.Id).Returns(contract);

            var result = await controller.Index(internalOrgId, order.CallOffId);

            var expected = new ContractBillingModel(contract?.ContractBilling)
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
            ContractBillingModel model,
            ContractBillingController controller)
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
            ContractBillingModel model,
            EntityFramework.Ordering.Models.Order order,
            Contract contract,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IContractsService mockContractsService,
            [Frozen] IContractBillingService mockContractBillingService,
            ContractBillingController controller)
        {
            contract.Order = order;
            mockOrderService.GetOrderThin(model.CallOffId, model.InternalOrgId).Returns(new OrderWrapper(order));

            mockContractsService.GetContract(order.Id).Returns(contract);

            mockContractBillingService.AddContractBilling(order.Id, contract.Id).Returns(contract);

            var result = await controller.Index(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(RequirementController.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddMilestone_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            ContractBillingController controller)
        {
            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.AddMilestone(internalOrgId, callOffId);

            var expected = new ContractBillingItemModel(callOffId, internalOrgId, order.GetAssociatedServices());

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("ContractBillingItem");
            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddMilestone_InvalidOrder_ReturnsNotFound(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            ContractBillingItemModel model,
            ContractBillingController controller)
        {
            order.OrderItems.Clear();

            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.AddMilestone(internalOrgId, callOffId, model);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddMilestone_ModelError_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            ContractBillingItemModel model,
            ContractBillingController controller)
        {
            order.OrderItems.Add(new OrderItem()
            {
                CatalogueItemId = model.SelectedOrderItemId,
                CatalogueItem = new CatalogueItem() { Name = "Test", Id = model.SelectedOrderItemId, CatalogueItemType = CatalogueItemType.AssociatedService, },
            });

            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            controller.ModelState.AddModelError("some-property", "some-error");

            var result = await controller.AddMilestone(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("ContractBillingItem");

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddMilestone_ReturnsExpectedResult(
            string internalOrgId,
            ContractBillingItemModel model,
            EntityFramework.Ordering.Models.Order order,
            Contract contract,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IContractsService mockContractsService,
            [Frozen] IContractBillingService mockContractBillingService,
            ContractBillingController controller)
        {
            order.OrderItems.Add(new OrderItem()
            {
                CatalogueItemId = model.SelectedOrderItemId,
                CatalogueItem = new CatalogueItem() { Name = "Test", Id = model.SelectedOrderItemId, CatalogueItemType = CatalogueItemType.AssociatedService, },
            });
            contract.Order = order;
            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockContractsService.GetContract(order.Id).Returns(contract);

            mockContractBillingService.AddBespokeContractBillingItem(order.Id, contract.Id, model.SelectedOrderItemId, model.Name, model.PaymentTrigger, model.Quantity.GetValueOrDefault()).Returns(Task.CompletedTask);

            var result = await controller.AddMilestone(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(ContractBillingController.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditMilestone_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            ContractBillingItem item,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IContractBillingService mockContractBillingService,
            ContractBillingController controller)
        {
            item.OrderItem = new OrderItem();

            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockContractBillingService.GetContractBillingItem(order.Id, item.Id).Returns(item);

            var expected = new ContractBillingItemModel(item, order.CallOffId, internalOrgId, order.GetAssociatedServices());

            var result = await controller.EditMilestone(internalOrgId, order.CallOffId, item.Id);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("ContractBillingItem");
            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditMilestone_InvalidOrder_ReturnsNotFoundResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            ContractBillingItemModel model,
            ContractBillingController controller)
        {
            order.OrderItems.Clear();

            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.EditMilestone(internalOrgId, order.CallOffId, model);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditMilestone_ModelError_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            ContractBillingItemModel model,
            ContractBillingController controller)
        {
            order.OrderItems.Add(new OrderItem()
            {
                CatalogueItemId = model.SelectedOrderItemId,
                CatalogueItem = new CatalogueItem() { Name = "Test", Id = model.SelectedOrderItemId, CatalogueItemType = CatalogueItemType.AssociatedService, },
            });

            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            controller.ModelState.AddModelError("some-property", "some-error");

            var result = await controller.EditMilestone(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("ContractBillingItem");

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditMilestone_ReturnsExpectedResult(
            string internalOrgId,
            ContractBillingItemModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IContractBillingService mockContractBillingService,
            ContractBillingController controller)
        {
            order.OrderItems.Add(new OrderItem()
            {
                CatalogueItemId = model.SelectedOrderItemId,
                CatalogueItem = new CatalogueItem() { Name = "Test", Id = model.SelectedOrderItemId, CatalogueItemType = CatalogueItemType.AssociatedService, },
            });

            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockContractBillingService.EditContractBillingItem(order.Id, model.ItemId, model.SelectedOrderItemId, model.Name, model.PaymentTrigger, model.Quantity.GetValueOrDefault()).Returns(Task.CompletedTask);

            var result = await controller.EditMilestone(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(ContractBillingController.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteContractBillingItem_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            ContractBillingItem item,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IContractBillingService mockContractBillingService,
            ContractBillingController controller)
        {
            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockContractBillingService.GetContractBillingItem(order.Id, item.Id).Returns(item);

            var result = await controller.DeleteContractBillingItem(internalOrgId, order.CallOffId, item.Id);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeEquivalentTo(new DeleteContractBillingItemModel(order.CallOffId, internalOrgId, item), x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteContractBillingItem_ReturnsExpectedResult(
            string internalOrgId,
            DeleteContractBillingItemModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IContractBillingService mockContractBillingService,
            ContractBillingController controller)
        {
            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockContractBillingService.DeleteContractBillingItem(order.Id, model.ItemId).Returns(Task.CompletedTask);

            var result = await controller.DeleteContractBillingItem(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(ContractBillingController.Index));
        }
    }
}
