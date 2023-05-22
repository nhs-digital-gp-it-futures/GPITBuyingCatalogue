using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Competitions;

[Collection(nameof(CompetitionsCollection))]
public sealed class CompetitionsDashboard : BuyerTestBase
{
    private const string InternalOrgId = "CG-03F";

    private static readonly Dictionary<string, string> Parameters =
        new()
        {
            { nameof(InternalOrgId), InternalOrgId },
        };

    public CompetitionsDashboard(LocalWebApplicationFactory factory)
        : base(
            factory,
            typeof(CompetitionsDashboardController),
            nameof(CompetitionsDashboardController.Index),
            Parameters)
    {
    }

    [Fact]
    public void AllSectionsDisplayed()
    {
        using var context = GetEndToEndDbContext();
        var organisation = context.Organisations.AsNoTracking()
            .FirstOrDefault(x => x.InternalIdentifier == InternalOrgId);

        CommonActions.PageTitle().Should().Be($"Manage competitions - {organisation.Name}".FormatForComparison());
        CommonActions.LedeText()
            .Should()
            .Be("Create new competitions or view and edit existing ones.".FormatForComparison());

        CommonActions.ElementIsDisplayed(CompetitionsDashboardObjects.CompetitionsTable).Should().BeTrue();
    }
}
