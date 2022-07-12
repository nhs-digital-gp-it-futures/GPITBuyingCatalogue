using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList
{
    public static class TaskListServiceTests
    {
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

        [Fact]
        public static void SetOrderTaskList_NullModel_Returns()
        {
            var expected = new OrderTaskList()
            {
                DescriptionStatus = TaskProgress.Completed,
                OrderingPartyStatus = TaskProgress.NotStarted,
            };

            var model = new OrderTaskList();

            TaskListService.SetOrderTaskList(TaskListOrderSections.Description, model);

            model.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void SetOrderTaskList_OrderingPartyCompleted_Returns()
        {
            var expected = new OrderTaskList()
            {
                DescriptionStatus = TaskProgress.Completed,
                OrderingPartyStatus = TaskProgress.Completed,
                SupplierStatus = TaskProgress.NotStarted,
            };

            var model = new OrderTaskList();

            var completedSections = TaskListOrderSections.Description | TaskListOrderSections.OrderingParty;

            TaskListService.SetOrderTaskList(completedSections, model);

            model.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void SetOrderTaskList_SupplierInProgress_Returns()
        {
            var expected = new OrderTaskList()
            {
                DescriptionStatus = TaskProgress.Completed,
                OrderingPartyStatus = TaskProgress.Completed,
                SupplierStatus = TaskProgress.InProgress,
            };

            var model = new OrderTaskList();

            var completedSections = TaskListOrderSections.Description | TaskListOrderSections.OrderingParty | TaskListOrderSections.Supplier;

            TaskListService.SetOrderTaskList(completedSections, model);

            model.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void SetOrderTaskList_Supplier_PreviousSectionNotCompleted_Returns()
        {
            var expected = new OrderTaskList()
            {
                DescriptionStatus = TaskProgress.Completed,
                OrderingPartyStatus = TaskProgress.NotStarted,
            };

            var model = new OrderTaskList();

            var completedSections = TaskListOrderSections.Description | TaskListOrderSections.Supplier;

            TaskListService.SetOrderTaskList(completedSections, model);

            model.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void SetOrderTaskList_SupplierComplete_Returns()
        {
            var expected = new OrderTaskList()
            {
                DescriptionStatus = TaskProgress.Completed,
                OrderingPartyStatus = TaskProgress.Completed,
                SupplierStatus = TaskProgress.Completed,
                CommencementDateStatus = TaskProgress.NotStarted,
            };

            var model = new OrderTaskList();

            var completedSections =
                TaskListOrderSections.Description
                | TaskListOrderSections.OrderingParty
                | TaskListOrderSections.Supplier
                | TaskListOrderSections.SupplierContact;

            TaskListService.SetOrderTaskList(completedSections, model);

            model.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void SetOrderTaskList_CommencementDateComplete_Returns()
        {
            var expected = new OrderTaskList()
            {
                DescriptionStatus = TaskProgress.Completed,
                OrderingPartyStatus = TaskProgress.Completed,
                SupplierStatus = TaskProgress.Completed,
                CommencementDateStatus = TaskProgress.Completed,
                SolutionOrService = TaskProgress.NotStarted,
            };

            var model = new OrderTaskList();

            var completedSections =
                TaskListOrderSections.Description
                | TaskListOrderSections.OrderingParty
                | TaskListOrderSections.Supplier
                | TaskListOrderSections.SupplierContact
                | TaskListOrderSections.CommencementDate;

            TaskListService.SetOrderTaskList(completedSections, model);

            model.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void SetOrderTaskList_SolutionInProgress_Returns()
        {
            var expected = new OrderTaskList()
            {
                DescriptionStatus = TaskProgress.Completed,
                OrderingPartyStatus = TaskProgress.Completed,
                SupplierStatus = TaskProgress.Completed,
                CommencementDateStatus = TaskProgress.Completed,
                SolutionOrService = TaskProgress.InProgress,
            };

            var model = new OrderTaskList();

            var completedSections =
                TaskListOrderSections.Description
                | TaskListOrderSections.OrderingParty
                | TaskListOrderSections.Supplier
                | TaskListOrderSections.SupplierContact
                | TaskListOrderSections.CommencementDate
                | TaskListOrderSections.SolutionOrServiceInProgress;

            TaskListService.SetOrderTaskList(completedSections, model);

            model.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void SetOrderTaskList_SolutionCompleted_Returns()
        {
            var expected = new OrderTaskList()
            {
                DescriptionStatus = TaskProgress.Completed,
                OrderingPartyStatus = TaskProgress.Completed,
                SupplierStatus = TaskProgress.Completed,
                CommencementDateStatus = TaskProgress.Completed,
                SolutionOrService = TaskProgress.Completed,
                FundingSource = TaskProgress.NotStarted,
            };

            var model = new OrderTaskList();

            var completedSections =
                TaskListOrderSections.Description
                | TaskListOrderSections.OrderingParty
                | TaskListOrderSections.Supplier
                | TaskListOrderSections.SupplierContact
                | TaskListOrderSections.CommencementDate
                | TaskListOrderSections.SolutionOrServiceInProgress
                | TaskListOrderSections.SolutionOrService;

            TaskListService.SetOrderTaskList(completedSections, model);

            model.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void SetOrderTaskList_FundingInProgress_Returns()
        {
            var expected = new OrderTaskList()
            {
                DescriptionStatus = TaskProgress.Completed,
                OrderingPartyStatus = TaskProgress.Completed,
                SupplierStatus = TaskProgress.Completed,
                CommencementDateStatus = TaskProgress.Completed,
                SolutionOrService = TaskProgress.Completed,
                FundingSource = TaskProgress.InProgress,
            };

            var model = new OrderTaskList();

            var completedSections =
                TaskListOrderSections.Description
                | TaskListOrderSections.OrderingParty
                | TaskListOrderSections.Supplier
                | TaskListOrderSections.SupplierContact
                | TaskListOrderSections.CommencementDate
                | TaskListOrderSections.SolutionOrServiceInProgress
                | TaskListOrderSections.SolutionOrService
                | TaskListOrderSections.FundingSourceInProgress;

            TaskListService.SetOrderTaskList(completedSections, model);

            model.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void SetOrderTaskList_FundingComplete_Returns()
        {
            var expected = new OrderTaskList()
            {
                DescriptionStatus = TaskProgress.Completed,
                OrderingPartyStatus = TaskProgress.Completed,
                SupplierStatus = TaskProgress.Completed,
                CommencementDateStatus = TaskProgress.Completed,
                SolutionOrService = TaskProgress.Completed,
                FundingSource = TaskProgress.Completed,
                ReviewAndCompleteStatus = TaskProgress.NotStarted,
            };

            var model = new OrderTaskList();

            var completedSections =
                TaskListOrderSections.Description
                | TaskListOrderSections.OrderingParty
                | TaskListOrderSections.Supplier
                | TaskListOrderSections.SupplierContact
                | TaskListOrderSections.CommencementDate
                | TaskListOrderSections.SolutionOrServiceInProgress
                | TaskListOrderSections.SolutionOrService
                | TaskListOrderSections.FundingSourceInProgress
                | TaskListOrderSections.FundingSource;

            TaskListService.SetOrderTaskList(completedSections, model);

            model.Should().BeEquivalentTo(expected);
        }
    }
}
