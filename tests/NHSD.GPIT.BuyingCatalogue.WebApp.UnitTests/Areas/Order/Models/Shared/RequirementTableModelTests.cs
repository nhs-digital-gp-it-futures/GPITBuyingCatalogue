using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Shared
{
    public static class RequirementTableModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string title, ICollection<Requirement> items, bool isAction, CallOffId callOffId, string internalOrgId)
        {
            var model = new RequirementTableModel(title, items, isAction, callOffId, internalOrgId);

            model.Title.Should().Be(title);
            model.Requirements.Should().BeEquivalentTo(items);
            model.IsAction.Should().Be(isAction);
            model.CallOffId.Should().Be(callOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
        }
    }
}
