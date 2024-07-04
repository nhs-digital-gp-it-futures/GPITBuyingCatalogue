using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.TaskListModels;

public static class CompetitionReviewCriteriaModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Organisation organisation,
        Competition competition,
        NonPriceElements nonPriceElements,
        Weightings weightings,
        List<Integration> integrations)
    {
        competition.Organisation = organisation;
        competition.NonPriceElements = nonPriceElements;
        competition.Weightings = weightings;

        var expectedNonPriceWeights = competition.NonPriceElements.GetNonPriceElements()
            .ToDictionary(x => x, x => competition.NonPriceElements.GetNonPriceWeight(x));

        var model = new CompetitionReviewCriteriaModel(competition, integrations);

        model.InternalOrgId.Should().Be(organisation.InternalIdentifier);
        model.CompetitionId.Should().Be(competition.Id);
        model.CompetitionName.Should().Be(competition.Name);
        model.NonPriceElements.Should().Be(nonPriceElements);
        model.CompetitionWeights.Should().Be(weightings);
        model.NonPriceWeights.Should().BeEquivalentTo(expectedNonPriceWeights);
    }

    [Theory]
    [CommonInlineAutoData(true, "Continue")]
    [CommonInlineAutoData(false, "Confirm competition criteria")]
    public static void ContinueButton_ReviewedCriteria_ExpectedContent(
        bool hasReviewedCriteria,
        string expectedContent,
        Organisation organisation,
        Competition competition,
        NonPriceElements nonPriceElements,
        Weightings weightings,
        List<Integration> integrations)
    {
        competition.Organisation = organisation;
        competition.NonPriceElements = nonPriceElements;
        competition.Weightings = weightings;
        competition.HasReviewedCriteria = hasReviewedCriteria;

        var model = new CompetitionReviewCriteriaModel(competition, integrations);

        model.ContinueButton.Should().Be(expectedContent);
    }
}
