using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Email
{
    public static class EmailMessageTemplateTests
    {
        [Fact]
        public static void Constructor_EmailAddressTemplate_InitializesSender()
        {
            var addressTemplate = new EmailAddressTemplate();

            var template = new EmailMessageTemplate(addressTemplate);

            template.Sender.Should().BeSameAs(addressTemplate);
        }

        [Fact]
        public static void GetSenderAsEmailAddress_NullAddress_ReturnsNull()
        {
            var template = new EmailMessageTemplate();

            var address = template.GetSenderAsEmailAddress();

            address.Should().BeNull();
        }

        [Fact]
        public static void GetSenderAsEmailAddress_ReturnsExpectedAddress()
        {
            var addressTemplate = new EmailAddressTemplate { Address = "someone@somewhere.test" };
            var messageTemplate = new EmailMessageTemplate { Sender = addressTemplate };

            var expectedAddress = addressTemplate.AsEmailAddress();
            var actualAddress = messageTemplate.GetSenderAsEmailAddress();

            actualAddress.Should().BeEquivalentTo(expectedAddress);
        }

        [Fact]
        public static void Sender_Set_NullSender_ThrowsArgumentNullException()
        {
            var template = new EmailMessageTemplate();

            Assert.Throws<ArgumentNullException>(() => template.Sender = null);
        }
    }
}
