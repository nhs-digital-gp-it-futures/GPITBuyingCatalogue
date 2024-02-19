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
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.TaskList;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection
{
    public static class TaskListControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(TaskListController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(TaskListController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(TaskListController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task TaskList_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            TaskListController controller)
        {
            mockOrderService
                .Setup(x => x.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.TaskList(internalOrgId, callOffId);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new TaskListModel(internalOrgId, callOffId, new OrderWrapper(order));

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink).Excluding(m => m.OnwardLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task TaskList_WithAdditionalServicesAvailable_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> additionalServices,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IAdditionalServicesService> mockAdditionalServicesService,
            TaskListController controller)
        {
            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockAdditionalServicesService
                .Setup(x => x.GetAdditionalServicesBySolutionId(order.OrderItems.First().CatalogueItemId, true))
                .ReturnsAsync(additionalServices);

            var result = await controller.TaskList(internalOrgId, callOffId);

            mockOrderService.VerifyAll();
            mockAdditionalServicesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new TaskListModel(internalOrgId, callOffId, new OrderWrapper(order))
            {
                AdditionalServicesAvailable = true,
                UnselectedAdditionalServicesAvailable = true,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink).Excluding(m => m.OnwardLink));
        }

        [Theory]
        [CommonInlineAutoData(1)]
        [CommonInlineAutoData(2)]
        public static async Task TaskList_EnsureOrderItemsForAmendment_ExpectedResult2(
            int revision,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            TaskListController controller)
        {
            order.Revision = revision;
            order.OrderType = OrderTypeEnum.Solution;

            mockOrderService
                .Setup(x => x.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockOrderService
                .Setup(x => x.EnsureOrderItemsForAmendment(internalOrgId, callOffId));

            var result = await controller.TaskList(internalOrgId, callOffId);

            mockOrderService.Verify(x => x.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId), Times.Once);

            if (revision == 1)
            {
                mockOrderService.Verify(x => x.EnsureOrderItemsForAmendment(internalOrgId, callOffId), Times.Never);
            }
            else
            {
                mockOrderService.Verify(x => x.EnsureOrderItemsForAmendment(internalOrgId, callOffId), Times.Once);
            }
        }
    }
}
