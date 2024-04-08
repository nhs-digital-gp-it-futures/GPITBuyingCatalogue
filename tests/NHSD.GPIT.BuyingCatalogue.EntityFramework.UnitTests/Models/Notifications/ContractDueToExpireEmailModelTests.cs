using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Notifications
{
    public static class ContractDueToExpireEmailModelTests
    {
        [Theory]
        [AutoData]
        public static void NotificationType(
            ContractDueToExpireEmailModel model)
        {
            model.NotificationType.Should().Be(EmailNotificationTypeEnum.ContractDueToExpire);
        }

        [Theory]
        [AutoData]
        public static void GetTemplatePersonalisation(
            ContractDueToExpireEmailModel model)
        {
            var result = model.GetTemplatePersonalisation();
            ((string)result[ContractDueToExpireEmailModel.LastNameToken]).Should().Be(model.LastName);
            ((string)result[ContractDueToExpireEmailModel.FirstNameToken]).Should().Be(model.FirstName);
            ((string)result[ContractDueToExpireEmailModel.OrderIdToken]).Should().Be(model.CallOffId);
            ((string)result[ContractDueToExpireEmailModel.DaysRemainingToken]).Should().Be($"{model.DaysRemaining}");
        }

        [Theory]
        [AutoData]
        public static void GetTemplateId(
            TemplateOptions options,
            ContractDueToExpireEmailModel model)
        {
            var result = model.GetTemplateId(options);
            result.Should().Be(options.ContractExpiryTemplateId);
        }
    }
}
