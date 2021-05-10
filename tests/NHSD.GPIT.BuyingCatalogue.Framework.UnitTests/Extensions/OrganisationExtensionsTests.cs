using FluentAssertions;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OrganisationExtensionsTests
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void SolutionExtension_ReturnsDefaultAddressWhenNotSet(string address)
        {
            var organisation = new Organisation { Address = address };

            var result = organisation.GetAddress();

            result.Should().BeEquivalentTo(new Address());
        }

        [Test]
        public static void SolutionExtension_ReturnsAddressWhenSet()
        {
            var address = new Address { Line1 = "Line 1", Line2 = "Line 2" };
            var json = JsonConvert.SerializeObject(address);
            var organisation = new Organisation { Address = json };

            var result = organisation.GetAddress();

            result.Should().BeEquivalentTo(address);
        }
    }
}
