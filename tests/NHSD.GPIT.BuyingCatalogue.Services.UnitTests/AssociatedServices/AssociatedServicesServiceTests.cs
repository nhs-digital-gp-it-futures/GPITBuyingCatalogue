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
                .FirstAsync(s => s.CatalogueItemId == solution.CatalogueItemId);

            updatedSolution.CatalogueItem.SupplierServiceAssociations.Should()
                .BeEquivalentTo(expected, config => config
                    .Excluding(ctx => ctx.AssociatedService)
                    .Excluding(ctx => ctx.CatalogueItem)
                    .Excluding(ctx => ctx.LastUpdated)
                    .Excluding(ctx => ctx.LastUpdatedBy));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task RemoveServiceFromSolution_Removes(
            Solution solution,
            AssociatedService associatedService,
            [Frozen] BuyingCatalogueDbContext dbContext,
            AssociatedServicesService service)
        {
            dbContext.Solutions.Add(solution);
            dbContext.AssociatedServices.Add(associatedService);
            dbContext.SupplierServiceAssociations.Add(
                new SupplierServiceAssociation(solution.CatalogueItemId, associatedService.CatalogueItemId));

            await dbContext.SaveChangesAsync();

            dbContext.ChangeTracker.Clear();

            await service.RemoveServiceFromSolution(solution.CatalogueItemId, associatedService.CatalogueItemId);

            dbContext.SupplierServiceAssociations.AsNoTracking()
                .FirstOrDefault(
                    x => x.CatalogueItemId == solution.CatalogueItemId
                        && x.AssociatedServiceId == associatedService.CatalogueItemId)
                .Should()
                .BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task RemoveServiceFromSolution_InvalidKeys_DoesNotRemove(
            Solution solution,
            Solution secondSolution,
            AssociatedService associatedService,
            [Frozen] BuyingCatalogueDbContext dbContext,
            AssociatedServicesService service)
        {
            dbContext.Solutions.Add(solution);
            dbContext.AssociatedServices.Add(associatedService);
            dbContext.SupplierServiceAssociations.Add(
                new SupplierServiceAssociation(solution.CatalogueItemId, associatedService.CatalogueItemId));

            await dbContext.SaveChangesAsync();

            dbContext.ChangeTracker.Clear();

            await service.RemoveServiceFromSolution(secondSolution.CatalogueItemId, associatedService.CatalogueItemId);

            dbContext.SupplierServiceAssociations.AsNoTracking()
                .FirstOrDefault(
                    x => x.CatalogueItemId == solution.CatalogueItemId
                        && x.AssociatedServiceId == associatedService.CatalogueItemId)
                .Should()
                .NotBeNull();
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
            context.ChangeTracker.Clear();

            var result = await service.AddAssociatedService(solution, model);

            var dbSolution = await context.CatalogueItems.Include(c => c.AssociatedService).FirstAsync(c => c.Id == result);

            dbSolution.Should().NotBeNull();
            dbSolution.Name.Should().Be(model.Name);
            dbSolution.CatalogueItemType.Should().Be(CatalogueItemType.AssociatedService);
            dbSolution.SupplierId.Should().Be(solution.SupplierId);
            dbSolution.PublishedStatus.Should().Be(PublicationStatus.Draft);
            dbSolution.AssociatedService.Should().NotBeNull();
            dbSolution.AssociatedService.Description.Should().Be(model.Description);
            dbSolution.AssociatedService.OrderGuidance.Should().Be(model.OrderGuidance);
            dbSolution.AssociatedService.PracticeReorganisationType.Should().Be(model.PracticeReorganisationType);
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
            context.ChangeTracker.Clear();

            await service.EditDetails(associatedService.Id, model);

            var dbSolution = await context.CatalogueItems.Include(c => c.AssociatedService).FirstAsync(c => c.Id == associatedService.Id);

            dbSolution.Should().NotBeNull();
            dbSolution.Name.Should().Be(model.Name);
            dbSolution.AssociatedService.Description.Should().Be(model.Description);
            dbSolution.AssociatedService.OrderGuidance.Should().Be(model.OrderGuidance);
            dbSolution.AssociatedService.PracticeReorganisationType.Should().Be(model.PracticeReorganisationType);
        }

        [Theory]
        [InMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.None)]
        public static async Task GetSolutionsWithMergerAndSplitTypesForAssociatedService_WhenNone_Returns_Solutions_WithEmptyMergerAndSplitValues(
            PracticeReorganisationTypeEnum reorganisationType,
            List<Solution> solutions,
            AssociatedService associatedService,
            AssociatedService otherAssociatedService,
            [Frozen] BuyingCatalogueDbContext context,
            AssociatedServicesService service)
        {
            associatedService.PracticeReorganisationType = reorganisationType;
            otherAssociatedService.PracticeReorganisationType = reorganisationType;
            solutions.ForEach(s => s.CatalogueItem.SupplierServiceAssociations = new HashSet<SupplierServiceAssociation> { new(s.CatalogueItemId, associatedService.CatalogueItemId), new(s.CatalogueItemId, otherAssociatedService.CatalogueItemId) });

            context.Solutions.AddRange(solutions);
            context.AssociatedServices.Add(associatedService);
            context.AssociatedServices.Add(otherAssociatedService);

            context.SaveChanges();
            context.ChangeTracker.Clear();

            var mergersAndSplits = await service.GetSolutionsWithMergerAndSplitTypesForButExcludingAssociatedService(associatedService.CatalogueItemId);

            mergersAndSplits.Should().NotBeEmpty();
            mergersAndSplits.Count.Should().Be(solutions.Count);
            mergersAndSplits.ForEach(s => s.SelectedMergerAndSplitsServices.Should().BeEquivalentTo(Array.Empty<PracticeReorganisationTypeEnum>()));
            mergersAndSplits.ForEach(s => s.IsValid.Should().BeTrue());
            mergersAndSplits.ForEach(s => s.With(associatedService.PracticeReorganisationType).IsValid.Should().BeTrue());
        }

        [Theory]
        [InMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Merger)]
        [InMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Split)]
        [InMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Split | PracticeReorganisationTypeEnum.Merger)]
        public static async Task GetSolutionsWithMergerAndSplitTypesForAssociatedService_Returns_Solutions_WithMergerAndSplitValues(
            PracticeReorganisationTypeEnum reorganisationType,
            List<Solution> solutions,
            AssociatedService associatedService,
            AssociatedService otherAssociatedService,
            [Frozen] BuyingCatalogueDbContext context,
            AssociatedServicesService service)
        {
            associatedService.PracticeReorganisationType = reorganisationType;
            otherAssociatedService.PracticeReorganisationType = reorganisationType;
            solutions.ForEach(s => s.CatalogueItem.SupplierServiceAssociations = new HashSet<SupplierServiceAssociation> { new(s.CatalogueItemId, associatedService.CatalogueItemId), new(s.CatalogueItemId, otherAssociatedService.CatalogueItemId) });

            context.Solutions.AddRange(solutions);
            context.AssociatedServices.Add(associatedService);
            context.AssociatedServices.Add(otherAssociatedService);

            context.SaveChanges();
            context.ChangeTracker.Clear();

            var mergersAndSplits = await service.GetSolutionsWithMergerAndSplitTypesForButExcludingAssociatedService(associatedService.CatalogueItemId);

            mergersAndSplits.Should().NotBeEmpty();
            mergersAndSplits.Count.Should().Be(solutions.Count);
            mergersAndSplits.ForEach(s => s.SelectedMergerAndSplitsServices.Should().BeEquivalentTo(new[] { reorganisationType }));
            mergersAndSplits.ForEach(s => s.IsValid.Should().BeTrue());
            mergersAndSplits.ForEach(s => s.With(associatedService.PracticeReorganisationType).IsNotValid.Should().BeTrue());
        }

        [Theory]
        [InMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.None)]
        [InMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Merger)]
        [InMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Split)]
        [InMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Split | PracticeReorganisationTypeEnum.Merger)]
        public static async Task GetSolutionsWithMergerAndSplitTypesForAssociatedService__ExlcudingOnlyService_Returns_Solutions_WithEmptyMergerAndSplitValues(
            PracticeReorganisationTypeEnum reorganisationType,
            List<Solution> solutions,
            AssociatedService associatedService,
            [Frozen] BuyingCatalogueDbContext context,
            AssociatedServicesService service)
        {
            associatedService.PracticeReorganisationType = reorganisationType;
            solutions.ForEach(s => s.CatalogueItem.SupplierServiceAssociations = new HashSet<SupplierServiceAssociation> { new(s.CatalogueItemId, associatedService.CatalogueItemId) });

            context.Solutions.AddRange(solutions);
            context.AssociatedServices.Add(associatedService);

            context.SaveChanges();
            context.ChangeTracker.Clear();

            var mergersAndSplits = await service.GetSolutionsWithMergerAndSplitTypesForButExcludingAssociatedService(associatedService.CatalogueItemId);

            mergersAndSplits.Should().NotBeEmpty();
            mergersAndSplits.Count.Should().Be(solutions.Count);
            mergersAndSplits.ForEach(s => s.SelectedMergerAndSplitsServices.Should().BeEquivalentTo(Array.Empty<PracticeReorganisationTypeEnum>()));
            mergersAndSplits.ForEach(s => s.IsValid.Should().BeTrue());
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
            context.ChangeTracker.Clear();

            var relatedSolutions = await service.GetAllSolutionsForAssociatedService(associatedService.CatalogueItemId);

            relatedSolutions.Should().NotBeEmpty();
            relatedSolutions.Count.Should().Be(solutions.Count);
        }

        [Theory]
        [InMemoryDbInlineAutoData(true)]
        [InMemoryDbInlineAutoData(false)]
        public static async Task GetPublishedAssociatedServicesForSolution_NullCatalogueItemId_ReturnsEmptySet(
            bool excludeMergersAndSplits,
            AssociatedServicesService service)
        {
            var result = await service.GetPublishedAssociatedServicesForSolution(null, excludeMergersAndSplits);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.None)]
        [InMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Merger)]
        [InMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Split)]
        [InMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Split | PracticeReorganisationTypeEnum.Merger)]
        public static async Task GetPublishedAssociatedServicesForSolution_NotExcludingMergersAndSplits_WithReturnsExpectedResult(
            PracticeReorganisationTypeEnum reorganisationType,
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem solution,
            CatalogueItem associatedService,
            AssociatedServicesService service)
        {
            solution.CatalogueItemType = CatalogueItemType.Solution;
            associatedService.CatalogueItemType = CatalogueItemType.AssociatedService;
            associatedService.PublishedStatus = PublicationStatus.Published;

            context.CatalogueItems.AddRange(solution, associatedService);
            context.AssociatedServices.Add(new AssociatedService { CatalogueItem = associatedService, PracticeReorganisationType = reorganisationType });
            context.SupplierServiceAssociations.Add(new SupplierServiceAssociation(solution.Id, associatedService.Id));

            await context.SaveChangesAsync();

            var result = await service.GetPublishedAssociatedServicesForSolution(solution.Id, false);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new[] { associatedService });
        }

        [Theory]
        [InMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Merger)]
        [InMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Split)]
        [InMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Split | PracticeReorganisationTypeEnum.Merger)]
        public static async Task GetPublishedAssociatedServicesForSolution_ExcludingMergersAndSplits_WithReturnsExpectedResult(
            PracticeReorganisationTypeEnum reorganisationType,
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem solution,
            CatalogueItem associatedService,
            AssociatedServicesService service)
        {
            solution.CatalogueItemType = CatalogueItemType.Solution;
            associatedService.CatalogueItemType = CatalogueItemType.AssociatedService;
            associatedService.PublishedStatus = PublicationStatus.Published;

            context.CatalogueItems.AddRange(solution, associatedService);
            context.AssociatedServices.Add(new AssociatedService { CatalogueItem = associatedService, PracticeReorganisationType = reorganisationType });
            context.SupplierServiceAssociations.Add(new SupplierServiceAssociation(solution.Id, associatedService.Id));

            await context.SaveChangesAsync();

            var result = await service.GetPublishedAssociatedServicesForSolution(solution.Id, true);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(Array.Empty<CatalogueItem>());
        }
    }
}
