using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
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
        public static Task AddServiceLevel_NullModel(
            ServiceLevelAgreementsService service)
                => Assert.ThrowsAsync<ArgumentNullException>("model", () => service.AddServiceLevelAgreement(null));

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

            await service.AddServiceLevelAgreement(model);

            var sla = context.ServiceLevelAgreements.Single(s => s.SolutionId == solution.CatalogueItemId);
            sla.SlaType.Should().Be(model.SlaLevel);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetServiceLevelAgreementForSolution_Valid(
            [Frozen] BuyingCatalogueDbContext context,
            ServiceLevelAgreementsService service,
            Solution solution)
        {
            context.CatalogueItems.Add(solution.CatalogueItem);
            await context.SaveChangesAsync();

            var result = await service.GetServiceLevelAgreementForSolution(solution.CatalogueItemId);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(solution.ServiceLevelAgreement);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetServiceLevelAgreementForSolution_InvalidSolutionId(
            ServiceLevelAgreementsService service,
            CatalogueItemId itemId)
        {
            var result = await service.GetServiceLevelAgreementForSolution(itemId);

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

            var sla = context.ServiceLevelAgreements.Single(s => s.SolutionId == solution.CatalogueItemId);

            sla.SlaType.Should().Be(SlaType.Type2);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetServiceAvailabilityTimes(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] BuyingCatalogueDbContext context,
            ServiceLevelAgreementsService service)
        {
            solution.ServiceLevelAgreement.ServiceHours.Add(serviceAvailabilityTimes);
            context.CatalogueItems.Add(solution.CatalogueItem);
            await context.SaveChangesAsync();

            var result = await service.GetServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id);

            result.Should().BeEquivalentTo(serviceAvailabilityTimes);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static Task SaveServiceAvailabilityTimes_NullSolution(
            ServiceAvailabilityTimesModel model,
            ServiceLevelAgreementsService service)
                => Assert.ThrowsAsync<ArgumentNullException>("solution", () => service.SaveServiceAvailabilityTimes(null, model));

        [Theory]
        [InMemoryDbAutoData]
        public static Task SaveServiceAvailabilityTimes_NullServiceAvailabilityTimesModel(
            Solution solution,
            ServiceLevelAgreementsService service)
                => Assert.ThrowsAsync<ArgumentNullException>("model", () => service.SaveServiceAvailabilityTimes(solution.CatalogueItem, null));

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SaveServiceAvailabilityTimes_ValidRequest(
            Solution solution,
            ServiceAvailabilityTimesModel model,
            [Frozen] BuyingCatalogueDbContext context,
            ServiceLevelAgreementsService service)
        {
            await service.SaveServiceAvailabilityTimes(solution.CatalogueItem, model);

            var serviceAvailabilityTimes = context.ServiceAvailabilityTimes.Single(s => s.SolutionId == solution.CatalogueItemId && s.LastUpdatedBy == model.UserId);

            serviceAvailabilityTimes.Should().NotBeNull();
            serviceAvailabilityTimes.ApplicableDays.Should().Be(model.ApplicableDays);
            serviceAvailabilityTimes.Category.Should().Be(model.SupportType);
            serviceAvailabilityTimes.TimeFrom.Should().Be(model.From);
            serviceAvailabilityTimes.TimeUntil.Should().Be(model.Until);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static Task UpdateServiceAvailabilityTimes_NullSolution(
            ServiceAvailabilityTimesModel model,
            int serviceAvailabilityTimesId,
            ServiceLevelAgreementsService service)
                => Assert.ThrowsAsync<ArgumentNullException>("solution", () => service.UpdateServiceAvailabilityTimes(null, serviceAvailabilityTimesId, model));

        [Theory]
        [InMemoryDbAutoData]
        public static Task UpdateServiceAvailabilityTimes_NullServiceAvailabilityTimesModel(
            Solution solution,
            int serviceAvailabilityTimesId,
            ServiceLevelAgreementsService service)
                => Assert.ThrowsAsync<ArgumentNullException>("model", () => service.UpdateServiceAvailabilityTimes(solution.CatalogueItem, serviceAvailabilityTimesId, null));

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateServiceAvailabilityTimes_ValidRequest(
            Solution solution,
            ServiceAvailabilityTimesModel model,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] BuyingCatalogueDbContext context,
            ServiceLevelAgreementsService service)
        {
            serviceAvailabilityTimes.SolutionId = solution.CatalogueItemId;
            context.CatalogueItems.Add(solution.CatalogueItem);
            context.ServiceAvailabilityTimes.Add(serviceAvailabilityTimes);

            await context.SaveChangesAsync();

            await service.UpdateServiceAvailabilityTimes(solution.CatalogueItem, serviceAvailabilityTimes.Id, model);

            var actualServiceAvailabilityTimes = context.ServiceAvailabilityTimes.Single(s => s.Id == serviceAvailabilityTimes.Id);

            actualServiceAvailabilityTimes.Should().NotBeNull();
            actualServiceAvailabilityTimes.ApplicableDays.Should().Be(model.ApplicableDays);
            actualServiceAvailabilityTimes.Category.Should().Be(model.SupportType);
            actualServiceAvailabilityTimes.TimeFrom.Should().Be(model.From);
            actualServiceAvailabilityTimes.TimeUntil.Should().Be(model.Until);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeleteServiceAvailabilityTimes_Valid(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] BuyingCatalogueDbContext context,
            ServiceLevelAgreementsService service)
        {
            serviceAvailabilityTimes.SolutionId = solution.CatalogueItemId;
            context.CatalogueItems.Add(solution.CatalogueItem);
            context.ServiceAvailabilityTimes.Add(serviceAvailabilityTimes);

            await context.SaveChangesAsync();

            await service.DeleteServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id);

            context.ServiceAvailabilityTimes.Count(s => s.Id == serviceAvailabilityTimes.Id).Should().Be(0);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetCountOfServiceAvailabilityTimes_WithExcludedIds(
            Solution solution,
            List<ServiceAvailabilityTimes> serviceAvailabilityTimes,
            [Frozen] BuyingCatalogueDbContext context,
            ServiceLevelAgreementsService service)
        {
            serviceAvailabilityTimes.ForEach(s => s.SolutionId = solution.CatalogueItemId);
            context.CatalogueItems.Add(solution.CatalogueItem);
            context.ServiceAvailabilityTimes.AddRange(serviceAvailabilityTimes);

            await context.SaveChangesAsync();

            var idToExclude = serviceAvailabilityTimes.First().Id;
            var expectedCount = serviceAvailabilityTimes.Count - 1;

            var actualCount = await service.GetCountOfServiceAvailabilityTimes(solution.CatalogueItemId, idToExclude);

            actualCount.Should().Be(expectedCount);
        }
    }
}
