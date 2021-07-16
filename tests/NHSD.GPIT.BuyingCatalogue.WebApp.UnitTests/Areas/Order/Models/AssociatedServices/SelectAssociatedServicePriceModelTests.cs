using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AssociatedServices
{
    public static class SelectAssociatedServicePriceModelTests
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

            var model = new SelectAssociatedServicePriceModel(odsCode, callOffId, solutionName, prices);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{callOffId}/associated-services/select/associated-service");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"List price for {solutionName}");
            model.OdsCode.Should().Be(odsCode);

            // TODO: model.Prices
        }
    }
}
