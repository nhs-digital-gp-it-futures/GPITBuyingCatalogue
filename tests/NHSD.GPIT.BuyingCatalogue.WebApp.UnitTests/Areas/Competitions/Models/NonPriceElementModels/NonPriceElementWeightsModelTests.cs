using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.NonPriceElementModels;

public static class NonPriceElementWeightsModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Competition competition,
        NonPriceWeights nonPriceWeights)
    {
        competition.NonPriceElements = new()
        {
            Features = new List<FeaturesCriteria> { new() },
            IntegrationTypes = new List<IntegrationType> { new() },
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
            HasReviewedCriteria = competition.HasReviewedCriteria,
        };

        var model = new NonPriceElementWeightsModel(competition);

        model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.NonPriceWeights));
    }

    [Theory]
    [MockInlineAutoData(true, "Continue")]
    [MockInlineAutoData(false, "Save and continue")]
    public static void ContinueButton_ReviewedCriteria_ExpectedContent(
        bool hasReviewedCriteria,
        string expectedContent,
        Competition competition)
    {
        competition.HasReviewedCriteria = hasReviewedCriteria;

        var model = new NonPriceElementWeightsModel(competition);

        model.ContinueButton.Should().Be(expectedContent);
    }

    [Theory]
    [MockInlineAutoData(true, "Non-price weightings")]
    [MockInlineAutoData(false, "How would you like to weight your non-price elements for this competition?")]
    public static void Title_ReviewedCriteria_ExpectedContent(
        bool hasReviewedCriteria,
        string expectedContent,
        Competition competition)
    {
        competition.HasReviewedCriteria = hasReviewedCriteria;

        var model = new NonPriceElementWeightsModel(competition);

        model.Title.Should().Be(expectedContent);
    }

    [Theory]
    [MockInlineAutoData(true, "These are the non-price elements weightings you applied for this competition.")]
    [MockInlineAutoData(false, "Give your chosen non-price elements weightings based on how important they are to you.")]
    public static void Advice_ReviewedCriteria_ExpectedContent(
        bool hasReviewedCriteria,
        string expectedContent,
        Competition competition)
    {
        competition.HasReviewedCriteria = hasReviewedCriteria;

        var model = new NonPriceElementWeightsModel(competition);

        model.Advice.Should().Be(expectedContent);
    }
}
