using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using MimeKit;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.Services.Email;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Email
{
    public static class MimeKitExtensionsTests
    {
        private static EmailMessageTemplate BasicTemplate => new(new EmailAddressTemplate("sender@somedomain.nhs.test"));

        private static ICollection<EmailAddress> SingleRecipient => new[] { new EmailAddress("recipient@somedomain.nhs.test") };

        [Fact]
        public static void AsMailboxAddress_NullAddress_ThrowsException()
        {
            EmailAddress address = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => address.AsMailboxAddress());
        }

        [Fact]
        public static void AsMailboxAddress_ReturnsExpectedType()
        {
            const string name = "Some Body";

            var emailAddress = new EmailAddress("a@b.test", name);
            var mailboxAddress = emailAddress.AsMailboxAddress();

            mailboxAddress.Should().BeOfType<MailboxAddress>();
        }

        [Fact]
        public static void AsMailboxAddress_InitializesName()
        {
            const string name = "Some Body";

            var emailAddress = new EmailAddress("a@b.test", name);
            var mailboxAddress = emailAddress.AsMailboxAddress();

            mailboxAddress.Name.Should().Be(name);
        }

        [Fact]
        public static void AsMailboxAddress_InitializesAddress()
        {
            const string address = "somebody@notarealaddress.test";

            var emailAddress = new EmailAddress(address, "Name");
            var mailboxAddress = emailAddress.AsMailboxAddress();

            mailboxAddress.Should().BeOfType<MailboxAddress>();
            mailboxAddress.Address.Should().Be(address);
        }

        [Fact]
        public static void AsMimeMessage_NullMessage_ThrowsException()
        {
            EmailMessage message = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => message.AsMimeMessage());
        }

        [Fact]
        public static void AsMimeMessage_NullSubject_SetsSubjectToEmptyString()
        {
            var emailMessage = new EmailMessage(BasicTemplate, SingleRecipient);

            var mimeMessage = emailMessage.AsMimeMessage(string.Empty);

            mimeMessage.Subject.Should().Be(string.Empty);
        }

        [Fact]
        public static void AsMimeMessage_ReturnsExpectedType()
        {
            var emailMessage = new EmailMessage(BasicTemplate, SingleRecipient);

            var mimeMessage = emailMessage.AsMimeMessage();

            mimeMessage.Should().BeOfType<MimeMessage>();
        }

        [Fact]
        public static void AsMimeMessage_InitializesSender()
        {
            const string sender = "sender@somedomain.test";

            var template = new EmailMessageTemplate(new EmailAddressTemplate(sender));
            var emailMessage = new EmailMessage(template, SingleRecipient);

            var mimeMessage = emailMessage.AsMimeMessage();

            IEnumerable<InternetAddress> from = mimeMessage.From;
            from.Should().HaveCount(1);
            from.First().As<MailboxAddress>().Address.Should().Be(sender);
        }

        [Fact]
        public static void AsMimeMessage_InitializesRecipients()
        {
            const string recipient1 = "recipient1@somedomain.test";
            const string recipient2 = "recipient2@somedomain.test";

            var recipients = new[] { new EmailAddress(recipient1), new EmailAddress(recipient2) };

            var emailMessage = new EmailMessage(BasicTemplate, recipients);

            var mimeMessage = emailMessage.AsMimeMessage();

            IEnumerable<InternetAddress> to = mimeMessage.To;
            to.Should().HaveCount(2);
            to.First().As<MailboxAddress>().Address.Should().Be(recipient1);
            to.Last().As<MailboxAddress>().Address.Should().Be(recipient2);
        }

        [Fact]
        public static void AsMimeMessage_InitializesSubject()
        {
            const string subject = "Subject";

            var template = BasicTemplate with { Subject = subject };

            var emailMessage = new EmailMessage(template, SingleRecipient);

            var mimeMessage = emailMessage.AsMimeMessage();

            mimeMessage.Subject.Should().Be(subject);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public static void AsMimeMessage_NullOrWhiteSpaceSubjectPrefix_InitializesSubject(string emailSubjectPrefix)
        {
            const string subject = "Subject";

            var template = BasicTemplate with { Subject = subject };

            var emailMessage = new EmailMessage(template, SingleRecipient);

            var mimeMessage = emailMessage.AsMimeMessage(emailSubjectPrefix);

            mimeMessage.Subject.Should().Be(subject);
        }

        [Fact]
        public static void AsMimeMessage_WithSubjectPrefix_InitializesSubject()
        {
            const string emailSubjectPrefix = "Prefix";
            const string subject = "Subject";

            var template = BasicTemplate with { Subject = subject };

            var emailMessage = new EmailMessage(template, SingleRecipient);

            var mimeMessage = emailMessage.AsMimeMessage(emailSubjectPrefix);

            mimeMessage.Subject.Should().Be($"{emailSubjectPrefix} {subject}");
        }

        [Fact]
        public static void AsMimeMessage_InitializesHtmlBody()
        {
            const string expectedContent = "HTML";

            var template = BasicTemplate with { HtmlContent = expectedContent };

            var emailMessage = new EmailMessage(template, SingleRecipient);

            var mimeMessage = emailMessage.AsMimeMessage();

            mimeMessage.HtmlBody.Should().Be(expectedContent);
        }

        [Fact]
        public static void AsMimeMessage_InitializesTextBody()
        {
            const string expectedContent = "Text";

            var template = BasicTemplate with { PlainTextContent = expectedContent };

            var emailMessage = new EmailMessage(template, SingleRecipient);

            var mimeMessage = emailMessage.AsMimeMessage();

            mimeMessage.TextBody.Should().Be(expectedContent);
        }

        [Fact]
        public static void AsMimeMessage_WithAttachment_HasExpectedAttachment()
        {
            var fileNames = new[] { "hello.csv", "world.txt" };
            var content = new[] { "Hello", "World" };
            var mimeTypes = new[] { "text/csv", "text/plain" };

            using var contentStream1 = new MemoryStream(Encoding.ASCII.GetBytes(content[0]));
            using var contentStream2 = new MemoryStream(Encoding.ASCII.GetBytes(content[1]));

            var emailMessage = new EmailMessage(
                BasicTemplate,
                SingleRecipient,
                new[] { new EmailAttachment(fileNames[0], contentStream1), new EmailAttachment(fileNames[1], contentStream2) });

            var mimeMessage = emailMessage.AsMimeMessage();
            var attachments = mimeMessage.Attachments.ToList();

            attachments.Should().HaveCount(2);

            for (var i = 0; i < fileNames.Length; i++)
            {
                var attachment = (TextPart)attachments[i];

                attachment.Should().NotBeNull();
                attachment.ContentType.MimeType.Should().Be(mimeTypes[i]);
                attachment.FileName.Should().Be(fileNames[i]);
                attachment.Text.Should().Be(content[i]);
            }
        }
    }
}
