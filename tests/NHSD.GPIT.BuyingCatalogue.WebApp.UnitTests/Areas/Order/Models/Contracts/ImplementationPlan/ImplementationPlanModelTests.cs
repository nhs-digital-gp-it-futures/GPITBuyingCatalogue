using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.ImplementationPlan
{
    public static class ImplementationPlanModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void NullBespokePlan_ThrowsException(
            EntityFramework.Ordering.Models.ImplementationPlan bespokePlan,
            Solution solution)
        {
            FluentActions
                .Invoking(() => new ImplementationPlanModel(null, bespokePlan, solution))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.ImplementationPlan defaultPlan,
            EntityFramework.Ordering.Models.ImplementationPlan bespokePlan,
            Solution solution)
        {
            bespokePlan.Milestones.Add(new ImplementationPlanMilestone());

            var model = new ImplementationPlanModel(defaultPlan, bespokePlan, solution);

            model.DefaultPlan.Should().Be(defaultPlan);
            model.BespokePlan.Should().Be(bespokePlan);
            model.Solution.Should().Be(solution);
            model.HasBespokeMilestones.Should().BeTrue();
            model.DefaultMilestoneLabelText.Should().Be("Default milestones and payment triggers");
        }

        [Theory]
        [CommonAutoData]
        public static void NullSolution_SupplierImplementationPlanCorrectlySet(
            EntityFramework.Ordering.Models.ImplementationPlan defaultPlan,
            EntityFramework.Ordering.Models.ImplementationPlan bespokePlan)
        {
            var model = new ImplementationPlanModel(defaultPlan, bespokePlan, null);

            model.SupplierImplementationPlan.Should().Be("The supplier has not provided a standard implementation plan. You should contact them to discuss this.");
        }

        [Theory]
        [CommonAutoData]
        public static void NullImplementationDetail_SupplierImplementationPlanCorrectlySet(
            EntityFramework.Ordering.Models.ImplementationPlan defaultPlan,
            EntityFramework.Ordering.Models.ImplementationPlan bespokePlan,
            Solution solution)
        {
            solution.ImplementationDetail = null;

            var model = new ImplementationPlanModel(defaultPlan, bespokePlan, solution);

            model.SupplierImplementationPlan.Should().Be("The supplier has not provided a standard implementation plan. You should contact them to discuss this.");
        }

        [Theory]
        [CommonAutoData]
        public static void NullBespokePlan_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.ImplementationPlan defaultPlan,
            Solution solution)
        {
            var model = new ImplementationPlanModel(defaultPlan, null, solution);

            model.HasBespokeMilestones.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void NoMilestones_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.ImplementationPlan defaultPlan,
            EntityFramework.Ordering.Models.ImplementationPlan bespokePlan,
            Solution solution)
        {
            bespokePlan.Milestones.Clear();

            var model = new ImplementationPlanModel(defaultPlan, bespokePlan, solution);

            model.HasBespokeMilestones.Should().BeFalse();
        }
    }
}
