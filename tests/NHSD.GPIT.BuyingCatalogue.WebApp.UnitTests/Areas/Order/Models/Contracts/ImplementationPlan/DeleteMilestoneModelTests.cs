using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.ImplementationPlan
{
    public static class DeleteMilestoneModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            CallOffId callOffId, string internalOrgId, ImplementationPlanMilestone milestone)
        {
            var model = new DeleteMilestoneModel(callOffId, internalOrgId, milestone);

            model.CallOffId.Should().Be(callOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.MilestoneId.Should().Be(milestone.Id);
            model.MilestoneName.Should().Be(milestone.Title);
            model.MilestonePaymentTrigger.Should().Be(milestone.PaymentTrigger);
        }
    }
}
