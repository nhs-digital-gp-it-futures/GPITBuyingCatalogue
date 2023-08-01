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
public class ValidateImportOrganisationNames : BuyerTestBase
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

    public ValidateImportOrganisationNames(LocalWebApplicationFactory factory)
        : base(
            factory,
            typeof(ImportServiceRecipientsController),
            nameof(ImportServiceRecipientsController.ValidateNames),
            Parameters)
    {
    }

    [Fact]
    public void AllSectionsDisplayed()
    {
        using var context = GetEndToEndDbContext();
        var catalogueItem = context.CatalogueItems.Find(CatalogueItemId);
        var catalogueItemName = catalogueItem!.Name;

        CommonActions.PageTitle().Should().BeEquivalentTo($"There is a problem with your Service Recipients - {catalogueItemName}".FormatForComparison());
        CommonActions.LedeText()
            .Should()
            .BeEquivalentTo(
                "There are discrepancies between the Service Recipient names in your Csv file and what we have on record."
                    .FormatForComparison());

        CommonActions.ElementIsDisplayed(ImportServiceRecipientObjects.OrganisationNameTable).Should().BeTrue();

        CommonActions.GoBackLinkDisplayed().Should().BeTrue();
        CommonActions.CancelLinkDisplayed().Should().BeTrue();
    }

    [Fact]
    public void ClickGoBackLink_NavigatesToCorrectPage()
    {
        CommonActions.ClickGoBackLink();

        CommonActions.PageLoadedCorrectGetIndex(
            typeof(ImportServiceRecipientsController),
            nameof(ImportServiceRecipientsController.ValidateOds));
    }

    [Theory]
    [InlineData(ServiceRecipientImportMode.Edit, nameof(ServiceRecipientsController.EditServiceRecipients))]
    [InlineData(ServiceRecipientImportMode.Add, nameof(ServiceRecipientsController.SelectServiceRecipients))]
    public void ClickCancel_NavigatesToCorrectPage(
        ServiceRecipientImportMode importMode,
        string expectedAction)
    {
        var queryParameters = new Dictionary<string, string> { { nameof(importMode), importMode.ToString() } };

        NavigateToUrl(
            typeof(ImportServiceRecipientsController),
            nameof(ImportServiceRecipientsController.ValidateNames),
            Parameters,
            queryParameters);

        CommonActions.ClickCancel();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                expectedAction)
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(ServiceRecipientImportMode.Edit, nameof(ServiceRecipientsController.EditServiceRecipients))]
    [InlineData(ServiceRecipientImportMode.Add, nameof(ServiceRecipientsController.SelectServiceRecipients))]
    public void ClickSubmit_NavigatesTyoCorrectPage(
        ServiceRecipientImportMode importMode,
        string expectedAction)
    {
        var queryParameters = new Dictionary<string, string> { { nameof(importMode), importMode.ToString() } };

        NavigateToUrl(
            typeof(ImportServiceRecipientsController),
            nameof(ImportServiceRecipientsController.ValidateNames),
            Parameters,
            queryParameters);
        CommonActions.ClickSave();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                expectedAction)
            .Should()
            .BeTrue();
    }
}
