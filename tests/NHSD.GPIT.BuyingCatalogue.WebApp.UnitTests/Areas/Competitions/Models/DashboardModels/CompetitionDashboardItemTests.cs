using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.DashboardModels;

public static class CompetitionDashboardItemTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsValues(
        Competition competition)
    {
        var model = new CompetitionDashboardItem(competition);

        model.Id.Should().Be(competition.Id);
        model.Name.Should().Be(competition.Name);
        model.Description.Should().Be(competition.Description);
        model.LastUpdated.Should().Be(competition.LastUpdated);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_NullCompletedDate_SetsProgressStatus(
        Competition competition)
    {
        competition.Completed = null;

        var model = new CompetitionDashboardItem(competition);

        model.Progress.Should().Be(TaskProgress.InProgress);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_ValidCompletedDate_SetsProgressStatus(
        Competition competition)
    {
        competition.Completed = DateTime.UtcNow;

        var model = new CompetitionDashboardItem(competition);

        model.Progress.Should().Be(TaskProgress.Completed);
    }
}
