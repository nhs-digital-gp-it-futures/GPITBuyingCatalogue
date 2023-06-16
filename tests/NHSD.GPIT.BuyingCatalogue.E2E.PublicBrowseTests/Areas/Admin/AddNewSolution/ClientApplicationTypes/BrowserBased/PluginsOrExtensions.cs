using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ApplicationTypes.BrowserBased
{
    [Collection(nameof(AdminCollection))]
    public sealed class PluginsOrExtensions : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public PluginsOrExtensions(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(BrowserBasedController),
                  nameof(BrowserBasedController.PlugInsOrExtensions),
                  Parameters)
        {
        }

        [Fact]
        public async Task PluginsOrExtensions_SavePage()
        {
            CommonActions.ClickRadioButtonWithText("Yes");

            var additionalInformation = TextGenerators.TextInputAddText(Objects.Common.CommonSelectors.AdditionalInfoTextArea, 500);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BrowserBasedController),
                nameof(BrowserBasedController.BrowserBased)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId);

            var plugins = JsonDeserializer.Deserialize<ServiceContracts.Solutions.ApplicationTypes>(solution.ApplicationType)
                ?.Plugins;

            plugins.Should().NotBeNull();
            plugins.Required.Should().BeTrue();
            plugins.AdditionalInformation.Should().Be(additionalInformation);
        }

        [Fact]
        public void PluginsOrExtensions_ErrorThrownMissingMandatory()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
        }

        public void Dispose()
        {
            ClearApplicationType(SolutionId);
        }
    }
}
