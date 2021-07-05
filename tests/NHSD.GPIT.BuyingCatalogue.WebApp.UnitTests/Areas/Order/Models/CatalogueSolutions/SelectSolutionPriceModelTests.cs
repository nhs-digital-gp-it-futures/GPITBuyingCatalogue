using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.CatalogueSolutions
{
    public static class SelectSolutionPriceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            CallOffId callOffId,
            string solutionName,
            List<CataloguePrice> prices)
        {
            // TODO: Add a customization
            prices.ForEach(p => p.CurrencyCode = "GBP");

            var model = new SelectSolutionPriceModel(odsCode, callOffId, solutionName, prices);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"List price for {solutionName}");
            model.OdsCode.Should().Be(odsCode);


            // TODO: model.Prices
        }
    }
}
