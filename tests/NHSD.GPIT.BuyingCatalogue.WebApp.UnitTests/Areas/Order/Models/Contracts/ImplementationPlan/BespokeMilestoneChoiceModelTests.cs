using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.ImplementationPlan
{
    public static class BespokeMilestoneChoiceModelTests
    {
        [Fact]
        public static void Options_ReturnsExpectedOptions()
        {
            var model = new BespokeMilestoneChoiceModel();

            model.Options.Should().BeEquivalentTo(
                new List<SelectOption<bool>>
                {
                    new(BespokeMilestoneChoiceModel.Yes, true),
                    new(BespokeMilestoneChoiceModel.No, false),
                },
                options => options.WithStrictOrdering());
        }
    }
}
