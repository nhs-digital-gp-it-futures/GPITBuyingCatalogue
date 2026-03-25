using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Competitions;

public static class CompetitionSolutionTests
{
    [Theory]
    [MockAutoData]
    public static void CalculateTotalPrice_SolutionNoServices_ReturnsExpected(
        CompetitionSolution solution)
    {
        const int contractLength = 12;
        const int price = 1000;
        const int quantity = 5;
        const int expectedPrice = price * quantity;

        solution.Services = [];
        solution.Price = new CompetitionCatalogueItemPrice
        {
            CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
            CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient,
            ProvisioningType = ProvisioningType.Patient,
            BillingPeriod = TimeUnit.PerYear,
            Tiers = [new CompetitionCatalogueItemPriceTier { LowerRange = 0, UpperRange = null, Price = price }],
        };
        solution.Quantities = [new CompetitionItemQuantity { Quantity = quantity }];

        var total = solution.CalculateTotalPrice(contractLength);

        total.Should().Be(expectedPrice);
    }

    [Theory]
    [MockAutoData]
    public static void CalculateTotalPrice_SolutionWithAdditionalService_ReturnsExpected(
        CompetitionSolution solution,
        CompetitionAdditionalService additionalService)
    {
        const int contractLength = 12;
        const int price = 1000;
        const int quantity = 5;
        const int expectedPrice = (price * quantity) * 2;

        additionalService.Price = new CompetitionCatalogueItemPrice
        {
            CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
            CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient,
            ProvisioningType = ProvisioningType.Patient,
            BillingPeriod = TimeUnit.PerYear,
            Tiers = [new CompetitionCatalogueItemPriceTier { LowerRange = 0, UpperRange = null, Price = price }],
        };
        additionalService.Quantities = [new CompetitionItemQuantity { Quantity = quantity }];

        solution.Services = [additionalService];
        solution.Price = new CompetitionCatalogueItemPrice
        {
            CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
            CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient,
            ProvisioningType = ProvisioningType.Patient,
            BillingPeriod = TimeUnit.PerYear,
            Tiers = [new CompetitionCatalogueItemPriceTier { LowerRange = 0, UpperRange = null, Price = price }],
        };
        solution.Quantities = [new CompetitionItemQuantity { Quantity = quantity }];

        var total = solution.CalculateTotalPrice(contractLength);

        total.Should().Be(expectedPrice);
    }

    [Theory]
    [MockAutoData]
    public static void CalculateTotalPrice_SolutionWithOneOffCostAdditionalService_ReturnsExpected(
        CompetitionSolution solution,
        CompetitionAdditionalService additionalService)
    {
        const int contractLength = 12;
        const int price = 1000;
        const int quantity = 5;
        const int expectedPrice = (price * quantity) * 2;

        additionalService.Price = new CompetitionCatalogueItemPrice
        {
            CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
            CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient,
            ProvisioningType = ProvisioningType.Patient,
            BillingPeriod = null,
            Tiers = [new CompetitionCatalogueItemPriceTier { LowerRange = 0, UpperRange = null, Price = price }],
        };
        additionalService.Quantities = [new CompetitionItemQuantity { Quantity = quantity }];

        solution.Services = [additionalService];
        solution.Price = new CompetitionCatalogueItemPrice
        {
            CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
            CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient,
            ProvisioningType = ProvisioningType.Patient,
            BillingPeriod = TimeUnit.PerYear,
            Tiers = [new CompetitionCatalogueItemPriceTier { LowerRange = 0, UpperRange = null, Price = price }],
        };
        solution.Quantities = [new CompetitionItemQuantity { Quantity = quantity }];

        var total = solution.CalculateTotalPrice(contractLength);

        total.Should().Be(expectedPrice);
    }

    [Theory]
    [MockAutoData]
    public static void CalculateTotalPrice_SolutionWithMonthlyAdditionalService_ReturnsExpected(
        CompetitionSolution solution,
        CompetitionAdditionalService additionalService)
    {
        const int contractLength = 12;
        const int price = 1000;
        const int quantity = 5;
        const int expectedSolutionPrice = price * quantity;
        const int expectedAdditionalServicePrice = (price * quantity) * 12;
        const int expectedTotalPrice = expectedSolutionPrice + expectedAdditionalServicePrice;

        additionalService.Price = new CompetitionCatalogueItemPrice
        {
            CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
            CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient,
            ProvisioningType = ProvisioningType.Patient,
            BillingPeriod = TimeUnit.PerMonth,
            Tiers = [new CompetitionCatalogueItemPriceTier { LowerRange = 0, UpperRange = null, Price = price }],
        };
        additionalService.Quantities = [new CompetitionItemQuantity { Quantity = quantity }];

        solution.Services = [additionalService];
        solution.Price = new CompetitionCatalogueItemPrice
        {
            CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
            CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient,
            ProvisioningType = ProvisioningType.Patient,
            BillingPeriod = TimeUnit.PerYear,
            Tiers = [new CompetitionCatalogueItemPriceTier { LowerRange = 0, UpperRange = null, Price = price }],
        };
        solution.Quantities = [new CompetitionItemQuantity { Quantity = quantity }];

        var total = solution.CalculateTotalPrice(contractLength);

        total.Should().Be(expectedTotalPrice);
    }

