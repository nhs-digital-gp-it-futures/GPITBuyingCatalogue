using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.YourAccount;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Models
{
    public static class YourAccountModelTests
    {
        [Fact]
        public static void Constructor_Requires_Organisation()
        {
            FluentActions.Invoking(() => new YourAccountModel(null))
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Theory]
        [MockAutoData]
        public static void Constructor_WithValidArguments_PropertiesCorrectlySet(
            Organisation organisation)
        {
            var model = new YourAccountModel(organisation);

            model.OrganisationAddress.Should().Be(organisation.Address);
            model.Index.Should().Be(0);
        }
    }
}
