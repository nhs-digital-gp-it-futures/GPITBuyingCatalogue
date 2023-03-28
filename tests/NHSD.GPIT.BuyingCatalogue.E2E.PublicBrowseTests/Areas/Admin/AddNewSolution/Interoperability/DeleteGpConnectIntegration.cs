using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
    public sealed class DeleteGpConnectIntegration : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new CatalogueItemId(99999, "001");
        private static readonly Guid IntegrationId = Integrations.GetIntegrations[2].Id;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(IntegrationId), IntegrationId.ToString() },
        };

        public DeleteGpConnectIntegration(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(InteroperabilityController),
                  nameof(InteroperabilityController.DeleteGpConnectIntegration),
                  Parameters)
        {
        }

        [Fact]
        public async Task DeleteGpConnectIntegration_DisplayedCorrectly()
        {
            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.FirstAsync(s => s.Id == SolutionId);

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Delete GP Connect Integration? - {solution.Name}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
        }

        [Fact]
        public async Task DeleteGpConnectIntegration_DeletesFromDb()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(InteroperabilityController),
                nameof(InteroperabilityController.Interoperability))
                .Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId);
            var integration = solution.GetIntegrations().FirstOrDefault(i => i.Id == IntegrationId);
            integration.Should().BeNull();
        }

        [Fact]
        public void DeleteGpConnectIntegration_ClickGoBack()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(InteroperabilityController),
                nameof(InteroperabilityController.EditIm1Integration))
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
