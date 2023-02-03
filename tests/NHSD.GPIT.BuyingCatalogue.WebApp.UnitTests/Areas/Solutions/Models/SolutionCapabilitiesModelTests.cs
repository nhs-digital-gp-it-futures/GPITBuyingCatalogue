using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class SolutionCapabilitiesModelTests
    {
        [Fact]
        public static void Constructor_NullSolutionCapability_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new CatalogueItemCapabilitiesModel(null, new CatalogueItem()));
        }

        [Fact]
        public static void Constructor_NullSolution_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new CatalogueItemCapabilitiesModel(new CatalogueItemCapability(), null));
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_ValidCapabilityAndSolution_PopulatesFields(
            CatalogueItemCapability solutionCapability,
            CatalogueItem catalogueItem)
        {
            var result = new CatalogueItemCapabilitiesModel(solutionCapability, catalogueItem);
            Assert.Equal(solutionCapability.Capability.Id, result.Id);
            Assert.Equal($"{solutionCapability.Capability.Name}, {solutionCapability.Capability.Version}", result.Name);
            Assert.Equal(solutionCapability.Capability.SourceUrl, result.SourceUrl);
            Assert.Equal(solutionCapability.Capability.Description, result.Description);
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_ValidCapabilityAndSolution_PopulateEpics(
            CatalogueItemCapability solutionCapability,
            CatalogueItem catalogueItem)
        {
            var isMetStatus = new CatalogueItemEpicStatus() { IsMet = true };
            var isNotMetStatus = new CatalogueItemEpicStatus() { IsMet = false };
            var mustEpic = new Epic() { IsActive = true, CompliancyLevel = CompliancyLevel.Must };
            var mayEpic = new Epic() { IsActive = true, CompliancyLevel = CompliancyLevel.May };

            var epics = new List<CatalogueItemEpic>()
            {
                new CatalogueItemEpic() { CapabilityId = solutionCapability.Capability.Id, Epic = mustEpic, Status = isMetStatus, },
                new CatalogueItemEpic() { CapabilityId = solutionCapability.Capability.Id, Epic = mustEpic, Status = isNotMetStatus, },
                new CatalogueItemEpic() { CapabilityId = solutionCapability.Capability.Id, Epic = mayEpic, Status = isMetStatus, },
                new CatalogueItemEpic() { CapabilityId = solutionCapability.Capability.Id, Epic = mayEpic, Status = isNotMetStatus, },
            };

            catalogueItem.CatalogueItemEpics = epics;

            var result = new CatalogueItemCapabilitiesModel(solutionCapability, catalogueItem);
            Assert.Single(result.MustEpicsMet);
            Assert.Single(result.MustEpicsNotMet);
            Assert.Single(result.MayEpicsMet);
            Assert.Single(result.MayEpicsNotMet);
        }
    }
}
