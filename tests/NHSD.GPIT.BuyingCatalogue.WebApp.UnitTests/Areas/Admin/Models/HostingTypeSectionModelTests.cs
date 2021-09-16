﻿using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class HostingTypeSectionModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            [Frozen] CatalogueItem catalogueItem,
            HostingTypeSectionModel expected)
        {
            var actual = new HostingTypeSectionModel(catalogueItem);

            actual.SolutionId.Should().BeEquivalentTo(expected.SolutionId);
            actual.SolutionName.Should().BeEquivalentTo(expected.SolutionName);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new HostingTypeSectionModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }

        [Theory]
        [CommonAutoData]
        public static void StatusHostingType_AvailableHosting_ReturnsCompleted(
            HostingTypeSectionModel model)
        {
            var actual = model.Status();

            actual.Should().Be(TaskProgress.Completed);
        }

        [Fact]
        public static void StatusHostingType_NoCloudTypeAdded_ReturnsNotStarted()
        {
            var model = new HostingTypeSectionModel();

            var actual = model.Status();

            actual.Should().Be(TaskProgress.NotStarted);
        }
    }
}
