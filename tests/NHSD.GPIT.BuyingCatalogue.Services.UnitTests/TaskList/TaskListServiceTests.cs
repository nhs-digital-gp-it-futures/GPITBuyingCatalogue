using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList
{
    public static class TaskListServiceTests
    {
        /* TODO - Reimplement Task list Unit Tests
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(TaskListService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task NullOrder_Returns_DefaultModel(
            TaskListService service)
        {
            var expected = new OrderTaskList();

            var actual = await service.GetTaskListStatusModelForOrder(null);

            actual.Should().BeEquivalentTo(expected);
        }

        /*TODO - Tiered Pricing - Fix Test
        [Theory]
        [CommonAutoData]
        public static void CompleteOrder_AllStatuses_Correct(
            Order order,
            OrderItem solutionOrderItem,
            OrderItem associatedServiceOrderItem,
            OrderItem additionalServiceOrderItem,
            TaskListService service)
        {
            order.ConfirmedFundingSource = true;
            solutionOrderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            associatedServiceOrderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            additionalServiceOrderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            order.AddOrUpdateOrderItem(solutionOrderItem);
            order.AddOrUpdateOrderItem(associatedServiceOrderItem);
            order.AddOrUpdateOrderItem(additionalServiceOrderItem);

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.DescriptionStatus.Should().Be(TaskProgress.Completed);
            actual.OrderingPartyStatus.Should().Be(TaskProgress.Completed);
            actual.SupplierStatus.Should().Be(TaskProgress.Completed);
            actual.CommencementDateStatus.Should().Be(TaskProgress.Completed);
            actual.ReviewAndCompleteStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static async Task NoOrderingParty_OrderingPartyStatus_NotStarted()
        {
            var orderSections = new OrderTaskListCompletedSections();

            var mockService = new Mock<TaskListService>().As<ITaskListService>();

            mockService.CallBase = true;

            mockService.Setup(tl => tl.GetOrderSectionFlags(It.IsAny<int>())).ReturnsAsync(orderSections);

            var actual = await mockService.Object.GetTaskListStatusModelForOrder(0);

            actual.OrderingPartyStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static async Task NoSupplier_SupplierStatus_NotStarted()
        {
            var orderSections = new OrderTaskListCompletedSections
            {
                OrderContactDetailsCompleted = true,
            };

            var mockService = new Mock<TaskListService>
            {
                CallBase = true,
            };

            mockService.Setup(tl => tl.GetOrderSectionFlags(It.IsAny<int>())).ReturnsAsync(orderSections);

            var actual = await mockService.Object.GetTaskListStatusModelForOrder(0);

            actual.SupplierStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static async Task NoSupplierContact_SupplierStatus_InProgress()
        {
            var orderSections = new OrderTaskListCompletedSections
            {
                OrderContactDetailsCompleted = true,
                SupplierSelected = true,
            };

            var mockService = new Mock<TaskListService>
            {
                CallBase = true,
            };

            mockService.Setup(tl => tl.GetOrderSectionFlags(It.IsAny<int>())).ReturnsAsync(orderSections);

            var actual = await mockService.Object.GetTaskListStatusModelForOrder(0);

            actual.SupplierStatus.Should().Be(TaskProgress.InProgress);
        }

        [Fact]
        public static async Task NoSupplier_NoOrderingParty_SupplierStatus_CannotStart()
        {
            var orderSections = new OrderTaskListCompletedSections();

            var mockService = new Mock<TaskListService>
            {
                CallBase = true,
            };

            mockService.Setup(tl => tl.GetOrderSectionFlags(It.IsAny<int>())).ReturnsAsync(orderSections);

            var actual = await mockService.Object.GetTaskListStatusModelForOrder(0);

            actual.SupplierStatus.Should().Be(TaskProgress.CannotStart);
        }

        [Fact]
        public static async Task NoCommencementDate_CommencementDateStatus_NotStarted()
        {
            var orderSections = new OrderTaskListCompletedSections
            {
                OrderContactDetailsCompleted = true,
                SupplierSelected = true,
                SupplierContactSelected = true,
            };

            var mockService = new Mock<TaskListService>
            {
                CallBase = true,
            };

            mockService.Setup(tl => tl.GetOrderSectionFlags(It.IsAny<int>())).ReturnsAsync(orderSections);

            var actual = await mockService.Object.GetTaskListStatusModelForOrder(0);

            actual.CommencementDateStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static async Task NoCommencementDate_NoSupplierContact_CommencementDateStatus_CannotStart()
        {
            var orderSections = new OrderTaskListCompletedSections
            {
                OrderContactDetailsCompleted = true,
                SupplierSelected = true,
            };

            var mockService = new Mock<TaskListService>
            {
                CallBase = true,
            };

            mockService.Setup(tl => tl.GetOrderSectionFlags(It.IsAny<int>())).ReturnsAsync(orderSections);

            var actual = await mockService.Object.GetTaskListStatusModelForOrder(0);

            actual.CommencementDateStatus.Should().Be(TaskProgress.CannotStart);
        }

        /*TODO - Tiered Pricing - Fix Test
        [Theory]
        [CommonAutoData]
        public static void NoFundingSource_ReviewAndCompleteStatus_CannotStart(
            Order order,
            TaskListService service)
        {
            order.FundingSourceOnlyGms = null;
            order.ConfirmedFundingSource = null;

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.ReviewAndCompleteStatus.Should().Be(TaskProgress.CannotStart);
        }*/
    }
}
