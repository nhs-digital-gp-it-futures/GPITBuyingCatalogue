using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.Frameworks;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Frameworks;

[Collection(nameof(AdminCollection))]
public sealed class Dashboard : AuthorityTestBase
{
    public Dashboard(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
        : base(factory, typeof(FrameworksController), nameof(FrameworksController.Dashboard), null, testOutputHelper)
    {
    }

    [Fact]
    public void AllElementsDisplayed()
    {
        CommonActions.PageTitle().Should().BeEquivalentTo("Manage frameworks".FormatForComparison());
        CommonActions.LedeText()
            .Should()
            .BeEquivalentTo("Add new frameworks or mark existing ones as expired.".FormatForComparison());

        CommonActions.ElementIsDisplayed(BreadcrumbObjects.HomeBreadcrumbLink).Should().BeTrue();

        CommonActions.ElementIsDisplayed(DashboardFrameworkObjects.FrameworksTable).Should().BeTrue();

        CommonActions.ContinueButtonDisplayed().Should().BeTrue();
    }

    [Fact]
    public void ClickHome_RedirectsToHomepage()
    {
        CommonActions.ClickLinkElement(BreadcrumbObjects.HomeBreadcrumbLink);

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void ClickContinue_RedirectsToHomepage()
    {
        CommonActions.ClickContinue();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index))
            .Should()
            .BeTrue();
    }
}
