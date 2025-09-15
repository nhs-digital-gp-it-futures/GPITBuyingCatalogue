using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ListPriceModels;

public static class AddTieredListPriceModelTests
{
    [Theory]
    [MockAutoData]
    public static void GetBillingPeriod_Patient_PerYear(
        AddTieredListPriceModel model)
    {
        model.SelectedProvisioningType = ProvisioningType.Patient;

        model.GetBillingPeriod().Should().Be(TimeUnit.PerYear);
    }

    [Theory]
    [MockAutoData]
    public static void GetBillingPeriod_DeclarativeAssociatedService_Null(
        AddTieredListPriceModel model)
    {
        model.SelectedProvisioningType = ProvisioningType.Declarative;
        model.CatalogueItemType = CatalogueItemType.AssociatedService;

        model.GetBillingPeriod().Should().BeNull();
    }

    [Theory]
    [MockInlineAutoData(null)]
    [MockInlineAutoData(TimeUnit.PerMonth)]
    [MockInlineAutoData(TimeUnit.PerYear)]
    public static void GetBillingPeriod_Declarative_SelectedTimeUnit(
        TimeUnit? selectedBillingPeriod,
        AddTieredListPriceModel model)
    {
        model.SelectedProvisioningType = ProvisioningType.Declarative;
        model.DeclarativeBillingPeriod = selectedBillingPeriod;
        model.CatalogueItemType = CatalogueItemType.AdditionalService;

        model.GetBillingPeriod().Should().Be(selectedBillingPeriod);
    }

    [Theory]
    [MockInlineAutoData(null)]
    [MockInlineAutoData(TimeUnit.PerMonth)]
    [MockInlineAutoData(TimeUnit.PerYear)]
    public static void GetBillingPeriod_OnDemand_SelectedTimeUnit(
        TimeUnit? selectedBillingPeriod,
        AddTieredListPriceModel model)
    {
        model.SelectedProvisioningType = ProvisioningType.OnDemand;
        model.OnDemandBillingPeriod = selectedBillingPeriod;
        model.CatalogueItemType = CatalogueItemType.AdditionalService;

        model.GetBillingPeriod().Should().Be(selectedBillingPeriod);
    }

    [Theory]
    [MockInlineAutoData(CataloguePriceQuantityCalculationType.PerServiceRecipient)]
    [MockInlineAutoData(CataloguePriceQuantityCalculationType.PerSolutionOrService)]
    public static void GetQuantityCalculationType_Declarative_SelectedQuantityCalculationType(
        CataloguePriceQuantityCalculationType quantityCalculationType,
        AddTieredListPriceModel model)
    {
        model.SelectedProvisioningType = ProvisioningType.Declarative;
        model.DeclarativeQuantityCalculationType = quantityCalculationType;
        model.OnDemandQuantityCalculationType = null;

        model.GetQuantityCalculationType().Should().Be(quantityCalculationType);
    }

    [Theory]
    [MockInlineAutoData(CataloguePriceQuantityCalculationType.PerServiceRecipient)]
    [MockInlineAutoData(CataloguePriceQuantityCalculationType.PerSolutionOrService)]
    public static void GetQuantityCalculationType_OnDemand_SelectedQuantityCalculationType(
        CataloguePriceQuantityCalculationType quantityCalculationType,
        AddTieredListPriceModel model)
    {
        model.SelectedProvisioningType = ProvisioningType.OnDemand;
        model.DeclarativeQuantityCalculationType = null;
        model.OnDemandQuantityCalculationType = quantityCalculationType;

        model.GetQuantityCalculationType().Should().Be(quantityCalculationType);
    }
}
