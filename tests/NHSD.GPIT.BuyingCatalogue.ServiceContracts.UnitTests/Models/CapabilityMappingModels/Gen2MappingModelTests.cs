using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Models.CapabilityMappingModels;

public static class Gen2MappingModelTests
{
    [Theory]
    [MockAutoData]
    public static void Constructing_SetsPropertiesAsExpected(
        List<Solution> solutions)
    {
        var capabilitiesImport = solutions.SelectMany(
            x => x.CatalogueItem.CatalogueItemCapabilities.Select(
                y => new Gen2CapabilitiesCsvModel
                {
                    SupplierId = x.CatalogueItem.SupplierId.ToString(),
                    SolutionId = x.CatalogueItemId.ToString(),
                    CapabilityId = $"C{y.CapabilityId}",
                }));

        var epicsImport = solutions.SelectMany(
                x => x.CatalogueItem.CatalogueItemEpics.Select(
                    y => new Gen2EpicsCsvModel
                    {
                        SupplierId = x.CatalogueItem.SupplierId.ToString(),
                        SolutionId = x.CatalogueItemId.ToString(),
                        CapabilityId = $"C{y.CapabilityId}",
                        EpicId = y.EpicId,
                    }))
            .ToList();

        var model = new Gen2MappingModel(capabilitiesImport, epicsImport);

        model.Solutions.Should().HaveCount(solutions.Count);
    }

    [Theory]
    [MockAutoData]
    public static void Constructing_WithAdditionalServices_SetsPropertiesAsExpected(
        List<Solution> solutions)
    {
        var additionalServiceIds = Enumerable.Range(0, solutions.Count).Select(x => $"A{x:D3}");
        var solutionsAndServices = solutions.Zip(additionalServiceIds).ToList();

        var capabilitiesImport = solutionsAndServices.SelectMany(
            x => x.First.CatalogueItem.CatalogueItemCapabilities.Select(
                y => new Gen2CapabilitiesCsvModel
                {
                    SupplierId = x.First.CatalogueItem.SupplierId.ToString(),
                    SolutionId = x.First.CatalogueItemId.ToString(),
                    AdditionalServiceId = x.Second.ToString(),
                    CapabilityId = $"C{y.CapabilityId}",
                }));

        var epicsImport = solutionsAndServices.SelectMany(
                x => x.First.CatalogueItem.CatalogueItemEpics.Select(
                    y => new Gen2EpicsCsvModel
                    {
                        SupplierId = x.First.CatalogueItem.SupplierId.ToString(),
                        SolutionId = x.First.CatalogueItemId.ToString(),
                        AdditionalServiceId = x.Second.ToString(),
                        CapabilityId = $"C{y.CapabilityId}",
                        EpicId = y.EpicId,
                    }))
            .ToList();

        var model = new Gen2MappingModel(capabilitiesImport, epicsImport);

        model.Solutions.Should().HaveCount(solutionsAndServices.Count);
        model.Solutions.Should().AllSatisfy(x => x.AdditionalServices.Should().ContainSingle());
    }
}
