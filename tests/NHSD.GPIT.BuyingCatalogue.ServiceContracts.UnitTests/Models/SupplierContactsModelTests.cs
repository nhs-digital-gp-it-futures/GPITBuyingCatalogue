using System.Linq;
using AutoFixture;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Models
{
    public static class SupplierContactsModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void ContactFor_ValidId_ReturnsMatchingContact(SupplierContactsModel model)
        {
            var contactId = model.Contacts[1].Id;

            var actual = model.ContactFor(contactId);

            actual.Should().BeEquivalentTo(model.Contacts[1]);
        }

        [Theory]
        [CommonAutoData]
        public static void ContactFor_InvalidId_ReturnsNull(SupplierContactsModel model, int contactId)
        {
            var actual = model.ContactFor(contactId);

            actual.Should().BeNull();
        }

        // TODO: convert to declarative AutoFixture test
        [Fact]
        public static void SetSolutionId_Sets_SolutionIdOnContacts()
        {
            var fixture = new Fixture();
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            fixture.Customize(new CallOffIdCustomization());
            fixture.Customize(new CatalogueItemIdCustomization());
            var model = fixture.Create<SupplierContactsModel>();

            model.SetSolutionId();

            foreach (var contact in model.Contacts)
            {
                contact.SolutionId.Should().Be(model.SolutionId);
            }
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
            var contacts = Enumerable.Range(1, 4).Select(_ => new Mock<MarketingContact>()).ToList();
            contacts[1].Setup(c => c.IsEmpty()).Returns(true);
            contacts[3].Setup(c => c.IsEmpty()).Returns(true);
            var model = new SupplierContactsModel
            {
                Contacts = contacts.Select(x => x.Object).ToArray(),
            };

            var actual = model.ValidContacts();

            actual.Should().BeEquivalentTo(contacts[0].Object, contacts[2].Object);
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
            var contacts = Enumerable.Range(1, 4).Select(_ => new Mock<MarketingContact>()).ToList();
            contacts[2].Setup(c => c.NewAndValid()).Returns(true);
            contacts[3].Setup(c => c.NewAndValid()).Returns(true);
            var model = new SupplierContactsModel
            {
                Contacts = contacts.Select(x => x.Object).ToArray(),
            };

            var actual = model.NewAndValidContacts();

            actual.Should().BeEquivalentTo(contacts[3].Object, contacts[2].Object);
        }

        [Fact]
        public static void NewAndValidContacts_NoContactsInModel_ReturnsEmptyList()
        {
            var actual = new SupplierContactsModel().NewAndValidContacts();

            actual.Should().BeEmpty();
        }
    }
}
