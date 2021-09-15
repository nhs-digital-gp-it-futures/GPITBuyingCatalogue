using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class ImplementationTimescaleModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            CatalogueItem catalogueItem,
            ImplementationTimescaleModel expected)
        {
            expected.SolutionId = catalogueItem.Id;
            expected.SolutionName = catalogueItem.Name;
            expected.Description = catalogueItem.Solution?.ImplementationDetail;

            var actual = new ImplementationTimescaleModel(catalogueItem);

            actual.SolutionId.Should().BeEquivalentTo(expected.SolutionId);
            actual.SolutionName.Should().BeEquivalentTo(expected.SolutionName);
            actual.Description.Should().BeEquivalentTo(expected.Description);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new ImplementationTimescaleModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }

        [Theory]
        [AutoData]
        public static void StatusImplementation_DescriptionAdded_ReturnsCompleted(string description)
        {
            var model = new ImplementationTimescaleModel { Description = description };

            var actual = model.ImplementationStatus();

            actual.Should().Be(TaskProgress.Completed);
        }

        [Fact]
        public static void StatusImplementation_NoDescriptionAdded_ReturnsNotStarted()
        {
            var model = new ImplementationTimescaleModel { Description = null };

            var actual = model.ImplementationStatus();

            actual.Should().Be(TaskProgress.NotStarted);
        }
    }
}
