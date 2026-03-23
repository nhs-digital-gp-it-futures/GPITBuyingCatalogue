using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList.Providers;

public static class DeclarationStatusProviderTests
{
    [Theory]
    [MockAutoData]
    public static void Get_NullWrapper_ReturnsCannotStart(
        OrderProgress progress,
        DeclarationStatusProvider provider)
    {
        var result = provider.Get(null, progress);

        result.Should().Be(TaskProgress.CannotStart);
    }

    [Theory]
    [MockAutoData]
    public static void Get_NullOrder_ReturnsCannotStart(
        OrderProgress progress,
        DeclarationStatusProvider provider)
    {
        var result = provider.Get(new OrderWrapper(), progress);

        result.Should().Be(TaskProgress.CannotStart);
    }

    [Theory]
    [MockAutoData]
    public static void Get_DataProcessingIncomplete_ReturnsCannotStart(
        Order order,
        OrderProgress progress,
        DeclarationStatusProvider provider)
    {
        progress.DataProcessingInformation = TaskProgress.NotStarted;
        var result = provider.Get(new OrderWrapper(order), progress);

        result.Should().Be(TaskProgress.CannotStart);
    }

    [Theory]
    [MockAutoData]
    public static void Get_TermsNotAccepted_ReturnsNotStarted(
        Order order,
        OrderProgress progress,
        DeclarationStatusProvider provider)
    {
        progress.DataProcessingInformation = TaskProgress.Completed;
        order.AcceptedTermsAndConditions = false;
        var result = provider.Get(new OrderWrapper(order), progress);

        result.Should().Be(TaskProgress.NotStarted);
    }

    [Theory]
    [MockAutoData]
    public static void Get_TermsAccepted_ReturnsCompleted(
        Order order,
        OrderProgress progress,
        DeclarationStatusProvider provider)
    {
        progress.DataProcessingInformation = TaskProgress.Completed;
        order.AcceptedTermsAndConditions = true;
        var result = provider.Get(new OrderWrapper(order), progress);

        result.Should().Be(TaskProgress.Completed);
    }
}
