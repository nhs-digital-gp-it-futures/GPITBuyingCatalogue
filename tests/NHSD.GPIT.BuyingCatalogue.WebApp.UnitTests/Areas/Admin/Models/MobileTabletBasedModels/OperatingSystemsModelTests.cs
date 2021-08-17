using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.MobileTabletBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.MobileTabletBasedModels
{
    public static class OperatingSystemsModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            CatalogueItem catalogueItem)
        {
            var actual = new OperatingSystemsModel(catalogueItem);

            var mobileOperatingSystems = catalogueItem.Solution.GetClientApplication().MobileOperatingSystems;

            actual.Description.Should().Be(mobileOperatingSystems.OperatingSystemsDescription);

            actual.BackLink.Should().Be($"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/mobiletablet");

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

        [Theory]
        [CommonAutoData]
        public static void IsComplete_CorrectlySet_WhenAnOperatingSystemChecked(
            OperatingSystemsModel model)
        {
            model.OperatingSystems.First().Checked = true;

            var actual = model.IsComplete;

            actual.Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void IsComplete_CorrectlySet_WhenNoConnectionTypeChecked(
          OperatingSystemsModel model)
        {
            Array.ForEach(model.OperatingSystems, c => c.Checked = false);

            var actual = model.IsComplete;

            actual.Should().BeFalse();
        }
    }
}
