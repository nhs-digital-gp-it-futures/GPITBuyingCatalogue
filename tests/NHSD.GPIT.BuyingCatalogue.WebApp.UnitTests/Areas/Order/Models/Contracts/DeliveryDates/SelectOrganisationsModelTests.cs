using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.DeliveryDates;

public static class SelectOrganisationsModelTests
{
    [Fact]
    public static void GetRecipientsByLocation_GroupsRecipientsAndSortsByName()
    {
        var model = new SelectOrganisationsModel
        {
            Recipients =
            [
                CreateRecipient("Location A", "Zulu organisation", "recipient-1"),
                CreateRecipient("Location A", "Alpha organisation", "recipient-2"),
                CreateRecipient("Location B", "Beta organisation", "recipient-3"),
            ],
        };

        var result = model.GetRecipientsByLocation();

        result.Should().HaveCount(2);
        result["Location A"].Should().ContainInOrder(model.Recipients[1], model.Recipients[0]);
        result["Location B"].Should().ContainSingle().Which.Should().Be(model.Recipients[2]);
    }

    [Fact]
    public static void GetQueryStringRecipientIds_EscapesAndJoinsRecipientIds()
    {
        var model = new SelectOrganisationsModel
        {
            Recipients =
            [
                CreateRecipient("Location A", "Organisation 1", "recipient/1"),
                CreateRecipient("Location A", "Organisation 2", "recipient 2"),
            ],
        };

        var result = model.GetQueryStringRecipientIds();

        result.Should().Be("recipient%2F1,recipient%202");
    }

    [Fact]
    public static void GetRecipientsByLocation_WithNoRecipients_ReturnsEmptyDictionary()
    {
        var model = new SelectOrganisationsModel
        {
            Recipients = [],
        };

        var result = model.GetRecipientsByLocation();

        result.Should().BeEmpty();
    }

    [Fact]
    public static void GetQueryStringRecipientIds_WithNoRecipients_ReturnsEmptyString()
    {
        var model = new SelectOrganisationsModel
        {
            Recipients = [],
        };

        var result = model.GetQueryStringRecipientIds();

        result.Should().BeEmpty();
    }

    private static SelectRecipientToEditModel CreateRecipient(
        string locationName,
        string recipientName,
        string recipientId)
    {
        return new SelectRecipientToEditModel
        {
            ParentSublocationName = locationName,
            ParentSublocationOdsCode = $"{locationName}-ods",
            RecipientOrganisationId = recipientId,
            RecipientName = recipientName,
            RecipientOdsCode = $"{recipientId}-ods",
        };
    }
}
