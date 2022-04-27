using System;
using System.Collections.Generic;
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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.Services.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.AssociatedServices
{
    public static class AssociatedServicesServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AssociatedServicesService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task RelateAssociatedServicesToSolution_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            List<AssociatedService> associatedServices,
            AssociatedServicesService service)
        {
            var expected = new List<SupplierServiceAssociation>();
            expected.AddRange(associatedServices.Select(s => new SupplierServiceAssociation
                {
                    CatalogueItemId = solution.CatalogueItem.Id,
                    AssociatedServiceId = s.CatalogueItem.Id,
                }));

            context.Solutions.Add(solution);
            context.AssociatedServices.AddRange(associatedServices);
            await context.SaveChangesAsync();

            await service.RelateAssociatedServicesToSolution(solution.CatalogueItemId, associatedServices.Select(a => a.CatalogueItem.Id));

            var updatedSolution = await context.Solutions
                .Include(s => s.CatalogueItem)
                .ThenInclude(ci => ci.SupplierServiceAssociations)
                .AsAsyncEnumerable()
                .SingleAsync(s => s.CatalogueItemId == solution.CatalogueItemId);

            updatedSolution.CatalogueItem.SupplierServiceAssociations.Should()
                .BeEquivalentTo(expected, config => config
                    .Excluding(ctx => ctx.AssociatedService)
                    .Excluding(ctx => ctx.CatalogueItem)
                    .Excluding(ctx => ctx.LastUpdated)
                    .Excluding(ctx => ctx.LastUpdatedBy));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static Task AddAssociatedService_NullCatalogueItem_ThrowsException(
        AssociatedServicesDetailsModel model,
        AssociatedServicesService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.AddAssociatedService(null, model));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static Task AddAssociatedService_NullModel_ThrowsException(
            CatalogueItem item,
            AssociatedServicesService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.AddAssociatedService(item, null));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddAssociatedService_UpdatesDatabase(
           [Frozen] BuyingCatalogueDbContext context,
           CatalogueItem solution,
           AssociatedServicesDetailsModel model,
           AssociatedServicesService service)
        {
            solution.Id = new CatalogueItemId(solution.Id.SupplierId, solution.Id.ItemId[4..]);
            context.CatalogueItems.Add(solution);
            await context.SaveChangesAsync();

            var result = await service.AddAssociatedService(solution, model);

            var dbSolution = await context.CatalogueItems.Include(c => c.AssociatedService).SingleAsync(c => c.Id == result);

            dbSolution.Should().NotBeNull();
            dbSolution.Name.Should().Be(model.Name);
            dbSolution.CatalogueItemType.Should().Be(CatalogueItemType.AssociatedService);
            dbSolution.SupplierId.Should().Be(solution.SupplierId);
            dbSolution.PublishedStatus.Should().Be(PublicationStatus.Draft);
            dbSolution.AssociatedService.Should().NotBeNull();
            dbSolution.AssociatedService.Description.Should().Be(model.Description);
            dbSolution.AssociatedService.OrderGuidance.Should().Be(model.OrderGuidance);
        }

        [Theory]
        [CommonAutoData]
        public static Task EditDetails_NullModel_ThrowsException(
            CatalogueItemId additionalServiceId,
            AssociatedServicesService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.EditDetails(additionalServiceId, null));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task EditDetails_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem solution,
            CatalogueItem associatedService,
            AssociatedServicesDetailsModel model,
            AssociatedServicesService service)
        {
            context.CatalogueItems.Add(solution);
            associatedService.CatalogueItemType = CatalogueItemType.AssociatedService;
            associatedService.AssociatedService = new AssociatedService { CatalogueItemId = solution.Id };
            context.CatalogueItems.Add(associatedService);
            await context.SaveChangesAsync();

            await service.EditDetails(associatedService.Id, model);

            var dbSolution = await context.CatalogueItems.Include(c => c.AssociatedService).SingleAsync(c => c.Id == associatedService.Id);

            dbSolution.Should().NotBeNull();
            dbSolution.Name.Should().Be(model.Name);
            dbSolution.AssociatedService.Description.Should().Be(model.Description);
            dbSolution.AssociatedService.OrderGuidance.Should().Be(model.OrderGuidance);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetAllSolutionsForAssociatedService_Returns_RelatedSolutions(
            List<Solution> solutions,
            AssociatedService associatedService,
            [Frozen] BuyingCatalogueDbContext context,
            AssociatedServicesService service)
        {
            solutions.ForEach(s => s.CatalogueItem.SupplierServiceAssociations = new HashSet<SupplierServiceAssociation> { new(s.CatalogueItemId, associatedService.CatalogueItemId) });

            context.Solutions.AddRange(solutions);
            context.AssociatedServices.Add(associatedService);

            context.SaveChanges();

            var relatedSolutions = await service.GetAllSolutionsForAssociatedService(associatedService.CatalogueItemId);

            relatedSolutions.Should().NotBeEmpty();
            relatedSolutions.Count.Should().Be(solutions.Count);
        }
    }
}
