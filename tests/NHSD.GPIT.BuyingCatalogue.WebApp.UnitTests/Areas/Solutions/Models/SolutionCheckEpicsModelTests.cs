using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class SolutionCheckEpicsModelTests
    {
        [Fact]
        public static void Class_Inherits_INoNavModel()
        {
            typeof(SolutionCheckEpicsModel)
                .Should()
                .BeAssignableTo<INoNavModel>();
        }

        [Theory]
        [MockAutoData]
        public static void NhsDefined_NhsDefinedArrayNotEmpty_ReturnsTrue(SolutionCheckEpicsModel model)
        {
            model.NhsDefined.Should().NotBeEmpty();

            model.HasNhsDefined().Should().BeTrue();
        }

        [Fact]
        public static void NhsDefined_NhsDefinedArrayEmpty_ReturnsFalse()
        {
            var model = new SolutionCheckEpicsModel { NhsDefined = Array.Empty<Epic>() };

            model.HasNhsDefined().Should().BeFalse();
        }

        [Fact]
        public static void NhsDefined_NhsDefinedArrayIsNull_ReturnsFalse()
        {
            var model = new SolutionCheckEpicsModel { NhsDefined = null };

            model.HasNhsDefined().Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void SupplierDefined_SupplierDefinedArrayNotEmpty_ReturnsTrue(SolutionCheckEpicsModel model)
        {
            model.SupplierDefined.Should().NotBeEmpty();

            model.HasSupplierDefined().Should().BeTrue();
        }

        [Fact]
        public static void SupplierDefined_SupplierDefinedArrayEmpty_ReturnsFalse()
        {
            var model = new SolutionCheckEpicsModel { SupplierDefined = Array.Empty<Epic>() };

            model.HasSupplierDefined().Should().BeFalse();
        }

        [Fact]
        public static void SupplierDefined_SupplierDefinedArrayIsNull_ReturnsFalse()
        {
            var model = new SolutionCheckEpicsModel { SupplierDefined = null };

            model.HasSupplierDefined().Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void HasNoEpics_NoNhsOrSupplierDefinedEpics_ReturnsTrue(
            CatalogueItemCapability solutionCapability,
            [Frozen] CatalogueItem catalogueItem)
        {
            solutionCapability.Capability.Epics = Array.Empty<Epic>();

            var model = new SolutionCheckEpicsModel(solutionCapability, catalogueItem);

            model.HasNoEpics().Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static void HasNoEpics_HasNhsDefinedEpicsOnly_ReturnsFalse(
            List<CatalogueItemEpic> epics,
            CatalogueItemCapability solutionCapability,
            [Frozen] CatalogueItem catalogueItem)
        {
            epics.ForEach(e => e.Epic = new Epic { SupplierDefined = false });
            catalogueItem.CatalogueItemEpics = epics;
            var model = new SolutionCheckEpicsModel(solutionCapability, catalogueItem);

            model.HasSupplierDefined().Should().BeFalse();
            model.HasNhsDefined().Should().BeTrue();
            model.HasNoEpics().Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void HasNoEpics_HasSupplierDefinedEpicsOnly_ReturnsFalse(
            List<CatalogueItemEpic> epics,
            CatalogueItemCapability solutionCapability,
            [Frozen] CatalogueItem catalogueItem)
        {
            epics.ForEach(e => e.Epic = new Epic { SupplierDefined = true });

            catalogueItem.CatalogueItemEpics = epics;
            var model = new SolutionCheckEpicsModel(solutionCapability, catalogueItem);

            model.HasSupplierDefined().Should().BeTrue();
            model.HasNhsDefined().Should().BeFalse();
            model.HasNoEpics().Should().BeFalse();
        }
    }
}
