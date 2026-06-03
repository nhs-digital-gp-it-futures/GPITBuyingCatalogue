using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList.Providers
{
    public static class DataProcessingStatusProviderTests
    {
        [Theory]
        [MockAutoData]
        public static void Get_OrderWrapperIsNull_ReturnsCannotStart(
            DataProcessingStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());
            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_OrderIsNull_ReturnsCannotStart(
            DataProcessingStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(), new OrderProgress());
            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_StateIsNull_ReturnsCannotStart(
            Order order,
            DataProcessingStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(order), null);
            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_AllDependentTaskCannotStart_ReturnsCannotStart(
            Order order,
            DataProcessingStatusProvider service)
        {
            var state = new OrderProgress();
            order.ContractFlags = null;
            var actual = service.Get(new OrderWrapper(order), state);
            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockInlineAutoData(OrderTaskListStatus.Description, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.Description, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.Description, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.Description, TaskProgress.Optional)]
        [MockInlineAutoData(OrderTaskListStatus.OrderingParty, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.OrderingParty, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.OrderingParty, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.OrderingParty, TaskProgress.Optional)]
        [MockInlineAutoData(OrderTaskListStatus.Supplier, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.Supplier, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.Supplier, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.Supplier, TaskProgress.Optional)]
        [MockInlineAutoData(OrderTaskListStatus.CommencementDate, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.CommencementDate, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.CommencementDate, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.CommencementDate, TaskProgress.Optional)]
        [MockInlineAutoData(OrderTaskListStatus.ServiceRecipients, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.ServiceRecipients, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.ServiceRecipients, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.ServiceRecipients, TaskProgress.Optional)]
        [MockInlineAutoData(OrderTaskListStatus.SolutionsOrServices, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.SolutionsOrServices, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.SolutionsOrServices, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.SolutionsOrServices, TaskProgress.Optional)]
        [MockInlineAutoData(OrderTaskListStatus.DeliveryDates, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.DeliveryDates, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.DeliveryDates, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.DeliveryDates, TaskProgress.Optional)]
        [MockInlineAutoData(OrderTaskListStatus.FundingSources, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.FundingSources, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.FundingSources, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.FundingSources, TaskProgress.Optional)]
        [MockInlineAutoData(OrderTaskListStatus.ImplementationPlan, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.ImplementationPlan, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.ImplementationPlan, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.ImplementationPlan, TaskProgress.Optional)]
        public static void Get_DependentTaskNotComplete_HasService_ReturnsICannotStart(
            OrderTaskListStatus task,
            TaskProgress status,
            Order order,
            DataProcessingStatusProvider service)
        {
            var state = GetDependentTaskOrderProgress(task, status, true);
            var actual = service.Get(new OrderWrapper(order), state);
            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockInlineAutoData(OrderTaskListStatus.Description, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.Description, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.Description, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.Description, TaskProgress.Optional)]
        [MockInlineAutoData(OrderTaskListStatus.OrderingParty, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.OrderingParty, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.OrderingParty, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.OrderingParty, TaskProgress.Optional)]
        [MockInlineAutoData(OrderTaskListStatus.Supplier, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.Supplier, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.Supplier, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.Supplier, TaskProgress.Optional)]
        [MockInlineAutoData(OrderTaskListStatus.CommencementDate, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.CommencementDate, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.CommencementDate, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.CommencementDate, TaskProgress.Optional)]
        [MockInlineAutoData(OrderTaskListStatus.ServiceRecipients, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.ServiceRecipients, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.ServiceRecipients, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.ServiceRecipients, TaskProgress.Optional)]
        [MockInlineAutoData(OrderTaskListStatus.SolutionsOrServices, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.SolutionsOrServices, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.SolutionsOrServices, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.SolutionsOrServices, TaskProgress.Optional)]
        [MockInlineAutoData(OrderTaskListStatus.DeliveryDates, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.DeliveryDates, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.DeliveryDates, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.DeliveryDates, TaskProgress.Optional)]
        [MockInlineAutoData(OrderTaskListStatus.FundingSources, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.FundingSources, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.FundingSources, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.FundingSources, TaskProgress.Optional)]
        [MockInlineAutoData(OrderTaskListStatus.ImplementationPlan, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.ImplementationPlan, TaskProgress.InProgress)]
        [MockInlineAutoData(OrderTaskListStatus.ImplementationPlan, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.ImplementationPlan, TaskProgress.Optional)]
        public static void Get_DependentTaskNotComplete_NoService_ReturnsICannotStart(
            OrderTaskListStatus task,
            TaskProgress status,
            Order order,
            DataProcessingStatusProvider service)
        {
            var state = GetDependentTaskOrderProgress(task, status, false);
            var actual = service.Get(new OrderWrapper(order), state);
            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockInlineAutoData(OrderTaskListStatus.AssociatedServicesBilling, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.AssociatedServicesBilling, TaskProgress.NotStarted)]
        [MockInlineAutoData(OrderTaskListStatus.AssociatedServicesRequirements, TaskProgress.CannotStart)]
        [MockInlineAutoData(OrderTaskListStatus.AssociatedServicesRequirements, TaskProgress.NotStarted)]
        public static void Get_AssociatedServiceTaskNotComplete_HasService_ReturnsCannotStart(
            OrderTaskListStatus task,
            TaskProgress status,
            Order order,
            DataProcessingStatusProvider service)
        {
            var state = GetDependentTaskOrderProgress(task, status, true);
            var actual = service.Get(new OrderWrapper(order), state);
            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockInlineAutoData(TaskProgress.Completed)]
        [MockInlineAutoData(TaskProgress.NotApplicable)]
        public static void Get_ContractInfoNotEntered_ReturnsNotStarted(
            TaskProgress status,
            Order order,
            DataProcessingStatusProvider service)
        {
            var state = new OrderProgress
            {
                DescriptionStatus = TaskProgress.Completed,
                OrderingPartyStatus = TaskProgress.Completed,
                SupplierStatus = TaskProgress.Completed,
                CommencementDateStatus = TaskProgress.Completed,
                ServiceRecipients = TaskProgress.Completed,
                SolutionOrService = TaskProgress.Completed,
                DeliveryDates = TaskProgress.Completed,
                FundingSource = TaskProgress.Completed,
                ImplementationPlan = TaskProgress.Completed,
                AssociatedServiceBilling = status,
                AssociatedServiceRequirements = status,
            };

            order.ContractFlags = null;

            var actual = service.Get(new OrderWrapper(order), state);
            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockInlineAutoData(TaskProgress.Completed)]
        [MockInlineAutoData(TaskProgress.NotApplicable)]
        public static void Get_ContractInfoEntered_ReturnsCompleted(
            TaskProgress status,
            bool useDefaultDataProcessing,
            Order order,
            DataProcessingStatusProvider service)
        {
            var state = new OrderProgress
            {
                DescriptionStatus = TaskProgress.Completed,
                OrderingPartyStatus = TaskProgress.Completed,
                SupplierStatus = TaskProgress.Completed,
                CommencementDateStatus = TaskProgress.Completed,
                ServiceRecipients = TaskProgress.Completed,
                SolutionOrService = TaskProgress.Completed,
                DeliveryDates = TaskProgress.Completed,
                FundingSource = TaskProgress.Completed,
                ImplementationPlan = TaskProgress.Completed,
                AssociatedServiceBilling = status,
                AssociatedServiceRequirements = status,
            };

            order.ContractFlags.UseDefaultDataProcessing = useDefaultDataProcessing;

            var actual = service.Get(new OrderWrapper(order), state);
            actual.Should().Be(TaskProgress.Completed);
        }

        private static OrderProgress GetDependentTaskOrderProgress(
            OrderTaskListStatus task,
            TaskProgress status,
            bool hasService)
        {
            var state = new OrderProgress
            {
                DescriptionStatus = TaskProgress.Completed,
                OrderingPartyStatus = TaskProgress.Completed,
                SupplierStatus = TaskProgress.Completed,
                CommencementDateStatus = TaskProgress.Completed,
                ServiceRecipients = TaskProgress.Completed,
                SolutionOrService = TaskProgress.Completed,
                DeliveryDates = TaskProgress.Completed,
                FundingSource = TaskProgress.Completed,
                ImplementationPlan = TaskProgress.Completed,
                AssociatedServiceBilling = TaskProgress.Completed,
                AssociatedServiceRequirements = TaskProgress.Completed,
            };

            if (!hasService)
            {
                state.AssociatedServiceBilling = TaskProgress.NotApplicable;
                state.AssociatedServiceRequirements = TaskProgress.NotApplicable;
            }

            switch (task)
            {
                case OrderTaskListStatus.Description:
                    state.DescriptionStatus = status;
                    break;
                case OrderTaskListStatus.OrderingParty:
                    state.OrderingPartyStatus = status;
                    break;
                case OrderTaskListStatus.Supplier:
                    state.SupplierStatus = status;
                    break;
                case OrderTaskListStatus.CommencementDate:
                    state.CommencementDateStatus = status;
                    break;
                case OrderTaskListStatus.ServiceRecipients:
                    state.ServiceRecipients = status;
                    break;
                case OrderTaskListStatus.SolutionsOrServices:
                    state.SolutionOrService = status;
                    break;
                case OrderTaskListStatus.DeliveryDates:
                    state.DeliveryDates = status;
                    break;
                case OrderTaskListStatus.FundingSources:
                    state.FundingSource = status;
                    break;
                case OrderTaskListStatus.ImplementationPlan:
                    state.ImplementationPlan = status;
                    break;
                case OrderTaskListStatus.AssociatedServicesBilling:
                    state.AssociatedServiceBilling = status;
                    break;
                case OrderTaskListStatus.AssociatedServicesRequirements:
                    state.AssociatedServiceRequirements = status;
                    break;
            }

            return state;
        }
    }
}