    [Theory]
    [MockAutoData]
    public static void CalculateTotalPrice_SolutionWithOneOffCostAssociatedService_ReturnsExpected(
        CompetitionSolution solution,
        CompetitionAssociatedService associatedService)
    {
        const int contractLength = 12;
        const int price = 1000;
        const int quantity = 5;
        const int expectedPrice = (price * quantity) * 2;

        associatedService.Price = new CompetitionCatalogueItemPrice
        {
            CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
            CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient,
            ProvisioningType = ProvisioningType.Patient,
            BillingPeriod = null,
            Tiers = [new CompetitionCatalogueItemPriceTier { LowerRange = 0, UpperRange = null, Price = price }],
        };
        associatedService.Quantities = [new CompetitionItemQuantity { Quantity = quantity }];

        solution.Services = [associatedService];
        solution.Price = new CompetitionCatalogueItemPrice
        {
            CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
            CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient,
            ProvisioningType = ProvisioningType.Patient,
            BillingPeriod = TimeUnit.PerYear,
            Tiers = [new CompetitionCatalogueItemPriceTier { LowerRange = 0, UpperRange = null, Price = price }],
        };
        solution.Quantities = [new CompetitionItemQuantity { Quantity = quantity }];

        var total = solution.CalculateTotalPrice(contractLength);

        total.Should().Be(expectedPrice);
    }

    [Theory]
    [MockAutoData]
    public static void CalculateTotalPrice_SolutionWithServicesNoPrice_ReturnsExpected(
        CompetitionSolution solution,
        CompetitionAssociatedService associatedService,
        CompetitionAdditionalService additionalService)
    {
        const int contractLength = 12;
        const int price = 1000;
        const int quantity = 5;
        const int expectedPrice = price * quantity;

        additionalService.Price = associatedService.Price = null;
        additionalService.Quantities = associatedService.Quantities = [];

        solution.Services = [associatedService, additionalService];
        solution.Price = new CompetitionCatalogueItemPrice
        {
            CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
            CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient,
            ProvisioningType = ProvisioningType.Patient,
            BillingPeriod = TimeUnit.PerYear,
            Tiers = [new CompetitionCatalogueItemPriceTier { LowerRange = 0, UpperRange = null, Price = price }],
        };
        solution.Quantities = [new CompetitionItemQuantity { Quantity = quantity }];

        var total = solution.CalculateTotalPrice(contractLength);

        total.Should().Be(expectedPrice);
    }

    [Theory]
    [MockAutoData]
    public static void CalculateTotalPrice_SolutionWithNullServices_ReturnsExpected(
        CompetitionSolution solution)
    {
        const int contractLength = 12;
        const int price = 1000;
        const int quantity = 5;
        const int expectedPrice = price * quantity;

        solution.Services = null;
        solution.Price = new CompetitionCatalogueItemPrice
        {
            CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
            CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient,
            ProvisioningType = ProvisioningType.Patient,
            BillingPeriod = TimeUnit.PerYear,
            Tiers = [new CompetitionCatalogueItemPriceTier { LowerRange = 0, UpperRange = null, Price = price }],
        };
        solution.Quantities = [new CompetitionItemQuantity { Quantity = quantity }];

        var total = solution.CalculateTotalPrice(contractLength);

        total.Should().Be(expectedPrice);
    }

    [Theory]
    [MockAutoData]
    public static void CalculateTotalPrice_SolutionWithNullPrice_ReturnsExpected(
        CompetitionSolution solution)
    {
        const int contractLength = 12;
        const int expectedPrice = 0;

        solution.Services = null;
        solution.Price = null;
        solution.Quantities = [];

        var total = solution.CalculateTotalPrice(contractLength);

        total.Should().Be(expectedPrice);
    }
}
