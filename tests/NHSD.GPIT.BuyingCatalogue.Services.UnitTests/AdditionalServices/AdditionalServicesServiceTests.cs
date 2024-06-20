using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.Services.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.AdditionalServices
{
    public static class AdditionalServicesServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AdditionalServicesService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static Task AddAdditionalService_NullSolution_ThrowsException(
            AdditionalServicesDetailsModel model,
            AdditionalServicesService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.AddAdditionalService(null, model));
        }

        [Theory]
        [MockAutoData]
        public static Task AddAdditionalService_NullModel_ThrowsException(
            CatalogueItem catalogueItem,
            AdditionalServicesService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.AddAdditionalService(catalogueItem, null));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddAdditionalService_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem solution,
            AdditionalServicesDetailsModel model,
            AdditionalServicesService service)
        {
            context.CatalogueItems.Add(solution);
            await context.SaveChangesAsync();

            var result = await service.AddAdditionalService(solution, model);

            var dbSolution = await context.CatalogueItems.Include(c => c.AdditionalService).FirstAsync(c => c.Id == result);

            dbSolution.Should().NotBeNull();
            dbSolution.Name.Should().Be(model.Name);
            dbSolution.CatalogueItemType.Should().Be(CatalogueItemType.AdditionalService);
            dbSolution.SupplierId.Should().Be(solution.SupplierId);
            dbSolution.PublishedStatus.Should().Be(PublicationStatus.Draft);
            dbSolution.AdditionalService.Should().NotBeNull();
            dbSolution.AdditionalService.FullDescription.Should().Be(model.Description);
        }

        [Theory]
        [MockAutoData]
        public static Task EditAdditionalService_NullModel_ThrowsException(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            AdditionalServicesService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.EditAdditionalService(solutionId, additionalServiceId, null));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task EditAdditionalService_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem solution,
            CatalogueItem additionalService,
            AdditionalServicesDetailsModel model,
            AdditionalServicesService service)
        {
            context.CatalogueItems.Add(solution);
            additionalService.CatalogueItemType = CatalogueItemType.AdditionalService;
            additionalService.AdditionalService = new AdditionalService { SolutionId = solution.Id };
            context.CatalogueItems.Add(additionalService);
            await context.SaveChangesAsync();

            await service.EditAdditionalService(solution.Id, additionalService.Id, model);

            var dbSolution = await context.CatalogueItems.Include(c => c.AdditionalService).FirstAsync(c => c.Id == additionalService.Id);

            dbSolution.Should().NotBeNull();
            dbSolution.Name.Should().Be(model.Name);
            dbSolution.AdditionalService.FullDescription.Should().Be(model.Description);
        }

        [Theory]
        [MockAutoData]
        public static async Task GetAdditionalServicesByServiceId_NullCatalogueItemId_ReturnsEmptySet(
            AdditionalServicesService service)
        {
            var result = await service.GetAdditionalServicesBySolutionId(null);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetAdditionalServicesByServiceId_ExpectedResult(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem solution,
            CatalogueItem additionalService,
            AdditionalServicesService service)
        {
            additionalService.CatalogueItemType = CatalogueItemType.AdditionalService;
            additionalService.AdditionalService = new AdditionalService { SolutionId = solution.Id };

            context.CatalogueItems.Add(solution);
            context.CatalogueItems.Add(additionalService);

            await context.SaveChangesAsync();

            var result = await service.GetAdditionalServicesBySolutionId(solution.Id);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new[] { additionalService });
        }
    }
}
