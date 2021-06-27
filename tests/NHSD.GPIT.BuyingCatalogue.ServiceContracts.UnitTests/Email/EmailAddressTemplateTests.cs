using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Email
{
    public static class EmailAddressTemplateTests
    {
        [Fact]
        public static void Constructor_String_InitializesAddress()
        {
            const string expectedAddress = "address@foo.test";

            var template = new EmailAddressTemplate(expectedAddress);

            template.Address.Should().Be(expectedAddress);
        }

        [Fact]
        public static void Constructor_String_String_InitializesDisplayName()
        {
            const string expectedDisplayName = "Super Man";

            var template = new EmailAddressTemplate("address@foo.test", expectedDisplayName);

            template.DisplayName.Should().Be(expectedDisplayName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public static void Address_Set_NullEmptyOrWhiteSpaceAddress_ThrowsArgumentException(string address)
        {
            Assert.Throws<ArgumentException>(() => _ = new EmailAddressTemplate { Address = address });
        }

        [Fact]
        public static void AsEmailAddress_NullAddress_ReturnsNull()
        {
            var template = new EmailAddressTemplate();

            var address = template.AsEmailAddress();

            address.Should().BeNull();
        }

        [Fact]
        public static void AsEmailAddress_ReturnsExpectedAddress()
        {
            const string expectedAddress = "address@foo.test";
            const string expectedDisplayName = "Daredevil";

            var template = new EmailAddressTemplate
            {
                Address = expectedAddress,
                DisplayName = expectedDisplayName,
            };

            var address = template.AsEmailAddress();

            address.Should().NotBeNull();
            address!.Address.Should().Be(expectedAddress);
            address.DisplayName.Should().Be(expectedDisplayName);
        }
    }
}
