using System.Collections.Generic;
using System.Diagnostics.Contracts;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;
using Contract = NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models.Contract;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models
{
    public static class OrderSummaryModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            ImplementationPlan defaultPlan,
            Contract contract,
            Order order)
        {
            contract.ImplementationPlan = new ImplementationPlan()
            {
                Milestones = new List<ImplementationPlanMilestone>() { new ImplementationPlanMilestone(), },
            };

            order.Contract = contract;

            var model = new OrderSummaryModel(new OrderWrapper(order), defaultPlan);

            model.DefaultImplementationPlan.Should().Be(defaultPlan);
            model.BespokePlan.Should().BeEquivalentTo(order.Contract.ImplementationPlan);
            model.HasBespokeMilestones.Should().BeTrue();
            model.DefaultMilestoneLabelText.Should().Be("Default milestones and payment triggers");
        }

        [Theory]
        [CommonAutoData]
        public static void NullBespokePlan_PropertiesCorrectlySet(
            ImplementationPlan defaultPlan,
            Contract contract,
            Order order)
        {
            contract.ImplementationPlan = null;

            order.Contract = contract;

            var model = new OrderSummaryModel(new OrderWrapper(order), defaultPlan);

            model.HasBespokeMilestones.Should().BeFalse();
            model.DefaultMilestoneLabelText.Should().Be("Milestones and payment triggers");
        }

        [Theory]
        [CommonAutoData]
        public static void NoMilestones_PropertiesCorrectlySet(
            ImplementationPlan defaultPlan,
            Contract contract,
            Order order)
        {
            contract.ImplementationPlan = new ImplementationPlan()
            {
                Milestones = new List<ImplementationPlanMilestone>(),
            };

            order.Contract = contract;

            var model = new OrderSummaryModel(new OrderWrapper(order), defaultPlan);

            model.HasBespokeMilestones.Should().BeFalse();
            model.DefaultMilestoneLabelText.Should().Be("Milestones and payment triggers");
        }
    }
}
