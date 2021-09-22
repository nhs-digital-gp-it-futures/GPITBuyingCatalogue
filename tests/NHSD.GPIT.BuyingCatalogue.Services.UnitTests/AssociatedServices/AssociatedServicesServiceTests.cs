using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions
{
    public static class AssociatedServicesServiceTests
    {
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
