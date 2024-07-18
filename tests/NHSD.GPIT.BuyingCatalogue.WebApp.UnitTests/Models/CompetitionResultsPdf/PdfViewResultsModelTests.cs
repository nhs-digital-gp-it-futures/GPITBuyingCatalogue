using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.CompetitionResultsPdf;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.CompetitionResultsPdf;

public static class PdfViewResultsModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Organisation organisation,
        Competition competition,
        FilterDetailsModel filterDetailsModel,
        ICollection<CompetitionSolution> nonShortlistedSolutions)
    {
        competition.Organisation = organisation;

        var model = new PdfViewResultsModel(competition, filterDetailsModel, nonShortlistedSolutions);

        model.Competition.Should().Be(competition);
        model.IsDirectAward().Should().BeFalse();
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SingleSolution_DirectAwardTrue(
        Organisation organisation,
        Competition competition,
        FilterDetailsModel filterDetailsModel,
        ICollection<CompetitionSolution> nonShortlistedSolutions)
    {
        nonShortlistedSolutions = new List<CompetitionSolution> { nonShortlistedSolutions.FirstOrDefault() };
        competition.Organisation = organisation;
        competition.CompetitionSolutions = new List<CompetitionSolution>();

        var model = new PdfViewResultsModel(competition, filterDetailsModel, nonShortlistedSolutions);

        model.Competition.Should().Be(competition);
        model.IsDirectAward().Should().BeTrue();
    }
}
