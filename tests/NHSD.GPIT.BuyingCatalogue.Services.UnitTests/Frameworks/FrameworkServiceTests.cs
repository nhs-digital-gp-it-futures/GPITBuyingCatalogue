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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.Services.Framework;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Frameworks
{
    public class FrameworkServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(FrameworkService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetFrameworksById_ReturnsExpected(
            EntityFramework.Catalogue.Models.Framework framework,
            [Frozen] BuyingCatalogueDbContext dbContext,
            FrameworkService service)
        {
            dbContext.Frameworks.Add(framework);
            await dbContext.SaveChangesAsync();

            var result = await service.GetFrameworksById(framework.Id);

            result.Should().BeEquivalentTo(framework);
        }

        [Theory]
        [InMemoryDbAutoData]
        public async Task GetFrameworksByCatalogueItems(
            EntityFramework.Catalogue.Models.Framework framework,
            FrameworkSolution frameworkSolutions,
            CatalogueItem catalogueItem,
            Solution solutions,
            [Frozen] BuyingCatalogueDbContext dbContext,
            FrameworkService service)
        {
            dbContext.FrameworkSolutions.Add(frameworkSolutions);
            dbContext.Frameworks.Add(framework);
            dbContext.CatalogueItems.Add(catalogueItem);
            dbContext.Solutions.Add(solutions);
            await dbContext.SaveChangesAsync();

            var expectedFrameworks = new List<FrameworkFilterInfo>
            {
               new FrameworkFilterInfo { Id = "Id6ac35090-33bb-4014-9e5d-6e5e930a8d6d", ShortName = "ShortName2f937c93-f70c-4481-84e1-2f6cf47c1690" },
               new FrameworkFilterInfo { Id = "Id21a8dcf3-4a94-44c0-9070-15430dbce12b", ShortName = "ShortName9e0032bf-3cad-45a1-afd9-df7f351274c8" },
               new FrameworkFilterInfo { Id = "Idf774ce4e-13f7-47d8-aa0f-e346d1df266f", ShortName = "ShortName3342fe21-4df6-4617-aa6b-4e7f7cb9025f" },
            };

            var result = await service.GetFrameworksByCatalogueItems(new List<CatalogueItem> { catalogueItem });

            result.Count().Equals(expectedFrameworks.Count());
        }
    }
}
