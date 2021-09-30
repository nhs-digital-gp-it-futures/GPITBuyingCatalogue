using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    public sealed class GPConnectIntegration : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            {
                nameof(SolutionId),
                SolutionId.ToString()
            },
        };

        public GPConnectIntegration(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(InteroperabilityController),
                  nameof(InteroperabilityController.AddGpConnectIntegration),
                  Parameters)
        {
        }

        [Theory]
        [InlineData("HTML View", "Provider")]
        [InlineData("Appointment Booking", "Provider")]
        [InlineData("Structured Record", "Provider")]
        [InlineData("HTML View", "Consumer")]
        [InlineData("Appointment Booking", "Consumer")]
        [InlineData("Structured Record", "Consumer")]
        public async Task GPConnectIntegration_SavePage(string integrationType, string providerOrConsumer)
        {
            CommonActions.SelectDropDownItemByValue(Objects.Admin.EditSolution.InteroperabilityObjects.IntegrationType, integrationType);

            CommonActions.SelectDropDownItemByValue(Objects.Admin.EditSolution.InteroperabilityObjects.ProviderOrConsumer, providerOrConsumer);

            var additionalInformation = TextGenerators.TextInputAddText(Objects.Common.CommonSelectors.AdditionalInfoTextArea, 1000);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(typeof(InteroperabilityController), nameof(InteroperabilityController.Interoperability)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();

            var integrations = (await context.Solutions.SingleAsync(s => s.CatalogueItemId == SolutionId)).Integrations;

            integrations.Should().NotBeNullOrWhiteSpace();

            var integrationsList = JsonDeserializer.Deserialize<List<Integration>>(integrations);

            integrationsList.Should().HaveCount(1);

            var firstIntegration = integrationsList.First();

            firstIntegration.IntegrationType.Should().Be("GP Connect");
            firstIntegration.Qualifier.Should().BeEquivalentTo(integrationType);
            firstIntegration.IsConsumer.Should().Be(providerOrConsumer == "Consumer");
            firstIntegration.AdditionalInformation.Should().Be(additionalInformation);
        }

        [Fact]
        public void GPConnectIntegration_ClickGoBackLink()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(typeof(InteroperabilityController), nameof(InteroperabilityController.Interoperability)).Should().BeTrue();
        }

        [Fact]
        public void GPConnectIntegration_MandatoryDataMissingThrowsError()
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
