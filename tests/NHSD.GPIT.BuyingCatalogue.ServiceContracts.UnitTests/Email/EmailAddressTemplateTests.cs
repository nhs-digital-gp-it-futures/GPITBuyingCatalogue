using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Email
{
    [TestFixture]
    internal static class EmailAddressTemplateTests
    {
        [Test]
        public static void Constructor_String_InitializesAddress()
        {
            const string expectedAddress = "address@foo.test";

            var template = new EmailAddressTemplate(expectedAddress);

            template.Address.Should().Be(expectedAddress);
        }

        [Test]
        public static void Constructor_String_String_InitializesDisplayName()
        {
            const string expectedDisplayName = "Super Man";

            var template = new EmailAddressTemplate("address@foo.test", expectedDisplayName);

            template.DisplayName.Should().Be(expectedDisplayName);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("\t")]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public static void Address_Set_NullEmptyOrWhiteSpaceAddress_ThrowsArgumentException(string address)
        {
            Assert.Throws<ArgumentException>(() => new EmailAddressTemplate { Address = address });
        }

        [Test]
        public static void AsEmailAddress_NullAddress_ReturnsNull()
        {
            var template = new EmailAddressTemplate();

            var address = template.AsEmailAddress();

            address.Should().BeNull();
        }

        [Test]
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
