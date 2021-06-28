using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Email
{
    public static class EmailMessageBodyTests
    {
        [Fact]
        public static void Constructor_String_ObjectArray_InitializesContent()
        {
            const string expectedContent = "Message content";

            var body = new EmailMessageBody(expectedContent);

            body.Content.Should().Be(expectedContent);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public static void Constructor_String_ObjectArray_NullOrWhiteSpaceContent_InitializesEmptyContent(string content)
        {
            var body = new EmailMessageBody(content);

            body.Content.Should().BeEmpty();
        }

        [Fact]
        public static void Constructor_String_ObjectArray_InitializesFormatItems()
        {
            const int one = 1;
            const string two = "2";

            var expectedFormatItems = new object[] { one, two };

            var body = new EmailMessageBody("content", one, two);

            body.FormatItems.Should().BeEquivalentTo(expectedFormatItems);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public static void ToString_NullOrWhiteSpaceContent_ReturnsEmptyString(string content)
        {
            var body = new EmailMessageBody(content);

            var formattedContent = body.ToString();

            formattedContent.Should().BeEmpty();
        }

        [Fact]
        public static void ToString_ReturnsFormattedString()
        {
            // ReSharper disable once StringLiteralTypo (date format)
            var body = new EmailMessageBody(
                "{0} {1:dd/MM/yyyy}",
                "Hello",
                new DateTime(2020, 8, 20));

            var formattedContent = body.ToString();

            formattedContent.Should().Be("Hello 20/08/2020");
        }
    }
}
