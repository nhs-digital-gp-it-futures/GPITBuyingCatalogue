using FluentAssertions;
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
        CommonActions.PageTitle().Should().BeEquivalentTo("Manage Frameworks".FormatForComparison());
    }
}
