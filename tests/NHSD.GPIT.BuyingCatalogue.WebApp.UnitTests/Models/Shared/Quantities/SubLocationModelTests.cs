using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Shared.Quantities;

public static class SubLocationModelTests
{
    [Theory]
    [MockAutoData]
    public static void Status_AllQuantitiesEntered_Completed(
        SubLocationModel model)
    {
        model.ServiceRecipients.ForEach(x => x.InputQuantity = "1");

        model.Status.Should().Be(TaskProgress.Completed);
    }

    [Theory]
    [MockAutoData]
    public static void Status_PartialQuantitiesEntered_InProgress(
        SubLocationModel model)
    {
        model.ServiceRecipients.ForEach(x => x.InputQuantity = "1");
        model.ServiceRecipients.First().InputQuantity = null;

        model.Status.Should().Be(TaskProgress.InProgress);
    }

    [Theory]
    [MockAutoData]
    public static void Status_NoQuantitiesEntered_NotStarted(
        SubLocationModel model)
    {
        model.ServiceRecipients.ForEach(x => x.InputQuantity = null);

        model.Status.Should().Be(TaskProgress.NotStarted);
    }
}
