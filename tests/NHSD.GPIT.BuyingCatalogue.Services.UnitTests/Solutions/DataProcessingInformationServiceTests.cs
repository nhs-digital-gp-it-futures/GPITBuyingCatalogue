using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions;

public static class DataProcessingInformationServiceTests
{
    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetSolutionWithDataProcessingInformation_ReturnsSolution(
        Solution solution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        DataProcessingInformationService service)
    {
        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = await service.GetSolutionWithDataProcessingInformation(solution.CatalogueItemId);

        result.Should().NotBeNull();
    }
}
