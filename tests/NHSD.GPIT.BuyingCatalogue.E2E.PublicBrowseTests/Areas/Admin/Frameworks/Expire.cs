using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.Frameworks;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Frameworks;

[Collection(nameof(AdminCollection))]
public sealed class Expire : AuthorityTestBase
{
    private const string FrameworkId = "DFOCVC001";

    private static readonly Dictionary<string, string> Parameters = new() { { nameof(FrameworkId), FrameworkId }, };

    public Expire(LocalWebApplicationFactory factory, ITestOutputHelper helper)
        : base(factory, typeof(FrameworksController), nameof(FrameworksController.Expire), Parameters, helper)
    {
    }

    [Fact]
    public void AllElementsDisplayed()
    {
        CommonActions.GoBackLinkDisplayed().Should().BeTrue();

        CommonActions.PageTitle()
            .Should()
            .BeEquivalentTo("Are you sure you want to mark this framework as expired?".FormatForComparison());

        CommonActions.ElementIsDisplayed(ExpireObjects.FrameworkExpiryWarning).Should().BeTrue();

        CommonActions.SaveButtonDisplayed().Should().BeTrue();
    }

    [Fact]
    public void ClickCancel_Redirects()
    {
        CommonActions.ClickCancel();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(FrameworksController),
                nameof(FrameworksController.Dashboard))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void ClickBacklink_Redirects()
    {
        CommonActions.ClickGoBackLink();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(FrameworksController),
                nameof(FrameworksController.Dashboard))
            .Should()
            .BeTrue();
    }
}
