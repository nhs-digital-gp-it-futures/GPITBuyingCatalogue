﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.SelectSolutionModels;

public static class SelectSolutionsModelTests
{
    public static IEnumerable<object[]> GetAdviceTestData => new[]
    {
        new object[] { null, "There were no results from your chosen filter.", },
        new object[]
        {
            Enumerable.Empty<SolutionModel>().ToList(), "There were no results from your chosen filter.",
        },
        new object[] { new List<SolutionModel> { new() }, "These are the results from your chosen filter." },
        new object[]
        {
            new List<SolutionModel> { new(), new(), new() },
            "These are the results from your chosen filter. You must provide a reason if any of the solutions listed are not taken through to your competition shortlist.",
        },
    };

    [Theory]
    [MockAutoData]
    public static void Construct_SetsProperties(
        string competitionName,
        Solution solution,
        string frameworkName,
        List<CompetitionSolution> competitionSolutions,
        FilterDetailsModel filterDetailsModel)
    {
        competitionSolutions.ForEach(x => x.Solution = solution);

        var model = new SelectSolutionsModel(competitionName, competitionSolutions, frameworkName, filterDetailsModel);

        model.CompetitionName.Should().Be(competitionName);
        model.Solutions.Should().HaveCount(competitionSolutions.Count);
    }

    [Theory]
    [MockAutoData]
    public static void HasSingleSolution_WithSingle_ReturnsTrue(
        SolutionModel solutionModel,
        SelectSolutionsModel model)
    {
        model.Solutions = new() { solutionModel };

        model.HasSingleSolution().Should().BeTrue();
    }

    [Theory]
    [MockAutoData]
    public static void HasSingleSolution_WithMultiple_ReturnsFalse(
        List<SolutionModel> solutions,
        SelectSolutionsModel model)
    {
        model.Solutions = solutions;

        model.HasSingleSolution().Should().BeFalse();
    }
}
