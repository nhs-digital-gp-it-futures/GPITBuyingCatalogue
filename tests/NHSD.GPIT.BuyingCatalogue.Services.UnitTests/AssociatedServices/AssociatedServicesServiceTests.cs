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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.Services.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
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
                    .Excluding(ctx => ctx.CatalogueItem));
        }
    }
}
