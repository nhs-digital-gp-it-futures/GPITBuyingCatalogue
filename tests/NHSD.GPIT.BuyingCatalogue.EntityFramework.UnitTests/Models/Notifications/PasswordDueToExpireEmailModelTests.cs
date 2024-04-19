using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Notifications;

public static class PasswordDueToExpireEmailModelTests
{
    [Theory]
    [AutoData]
    public static void NotificationType(
        PasswordDueToExpireEmailModel model)
    {
        model.NotificationType.Should().Be(EmailNotificationTypeEnum.PasswordDueToExpire);
    }

    [Theory]
    [InlineAutoData(5, "days")]
    [InlineAutoData(1, "day")]
    public static void GetTemplatePersonalisation(
        int daysRemaining,
        string expectedDayFormat,
        PasswordDueToExpireEmailModel model)
    {
        model.DaysRemaining = daysRemaining;

        var result = model.GetTemplatePersonalisation();
        ((string)result[PasswordDueToExpireEmailModel.LastNameToken]).Should().Be(model.LastName);
        ((string)result[PasswordDueToExpireEmailModel.FirstNameToken]).Should().Be(model.FirstName);
        ((string)result[PasswordDueToExpireEmailModel.DayStyleToken]).Should().Be(expectedDayFormat);
        ((int)result[PasswordDueToExpireEmailModel.DaysRemainingToken]).Should().Be(model.DaysRemaining);
    }

    [Theory]
    [AutoData]
    public static void GetTemplateId(
        TemplateOptions options,
        PasswordDueToExpireEmailModel model)
    {
        var result = model.GetTemplateId(options);
        result.Should().Be(options.PasswordExpiryTemplateId);
    }
}
