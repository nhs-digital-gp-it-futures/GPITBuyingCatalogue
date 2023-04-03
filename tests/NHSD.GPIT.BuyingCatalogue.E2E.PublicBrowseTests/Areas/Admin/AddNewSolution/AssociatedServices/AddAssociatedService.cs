using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    [Collection(nameof(AdminCollection))]
    public sealed class AddAssociatedService : AuthorityTestBase
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AddAssociatedService(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AssociatedServicesController),
                  nameof(AssociatedServicesController.AddAssociatedService),
                  Parameters)
        {
        }

        [Fact]
        public async Task AddAssociatedService_CorrectlyDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var supplierName = (await context.CatalogueItems.Include(ci => ci.Supplier).FirstAsync(s => s.Id == SolutionId)).Supplier.Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Associated Service details - {supplierName}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Name).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Description).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.OrderGuidance).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AddAssociatedService_ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AssociatedServicesController),
                    nameof(AssociatedServicesController.AssociatedServices))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task AddAssociatedService_CompleteAssociatedService()
        {
            var name = TextGenerators.TextInputAddText(CommonSelectors.Name, 255);
            var description = TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);
            var orderGuidance = TextGenerators.TextInputAddText(CommonSelectors.OrderGuidance, 1000);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.EditAssociatedService))
                .Should()
                .BeTrue();

            var id = Driver.Url.Split('/')[^2];

            await using var context = GetEndToEndDbContext();

            var associatedServiceItem = await context
                .CatalogueItems
                .Include(ci => ci.AssociatedService)
                .FirstAsync(ci => ci.Id == CatalogueItemId.ParseExact(id));

            associatedServiceItem.Name.Should().Be(name);

            associatedServiceItem
                .AssociatedService
                .Description
                .Should().Be(description);

            associatedServiceItem
                .AssociatedService
                .OrderGuidance
                .Should().Be(orderGuidance);
        }

        [Fact]
        public async Task AddAssociatedService_NameAlreadyExists()
        {
            await using var context = GetEndToEndDbContext();

            var associatedService = await context
                .CatalogueItems
                .Where(ci => ci.CatalogueItemType == CatalogueItemType.AssociatedService)
                .FirstAsync(ci => ci.SupplierId == SolutionId.SupplierId);

            CommonActions.ElementAddValue(CommonSelectors.Name, associatedService.Name);

            TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);
            TextGenerators.TextInputAddText(CommonSelectors.OrderGuidance, 1000);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.AddAssociatedService))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(CommonSelectors.Name, "Associated Service name already exists. Enter a different name.");
        }

        [Fact]
        public void AddAssociatedService_MandatoryDataMissing()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.AddAssociatedService))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
        }
    }
}
