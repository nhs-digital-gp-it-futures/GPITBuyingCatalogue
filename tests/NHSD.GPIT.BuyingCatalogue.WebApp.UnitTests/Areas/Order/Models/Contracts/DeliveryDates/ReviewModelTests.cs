using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.DeliveryDates;

public static class ReviewModelTests
{
    [Theory]
    [MockAutoData]
    public static void WithValidArguments_PropertiesCorrectlySet(
        EntityFramework.Ordering.Models.Order order,
        OrderItem additionalService,
        OrderItem associatedService,
        OrderItem associatedServiceForAdditionalService,
        OrderItem solution,
        DateTime date)
    {
        additionalService.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
        associatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
        associatedServiceForAdditionalService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
        solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

        associatedService.ParentId = solution.Id;
        associatedService.Parent = solution;

        associatedServiceForAdditionalService.ParentId = additionalService.Id;
        associatedServiceForAdditionalService.Parent = additionalService;

        additionalService.Services.Clear();
        additionalService.Services.Add(associatedServiceForAdditionalService);

        order.OrderItems.Clear();
        order.OrderItems.Add(additionalService);
        order.OrderItems.Add(associatedService);
        order.OrderItems.Add(associatedServiceForAdditionalService);
        order.OrderItems.Add(solution);

        order.DeliveryDate = date;
        var model = new ReviewModel(new OrderWrapper(order));

        model.InternalOrgId.Should().Be(order.OrderingParty.InternalIdentifier);
        model.CallOffId.Should().Be(order.CallOffId);
        model.OrderType.Should().Be(order.OrderType);
        model.SolutionName.Should().Be(order.OrderType.GetSolutionNameFromOrder(order));
        model.PracticeReorganisationName.Should().Be(order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient.Name);
        model.DeliveryDate.Should().Be(order.DeliveryDate);
        model.AssociatedServiceIds.Count.Should().Be(1);
    }

    [Theory]
    [MockAutoData]
    public static void OrderItemSublocationRecipients_ReturnsAllRecipientsWithDeliveryDates(
        EntityFramework.Ordering.Models.Order order,
        OrderItem solution,
        OrderSublocation firstSublocation,
        OrderSublocation secondSublocation,
        OrderSublocationRecipient firstRecipient,
        OrderSublocationRecipient secondRecipient,
        OrderSublocationRecipient thirdRecipient,
        OrderSublocationRecipient forthRecipient,
        DateTime firstDate,
        DateTime secondDate,
        string firstOds,
        string secondOds,
        string thirdOds,
        string forthOds)
    {
        solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

        firstSublocation.SublocationRecipients.Clear();
        firstSublocation.SublocationOrganisation = new OdsOrganisation
        {
            Id = "sublocationOds1",
            Name = "Sublocation Organisation1",
        };

        secondSublocation.SublocationRecipients.Clear();
        secondSublocation.SublocationOrganisation = new OdsOrganisation
        {
            Id = "sublocationOds2",
            Name = "Sublocation Organisation2",
        };

        order.OrderSublocations.Clear();
        order.OrderItems.Clear();

        var firstOrganisation = "firstOrganisation";
        var secondOrganisation = "secondOrganisation";
        var thirdOrganisation = "thirdOrganisation";
        var forthOrganisation = "forthOrganisation";

        AddRecipientWithDeliveryDate(firstSublocation, firstRecipient, firstDate, firstOds, firstOrganisation);
        AddRecipientWithDeliveryDate(firstSublocation, secondRecipient, secondDate, secondOds, secondOrganisation);
        AddRecipientWithDeliveryDate(secondSublocation, thirdRecipient, firstDate, thirdOds, thirdOrganisation);
        AddRecipientWithDeliveryDate(secondSublocation, forthRecipient, firstDate, forthOds, forthOrganisation);

        order.OrderSublocations.Add(firstSublocation);
        order.OrderSublocations.Add(secondSublocation);
        order.OrderItems.Add(solution);

        var model = new ReviewModel(new OrderWrapper(order));

        var result = model.Recipients;
        result.Should().BeEquivalentTo(
            new Dictionary<string, OrderSublocationRecipient[]>
            {
                {
                    "Sublocation Organisation1",
                    new[]
                    {
                        firstRecipient,
                        secondRecipient,
                    }
                },
                {
                    "Sublocation Organisation2",
                    new[]
                    {
                        thirdRecipient,
                        forthRecipient,
                    }
                },
            });
    }

    private static void AddRecipientWithDeliveryDate(
        OrderSublocation sublocation,
        OrderSublocationRecipient recipient,
        DateTime date,
        string odsCode,
        string organisationName)
    {
        recipient.ParentSublocation = sublocation;
        recipient.RecipientOdsCode = odsCode;
        recipient.RecipientOdsOrganisation = new OdsOrganisation { Id = odsCode, Name = organisationName };

        sublocation.SublocationRecipients.Add(recipient);
    }

    [Fact]
    public static void HasUpdatedDeliveryDates_WhenDeliveryDateIsNull_ReturnsFalse()
    {
        var model = new ReviewModel
        {
            DeliveryDate = null,
        };
        var recipient = new OrderSublocationRecipient();

        var result = model.HasUpdatedDeliveryDates(recipient);

        result.Should().BeFalse();
    }

    [Fact]
    public static void HasUpdatedDeliveryDates_WhenAllRecipientDatesMatch_ReturnsFalse()
    {
        var deliveryDate = new DateTime(2025, 1, 1);
        var model = new ReviewModel
        {
            DeliveryDate = deliveryDate,
        };
        var recipient = new OrderSublocationRecipient();
        recipient.OrderItemSublocationRecipients.Add(new OrderItemSublocationRecipient
        {
            DeliveryDate = deliveryDate,
        });

        var result = model.HasUpdatedDeliveryDates(recipient);

        result.Should().BeFalse();
    }

    [Fact]
    public static void HasUpdatedDeliveryDates_WhenRecipientDateDiffers_ReturnsTrue()
    {
        var deliveryDate = new DateTime(2025, 1, 1);
        var model = new ReviewModel
        {
            DeliveryDate = deliveryDate,
        };
        var recipient = new OrderSublocationRecipient();
        recipient.OrderItemSublocationRecipients.Add(new OrderItemSublocationRecipient
        {
            DeliveryDate = deliveryDate.AddDays(1),
        });

        var result = model.HasUpdatedDeliveryDates(recipient);

        result.Should().BeTrue();
}
}
