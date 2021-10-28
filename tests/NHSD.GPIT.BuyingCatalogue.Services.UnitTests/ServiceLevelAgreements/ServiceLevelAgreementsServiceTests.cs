using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.ServiceLevelAgreements
{
    public static class ServiceLevelAgreementsServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ServiceLevelAgreementsService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddServiceLevel_Valid(
            [Frozen] BuyingCatalogueDbContext context,
            ServiceLevelAgreementsService service,
            Solution solution)
        {
            var model = new AddSlaModel
            {
                Solution = solution.CatalogueItem,
                SlaLevel = SlaType.Type1,
            };
            solution.ServiceLevelAgreement = null;
            context.CatalogueItems.Add(solution.CatalogueItem);
            await context.SaveChangesAsync();

            await service.AddServiceLevelAsync(model);

            var sla = await context.ServiceLevelAgreements.SingleAsync(s => s.SolutionId == solution.CatalogueItemId);
            sla.SlaType.Should().Be(model.SlaLevel);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetAllServiceLevelAgreementsForSolution_Valid(
            [Frozen] BuyingCatalogueDbContext context,
            ServiceLevelAgreementsService service,
            Solution solution)
        {
            context.CatalogueItems.Add(solution.CatalogueItem);
            await context.SaveChangesAsync();

            var result = await service.GetAllServiceLevelAgreementsForSolution(solution.CatalogueItemId);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(solution.ServiceLevelAgreement);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetAllServiceLevelAgreementsForSolution_InvalidSolutionId(
            ServiceLevelAgreementsService service,
            CatalogueItemId itemId)
        {
            var result = await service.GetAllServiceLevelAgreementsForSolution(itemId);

            result.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateServiceLevelTypeAsync(
            [Frozen] BuyingCatalogueDbContext context,
            ServiceLevelAgreementsService service,
            Solution solution)
        {
            solution.ServiceLevelAgreement.SlaType = SlaType.Type1;
            context.CatalogueItems.Add(solution.CatalogueItem);
            await context.SaveChangesAsync();

            await service.UpdateServiceLevelTypeAsync(solution.CatalogueItem, SlaType.Type2);

            var sla = await context.ServiceLevelAgreements.SingleAsync(s => s.SolutionId == solution.CatalogueItemId);

            sla.SlaType.Should().Be(SlaType.Type2);
        }
    }
}
