using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList
{
    public static class OrderProgressServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderProgressService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInlineAutoData(TaskProgress.CannotStart)]
        [MockInlineAutoData(TaskProgress.InProgress)]
        [MockInlineAutoData(TaskProgress.NotStarted)]
        [MockInlineAutoData(TaskProgress.Optional)]
        [MockInlineAutoData(TaskProgress.NotApplicable)]
        public static void Get_AssociatedServiceBillingNotComplete_ReturnsCannotStart(
    TaskProgress billingStatus,
    Order order,
    AssociatedServiceRequirementsStatusProvider service)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;

            var state = new OrderProgress
            {
                AssociatedServiceBilling = billingStatus,
            };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_AssociatedServiceBillingComplete_RequirementsNotEntered_ReturnsNotStarted(
            Order order,
            AssociatedServiceRequirementsStatusProvider service)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.Contract = new Contract
            {
                ContractBilling = new ContractBilling
                {
                    HasConfirmedRequirements = false,
                },
            };

            var state = new OrderProgress
            {
                AssociatedServiceBilling = TaskProgress.Completed,
            };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void Get_AssociatedServiceBillingComplete_RequirementsEntered_ReturnsCompleted(
            Order order,
            AssociatedServiceRequirementsStatusProvider service)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.Contract = new Contract
            {
                ContractBilling = new ContractBilling
                {
                    HasConfirmedRequirements = true,
                },
            };

            var state = new OrderProgress
            {
                AssociatedServiceBilling = TaskProgress.Completed,
            };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.Completed);
        }
    }
}
