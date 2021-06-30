using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Email
{
    public static class EmailMessageTests
    {
        private static EmailMessageTemplate EmptyTemplate => new(new EmailAddressTemplate("from@sender.test"));

        private static ICollection<EmailAddress> SingleRecipient => new[] { new EmailAddress("to@recipient.test") };

        [Fact]
        public static void Constructor_EmailMessageTemplate_NullTemplate_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new EmailMessage(null!, SingleRecipient));
        }

        [Fact]
        public static void Constructor_EmailMessageTemplate_NullSender_ThrowsArgumentException()
        {
            var template = new EmailMessageTemplate();

            Assert.Throws<ArgumentException>(() => _ = new EmailMessage(template, SingleRecipient));
        }

        [Fact]
        public static void Constructor_EmailMessageTemplate_NullRecipients_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new EmailMessage(EmptyTemplate, null!));
        }

        [Fact]
        public static void Constructor_EmailMessageTemplate_EmptyRecipients_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _ = new EmailMessage(EmptyTemplate, Array.Empty<EmailAddress>()));
        }

        [Fact]
        public static void Constructor_EmailMessageTemplate_NullFormatItems_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new EmailMessage(EmptyTemplate, SingleRecipient, null, null!));
        }

        [Fact]
        public static void Constructor_EmailMessageTemplate_InitializesSender()
        {
            const string sender = "from@sender.test";
            var template = new EmailMessageTemplate { Sender = new EmailAddressTemplate { Address = sender } };

            var message = new EmailMessage(template, SingleRecipient);

            message.Sender.Address.Should().Be(sender);
        }

        [Fact]
        [SuppressMessage("ReSharper", "CoVariantArrayConversion", Justification = "Type will match")]
        public static void Constructor_EmailMessageTemplate_InitializesRecipients()
        {
            var recipient1 = new EmailAddress("to@recipient1.test");
            var recipient2 = new EmailAddress("to@recipient2.test");
            var recipients = new[] { recipient1, recipient2 };

            var message = new EmailMessage(EmptyTemplate, recipients);

            message.Recipients.Should().HaveCount(2);
            message.Recipients.Should().BeEquivalentTo(recipients);
        }

        [Fact]
        public static void Constructor_EmailMessageTemplate_InitializesSubject()
        {
            const string subject = "Ant Morphology";

            var template = EmptyTemplate with { Subject = subject };

            var message = new EmailMessage(template, SingleRecipient);

            message.Subject.Should().BeSameAs(subject);
        }

        [Fact]
        public static void Constructor_EmailMessageTemplate_InitializesHtmlBodyContent()
        {
            const string htmlContent = "HTML Body";

            var template = EmptyTemplate with { HtmlContent = htmlContent };

            var message = new EmailMessage(template, SingleRecipient);

            message.HtmlBody.Should().NotBeNull();
            message.HtmlBody!.Content.Should().BeSameAs(htmlContent);
        }

        [Fact]
        public static void Constructor_EmailMessageTemplate_InitializesHtmlBodyFormatItems()
        {
            const int one = 1;
            const string two = "2";

            var formatItems = new object[] { one, two };

            var message = new EmailMessage(EmptyTemplate, SingleRecipient, null, formatItems);

            message.HtmlBody.Should().NotBeNull();
            message.HtmlBody!.FormatItems.Should().BeEquivalentTo(formatItems);
        }

        [Fact]
        public static void Constructor_EmailMessageTemplate_InitializesTextBodyContent()
        {
            const string textContent = "Plain-text Body";

            var template = EmptyTemplate with { PlainTextContent = textContent };

            var message = new EmailMessage(template, SingleRecipient);

            message.TextBody.Should().NotBeNull();
            message.TextBody!.Content.Should().BeSameAs(textContent);
        }

        [Fact]
        public static void Constructor_EmailMessageTemplate_InitializesTextBodyFormatItems()
        {
            const int one = 1;
            const string two = "2";

            var formatItems = new object[] { one, two };

            var message = new EmailMessage(EmptyTemplate, SingleRecipient, null, formatItems);

            message.TextBody.Should().NotBeNull();
            message.TextBody!.FormatItems.Should().BeEquivalentTo(formatItems);
        }

        [Fact]
        [SuppressMessage("ReSharper", "CoVariantArrayConversion", Justification = "Type will match")]
        public static void Constructor_EmailMessageTemplate_InitializesAttachments()
        {
            var attachment1 = new EmailAttachment("file", Mock.Of<Stream>());
            var attachment2 = new EmailAttachment("file", Mock.Of<Stream>());
            var attachments = new[] { attachment1, attachment2 };

            var message = new EmailMessage(EmptyTemplate, SingleRecipient, attachments);

            message.Attachments.Should().HaveCount(2);
            message.Attachments.Should().BeEquivalentTo(attachments);
        }

        [Fact]
        public static void HasAttachments_NoAttachments_ReturnsFalse()
        {
            var message = new EmailMessage(EmptyTemplate, SingleRecipient);

            message.HasAttachments.Should().BeFalse();
        }

        [Fact]
        public static void HasAttachments_WithAttachment_ReturnsTrue()
        {
            var message = new EmailMessage(
                EmptyTemplate,
                SingleRecipient,
                new[] { new EmailAttachment("fileName", Mock.Of<Stream>()) });

            message.HasAttachments.Should().BeTrue();
        }
    }
}
