﻿using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.CompetitionResultsPdf;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.CompetitionResultsPdf;

public static class PdfViewResultsModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Organisation organisation,
        Competition competition,
        FilterDetailsModel filterDetailsModel,
        ICollection<CompetitionSolution> nonShortlistedSolutions)
    {
        competition.Organisation = organisation;

        var model = new PdfViewResultsModel(competition, filterDetailsModel, nonShortlistedSolutions);

        model.Competition.Should().Be(competition);
    }
}
