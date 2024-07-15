using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.SupplierDefinedEpics
{
    public static class EditSupplierDefinedEpicModelTests
    {
        [Theory]
        [MockAutoData]
        public static void CanDelete_InactiveEpicNoReferences_True(
            Epic epic,
            Capability capability)
        {
            epic.Capabilities.Add(capability);
            epic.IsActive = false;

            var model = new EditSupplierDefinedEpicModel(epic, Array.Empty<CatalogueItem>());

            model.CanDelete.Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static void CanDelete_InactiveEpicWithReferences_False(
            Epic epic,
            IList<CatalogueItem> referencingItems,
            Capability capability)
        {
            epic.Capabilities.Add(capability);
            epic.IsActive = false;

            var model = new EditSupplierDefinedEpicModel(epic, referencingItems);

            model.CanDelete.Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void CanDelete_ActiveEpicNoReferences_False(
            Epic epic,
            Capability capability)
        {
            epic.Capabilities.Add(capability);
            epic.IsActive = true;

            var model = new EditSupplierDefinedEpicModel(epic, Array.Empty<CatalogueItem>());

            model.CanDelete.Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void CanDelete_ActiveEpicWithReferences_False(
            Epic epic,
            IList<CatalogueItem> referencingItems,
            Capability capability)
        {
            epic.Capabilities.Add(capability);
            epic.IsActive = true;

            var model = new EditSupplierDefinedEpicModel(epic, referencingItems);

            model.CanDelete.Should().BeFalse();
        }
    }
}
