using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.Interoperability
{
    [Collection(nameof(AdminCollection))]
    public sealed class EditGpConnectIntegration : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new CatalogueItemId(99999, "001");
        private static readonly Guid IntegrationId = Integrations.GetIntegrations[2].Id;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(IntegrationId), IntegrationId.ToString() },
        };

        public EditGpConnectIntegration(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(InteroperabilityController),
                  nameof(InteroperabilityController.EditGpConnectIntegration),
                  Parameters)
        {
        }

        [Fact]
        public async Task EditGpConnectIntegration_CorrectlyDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.FirstAsync(ci => ci.Id == SolutionId);

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Edit a GP Connect Integration - {solution.Name}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(InteroperabilityObjects.SelectedIntegrationType).Should().BeTrue();
            CommonActions.ElementIsDisplayed(InteroperabilityObjects.SelectedProviderOrConsumer).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.AdditionalInfoTextArea).Should().BeTrue();

            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
        }

        [Fact]
        public async Task EditGpConnectIntegration_ValidSave()
        {
            var integrationType = CommonActions.SelectRandomDropDownItem(InteroperabilityObjects.SelectedIntegrationType);
            var providerConsumer = CommonActions.SelectRandomDropDownItem(InteroperabilityObjects.SelectedProviderOrConsumer);
            var additionalInfo = TextGenerators.TextInputAddText(CommonSelectors.AdditionalInfoTextArea, 1000);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(InteroperabilityController),
                nameof(InteroperabilityController.Interoperability))
                .Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId);
            var integration = solution.GetIntegrations().First(i => i.Id == IntegrationId);
            integration.Qualifier.Should().BeEquivalentTo(integrationType);
            integration.IsConsumer.Should().Be(providerConsumer == "Consumer");
            integration.AdditionalInformation.Should().BeEquivalentTo(additionalInfo);
        }

        [Fact]
        public void EditGpConnectIntegration_MissingAdditionalInfoValue()
        {
            TextGenerators.TextInputAddText(CommonSelectors.AdditionalInfoTextArea, 0);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(InteroperabilityController),
                nameof(InteroperabilityController.EditGpConnectIntegration))
                .Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
        }

        [Fact]
        public void EditGpConnectIntegration_ClickGoBack()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(InteroperabilityController),
                nameof(InteroperabilityController.Interoperability))
                .Should().BeTrue();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.First(s => s.CatalogueItemId == SolutionId);
            solution.Integrations = JsonSerializer.Serialize(Integrations.GetIntegrations);
            context.SaveChanges();
        }
    }
}
