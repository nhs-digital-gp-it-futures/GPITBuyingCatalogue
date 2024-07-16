using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.CompetitionResultsPdf;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers;

public static class CompetitionResultsPdfControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionResultsPdfController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_NullCompetition_ReturnsNotFoundResult(
        string internalOrgId,
        int competitionId,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionResultsPdfController controller)
    {
        competitionsService.GetCompetitionForResults(internalOrgId, competitionId).Returns((Competition)null);

        var result = (await controller.Index(internalOrgId, competitionId)).As<NotFoundResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_ValidCompetition_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        FilterDetailsModel filterDetailsModel,
        ICollection<CompetitionSolution> nonShortlistedSolutions,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IManageFiltersService filtersService,
        CompetitionResultsPdfController controller)
    {
        filtersService.GetFilterDetails(Arg.Any<int>(), competition.FilterId).Returns(filterDetailsModel);

        competition.Organisation = organisation;

        competitionsService.GetCompetitionForResults(organisation.InternalIdentifier, competition.Id).Returns(competition);

        competitionsService.GetNonShortlistedSolutions(organisation.InternalIdentifier, competition.Id).Returns(nonShortlistedSolutions);

        var expectedModel = new PdfViewResultsModel(competition, filterDetailsModel, nonShortlistedSolutions);

        var result = (await controller.Index(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
           .BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.PdfUrl));
    }
}
