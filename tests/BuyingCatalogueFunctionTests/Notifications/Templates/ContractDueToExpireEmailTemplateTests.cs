using AutoFixture.Xunit2;
using BuyingCatalogueFunction.Notifications.Templates;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using Xunit;

namespace BuyingCatalogueFunctionTests.Notifications.Notifications
{
    public static class ContractDueToExpireEmailTemplateTests
    {
        [Theory]
        [AutoData]
        public static void NotificationType(
            ContractDueToExpireEmailTemplate template)
        {
            template.NotificationType.Should().Be(EmailNotificationTypeEnum.ContractDueToExpire);
        }

        [Theory]
        [AutoData]
        public static void GetTemplatePersonalisation(
            ContractDueToExpireEmailTemplate template)
        {
            var result = template.GetTemplatePersonalisation();
            ((string)result[ContractDueToExpireEmailTemplate.LastNameToken]).Should().Be(template.Model.LastName);
            ((string)result[ContractDueToExpireEmailTemplate.FirstNameToken]).Should().Be(template.Model.FirstName);
            ((string)result[ContractDueToExpireEmailTemplate.OrderIdToken]).Should().Be(template.Model.CallOffId);
            ((string)result[ContractDueToExpireEmailTemplate.DaysRemainingToken]).Should().Be($"{template.Model.DaysRemaining}");
        }

        [Theory]
        [AutoData]
        public static void GetTemplateId(
            TemplateOptions options,
            ContractDueToExpireEmailTemplate template)
        {
            var result = template.GetTemplateId(options);
            result.Should().Be(options.ContractExpiryTemplateId);
        }
    }
}

