using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AdditionalServices
{
    public static class SelectAdditionalServicePriceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            CallOffId callOffId,
            string solutionName,
            List<CataloguePrice> prices)
        {
            var model = new SelectAdditionalServicePriceModel(odsCode, callOffId, solutionName, prices);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{callOffId}/additional-services/select/additional-service");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"List price for {solutionName}");
            model.OdsCode.Should().Be(odsCode);
            model.Prices.Should().HaveCount(prices.Count);
            model.Prices.First().CataloguePriceId.Should().Be(prices.First().CataloguePriceId);
            model.Prices.First().Description.Should()
                .Be($"£{prices.First().Price} {prices.First().PricingUnit?.Description} {prices.First().TimeUnit?.Description()}");
        }
    }
}
