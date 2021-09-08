using System;
using System.IO;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Email
{
    public static class EmailAttachmentTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public static void Constructor_NullEmptyOrWhiteSpaceFileName_ThrowsArgumentException(string fileName)
        {
            Assert.Throws<ArgumentException>(() => _ = new EmailAttachment(fileName, Mock.Of<Stream>()));
        }

        [Fact]
        public static void Constructor_NullContent_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new EmailAttachment("fileName", null!));
        }

        [Fact]
        public static void Constructor_InitializesFileName()
        {
            const string fileName = "file.pdf";

            var attachment = new EmailAttachment(fileName, Mock.Of<Stream>());

            attachment.FileName.Should().Be(fileName);
        }

        [Fact]
        public static void Constructor_InitializesContent()
        {
            var content = Mock.Of<Stream>();

            var attachment = new EmailAttachment("fileName", content);

            attachment.Content.Should().BeSameAs(content);
        }
    }
}
