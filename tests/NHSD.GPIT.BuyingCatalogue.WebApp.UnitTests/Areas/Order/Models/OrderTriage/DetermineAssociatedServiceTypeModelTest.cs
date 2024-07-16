using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.OrderTriage
{
    public static class DetermineAssociatedServiceTypeModelTest
    {
        [Theory]
        [MockInlineAutoData(true, true)]
        [MockInlineAutoData(true, false)]
        [MockInlineAutoData(false, true)]
        public static void GetPageTitle_Either_Merger_Or_Split_Or_Both_Enabled(
            bool mergerEnabled,
            bool splitEnabled,
            string organisationName)
        {
            var model = new DetermineAssociatedServiceTypeModel(organisationName, mergerEnabled, splitEnabled);
            model.GetPageTitle().Should().Be(DetermineAssociatedServiceTypeModel.PageTitle with { Caption = organisationName });
        }

        [Theory]
        [MockInlineAutoData(false, false)]
        public static void GetPageTitle_Only_Other(
            bool mergerEnabled,
            bool splitEnabled,
            string organisationName)
        {
            var model = new DetermineAssociatedServiceTypeModel(organisationName, mergerEnabled, splitEnabled);
            model.GetPageTitle().Should().Be(DetermineAssociatedServiceTypeModel.NoSuppliersForMergerAndSplitSPageTitle with { Caption = organisationName });
        }
    }
}
