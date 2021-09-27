using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ListPriceModels
{
    public static class EditListPriceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Construct_ValidCatalogueItem_SetsPropertiesAsExpected(
            CatalogueItem catalogueItem)
        {
            var editListPriceModel = new EditListPriceModel(catalogueItem);

            editListPriceModel.SolutionName.Should().Be(catalogueItem.Name);
        }

        [Theory]
        [CommonAutoData]
        public static void GetPricingUnit_SetsDetailsAsExpected(
            CatalogueItem catalogueItem)
        {
            const string expectedUnitDefinition = "definition";
            const string expectedUnitDescription = "per patient";

            var editListPriceModel = new EditListPriceModel(catalogueItem)
            {
                Unit = expectedUnitDescription,
                UnitDefinition = expectedUnitDefinition,
            };

            var actualUnit = editListPriceModel.GetPricingUnit();

            actualUnit.Should().NotBeNull();
            actualUnit.Definition.Should().Be(expectedUnitDefinition);
            actualUnit.Description.Should().Be(expectedUnitDescription);
        }

        [Theory]
        [CommonInlineAutoData(ProvisioningType.Patient, TimeUnit.PerMonth, TimeUnit.PerMonth, TimeUnit.PerYear)]
        [CommonInlineAutoData(ProvisioningType.Declarative, TimeUnit.PerMonth, TimeUnit.PerYear, TimeUnit.PerMonth)]
        [CommonInlineAutoData(ProvisioningType.OnDemand, TimeUnit.PerMonth, TimeUnit.PerYear, TimeUnit.PerYear)]
        public static void GetTimeUnit_WithPatientProvisioningType_ReturnsTimeUnit(
            ProvisioningType provisioningType,
            TimeUnit? declarativeTimeUnit,
            TimeUnit? onDemandTimeUnit,
            TimeUnit? expectedTimeUnit,
            CatalogueItem catalogueItem)
        {
            var editListPriceModel = new EditListPriceModel(catalogueItem)
            {
                SelectedProvisioningType = provisioningType,
                DeclarativeTimeUnit = declarativeTimeUnit,
                OnDemandTimeUnit = onDemandTimeUnit,
            };

            var actualTimeUnit = editListPriceModel.GetTimeUnit(provisioningType);

            actualTimeUnit.Should().Be(expectedTimeUnit);
        }
    }
}
