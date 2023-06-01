using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Competitions;

[Collection(nameof(CompetitionsCollection))]
public class BeforeYouStart : BuyerTestBase
{
    private const string InternalOrgId = "CG-03F";

    private static readonly Dictionary<string, string> Parameters =
        new() { { nameof(InternalOrgId), InternalOrgId }, };

    public BeforeYouStart(LocalWebApplicationFactory factory)
        : base(
            factory,
            typeof(CompetitionsDashboardController),
            nameof(CompetitionsDashboardController.BeforeYouStart),
            Parameters)
    {
    }

    [Fact]
    public void AllSectionsDisplayed()
    {
        CommonActions.PageTitle().Should().Be("Before you create a competition".FormatForComparison());
        CommonActions.LedeText().Should().Be("Before you create a competition you should have:".FormatForComparison());

        CommonActions.SaveButtonDisplayed().Should().BeTrue();

        CommonActions.GoBackLinkDisplayed().Should().BeTrue();
    }

    [Fact]
    public void ClickGoBackLink_NavigatesCorrectly()
    {
        CommonActions.ClickGoBackLink();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(CompetitionsDashboardController),
                nameof(CompetitionsDashboardController.Index))
            .Should()
            .BeTrue();
    }
}
