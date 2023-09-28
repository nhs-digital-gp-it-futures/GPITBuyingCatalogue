using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.NonPriceElementModels;

public static class NonPriceElementWeightsModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Competition competition,
        NonPriceWeights nonPriceWeights)
    {
        competition.NonPriceElements = new()
        {
            Features = new List<FeaturesCriteria> { new() },
            Interoperability = new List<InteroperabilityCriteria> { new() },
            Implementation = new(),
            ServiceLevel = new(),
            NonPriceWeights = nonPriceWeights,
        };

        var expectedModel = new NonPriceElementWeightsModel
        {
            CompetitionName = competition.Name,
            HasInteroperability = true,
            HasImplementation = true,
            HasServiceLevel = true,
            HasFeatures = true,
            Implementation = nonPriceWeights.Implementation,
            Interoperability = nonPriceWeights.Interoperability,
            ServiceLevel = nonPriceWeights.ServiceLevel,
            Features = nonPriceWeights.Features,
        };

        var model = new NonPriceElementWeightsModel(competition);

        model.Should().BeEquivalentTo(expectedModel);
    }
}
