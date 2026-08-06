using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Notifications;

public static class AccountDeactivationEmailModelTests
{
    [Theory]
    [AutoData]
    public static void NotificationType(AccountDeactivationEmailModel model)
    {
        model.NotificationType.Should().Be(EmailNotificationTypeEnum.AccountDeactivation);
    }

    [Theory]
    [AutoData]
    public static void GetTemplateId(
        TemplateOptions options,
        AccountDeactivationEmailModel model)
    {
        var result = model.GetTemplateId(options);
        result.Should().Be(options.AccountDeactivationTemplateId);
    }
}
