using System;
using System.Collections.ObjectModel;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.DashboardModels;

public static class CompetitionDashboardItemTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsValues(
        Competition competition)
    {
        var model = new CompetitionDashboardItem(competition);

        model.Id.Should().Be(competition.Id);
        model.Name.Should().Be(competition.Name);
        model.Description.Should().Be(competition.Description);
        model.Created.Should().Be(competition.Created);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_NullCompletedDate_SetsProgressStatus(
        Competition competition)
    {
        competition.Completed = null;

        var model = new CompetitionDashboardItem(competition);

        model.Progress.Should().Be(TaskProgress.InProgress);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_ValidCompletedDate_SetsProgressStatus(
        Competition competition)
    {
        competition.Completed = DateTime.UtcNow;

        var model = new CompetitionDashboardItem(competition);

        model.Progress.Should().Be(TaskProgress.Completed);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SingleItem_IsDirectAward(
        Competition competition,
        CompetitionSolution competitionSolution)
    {
        competition.CompetitionSolutions = new Collection<CompetitionSolution> { competitionSolution };

        var model = new CompetitionDashboardItem(competition);

        model.IsDirectAward().Should().Be(true);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_MultipleItems_IsNotDirectAward(
        Competition competition,
        Collection<CompetitionSolution> competitionSolutions)
    {
        competition.CompetitionSolutions = competitionSolutions;

        var model = new CompetitionDashboardItem(competition);

        model.IsDirectAward().Should().Be(false);
    }
}
