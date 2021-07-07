using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AdditionalServices
{
    public static class SelectAdditionalServiceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            CallOffId callOffId,
            List<CatalogueItem> solutions, 
            CatalogueItemId? selectedSolutionId)
        {
            var model = new SelectAdditionalServiceModel(odsCode, callOffId, solutions, selectedSolutionId);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{callOffId}/additional-services");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Add Additional Service for {callOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.Solutions.Should().BeEquivalentTo(solutions);
            model.SelectedAdditionalServiceId.Should().Be(selectedSolutionId);
        }
    }
}
