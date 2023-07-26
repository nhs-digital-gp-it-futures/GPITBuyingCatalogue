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
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(RequirementController).GetConstructors();

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
           RequirementController controller)
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
                .Setup(x => x.GetContractWithContractBillingRequirements(order.Id))
                .ReturnsAsync(contract);

            var result = await controller.Index(internalOrgId, order.CallOffId);

            mockOrderService.VerifyAll();
            mockContractsService.VerifyAll();

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
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_Index_ReturnsExpectedResult(
            string internalOrgId,
            RequirementModel model,
            EntityFramework.Ordering.Models.Order order,
            Contract contract,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IContractsService> mockContractsService,
            [Frozen] Mock<IRequirementsService> mockRequirementsService,
            RequirementController controller)
        {
            contract.Order = order;
            mockOrderService
                .Setup(s => s.GetOrderThin(model.CallOffId, model.InternalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockContractsService
                .Setup(x => x.GetContract(order.Id))
                .ReturnsAsync(contract);

            mockRequirementsService
                .Setup(x => x.SetRequirementComplete(order.Id, contract.Id))
                .ReturnsAsync(contract);

            var result = await controller.Index(internalOrgId, order.CallOffId, model);

            mockOrderService.VerifyAll();
            mockContractsService.VerifyAll();
            mockRequirementsService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddRequirement_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            RequirementController controller)
        {
            mockOrderService
                .Setup(s => s.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.AddRequirement(internalOrgId, callOffId);

            var expected = new RequirementDetailsModel(callOffId, internalOrgId, order.GetAssociatedServices());

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("RequirementDetails");
            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddRequirement_InvalidOrder_ReturnsNotFound(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            RequirementDetailsModel model,
            RequirementController controller)
        {
            order.OrderItems.Clear();

            mockOrderService
                .Setup(s => s.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.AddRequirement(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();

            result.Should().BeOfType<NotFoundResult>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddRequirement_ModelError_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            RequirementDetailsModel model,
            RequirementController controller)
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

            var result = await controller.AddRequirement(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("RequirementDetails");

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddRequirement_ReturnsExpectedResult(
            string internalOrgId,
            RequirementDetailsModel model,
            EntityFramework.Ordering.Models.Order order,
            Contract contract,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IContractsService> mockContractsService,
            [Frozen] Mock<IRequirementsService> mockRequirementsService,
            RequirementController controller)
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

            mockRequirementsService
                .Setup(x => x.AddRequirement(order.Id, contract.Id, model.SelectedOrderItemId, model.Details))
                .Returns(Task.CompletedTask);

            var result = await controller.AddRequirement(internalOrgId, order.CallOffId, model);

            mockOrderService.VerifyAll();
            mockContractsService.VerifyAll();
            mockRequirementsService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(RequirementController.Index));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditRequirement_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Requirement item,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IRequirementsService> mockRequirementsService,
            RequirementController controller)
        {
            item.OrderItem = new OrderItem();

            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockRequirementsService
                .Setup(x => x.GetRequirement(order.Id, item.Id))
                .ReturnsAsync(item);

            var expected = new RequirementDetailsModel(item, order.CallOffId, internalOrgId, order.GetAssociatedServices());

            var result = await controller.EditRequirement(internalOrgId, order.CallOffId, item.Id);

            mockOrderService.VerifyAll();
            mockRequirementsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("RequirementDetails");
            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditRequirement_InvalidOrder_ReturnsNotFoundResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            RequirementDetailsModel model,
            RequirementController controller)
        {
            order.OrderItems.Clear();

            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.EditRequirement(internalOrgId, order.CallOffId, model);

            mockOrderService.VerifyAll();

            result.Should().BeOfType<NotFoundResult>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditRequirement_ModelError_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            RequirementDetailsModel model,
            RequirementController controller)
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

            var result = await controller.EditRequirement(internalOrgId, order.CallOffId, model);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().Be("RequirementDetails");

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditRequirement_ReturnsExpectedResult(
            string internalOrgId,
            RequirementDetailsModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IRequirementsService> mockRequirementsService,
            RequirementController controller)
        {
            order.OrderItems.Add(new OrderItem()
            {
                CatalogueItemId = model.SelectedOrderItemId,
                CatalogueItem = new CatalogueItem() { Name = "Test", Id = model.SelectedOrderItemId, CatalogueItemType = CatalogueItemType.AssociatedService, },
            });

            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockRequirementsService
                .Setup(x => x.EditRequirement(order.Id, model.ItemId, model.SelectedOrderItemId, model.Details))
                .Returns(Task.CompletedTask);

            var result = await controller.EditRequirement(internalOrgId, order.CallOffId, model);

            mockOrderService.VerifyAll();
            mockRequirementsService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(RequirementController.Index));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteRequirement_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Requirement item,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IRequirementsService> mockRequirementsService,
            RequirementController controller)
        {
            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockRequirementsService
                .Setup(x => x.GetRequirement(order.Id, item.Id))
                .ReturnsAsync(item);

            var result = await controller.DeleteRequirement(internalOrgId, order.CallOffId, item.Id);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeEquivalentTo(new DeleteRequirementModel(order.CallOffId, internalOrgId, item.Id), x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteRequirement_ReturnsExpectedResult(
            string internalOrgId,
            DeleteRequirementModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IRequirementsService> mockRequirementsService,
            RequirementController controller)
        {
            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockRequirementsService
                .Setup(x => x.DeleteRequirement(order.Id, model.ItemId))
                .Returns(Task.CompletedTask);

            var result = await controller.DeleteRequirement(internalOrgId, order.CallOffId, model);

            mockOrderService.VerifyAll();
            mockRequirementsService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(RequirementController.Index));
        }
    }
}
