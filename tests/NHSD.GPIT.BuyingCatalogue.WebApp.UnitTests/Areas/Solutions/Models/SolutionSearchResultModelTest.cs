using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SolutionsFilterModels;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class SolutionSearchResultModelTest
    {
        [Theory]
        [CommonAutoData]
        public static void Constructor_Sets_Properties_NoLinks_Defaults_To_False(CatalogueItem catalogueItem, ICollection<CapabilitiesAndCountModel> capabilitiesAndCounts)
        {
            var model = new SolutionSearchResultModel(catalogueItem, capabilitiesAndCounts);

            model.CatalogueItem.Should().Be(catalogueItem);
            model.SelectedCapabilityIds.Should().BeEquivalentTo(capabilitiesAndCounts);
            model.NoLinks.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_Sets_Properties_NoLinks_True(CatalogueItem catalogueItem, ICollection<CapabilitiesAndCountModel> capabilitiesAndCounts)
        {
            var model = new SolutionSearchResultModel(catalogueItem, capabilitiesAndCounts, true);

            model.CatalogueItem.Should().Be(catalogueItem);
            model.SelectedCapabilityIds.Should().BeEquivalentTo(capabilitiesAndCounts);
            model.NoLinks.Should().BeTrue();
        }
    }
}
