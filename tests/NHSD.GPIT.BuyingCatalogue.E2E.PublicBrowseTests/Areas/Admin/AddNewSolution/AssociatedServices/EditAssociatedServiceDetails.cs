using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.AssociatedServices
{
    [Collection(nameof(AdminCollection))]
    public sealed class EditAssociatedServiceDetails : AuthorityTestBase
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly CatalogueItemId AssociatedServiceId = new(99999, "S-999");

        private static readonly CatalogueItemId ExistingAssociatedServiceId = new(99999, "S-998");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AssociatedServiceId), AssociatedServiceId.ToString() },
        };

        public EditAssociatedServiceDetails(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AssociatedServicesController),
                  nameof(AssociatedServicesController.EditAssociatedServiceDetails),
                  Parameters)
        {
        }

        [Fact]
        public async Task EditAssociatedServicesDetails_UpdateValidValues()
        {
            CommonActions.ClearInputElement(CommonSelectors.Name);
            CommonActions.ClearInputElement(CommonSelectors.Description);
            CommonActions.ClearInputElement(CommonSelectors.OrderGuidance);

            var name = TextGenerators.TextInputAddText(CommonSelectors.Name, 255);
            var description = TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);
            var orderGuidance = TextGenerators.TextInputAddText(CommonSelectors.OrderGuidance, 1000);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.EditAssociatedService))
                .Should().BeTrue();

            await using var context = GetEndToEndDbContext();

            var associatedService = await context.CatalogueItems
                .Include(ci => ci.AssociatedService)
                .FirstAsync(ci => ci.Id == AssociatedServiceId);

            associatedService.Name.Should().Be(name);
            associatedService.AssociatedService.Description.Should().Be(description);
            associatedService.AssociatedService.OrderGuidance.Should().Be(orderGuidance);
        }

        [Fact]
        public void EditAssociatedServiceDetails_MissingDataThrowsError()
        {
            CommonActions.ClearInputElement(CommonSelectors.Name);
            CommonActions.ClearInputElement(CommonSelectors.Description);
            CommonActions.ClearInputElement(CommonSelectors.OrderGuidance);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.EditAssociatedServiceDetails))
                .Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
        }

        [Fact]
        public async Task EditAssociatedServiceDetails_DuplicateNameOfService()
        {
            await using var context = GetEndToEndDbContext();

            var existingAssociatedService = await context
                .CatalogueItems
                .FirstOrDefaultAsync(ci => ci.Id == ExistingAssociatedServiceId);

            var name = existingAssociatedService.Name;

            CommonActions.ElementAddValue(CommonSelectors.Name, name);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.EditAssociatedServiceDetails))
                .Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(CommonSelectors.Name, "Associated Service name already exists. Enter a different name.");
        }
    }
}
