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
using Moq;
using MoreLinq.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
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
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ContractBillingController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ReturnsExpectedResult(
           string internalOrgId,
           EntityFramework.Ordering.Models.Order order,
           Contract contract,
           [Frozen] Mock<IOrderService> mockOrderService,
           [Frozen] Mock<IContractsService> mockContractsService,
           ContractBillingController controller)
        {
            var solution = order.OrderItems.First();

            contract.Order = order;
            contract.ContractBilling = null;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockContractsService
                .Setup(x => x.GetContractWithContractBilling(order.Id))
                .ReturnsAsync(contract);

            var result = await controller.Index(internalOrgId, order.CallOffId);

            mockOrderService.VerifyAll();
            mockContractsService.VerifyAll();

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
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_Index_ReturnsExpectedResult(
            string internalOrgId,
            ContractBillingModel model,
            EntityFramework.Ordering.Models.Order order,
            Contract contract,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IContractsService> mockContractsService,
            [Frozen] Mock<IContractBillingService> mockContractBillingService,
            ContractBillingController controller)
        {
            contract.Order = order;
            mockOrderService
                .Setup(s => s.GetOrderThin(model.CallOffId, model.InternalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockContractsService
                .Setup(x => x.GetContract(order.Id))
                .ReturnsAsync(contract);

            mockContractBillingService
                .Setup(x => x.AddContractBilling(order.Id, contract.Id))
                .ReturnsAsync(contract);

            var result = await controller.Index(internalOrgId, order.CallOffId, model);

            mockOrderService.VerifyAll();
            mockContractsService.VerifyAll();
            mockContractBillingService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddMilestone_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            ContractBillingController controller)
        {
            mockOrderService
                .Setup(s => s.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.AddMilestone(internalOrgId, callOffId);

            var expected = new ContractBillingItemModel(callOffId, internalOrgId, order.GetAssociatedServices());

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("ContractBillingItem");
            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddMilestone_InvalidOrder_ReturnsNotFound(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            ContractBillingItemModel model,
            ContractBillingController controller)
        {
            order.OrderItems.Clear();

            mockOrderService
                .Setup(s => s.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            controller.ModelState.AddModelError("some-property", "some-error");

            var result = await controller.AddMilestone(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();

            result.Should().BeOfType<NotFoundResult>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddMilestone_ModelError_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            ContractBillingItemModel model,
            ContractBillingController controller)
        {
            order.OrderItems.Add(new OrderItem()
            {
                CatalogueItemId = model.SelectedOrderItemId,
                CatalogueItem = new CatalogueItem() { Name = "Test", Id = model.SelectedOrderItemId, CatalogueItemType = CatalogueItemType.AssociatedService, },
            });

            mockOrderService
                .Setup(s => s.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            controller.ModelState.AddModelError("some-property", "some-error");

            var result = await controller.AddMilestone(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("ContractBillingItem");

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddMilestone_ReturnsExpectedResult(
            string internalOrgId,
            ContractBillingItemModel model,
            EntityFramework.Ordering.Models.Order order,
            Contract contract,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IContractsService> mockContractsService,
            [Frozen] Mock<IContractBillingService> mockContractBillingService,
            ContractBillingController controller)
        {
            order.OrderItems.Add(new OrderItem()
            {
                CatalogueItemId = model.SelectedOrderItemId,
                CatalogueItem = new CatalogueItem() { Name = "Test", Id = model.SelectedOrderItemId, CatalogueItemType = CatalogueItemType.AssociatedService, },
            });
            contract.Order = order;
            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockContractsService
                .Setup(x => x.GetContract(order.Id))
                .ReturnsAsync(contract);

            mockContractBillingService
                .Setup(x => x.AddBespokeContractBillingItem(order.Id, contract.Id, model.SelectedOrderItemId, model.Name, model.PaymentTrigger, model.Quantity))
                .Returns(Task.CompletedTask);

            var result = await controller.AddMilestone(internalOrgId, order.CallOffId, model);

            mockOrderService.VerifyAll();
            mockContractsService.VerifyAll();
            mockContractBillingService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(ContractBillingController.Index));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditMilestone_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            ContractBillingItem item,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IContractBillingService> mockContractBillingService,
            ContractBillingController controller)
        {
            item.OrderItem = new OrderItem();

            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockContractBillingService
                .Setup(x => x.GetContractBillingItem(order.Id, item.Id))
                .ReturnsAsync(item);

            var expected = new ContractBillingItemModel(item, order.CallOffId, internalOrgId, order.GetAssociatedServices());

            var result = await controller.EditMilestone(internalOrgId, order.CallOffId, item.Id);

            mockOrderService.VerifyAll();
            mockContractBillingService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("ContractBillingItem");
            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditMilestone_InvalidOrder_ReturnsNotFoundResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            ContractBillingItemModel model,
            ContractBillingController controller)
        {
            order.OrderItems.Clear();

            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            controller.ModelState.AddModelError("some-property", "some-error");

            var result = await controller.EditMilestone(internalOrgId, order.CallOffId, model);

            mockOrderService.VerifyAll();

            result.Should().BeOfType<NotFoundResult>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditMilestone_ModelError_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            ContractBillingItemModel model,
            ContractBillingController controller)
        {
            order.OrderItems.Add(new OrderItem()
            {
                CatalogueItemId = model.SelectedOrderItemId,
                CatalogueItem = new CatalogueItem() { Name = "Test", Id = model.SelectedOrderItemId, CatalogueItemType = CatalogueItemType.AssociatedService, },
            });

            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            controller.ModelState.AddModelError("some-property", "some-error");

            var result = await controller.EditMilestone(internalOrgId, order.CallOffId, model);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("ContractBillingItem");

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditMilestone_ReturnsExpectedResult(
            string internalOrgId,
            ContractBillingItemModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IContractBillingService> mockContractBillingService,
            ContractBillingController controller)
        {
            order.OrderItems.Add(new OrderItem()
            {
                CatalogueItemId = model.SelectedOrderItemId,
                CatalogueItem = new CatalogueItem() { Name = "Test", Id = model.SelectedOrderItemId, CatalogueItemType = CatalogueItemType.AssociatedService, },
            });

            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockContractBillingService
                .Setup(x => x.EditContractBillingItem(order.Id, model.ItemId, model.SelectedOrderItemId, model.Name, model.PaymentTrigger, model.Quantity))
                .Returns(Task.CompletedTask);

            var result = await controller.EditMilestone(internalOrgId, order.CallOffId, model);

            mockOrderService.VerifyAll();
            mockContractBillingService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(ContractBillingController.Index));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteContractBillingItem_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            ContractBillingItem item,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IContractBillingService> mockContractBillingService,
            ContractBillingController controller)
        {
            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockContractBillingService
                .Setup(x => x.GetContractBillingItem(order.Id, item.Id))
                .ReturnsAsync(item);

            var result = await controller.DeleteContractBillingItem(internalOrgId, order.CallOffId, item.Id);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeEquivalentTo(new DeleteContractBillingItemModel(order.CallOffId, internalOrgId, item), x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteContractBillingItem_ReturnsExpectedResult(
            string internalOrgId,
            DeleteContractBillingItemModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IContractBillingService> mockContractBillingService,
            ContractBillingController controller)
        {
            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockContractBillingService
                .Setup(x => x.DeleteContractBillingItem(order.Id, model.ItemId))
                .Returns(Task.CompletedTask);

            var result = await controller.DeleteContractBillingItem(internalOrgId, order.CallOffId, model);

            mockOrderService.VerifyAll();
            mockContractBillingService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(ContractBillingController.Index));
        }
    }
}
