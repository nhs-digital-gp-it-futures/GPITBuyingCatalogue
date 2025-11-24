using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.ServiceRecipients;

public static class RecipientForPracticeReorganisationModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Organisation organisation,
        EntityFramework.Ordering.Models.Order order)
    {
        order.OrderType = OrderTypeEnum.AssociatedServiceMerger;

        var model = new RecipientForPracticeReorganisationModel(organisation, order);

        model.Caption.Should().Be($"Order {order.CallOffId}");
        model.OrganisationName.Should().Be(organisation.Name);
        model.OrganisationType.Should().Be(organisation.OrganisationType.GetValueOrDefault());
        model.SubLocations.Should()
            .BeEquivalentTo(
                order.OrderSublocations
                    .Select(x => new SublocationModel(
                        x,
                        false))
                    .OrderBy(x => x.Name)
                    .ToArray());
    }

    [Theory]
    [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
    [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
    public static void Construct_SetsTitleAndAdviceAsExpected(
        OrderTypeEnum orderType,
        Organisation organisation,
        EntityFramework.Ordering.Models.Order order)
    {
        order.OrderType = orderType;

        var model = new RecipientForPracticeReorganisationModel(organisation, order);

        var expectedAdvice = orderType == OrderTypeEnum.AssociatedServiceSplit
            ? "Select the service recipient that will be losing patients as part of the split."
            : "Select the service recipient that will still exist after the merger.";

        model.Title.Should().Be(order.OrderType.GetPracticeReorganisationRecipientTitle());
        model.Advice.Should().Be(expectedAdvice);
    }
}
