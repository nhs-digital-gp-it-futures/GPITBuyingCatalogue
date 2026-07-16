using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Notifications;

public static class InactiveAccountEmailModelTests
{
    [Theory]
    [AutoData]
    public static void NotificationType(InactiveAccountEmailModel model)
    {
        model.NotificationType.Should().Be(EmailNotificationTypeEnum.InactiveAccount);
    }

    [Theory]
    [InlineAutoData(30, "days")]
    [InlineAutoData(14, "days")]
    [InlineAutoData(7, "days")]
    [InlineAutoData(1, "day")]
    public static void GetTemplatePersonalisation(
        int daysRemaining,
        string expectedDayFormat,
        InactiveAccountEmailModel model)
    {
        model.DaysFromThreshold = daysRemaining;

        var result = model.GetTemplatePersonalisation();
        ((string)result[InactiveAccountEmailModel.DayStyleToken]).Should().Be(expectedDayFormat);
        ((int)result[InactiveAccountEmailModel.DeactivationCountdownToken]).Should().Be(model.DaysFromThreshold);
    }

    [Theory]
    [AutoData]
    public static void GetTemplateId(
        TemplateOptions options,
        InactiveAccountEmailModel model)
    {
        var result = model.GetTemplateId(options);
        result.Should().Be(options.InactiveAccountTemplateId);
    }
}
