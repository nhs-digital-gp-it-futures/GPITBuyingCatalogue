using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.MobileTabletBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.MobileTabletBasedModels
{
    public static class OperatingSystemsModelTests
    {
        [Theory]
        [MockAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            Solution solution)
        {
            var catalogueItem = solution.CatalogueItem;
            var actual = new OperatingSystemsModel(catalogueItem);

            var mobileOperatingSystems = solution.ApplicationTypeDetail.MobileOperatingSystems;

            actual.Description.Should().Be(mobileOperatingSystems.OperatingSystemsDescription);

            actual.OperatingSystems.Should().BeEquivalentTo(new SupportedOperatingSystemModel[]
            {
                new() { OperatingSystemName = "Apple IOS", Checked = mobileOperatingSystems.OperatingSystems.Any(t => t.EqualsIgnoreCase("Apple IOS")) },
                new() { OperatingSystemName = "Android", Checked = mobileOperatingSystems.OperatingSystems.Any(t => t.EqualsIgnoreCase("Android")) },
                new() { OperatingSystemName = "Other", Checked = mobileOperatingSystems.OperatingSystems.Any(t => t.EqualsIgnoreCase("Other")) },
            });
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new OperatingSystemsModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }
    }
}
