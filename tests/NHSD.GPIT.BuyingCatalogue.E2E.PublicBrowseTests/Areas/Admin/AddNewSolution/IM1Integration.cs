using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    public sealed class IM1Integration : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            {
                nameof(SolutionId),
                SolutionId.ToString()
            },
        };

        public IM1Integration(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(InteroperabilityController),
                  nameof(InteroperabilityController.AddIm1Integration),
                  Parameters)
        {
        }

        [Theory]
        [InlineData("Bulk", "Provider")]
        [InlineData("Transactional", "Provider")]
        [InlineData("Patient Facing", "Provider")]
        [InlineData("Bulk", "Consumer")]
        [InlineData("Transactional", "Consumer")]
        [InlineData("Patient Facing", "Consumer")]
        public async Task IM1Integration_SavePage(string integrationType, string providerOrConsumer)
        {
            CommonActions.SelectDropDownItemByValue(Objects.Admin.EditSolution.InteroperabilityObjects.IntegrationType, integrationType);

            CommonActions.SelectDropDownItemByValue(Objects.Admin.EditSolution.InteroperabilityObjects.ProviderOrConsumer, providerOrConsumer);

            var integratesWith = TextGenerators.TextInputAddText(Objects.Admin.EditSolution.InteroperabilityObjects.IntegratesWith, 100);

            var description = TextGenerators.TextInputAddText(Objects.Common.CommonSelectors.Description, 1000);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(typeof(InteroperabilityController), nameof(InteroperabilityController.Interoperability)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();

            var integrations = (await context.Solutions.SingleAsync(s => s.CatalogueItemId == SolutionId)).Integrations;

            integrations.Should().NotBeNullOrWhiteSpace();

            var integrationsList = JsonSerializer.Deserialize<List<Integration>>(integrations);

            integrationsList.Should().HaveCount(1);

            var firstIntegration = integrationsList.First();

            firstIntegration.IntegrationType.Should().Be("IM1");
            firstIntegration.Qualifier.Should().BeEquivalentTo(integrationType);
            firstIntegration.IsConsumer.Should().Be(providerOrConsumer == "Consumer");
            firstIntegration.IntegratesWith.Should().Be(integratesWith);
            firstIntegration.Description.Should().Be(description);
        }

        [Fact]
        public void IM1Integration_ClickGoBackLink()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(typeof(InteroperabilityController), nameof(InteroperabilityController.Interoperability)).Should().BeTrue();
        }

        [Fact]
        public void IM1Integration_MandatoryDataMissingThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();

            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.CatalogueItemId == SolutionId);
            solution.Integrations = null;
            context.SaveChanges();
        }
    }
}
