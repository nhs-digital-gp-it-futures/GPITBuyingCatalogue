using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.ImportServiceRecipients;

[Collection(nameof(OrderingCollection))]
public class ImportServiceRecipients : BuyerTestBase
{
    private const string InternalOrgId = "CG-03F";
    private const int OrderId = 90005;
    private static readonly CallOffId CallOffId = new(OrderId, 1);
    private static readonly CatalogueItemId CatalogueItemId = new(99999, "001");

    private static readonly Dictionary<string, string> Parameters = new()
    {
        { nameof(InternalOrgId), InternalOrgId },
        { nameof(CallOffId), $"{CallOffId}" },
        { nameof(CatalogueItemId), $"{CatalogueItemId}" },
    };

    public ImportServiceRecipients(LocalWebApplicationFactory factory)
    : base(factory, typeof(ImportServiceRecipientsController), nameof(ImportServiceRecipientsController.Index), Parameters)
    {
    }

    [Fact]
    public void AllSectionsDisplayed()
    {
        using var context = GetEndToEndDbContext();
        var catalogueItem = context.CatalogueItems.Find(CatalogueItemId);
        var catalogueItemName = catalogueItem!.Name;

        CommonActions.PageTitle().Should().BeEquivalentTo($"Upload Service Recipients-{catalogueItemName}".FormatForComparison());
        CommonActions.LedeText().Should().BeEquivalentTo("Create a CSV with your Service Recipients in the first column and their ODS codes in the second column.".FormatForComparison());
        CommonActions.GoBackLinkDisplayed().Should().BeTrue();

        CommonActions.ElementIsDisplayed(ImportServiceRecipientObjects.FileInput).Should().BeTrue();
        CommonActions.SaveButtonDisplayed().Should().BeTrue();
    }

    [Theory]
    [InlineData(ServiceRecipientImportMode.Edit, nameof(ServiceRecipientsController.EditServiceRecipients))]
    [InlineData(ServiceRecipientImportMode.Add, nameof(ServiceRecipientsController.SelectServiceRecipients))]
    public void ClickGoBackLink_NavigatesToCorrectPage(
        ServiceRecipientImportMode importMode,
        string expectedAction)
    {
        var queryParameters = new Dictionary<string, string> { { nameof(importMode), importMode.ToString() }, };

        NavigateToUrl(
            typeof(ImportServiceRecipientsController),
            nameof(ImportServiceRecipientsController.Index),
            Parameters,
            queryParameters);

        CommonActions.ClickGoBackLink();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                expectedAction)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void ClickSave_NoFile_SetsModelError()
    {
        CommonActions.ClickSave();

        CommonActions.PageLoadedCorrectGetIndex(
            typeof(ImportServiceRecipientsController),
            nameof(ImportServiceRecipientsController.Index));

        CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
        CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
    }
}
