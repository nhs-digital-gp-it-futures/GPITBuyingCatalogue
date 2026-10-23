using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.DeliveryDates;

public static class SelectRecipientToEditModelTests
{
    [Fact]
    public static void Properties_CanBeSetAndRetrieved()
    {
        var model = new SelectRecipientToEditModel
        {
            Selected = true,
            ParentSublocationName = "Sublocation organisation",
            ParentSublocationOdsCode = "SLOC1",
            RecipientOrganisationId = "recipient-id",
            RecipientName = "Recipient organisation",
            RecipientOdsCode = "RECIPIENT1",
            Status = "Updated",
        };

        model.Selected.Should().BeTrue();
        model.ParentSublocationName.Should().Be("Sublocation organisation");
        model.ParentSublocationOdsCode.Should().Be("SLOC1");
        model.RecipientOrganisationId.Should().Be("recipient-id");
        model.RecipientName.Should().Be("Recipient organisation");
        model.RecipientOdsCode.Should().Be("RECIPIENT1");
        model.Status.Should().Be("Updated");
    }

    [Fact]
    public static void Selected_DefaultsToFalse()
    {
        var model = new SelectRecipientToEditModel
        {
            ParentSublocationName = "Sublocation organisation",
            ParentSublocationOdsCode = "SLOC1",
            RecipientOrganisationId = "recipient-id",
            RecipientName = "Recipient organisation",
            RecipientOdsCode = "RECIPIENT1",
        };

        model.Selected.Should().BeFalse();
        model.Status.Should().BeNull();
    }
}
