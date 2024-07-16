using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Shared
{
    public static class MilestoneTableModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string title, ICollection<ImplementationPlanMilestone> milestones, bool isAction, CallOffId callOffId, string internalOrgId)
        {
            var model = new MilestoneTableModel(title, milestones, isAction, callOffId, internalOrgId);

            model.Title.Should().Be(title);
            model.Milestones.Should().BeEquivalentTo(milestones);
            model.IsAction.Should().Be(isAction);
            model.CallOffId.Should().Be(callOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
        }
    }
}
