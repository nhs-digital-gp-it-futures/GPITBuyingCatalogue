using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.TaskListModels;

public static class CompetitionTaskListViewModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsProperties(
        Organisation organisation,
        CompetitionTaskListModel competitionTaskListModel)
    {
        var model = new CompetitionTaskListViewModel(organisation, competitionTaskListModel);

        model.OrganisationName.Should().Be(organisation.Name);
        model.InternalOrgId.Should().Be(organisation.InternalIdentifier);
        model.TaskListModel.Should().BeEquivalentTo(competitionTaskListModel);
    }

    [Theory]
    [MockInlineAutoData(TaskProgress.Completed, "View the award criteria you selected to help you compare your shortlisted solutions.")]
    [MockInlineAutoData(TaskProgress.NotStarted, "Select the award criteria you want to use to compare your shortlisted solutions.")]
    public static void AwardCriteriaText_ReviewedCriteria_ExpectedContent(
        TaskProgress taskProgress,
        string expectedContent,
        Organisation organisation,
        CompetitionTaskListModel competitionTaskListModel)
    {
        competitionTaskListModel.ReviewCompetitionCriteria = taskProgress;

        var model = new CompetitionTaskListViewModel(organisation, competitionTaskListModel);

        model.AwardCriteriaText.Should().Be(expectedContent);
    }

    [Theory]
    [MockInlineAutoData(TaskProgress.Completed, "View the award criteria weightings you gave for price and non-price elements.")]
    [MockInlineAutoData(TaskProgress.NotStarted, "Give your chosen award criteria weightings based on how important they are to you.")]
    public static void AwardCriteriaWeightingsText_ReviewedCriteria_ExpectedContent(
        TaskProgress taskProgress,
        string expectedContent,
        Organisation organisation,
        CompetitionTaskListModel competitionTaskListModel)
    {
        competitionTaskListModel.ReviewCompetitionCriteria = taskProgress;

        var model = new CompetitionTaskListViewModel(organisation, competitionTaskListModel);

        model.AwardCriteriaWeightingsText.Should().Be(expectedContent);
    }

    [Theory]
    [MockInlineAutoData(TaskProgress.Completed, "View any non-price elements you added to help you score your shortlisted solutions.")]
    [MockInlineAutoData(TaskProgress.NotStarted, "Select any non-price elements you want to add to help you score your shortlisted solutions.")]
    public static void NonPriceElementsText_ReviewedCriteria_ExpectedContent(
        TaskProgress taskProgress,
        string expectedContent,
        Organisation organisation,
        CompetitionTaskListModel competitionTaskListModel)
    {
        competitionTaskListModel.ReviewCompetitionCriteria = taskProgress;

        var model = new CompetitionTaskListViewModel(organisation, competitionTaskListModel);

        model.NonPriceElementsText.Should().Be(expectedContent);
    }

    [Theory]
    [MockInlineAutoData(TaskProgress.Completed, "View the non-price elements weightings you applied for this competition.")]
    [MockInlineAutoData(TaskProgress.NotStarted, "Give your chosen non-price elements weightings based on how important they are to you.")]
    public static void NonPriceWeightingsText_ReviewedCriteria_ExpectedContent(
        TaskProgress taskProgress,
        string expectedContent,
        Organisation organisation,
        CompetitionTaskListModel competitionTaskListModel)
    {
        competitionTaskListModel.ReviewCompetitionCriteria = taskProgress;

        var model = new CompetitionTaskListViewModel(organisation, competitionTaskListModel);

        model.NonPriceWeightingsText.Should().Be(expectedContent);
    }
}
