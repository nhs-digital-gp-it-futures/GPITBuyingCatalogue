using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Models
{
    public static class SupplierContactsModelTests
    {
        [Theory]
        [MockAutoData]
        public static void ContactFor_ValidId_ReturnsMatchingContact(SupplierContactsModel model)
        {
            var contactId = model.Contacts[1].Id;

            var actual = model.ContactFor(contactId);

            actual.Should().BeEquivalentTo(model.Contacts[1]);
        }

        [Theory]
        [MockAutoData]
        public static void ContactFor_InvalidId_ReturnsNull(SupplierContactsModel model, int contactId)
        {
            var actual = model.ContactFor(contactId);

            actual.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static void SetSolutionId_Sets_SolutionIdOnContacts(
            SupplierContactsModel model)
        {
            model.SetSolutionId();

            model.Contacts.Select(c => c.SolutionId).Should().AllBeEquivalentTo(model.SolutionId);
        }

        [Fact]
        public static void SetSolutionId_ContactsAreNull_NoExceptionThrown()
        {
            var fixture = new Fixture();
            fixture.Customize(new CatalogueItemIdCustomization());
            var model = fixture.Build<SupplierContactsModel>()
                .Without(m => m.Contacts).Create();

            var ex = Record.Exception(() => model.SetSolutionId());

            ex.Should().BeNull();
        }

        [Fact]
        public static void ValidContacts_ContactsInModel_ReturnsNonEmptyContacts()
        {
            var emptyContact = new MarketingContact() { FirstName = null, LastName = null, Department = null, PhoneNumber = null, Email = null };
            var nonEmptyContact = new MarketingContact() { FirstName = "Test", LastName = "Test", Department = "Test", PhoneNumber = "Test", Email = "Test" };
            var contacts = new List<MarketingContact>() { nonEmptyContact, emptyContact, nonEmptyContact, emptyContact };

            var model = new SupplierContactsModel
            {
                Contacts = contacts.ToArray(),
            };

            var actual = model.ValidContacts();

            actual.Should().BeEquivalentTo(new[] { contacts[0], contacts[2] });
        }

        [Fact]
        public static void ValidContacts_NoContactsInModel_ReturnsEmptyList()
        {
            var actual = new SupplierContactsModel().ValidContacts();

            actual.Should().BeEmpty();
        }

        [Fact]
        public static void NewAndValidContacts_ContactsInModel_ReturnsNewAndValidContacts()
        {
            var emptyContact = new MarketingContact() { FirstName = null, LastName = null, Department = null, PhoneNumber = null, Email = null };
            var newAndValidContact = new MarketingContact() { Id = default, FirstName = "Test", LastName = "Test", Department = "Test", PhoneNumber = "Test", Email = "Test" };
            var contacts = new List<MarketingContact>() { emptyContact, emptyContact, newAndValidContact, newAndValidContact };
            var model = new SupplierContactsModel
            {
                Contacts = contacts.ToArray(),
            };

            var actual = model.NewAndValidContacts();

            actual.Should().BeEquivalentTo(new[] { contacts[3], contacts[2] });
        }

        [Fact]
        public static void NewAndValidContacts_NoContactsInModel_ReturnsEmptyList()
        {
            var actual = new SupplierContactsModel().NewAndValidContacts();

            actual.Should().BeEmpty();
        }
    }
}
