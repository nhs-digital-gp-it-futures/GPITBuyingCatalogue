using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.SharedMocks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    public static class EmailServiceExtensionsTests
    {
        [Fact]
        public static async Task SendEmailAsync_MessageHasExpectedRecipient()
        {
            var recipient = new EmailAddress("to@recipient.test");
            var service = new MockEmailService();
            var template = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"));

            await service.SendEmailAsync(template, recipient);

            service.SentMessage.Recipients.Should().HaveCount(1);
            service.SentMessage.Recipients[0].Should().BeSameAs(recipient);
        }

        [Fact]
        public static async Task SendEmailAsync_MessageHasExpectedFormatItems()
        {
            object[] formatItems = { 1, "2" };
            var service = new MockEmailService();
            var template = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"));

            await service.SendEmailAsync(template, new EmailAddress("to@recipient.test"), formatItems);

            service.SentMessage.HtmlBody!.FormatItems.Should().BeEquivalentTo(formatItems);
            service.SentMessage.TextBody!.FormatItems.Should().BeEquivalentTo(formatItems);
        }

        [Fact]
        public static async Task SendEmailAsync_UsesMessageTemplate()
        {
            // ReSharper disable once StringLiteralTypo
            const string subject = "Banitsa";

            var service = new MockEmailService();
            var template = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"))
            {
                Subject = subject,
            };

            await service.SendEmailAsync(template, new EmailAddress("to@recipient.test"));

            service.SentMessage.Subject.Should().Be(subject);
        }
    }
}
