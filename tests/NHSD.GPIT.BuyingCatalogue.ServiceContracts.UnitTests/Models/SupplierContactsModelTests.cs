using System.Linq;
using AutoFixture;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SupplierContactsModelTests
    {
        [Test]
        public static void SetSolutionId_Sets_SolutionIdOnContacts()
        {
            var fixture = new Fixture();
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var model = fixture.Create<SupplierContactsModel>();
            
            model.SetSolutionId();

            foreach (var contact in model.Contacts)
            {
                contact.SolutionId.Should().Be(model.SolutionId);
            }
        }

        [Test]
        public static void SetSolutionId_ContactsAreNull_NoExceptionThrown()
        {
            var fixture = new Fixture();
            var model = fixture.Build<SupplierContactsModel>()
                .Without(x => x.Contacts).Create();
            
            Assert.DoesNotThrow(() => model.SetSolutionId());
        }

        [Test]
        public static void ValidContacts_ContactsInModel_ReturnsNonEmptyContacts()
        {
            var contacts = Enumerable.Range(1, 4).Select(x => new Mock<MarketingContact>()).ToList();
            contacts[1].Setup(x => x.IsEmpty())
                .Returns(true);
            contacts[3].Setup(x => x.IsEmpty())
                .Returns(true);
            var model = new SupplierContactsModel
            {
                Contacts = contacts.Select(x => x.Object).ToArray(),
            };

            var actual = model.ValidContacts();

            actual.Should().BeEquivalentTo(contacts[0].Object, contacts[2].Object);
        }

        [Test]
        public static void ValidContacts_NoContactsInModel_ReturnsEmptyList()
        {
            var actual = new SupplierContactsModel().ValidContacts();

            actual.Should().BeEmpty();
        }

        [Test]
        public static void NewAndValidContacts_ContactsInModel_ReturnsNewAndValidContacts()
        {
            var contacts = Enumerable.Range(1, 4).Select(x => new Mock<MarketingContact>()).ToList();
            contacts[2].Setup(x => x.NewAndValid())
                .Returns(true);
            contacts[3].Setup(x => x.NewAndValid())
                .Returns(true);
            var model = new SupplierContactsModel
            {
                Contacts = contacts.Select(x => x.Object).ToArray(),
            };

            var actual = model.NewAndValidContacts();

            actual.Should().BeEquivalentTo(contacts[3].Object, contacts[2].Object);
        }

        [Test]
        public static void NewAndValidContacts_NoContactsInModel_ReturnsEmptyList()
        {
            var actual = new SupplierContactsModel().NewAndValidContacts();

            actual.Should().BeEmpty();
        }
    }
}