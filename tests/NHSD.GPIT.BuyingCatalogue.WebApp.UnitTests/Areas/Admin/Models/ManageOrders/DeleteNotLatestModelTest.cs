using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ManageOrders
{
    public static class DeleteNotLatestModelTest
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            CallOffId callOffId)
        {
            var model = new DeleteNotLatestModel(callOffId);

            model.CallOffId.Should().Be(callOffId);
            model.IsAmendment.Should().Be(callOffId.IsAmendment);
        }
    }
}
