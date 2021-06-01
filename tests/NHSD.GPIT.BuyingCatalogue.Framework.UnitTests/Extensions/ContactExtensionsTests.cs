using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ContactExtensionsTests
    {       
        [Test]
        public static void ToDomain_NullModel_ReturnsExpectedContact()
        {
            ContactExtensions.ToDomain(null).Should().BeEquivalentTo(new Contact());
        }

        [Test]
        [AutoData]
        public static void ToDomain_ReturnsExpectedContact(PrimaryContactModel model)
        {
            var actual = model.ToDomain();

            var expected = new Contact
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.EmailAddress,
                Phone = model.TelephoneNumber,
            };

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
