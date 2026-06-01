using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList.Providers;

public static class AssociatedServicesRequirementsStatusProviderTests
{
    private static readonly OrderProgress ValidOrderState = new()
    {
        SolutionOrService = TaskProgress.Completed,
    };

    [Theory]
    [MockAutoData]
    public static void Get_OrderWrapperIsNull_ReturnsCannotStart(
        AssociatedServiceRequirementsStatusProvider service)
    {
        var actual = service.Get(null, new OrderProgress());

        actual.Should().Be(TaskProgress.CannotStart);
    }

    [Theory]
    [MockAutoData]
    public static void Get_OrderIsNull_ReturnsCannotStart(
        AssociatedServiceRequirementsStatusProvider service)
    {
        var actual = service.Get(new OrderWrapper(), new OrderProgress());

        actual.Should().Be(TaskProgress.CannotStart);
    }

    [Theory]
    [MockAutoData]
    public static void Get_StateIsNull_ReturnsCannotStart(
        Order order,
        AssociatedServiceRequirementsStatusProvider service)
    {
        var actual = service.Get(new OrderWrapper(order), null);

        actual.Should().Be(TaskProgress.CannotStart);
    }

    [Theory]
    [MockAutoData]
    public static void Get_NoAssociatedServices_ReturnsNotApplicable(
        Order order,
        AssociatedServiceRequirementsStatusProvider service)
    {
        order.OrderType = OrderTypeEnum.Solution;
        order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

        var actual = service.Get(new OrderWrapper(order), new OrderProgress());

        actual.Should().Be(TaskProgress.NotApplicable);
    }

    [Theory]
    [MockInlineAutoData(TaskProgress.CannotStart)]
    [MockInlineAutoData(TaskProgress.InProgress)]
    [MockInlineAutoData(TaskProgress.NotStarted)]
    [MockInlineAutoData(TaskProgress.Optional)]
    public static void Get_ValidOrderStateNotComplete_ReturnsCannotStart(
        TaskProgress status,
        Order order,
        AssociatedServiceRequirementsStatusProvider service)
    {
        var state = new OrderProgress
        {
            SolutionOrService = status,
        };

        order.OrderType = OrderTypeEnum.AssociatedServiceOther;
        order.Contract = null;

        var actual = service.Get(new OrderWrapper(order), state);

        actual.Should().Be(TaskProgress.CannotStart);
    }

    [Theory]
    [MockAutoData]
    public static void Get_NoContractInfoEntered_ReturnsNotStarted(
        Order order,
        AssociatedServiceRequirementsStatusProvider service)
    {
        order.OrderType = OrderTypeEnum.AssociatedServiceOther;
        order.Contract = new Contract { ContractBilling = new ContractBilling { HasConfirmedRequirements = false }, };

        var actual = service.Get(new OrderWrapper(order), ValidOrderState);

        actual.Should().Be(TaskProgress.NotStarted);
    }

    [Theory]
    [MockAutoData]
    public static void Get_RequirementsCompleted_ReturnsInCompleted(
        Order order,
        AssociatedServiceRequirementsStatusProvider service)
    {
        order.OrderType = OrderTypeEnum.AssociatedServiceOther;
        order.Contract = new Contract() { ContractBilling = new ContractBilling() { HasConfirmedRequirements = true, }, };

        var actual = service.Get(new OrderWrapper(order), ValidOrderState);

        actual.Should().Be(TaskProgress.Completed);
    }
}
