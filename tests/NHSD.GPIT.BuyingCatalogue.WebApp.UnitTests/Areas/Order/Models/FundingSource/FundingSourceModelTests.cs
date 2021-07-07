using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSource;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.FundingSource
{
    public static class FundingSourceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode, 
            CallOffId callOffId, 
            bool? fundingSourceOnlyGms
            )
        {
            var model = new FundingSourceModel(odsCode, callOffId, fundingSourceOnlyGms);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{callOffId}");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Funding source for {callOffId}");
            model.FundingSourceOnlyGms.Should().Be(fundingSourceOnlyGms.ToYesNo());
        }
    }
}
