using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.ImplementationPlan
{
    public static class MilestoneModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_Add_PropertiesCorrectlySet(
            CallOffId callOffId, string internalOrgId)
        {
            var model = new MilestoneModel(callOffId, internalOrgId);

            model.CallOffId.Should().Be(callOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.IsEdit.Should().BeFalse();
            model.Title.Should().Be("Add a bespoke implementation milestone");
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_Edit_PropertiesCorrectlySet(
            CallOffId callOffId, string internalOrgId, ImplementationPlanMilestone milestone)
        {
            var model = new MilestoneModel(milestone, callOffId, internalOrgId);

            model.Name.Should().Be(milestone.Title);
            model.PaymentTrigger.Should().Be(milestone.PaymentTrigger);
            model.IsEdit.Should().BeTrue();
            model.Title.Should().Be("Edit a bespoke implementation milestone");
        }
    }
}
