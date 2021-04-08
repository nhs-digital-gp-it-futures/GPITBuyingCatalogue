using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Email
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class EmailAddressTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("\t")]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public static void Constructor_String_NullEmptyOrWhiteSpaceAddress_ThrowsArgumentException(string address)
        {
            Assert.Throws<ArgumentException>(() => new EmailAddress(address));
        }

        [Test]
        public static void Constructor_String_InitializesAddress()
        {
            const string address = "somebody@notarealaddress.test";

            var emailAddress = new EmailAddress(address);

            emailAddress.Address.Should().Be(address);
        }

        [Test]
        public static void Constructor_String_DoesNotInitializeDisplayName()
        {
            var emailAddress = new EmailAddress("somebody@notarealaddress.test");

            emailAddress.DisplayName.Should().BeNull();
        }

        [Test]
        public static void Constructor_String_String_InitializesAddress()
        {
            const string address = "somebody@notarealaddress.test";

            var emailAddress = new EmailAddress(address, "Name");

            emailAddress.Address.Should().Be(address);
        }

        [Test]
        public static void Constructor_String_String_InitializesDisplayName()
        {
            const string name = "Some Body";

            var emailAddress = new EmailAddress("a@b.test", name);

            emailAddress.DisplayName.Should().Be(name);
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public static void Constructor_EmailAddressTemplate_NullTemplate_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailAddress(null!));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public static void Constructor_EmailAddressTemplate_NullAddress_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new EmailAddress(new EmailAddressTemplate()));
        }

        [Test]
        public static void Constructor_EmailAddressTemplate_InitializesAddress()
        {
            const string expectedAddress = "bob@marley.test";

            var emailAddress = new EmailAddress(new EmailAddressTemplate { Address = expectedAddress });

            emailAddress.Address.Should().Be(expectedAddress);
        }

        [Test]
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
