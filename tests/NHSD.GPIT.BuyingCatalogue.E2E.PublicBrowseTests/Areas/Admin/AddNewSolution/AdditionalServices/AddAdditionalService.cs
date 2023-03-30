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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.AdditionalServices
{
    [Collection(nameof(AdminCollection))]
    public sealed class AddAdditionalService : AuthorityTestBase
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AddAdditionalService(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.AddAdditionalService),
                  Parameters)
        {
        }

        [Fact]
        public async Task AddAdditionalService_CorrectlyDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.FirstAsync(s => s.Id == SolutionId)).Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Additional Service details - {solutionName}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Name).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Description).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AddAdditionalService_ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.Index))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task AddAdditionalService_CompleteAdditionalService()
        {
            var name = TextGenerators.TextInputAddText(CommonSelectors.Name, 255);
            var description = TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService))
                .Should()
                .BeTrue();

            var id = Driver.Url.Split('/')[^2];

            await using var context = GetEndToEndDbContext();

            var additionalServiceItem = await context
                .CatalogueItems
                .Include(ci => ci.AdditionalService)
                .FirstAsync(ci => ci.Id == CatalogueItemId.ParseExact(id));

            additionalServiceItem.Name.Should().Be(name);

            additionalServiceItem
                .AdditionalService
                .FullDescription
                .Should().Be(description);
        }

        [Fact]
        public async Task AddAdditionalService_NameAlreadyExists()
        {
            await using var context = GetEndToEndDbContext();

            var additionalService = await context
                .CatalogueItems
                .Where(ci => ci.CatalogueItemType == CatalogueItemType.AdditionalService)
                .FirstAsync(ci => ci.SupplierId == SolutionId.SupplierId);

            CommonActions.ElementAddValue(CommonSelectors.Name, additionalService.Name);

            TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.AddAdditionalService))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(CommonSelectors.Name, "Additional Service name already exists. Enter a different name.");
        }

        [Fact]
        public void AddAdditionalService_MandatoryDataMissing()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.AddAdditionalService))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
        }
    }
}
