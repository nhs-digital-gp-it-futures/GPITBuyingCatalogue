using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Contacts;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Contacts
{
    public static class ContactDetailsServiceTests
    {
        [Theory]
        [CommonAutoData]
        public static void AddOrUpdatePrimaryContact_ReturnsUpdatedContact(
            Contact existingContact,
            PrimaryContactModel newOrUpdatedContact,
            ContactDetailsService service)
        {
            var actual = service.AddOrUpdatePrimaryContact(existingContact, newOrUpdatedContact);

            actual.FirstName.Should().Be(newOrUpdatedContact.FirstName);
            actual.LastName.Should().Be(newOrUpdatedContact.LastName);
            actual.Email.Should().Be(newOrUpdatedContact.EmailAddress);
            actual.Phone.Should().Be(newOrUpdatedContact.TelephoneNumber);
        }

        [Theory]
        [CommonAutoData]
        public static void AddOrUpdatePrimaryContact_NullUpdatedContact_ReturnsExistingContact(
            Contact existingContact,
            ContactDetailsService service)
        {
            var actual = service.AddOrUpdatePrimaryContact(existingContact, null);

            actual.Should().Be(existingContact);
        }

        [Theory]
        [CommonAutoData]
        public static void AddOrUpdatePrimaryContact_NoExistingContact_ReturnsNewContact(
           PrimaryContactModel newOrUpdatedContact,
           ContactDetailsService service)
        {
            var actual = service.AddOrUpdatePrimaryContact(null, newOrUpdatedContact);

            actual.FirstName.Should().Be(newOrUpdatedContact.FirstName);
            actual.LastName.Should().Be(newOrUpdatedContact.LastName);
            actual.Email.Should().Be(newOrUpdatedContact.EmailAddress);
            actual.Phone.Should().Be(newOrUpdatedContact.TelephoneNumber);
        }

        [Theory]
        [CommonAutoData]
        public static void AddOrUpdatePrimaryContact_NoContacts_ReturnsDefaultContact(
           ContactDetailsService service)
        {
            var actual = service.AddOrUpdatePrimaryContact(null, null);

            actual.Should().BeEquivalentTo(new Contact());
        }
    }
}
