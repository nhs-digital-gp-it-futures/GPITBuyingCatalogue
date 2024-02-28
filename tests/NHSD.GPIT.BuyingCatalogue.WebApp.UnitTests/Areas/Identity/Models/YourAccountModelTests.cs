using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.YourAccount;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
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
        [CommonAutoData]
        public static void Constructor_WithValidArguments_PropertiesCorrectlySet(
            Organisation organisation)
        {
            var model = new YourAccountModel(organisation);

            model.OrganisationAddress.Should().Be(organisation.Address);
            model.Index.Should().Be(0);
        }
    }
}
