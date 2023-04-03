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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ServiceLevelAgreements.ServiceLevels
{
    [Collection(nameof(AdminCollection))]
    public sealed class AddServiceLevel : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AddServiceLevel(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ServiceLevelAgreementsController),
                  nameof(ServiceLevelAgreementsController.AddServiceLevel),
                  Parameters)
        {
        }

        [Fact]
        public async Task AddServiceLevel_CorrectlyDisplayed()
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

            CommonActions.ElementExists(ServiceLevelObjects.DeleteLink).Should().BeFalse();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AddServiceLevel_ClickGoBackLink()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddServiceLevel_ClickSave_Valid()
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
        public void AddServiceLevel_ClickSave_NoInput()
        {
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

            CommonActions.ElementShowingCorrectErrorMessage(ServiceLevelObjects.CreditsRadioListError, "Error: Select if service credits are applied")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddServiceLevel_ClickSave_Duplicate()
        {
            using var context = GetEndToEndDbContext();
            var serviceLevel = context.SlaServiceLevels.First(sl => sl.SolutionId == SolutionId);

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

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();

            var serviceLevels = context.SlaServiceLevels.OrderByDescending(x => x.LastUpdated)
                .Where(x => x.SolutionId == SolutionId)
                .ToList();

            if (serviceLevels.Count <= 1)
                return;

            context.SlaServiceLevels.Remove(serviceLevels.First());
            context.SaveChanges();
        }
    }
}
