using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Email
{
    public static class EmailAddressTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public static void Constructor_String_NullEmptyOrWhiteSpaceAddress_ThrowsArgumentException(string address)
        {
            Assert.Throws<ArgumentException>(() => _ = new EmailAddress(address));
        }

        [Fact]
        public static void Constructor_String_InitializesAddress()
        {
            const string address = "somebody@notarealaddress.test";

            var emailAddress = new EmailAddress(address);

            emailAddress.Address.Should().Be(address);
        }

        [Fact]
        public static void Constructor_String_DoesNotInitializeDisplayName()
        {
            var emailAddress = new EmailAddress("somebody@notarealaddress.test");

            emailAddress.DisplayName.Should().BeNull();
        }

        [Fact]
        public static void Constructor_String_String_InitializesAddress()
        {
            const string address = "somebody@notarealaddress.test";

            var emailAddress = new EmailAddress(address, "Name");

            emailAddress.Address.Should().Be(address);
        }

        [Fact]
        public static void Constructor_String_String_InitializesDisplayName()
        {
            const string name = "Some Body";

            var emailAddress = new EmailAddress("a@b.test", name);

            emailAddress.DisplayName.Should().Be(name);
        }

        [Fact]
        public static void Constructor_EmailAddressTemplate_NullTemplate_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new EmailAddress(null!));
        }

        [Fact]
        public static void Constructor_EmailAddressTemplate_NullAddress_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _ = new EmailAddress(new EmailAddressTemplate()));
        }

        [Fact]
        public static void Constructor_EmailAddressTemplate_InitializesAddress()
        {
            const string expectedAddress = "bob@marley.test";

            var emailAddress = new EmailAddress(new EmailAddressTemplate { Address = expectedAddress });

            emailAddress.Address.Should().Be(expectedAddress);
        }

        [Fact]
        public static void Constructor_EmailAddressTemplate_InitializesDisplayName()
        {
            const string expectedDisplayName = "Bob Marley ";

            var template = new EmailAddressTemplate
            {
                Address = "bob@marley.test",
                DisplayName = expectedDisplayName,
            };

            var emailAddress = new EmailAddress(template);

            emailAddress.DisplayName.Should().Be(expectedDisplayName);
        }
    }
}
