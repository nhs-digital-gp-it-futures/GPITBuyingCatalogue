﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
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
            [Frozen] BuyingCatalogueDbContext dbContext,
            FrameworkService service)
        {
            dbContext.Frameworks.Add(framework);
            dbContext.FrameworkSolutions.Add(frameworkSolutions);
            dbContext.CatalogueItems.Add(catalogueItem);
            await dbContext.SaveChangesAsync();

            var expectedFrameworks = dbContext.Frameworks
                        .Where(f => dbContext.FrameworkSolutions.Any(fs => fs.FrameworkId == f.Id || catalogueItem.Id == fs.SolutionId))
                        .Select(g => new FrameworkFilterInfo
                        {
                            Id = g.Id,
                            ShortName = g.ShortName,
                            Name = g.Name,
                            CountOfActiveSolutions = dbContext.FrameworkSolutions.Count(fs => fs.FrameworkId == g.Id && catalogueItem.Id == fs.SolutionId),
                        });

            var result = await service.GetFrameworksByCatalogueItems(new List<CatalogueItem> { catalogueItem });

            result.Should().BeEquivalentTo(expectedFrameworks);
        }
    }
}
