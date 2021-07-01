using FluentAssertions;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    public static class OrganisationExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public static void SolutionExtension_ReturnsDefaultAddressWhenNotSet(string address)
        {
            var organisation = new Organisation { Address = address };

            var result = organisation.GetAddress();

            result.Should().BeEquivalentTo(new Address());
        }

        [Fact]
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
