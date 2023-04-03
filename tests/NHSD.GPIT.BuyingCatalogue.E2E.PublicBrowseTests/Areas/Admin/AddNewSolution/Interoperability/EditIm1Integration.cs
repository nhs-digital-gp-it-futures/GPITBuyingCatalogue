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
    public sealed class EditIm1Integration : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new CatalogueItemId(99999, "001");
        private static readonly Guid IntegrationId = Integrations.GetIntegrations[0].Id;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(IntegrationId), IntegrationId.ToString() },
        };

        public EditIm1Integration(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(InteroperabilityController),
                  nameof(InteroperabilityController.EditIm1Integration),
                  Parameters)
        {
        }

        [Fact]
        public async Task EditIm1Integration_CorrectlyDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.FirstAsync(ci => ci.Id == SolutionId);

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Edit an IM1 Integration - {solution.Name}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(InteroperabilityObjects.SelectedIntegrationType).Should().BeTrue();
            CommonActions.ElementIsDisplayed(InteroperabilityObjects.SelectedProviderOrConsumer).Should().BeTrue();
            CommonActions.ElementIsDisplayed(InteroperabilityObjects.IntegratesWith).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Description).Should().BeTrue();

            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
        }

        [Fact]
        public async Task EditIm1Integration_ValidSave()
        {
            var integrationType = CommonActions.SelectRandomDropDownItem(InteroperabilityObjects.SelectedIntegrationType);
            var providerConsumer = CommonActions.SelectRandomDropDownItem(InteroperabilityObjects.SelectedProviderOrConsumer);
            var integratesWith = TextGenerators.TextInputAddText(InteroperabilityObjects.IntegratesWith, 100);
            var description = TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);

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
            integration.IntegratesWith.Should().BeEquivalentTo(integratesWith);
            integration.Description.Should().BeEquivalentTo(description);
        }

        [Fact]
        public void EditIm1Integration_MissingIntegratesWithValue()
        {
            TextGenerators.TextInputAddText(InteroperabilityObjects.IntegratesWith, 0);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(InteroperabilityController),
                nameof(InteroperabilityController.EditIm1Integration))
                .Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
        }

        [Fact]
        public void EditIm1Integration_ClickGoBack()
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
