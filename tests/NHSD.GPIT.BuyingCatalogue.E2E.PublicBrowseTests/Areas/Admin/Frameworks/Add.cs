using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.Frameworks;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Frameworks;

[Collection(nameof(AdminCollection))]
public sealed class Add : AuthorityTestBase
{
    public Add(LocalWebApplicationFactory factory, ITestOutputHelper helper)
        : base(factory, typeof(FrameworksController), nameof(FrameworksController.Add), null, helper)
    {
    }

    [Fact]
    public void AllElementsDisplayed()
    {
        CommonActions.PageTitle()
            .Should()
            .Be("Add a framework".FormatForComparison());

        CommonActions.LedeText()
            .Should()
            .Be("Provide details for this framework.".FormatForComparison());

        CommonActions.ElementIsDisplayed(AddFrameworkObjects.FrameworkNameInput).Should().BeTrue();
        CommonActions.ElementIsDisplayed(AddFrameworkObjects.IsLocalFundingOnlyInput).Should().BeTrue();

        CommonActions.SaveButtonDisplayed().Should().BeTrue();
        CommonActions.CancelLinkDisplayed().Should().BeTrue();
    }

    [Fact]
    public void Cancel_RedirectsToDashboard()
    {
        CommonActions.ClickCancel();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(FrameworksController),
                nameof(FrameworksController.Dashboard))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Save_ValidInput_RedirectsToDashboard()
    {
        var frameworkName = TextGenerators.TextInputAddText(AddFrameworkObjects.FrameworkNameInput, 5);
        CommonActions.ClickRadioButtonWithValue(true.ToString());

        CommonActions.ClickSave();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(FrameworksController),
                nameof(FrameworksController.Dashboard))
            .Should()
            .BeTrue();

        using var context = GetEndToEndDbContext();
        var framework = context.Frameworks.First(x => x.ShortName == frameworkName);

        context.Frameworks.Remove(framework);
        context.SaveChangesAsync();
    }
}
