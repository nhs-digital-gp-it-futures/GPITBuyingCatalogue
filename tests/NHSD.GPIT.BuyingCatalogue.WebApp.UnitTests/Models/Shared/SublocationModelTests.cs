using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Shared;

public static class SublocationModelTests
{
    [Theory]
    [MockAutoData]
    public static void Constructor_SetsPropertiesAsExpected(
        OrderSublocation orderSublocation)
    {
        const bool isSelected = false;

        var model = new SublocationModel(
            orderSublocation, isSelected);

        var expectedRecipients =
            orderSublocation.SublocationRecipients.Select(x => new ServiceRecipientModel(x, isSelected));

        model.Name.Should().Be(orderSublocation.SublocationOrganisation.Name);
        model.ServiceRecipients.Should().BeEquivalentTo(expectedRecipients);
    }

    [Theory]
    [MockInlineAutoData(true)]
    [MockInlineAutoData(false)]
    public static void AllRecipientsSelected_SetsPropertiesAsExpected(
        bool selected,
        OrderSublocation orderSublocation)
    {
        var model = new SublocationModel(
            orderSublocation,
            selected);

        model.Name.Should().Be(orderSublocation.SublocationOrganisation.Name);
        model.AllRecipientsSelected.Should().Be(selected);
    }
}
