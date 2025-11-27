using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Shared.ServiceRecipientModels;

public static class ConfirmChangesModelTests
{
    [Theory]
    [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
    [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
    public static void Construct_SetsPropertiesAsExpected(
        OrderTypeEnum orderType,
        CallOffId callOffId,
        List<OrderSublocationRecipient> selectedRecipients,
        OrderSublocationRecipient practiceReorganisationRecipient)
    {
        var model = new ConfirmChangesModel(
            callOffId,
            orderType,
            selectedRecipients,
            practiceReorganisationRecipient);

        var processType = orderType == OrderTypeEnum.AssociatedServiceSplit ? "split" : "merger";

        var expectedAdvice = $"Review the practices involved in the {processType} you're ordering.";

        model.Title.Should().Be(ConfirmChangesModel.TitleText);
        model.Advice.Should().Be(expectedAdvice);
        model.Caption.Should().Be($"Order {callOffId}");
        model.OrderType.Should().Be((OrderType)orderType);
        model.Selected.Should().BeEquivalentTo(selectedRecipients);
        model.PracticeReorganisationRecipient.Should().Be(practiceReorganisationRecipient);
    }

    [Theory]
    [MockAutoData]
    public static void AddRemoveRecipientsLink_Default_ReturnsDefaultBacklink(
        CallOffId callOffId,
        List<OrderSublocationRecipient> selectedRecipients,
        OrderSublocationRecipient practiceReorganisationRecipient)
    {
        var model = new ConfirmChangesModel(
            callOffId,
            OrderTypeEnum.AssociatedServiceMerger,
            selectedRecipients,
            practiceReorganisationRecipient);

        model.AddRemoveRecipientsLink.Should().Be(model.BackLink);
    }

    [Theory]
    [MockAutoData]
    public static void AddRemoveRecipientsLink_Defined_ReturnsBacklink(
        string backlink,
        ConfirmChangesModel model)
    {
        model.AddRemoveRecipientsLink = backlink;
        model.AddRemoveRecipientsLink.Should().Be(backlink);
        model.AddRemoveRecipientsLink.Should().NotBe(model.BackLink);
    }

    [Theory]
    [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
    [MockInlineAutoData(OrderTypeEnum.Solution)]
    [MockInlineAutoData(OrderTypeEnum.Unknown)]
    public static void Construct_UnsupportedOrderType_ThrowsArgumentOutOfRangeException(
        OrderTypeEnum orderType,
        CallOffId callOffId,
        List<OrderSublocationRecipient> selectedRecipients,
        OrderSublocationRecipient practiceReorganisationRecipient)
    {
        FluentActions.Invoking(() => new ConfirmChangesModel(
                callOffId,
                orderType,
                selectedRecipients,
                practiceReorganisationRecipient))
            .Should()
            .Throw<ArgumentOutOfRangeException>(nameof(orderType));
    }
}
