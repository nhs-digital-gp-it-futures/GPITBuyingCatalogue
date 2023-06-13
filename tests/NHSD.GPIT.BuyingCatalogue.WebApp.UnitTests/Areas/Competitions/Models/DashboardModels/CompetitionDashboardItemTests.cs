using System;
using FluentAssertions;
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
        int id,
        string name,
        string description,
        DateTime lastUpdated)
    {
        var model = new CompetitionDashboardItem(id, name, description, lastUpdated, null);

        model.Id.Should().Be(id);
        model.Name.Should().Be(name);
        model.Description.Should().Be(description);
        model.LastUpdated.Should().Be(lastUpdated);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_NullCompletedDate_SetsProgressStatus(
        int id,
        string name,
        string description,
        DateTime lastUpdated)
    {
        var model = new CompetitionDashboardItem(id, name, description, lastUpdated, null);

        model.Progress.Should().Be(TaskProgress.InProgress);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_ValidCompletedDate_SetsProgressStatus(
        int id,
        string name,
        string description,
        DateTime lastUpdated,
        DateTime completed)
    {
        var model = new CompetitionDashboardItem(id, name, description, lastUpdated, completed);

        model.Progress.Should().Be(TaskProgress.Completed);
    }
}
