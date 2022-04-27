using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AssociatedServices
{
    public static class SelectAssociatedServicePriceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            string solutionName,
            List<CataloguePrice> prices)
        {
            prices.ForEach(cp => cp.CurrencyCode = "GBP");
            var model = new SelectAssociatedServicePriceModel(internalOrgId, solutionName, prices);

            model.Title.Should().Be($"List price for {solutionName}");
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Prices.Should().HaveCount(prices.Count);
            model.Prices.First().CataloguePriceId.Should().Be(prices.First().CataloguePriceId);
            model.Prices.First().Description.Should()
                .Be($"£{prices.First().Price} {prices.First().PricingUnit?.Description} {prices.First().TimeUnit?.Description()}");
        }
    }
}
