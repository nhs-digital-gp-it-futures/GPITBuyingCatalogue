using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    public static class ContactExtensionsTests
    {
        [Fact]
        public static void ToDomain_NullModel_ReturnsExpectedContact()
        {
            ContactExtensions.ToDomain(null).Should().BeEquivalentTo(new Contact());
        }

        [Theory]
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
                Department = model.Department,
            };

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
