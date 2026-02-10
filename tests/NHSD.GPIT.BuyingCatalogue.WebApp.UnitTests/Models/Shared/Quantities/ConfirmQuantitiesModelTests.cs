using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Shared.Quantities;

public static class ConfirmQuantitiesModelTests
{
    [Theory]
    [MockInlineAutoData(ConfirmQuantitiesModel.AdvicePatientText, ProvisioningType.Patient)]
    [MockInlineAutoData(ConfirmQuantitiesModel.AdviceText, ProvisioningType.OnDemand)]
    [MockInlineAutoData(ConfirmQuantitiesModel.AdviceText, ProvisioningType.Declarative)]
    public static void Competition_Advice_ProvisioningType_SetsExpected(
        string expectedAdvice,
        ProvisioningType provisioningType,
        CatalogueItem catalogueItem,
        CompetitionCatalogueItemPrice price,
        List<ServiceRecipientQuantityDto> recipients)
    {
        price.ProvisioningType = provisioningType;

        var model = new ConfirmQuantitiesModel(catalogueItem, price, recipients);

        model.Advice.Should().Be(string.Format(expectedAdvice, "competition"));
    }

    [Theory]
    [MockInlineAutoData(ConfirmQuantitiesModel.AdvicePatientText, ProvisioningType.Patient)]
    [MockInlineAutoData(ConfirmQuantitiesModel.AdviceText, ProvisioningType.OnDemand)]
    [MockInlineAutoData(ConfirmQuantitiesModel.AdviceText, ProvisioningType.Declarative)]
    public static void Order_Advice_ProvisioningType_SetsExpected(
        string expectedAdvice,
        ProvisioningType provisioningType,
        CatalogueItem catalogueItem,
        OrderItemPrice price,
        List<ServiceRecipientQuantityDto> recipients)
    {
        price.ProvisioningType = provisioningType;

        var model = new ConfirmQuantitiesModel(catalogueItem, price, recipients);

        model.Advice.Should().Be(string.Format(expectedAdvice, "order"));
    }
}
