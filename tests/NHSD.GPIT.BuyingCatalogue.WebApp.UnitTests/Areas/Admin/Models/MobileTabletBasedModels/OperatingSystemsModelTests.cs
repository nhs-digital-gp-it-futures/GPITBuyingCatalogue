﻿using System;
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
        [InlineData("", false, false)]
        [InlineData(" ", false, false)]
        [InlineData(null, false, false)]
        [InlineData("A description", false, false)]
        [InlineData("A description", true, true)]
        public static void IsComplete_CorrectlySet(
            string description,
            bool hasOperatingSystems,
            bool expectedCompletionState)
        {
            var operatingSystems = hasOperatingSystems ? new SupportedOperatingSystemModel[1] { new SupportedOperatingSystemModel() } : Array.Empty<SupportedOperatingSystemModel>();

            var model = new OperatingSystemsModel { Description = description, OperatingSystems = operatingSystems };

            var actual = model.IsComplete;

            actual.Should().Be(expectedCompletionState);
        }
    }
}
