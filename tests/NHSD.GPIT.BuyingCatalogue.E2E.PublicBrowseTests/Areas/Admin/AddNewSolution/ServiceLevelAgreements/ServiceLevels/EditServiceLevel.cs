using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ServiceLevelAgreements.ServiceLevels
{
    [Collection(nameof(AdminCollection))]
    public sealed class EditServiceLevel : AuthorityTestBase, IDisposable
    {
        private const int SingleServiceLevelId = 1;
        private const int ServiceLevelId = 2;

        private static readonly CatalogueItemId SingleServiceLevelSolutionId = new(99998, "001");
        private static readonly CatalogueItemId SolutionId = new(99998, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(ServiceLevelId), ServiceLevelId.ToString() },
        };

        private static readonly Dictionary<string, string> SingleServiceLevelParameters = new()
        {
            { nameof(SolutionId), SingleServiceLevelSolutionId.ToString() },
            { nameof(ServiceLevelId), SingleServiceLevelId.ToString() },
        };

        public EditServiceLevel(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ServiceLevelAgreementsController),
                  nameof(ServiceLevelAgreementsController.EditServiceLevel),
                  Parameters)
        {
        }

        [Fact]
        public async Task EditServiceLevel_CorrectlyDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.FirstAsync(ci => ci.Id == SolutionId);

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Service levels - {solution.Name}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceLevelObjects.ServiceTypeInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceLevelObjects.ServiceLevelInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceLevelObjects.HowMeasuredInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceLevelObjects.CreditsRadioListInput).Should().BeTrue();

            CommonActions.ElementExists(ServiceLevelObjects.DeleteLink).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void EditServiceLevel_ClickGoBackLink()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditServiceLevel_ClickSave_Valid()
        {
            var supportType = TextGenerators.TextInputAddText(ServiceLevelObjects.ServiceTypeInput, 20);
            var serviceLevel = TextGenerators.TextInputAddText(ServiceLevelObjects.ServiceLevelInput, 20);
            var howMeasured = TextGenerators.TextInputAddText(ServiceLevelObjects.HowMeasuredInput, 20);

            CommonActions.ClickRadioButtonWithText("Yes");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();

            using var context = GetEndToEndDbContext();
            var serviceLevels = context.SlaServiceLevels.FirstOrDefault(sl =>
                string.Equals(sl.TypeOfService, supportType)
                && string.Equals(sl.ServiceLevel, serviceLevel)
                && string.Equals(sl.HowMeasured, howMeasured)
                && sl.ServiceCredits);

            serviceLevels.Should().NotBeNull();
        }

        [Fact]
        public void EditServiceLevel_ClickSave_NoInput()
        {
            CommonActions.ClearInputElement(ServiceLevelObjects.ServiceTypeInput);
            CommonActions.ClearInputElement(ServiceLevelObjects.ServiceLevelInput);
            CommonActions.ClearInputElement(ServiceLevelObjects.HowMeasuredInput);

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed()
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryLinksExist()
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(ServiceLevelObjects.ServiceTypeError, "Enter a type of service")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(ServiceLevelObjects.ServiceLevelError, "Enter a service level")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(ServiceLevelObjects.HowMeasuredError, "Enter how service levels are measured")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditServiceLevell_ClickSave_Duplicate()
        {
            using var context = GetEndToEndDbContext();
            var serviceLevel = context.SlaServiceLevels.First(sl => sl.SolutionId == SolutionId && sl.Id != ServiceLevelId);

            CommonActions.ElementAddValue(ServiceLevelObjects.ServiceTypeInput, serviceLevel.TypeOfService);
            CommonActions.ElementAddValue(ServiceLevelObjects.ServiceLevelInput, serviceLevel.ServiceLevel);
            CommonActions.ElementAddValue(ServiceLevelObjects.HowMeasuredInput, serviceLevel.HowMeasured);
            CommonActions.ClickRadioButtonWithText(serviceLevel.ServiceCredits.ToYesNo());

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed()
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryLinksExist()
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(ServiceLevelObjects.ServiceTypeError, "Service level with these details already exists")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(ServiceLevelObjects.ServiceLevelError, "Service level with these details already exists")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(ServiceLevelObjects.HowMeasuredError, "Service level with these details already exists")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(ServiceLevelObjects.CreditsRadioListError, "Error: Service level with these details already exists")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditServiceLevel_SingleServiceLevel_DeleteLinkHidden()
        {
            NavigateToUrl(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevel),
                SingleServiceLevelParameters);

            CommonActions.ElementExists(ServiceLevelObjects.DeleteLink).Should().BeFalse();
        }

        [Fact]
        public async Task EditServiceLevel_UnpublishedSolutionSingleServiceLevel_DeleteLinkVisible()
        {
            await using var context = GetEndToEndDbContext();
            var catalogueItem = await context.CatalogueItems.FirstAsync(c => c.Id == SingleServiceLevelSolutionId);
            catalogueItem.PublishedStatus = PublicationStatus.Unpublished;
            await context.SaveChangesAsync();

            NavigateToUrl(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevel),
                SingleServiceLevelParameters);

            CommonActions.ElementExists(ServiceLevelObjects.DeleteLink).Should().BeTrue();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var solution = context.CatalogueItems.First(c => c.Id == SingleServiceLevelSolutionId);

            if (solution.PublishedStatus == PublicationStatus.Published) return;

            solution.PublishedStatus = PublicationStatus.Published;
            context.SaveChanges();
        }
    }
}
