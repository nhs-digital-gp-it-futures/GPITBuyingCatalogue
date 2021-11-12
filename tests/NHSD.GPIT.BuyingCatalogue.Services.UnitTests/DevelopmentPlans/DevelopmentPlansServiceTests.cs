using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.DevelopmentPlans
{
    public static class DevelopmentPlansServiceTests
    {
        [Theory]
        [CommonAutoData]
        public static async Task SaveRoadMap_CallsSaveChangesAsync_OnRepository(
            [Frozen] Mock<IDbRepository<Solution, BuyingCatalogueDbContext>> solutionRepositoryMock,
            DevelopmentPlansService service)
        {
            solutionRepositoryMock
                .Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            await service.SaveDevelopmentPlans(new CatalogueItemId(100000, "001"), "123");

            solutionRepositoryMock.Verify(r => r.SaveChangesAsync());
        }
    }
}
