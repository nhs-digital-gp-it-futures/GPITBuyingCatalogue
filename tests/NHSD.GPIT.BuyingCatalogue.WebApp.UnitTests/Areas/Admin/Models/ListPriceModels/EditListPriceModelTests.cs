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

            editListPriceModel.SolutionId.Should().Be(catalogueItem.Id);
            editListPriceModel.SolutionName.Should().Be(catalogueItem.Name);
        }

        [Theory]
        [CommonAutoData]
        public static void GetPricingUnit_SetsDetailsAsExpected(
            CatalogueItem catalogueItem)
        {
            const string expectedUnitName = "patient";
            const string expectedUnitTierName = "patient";
            const string expectedUnitDefinition = "definition";
            const string expectedUnitDescription = "per patient";

            var editListPriceModel = new EditListPriceModel(catalogueItem)
            {
                Unit = expectedUnitDescription,
                UnitDefinition = expectedUnitDefinition,
            };

            var actualUnit = editListPriceModel.GetPricingUnit();

            actualUnit.Should().NotBeNull();
            actualUnit.Name.Should().Be(expectedUnitName);
            actualUnit.TierName.Should().Be(expectedUnitTierName);
            actualUnit.Definition.Should().Be(expectedUnitDefinition);
            actualUnit.Description.Should().Be(expectedUnitDescription);
        }

        [Theory]
        [CommonAutoData]
        public static void GetPricingUnit_UnitDescriptionDoesNotStartWithPer_PrependsPerToDescription(
            CatalogueItem catalogueItem)
        {
            const string unitDescription = "patient";
            const string expectedUnitDescription = "per patient";

            var editListPriceModel = new EditListPriceModel(catalogueItem)
            {
                Unit = unitDescription,
            };

            var actualUnit = editListPriceModel.GetPricingUnit();

            actualUnit.Should().NotBeNull();
            actualUnit.Description.Should().Be(expectedUnitDescription);
        }

        [Theory]
        [CommonInlineAutoData(ProvisioningType.Patient)]
        [CommonInlineAutoData(ProvisioningType.Declarative)]
        [CommonInlineAutoData(ProvisioningType.OnDemand)]
        public static void GetProvisioningType_SetsDetailsAsExpected(
            ProvisioningType provisioningType,
            CatalogueItem catalogueItem)
        {
            var editListPriceModel = new EditListPriceModel(catalogueItem)
            {
                SelectedProvisioningType = provisioningType.ToString(),
            };

            editListPriceModel.TryGetProvisioningType(out var actualProvisioningType);

            actualProvisioningType.Should().Be(provisioningType);
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
                SelectedProvisioningType = provisioningType.ToString(),
                DeclarativeTimeUnit = declarativeTimeUnit,
                OnDemandTimeUnit = onDemandTimeUnit,
            };

            var actualTimeUnit = editListPriceModel.GetTimeUnit(provisioningType);

            actualTimeUnit.Should().Be(expectedTimeUnit);
        }

        [Theory]
        [CommonAutoData]
        public static void TryParsePrice_WithValidPrice_ParsesSuccessfully(
            CatalogueItem catalogueItem)
        {
            const decimal expectedPrice = 3.12M;

            var editListPriceModel = new EditListPriceModel(catalogueItem)
            {
                Price = expectedPrice.ToString(),
            };

            var result = editListPriceModel.TryParsePrice(out var actualPrice);

            result.Should().BeTrue();
            actualPrice.Should().Be(expectedPrice);
        }

        [Theory]
        [CommonAutoData]
        public static void TryParsePrice_WithInalidPrice_DoesNotParse(
            CatalogueItem catalogueItem)
        {
            var invalidPrice = string.Empty;

            var editListPriceModel = new EditListPriceModel(catalogueItem)
            {
                Price = invalidPrice,
            };

            var result = editListPriceModel.TryParsePrice(out var actualPrice);

            result.Should().BeFalse();
        }
    }
}
