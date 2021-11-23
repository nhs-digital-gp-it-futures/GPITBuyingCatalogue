using System;
using System.Linq;
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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.Services.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.DevelopmentPlans
{
    public static class DevelopmentPlansServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(DevelopmentPlansService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SaveDevelopmentPlans_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            string developmentPlan,
            DevelopmentPlansService service)
        {
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.SaveDevelopmentPlans(solution.CatalogueItemId, developmentPlan);

            var actual = await context.Solutions.AsQueryable().SingleAsync();

            actual.RoadMap.Should().Be(developmentPlan);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetWorkoffPlans_RetrievesFromDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution1,
            Solution solution2,
            DevelopmentPlansService service)
        {
            context.Solutions.Add(solution1);
            context.Solutions.Add(solution2);
            await context.SaveChangesAsync();

            var plans = await service.GetWorkOffPlans(solution1.CatalogueItemId);

            plans.Count.Should().Be(solution1.WorkOffPlans.Count);
            plans.ForEach(w => w.SolutionId.Should().Be(solution1.CatalogueItemId));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetWorkoffPlan_RetrievesFromDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            DevelopmentPlansService service)
        {
            context.Solutions.Add(solution);

            await context.SaveChangesAsync();

            var dbPlan = await service.GetWorkOffPlan(solution.WorkOffPlans.First().Id);

            dbPlan.Should().NotBeNull();
            dbPlan.Id.Should().Be(solution.WorkOffPlans.First().Id);
        }

        [Theory]
        [CommonAutoData]
        public static Task SaveWorkOffPlan_NullModel_ThrowsException(
            CatalogueItemId catalogueItemId,
            DevelopmentPlansService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.SaveWorkOffPlan(catalogueItemId, null));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SaveWorkoffPlan_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            SaveWorkOffPlanModel model,
            DevelopmentPlansService service)
        {
            solution.WorkOffPlans.Clear();
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.SaveWorkOffPlan(solution.CatalogueItemId, model);

            var actual = await context.WorkOffPlans.AsQueryable().SingleAsync();

            actual.SolutionId.Should().Be(solution.CatalogueItemId);
            actual.StandardId.Should().Be(model.StandardId);
            actual.Details.Should().Be(model.Details);
            actual.CompletionDate.Should().Be(model.CompletionDate);
        }

        [Theory]
        [CommonAutoData]
        public static Task UpdateWorkOffPlan_NullModel_ThrowsException(
            int planId,
            DevelopmentPlansService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateWorkOffPlan(planId, null));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateWorkOffPlan_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            SaveWorkOffPlanModel model,
            DevelopmentPlansService service)
        {
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.UpdateWorkOffPlan(solution.WorkOffPlans.First().Id, model);

            var actual = await context.WorkOffPlans.AsQueryable().SingleAsync(w => w.Id == solution.WorkOffPlans.First().Id);

            actual.SolutionId.Should().Be(solution.CatalogueItemId);
            actual.StandardId.Should().Be(model.StandardId);
            actual.Details.Should().Be(model.Details);
            actual.CompletionDate.Should().Be(model.CompletionDate);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeleteWorkOffPlan_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            DevelopmentPlansService service)
        {
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            var itemToDelete = solution.WorkOffPlans.First().Id;

            await service.DeleteWorkOffPlan(itemToDelete);

            var dbSolution = await context.Solutions.Include(s => s.WorkOffPlans).SingleAsync(s => s.CatalogueItemId == solution.CatalogueItemId);

            dbSolution.WorkOffPlans.Should().NotBeNullOrEmpty();
            dbSolution.WorkOffPlans.Any(w => w.Id == itemToDelete).Should().BeFalse();
        }
    }
}
