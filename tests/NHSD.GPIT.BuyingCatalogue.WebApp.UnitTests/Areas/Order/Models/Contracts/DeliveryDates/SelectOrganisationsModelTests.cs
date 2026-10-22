using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using Xunit;
using OrderEntity = NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models.Order;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.DeliveryDates;

public static class SelectOrganisationsModelTests
{
    [Fact]
    public static void Constructor_WithValidArguments_PropertiesCorrectlySet()
    {
        var orderItem = new OrderItem
        {
            Id = 42,
            CatalogueItem = new CatalogueItem { Name = "Catalogue item" },
        };
        var organisation = new OdsOrganisation
        {
            Id = "recipient-org-id",
            Name = "Recipient organisation",
        };
        var sublocation = new OrderSublocation
        {
            SublocationOdsCode = "sublocation-ods-code",
            SublocationOrganisation = new OdsOrganisation
            {
                Id = "parent-org-id",
                Name = "Parent organisation",
            },
        };
        var recipient = new OrderSublocationRecipient
        {
            ParentSublocation = sublocation,
            ParentSublocationOdsCode = sublocation.SublocationOdsCode,
            RecipientOdsCode = "recipient-ods-code",
            RecipientOdsOrganisation = organisation,
        };
        recipient.OrderItemSublocationRecipients.Add(new OrderItemSublocationRecipient
        {
            OrderItem = orderItem,
            OrderItemId = orderItem.Id,
        });
        sublocation.SublocationRecipients.Add(recipient);

        var order = new OrderEntity
        {
            OrderNumber = 123,
            Revision = 1,
            AssociatedServicesOnlyDetails = new AssociatedServicesOnlyDetails(),
            OrderItems = [orderItem],
            OrderSublocations = [sublocation],
        };

        var model = new SelectOrganisationsModel(
            new OrderWrapper(order),
            "internal-org-id",
            orderItem.Id);

        model.InternalOrgId.Should().Be("internal-org-id");
        model.CallOffId.Should().Be(order.CallOffId);
        model.OrderItemId.Should().Be(orderItem.Id);
        model.Recipients.Should().ContainSingle();

        var recipientModel = model.Recipients[0];
        recipientModel.Selected.Should().BeFalse();
        recipientModel.ParentSublocationName.Should().Be("Parent organisation");
        recipientModel.ParentSublocationOdsCode.Should().Be("sublocation-ods-code");
        recipientModel.RecipientOrganisationId.Should().Be("recipient-org-id");
        recipientModel.RecipientName.Should().Be("Recipient organisation");
        recipientModel.RecipientOdsCode.Should().Be("recipient-ods-code");
    }

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
