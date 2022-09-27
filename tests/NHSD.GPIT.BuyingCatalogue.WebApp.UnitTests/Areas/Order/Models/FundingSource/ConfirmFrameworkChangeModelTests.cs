using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSources;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.FundingSource
{
    public static class ConfirmFrameworkChangeModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void ConfirmFrameworkChangeModel_WithArguments_SetsCorrectly(
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Catalogue.Models.Framework framework)
        {
            var model = new ConfirmFrameworkChangeModel(order, framework);

            model.Title.Should().Be(ConfirmFrameworkChangeModel.TitleText);
            model.Caption.Should().Be($"Order {order.CallOffId}");
            model.CurrentFramework.Should().Be(order.SelectedFramework);
            model.SelectedFramework.Should().Be(framework);
            model.ConfirmChanges.Should().BeNull();
        }
    }
}
