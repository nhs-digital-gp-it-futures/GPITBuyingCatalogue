using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using BuyingCatalogueFunction.EpicsAndCapabilities.Models;
using BuyingCatalogueFunction.EpicsAndCapabilities.Services;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace BuyingCatalogueFunctionTests.EpicsAndCapabilities.Services
{
    public static class StandardServiceTests
    {
        [Theory]
        [InMemoryDbAutoData]
        public static async Task Read(StandardService service)
        {
            var stream = new MemoryStream(
                Encoding.UTF8.GetBytes(
                    new StringBuilder()
                        .AppendLine("ID,Name,Version,Type,URL,Description,Framework")
                        .AppendLine("S49,StandardName,1.0.0,Context Specific Standard,https://test.com,\"Description\",GP IT Futures|Tech Innovation")
                        .ToString())
            );

            var results = await service.Read(stream);
            results.Count.Should().Be(1);
            var result = results.First();

            result.Id.Should().Be("S49");
            result.Name.Should().Be("StandardName");
            result.Version.Should().Be("1.0.0");
            result.StandardType.Should().Be(StandardType.ContextSpecific);
            result.Url.Should().Be("https://test.com");
            result.Description.Should().Be("Description");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task ProcessNull(StandardService service)
        {
            await service.Invoking(async s => await s.Process(null))
                .Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Process_New_Standard(
            Standard standard,
            [Frozen] BuyingCatalogueDbContext dbContext,
            StandardService service)
        {
            var toProcess = new StandardCsv()
            {
                Id = standard.Id,
                Name = standard.Name,
                Version = standard.Version,
                Description = standard.Description,
                StandardType = standard.StandardType,
                Url = standard.Url,
            };

            await service.Process(new List<StandardCsv>() { toProcess });
            dbContext.ChangeTracker.Clear();

            var result = dbContext.Standards
                .First(c => c.Id == standard.Id);

            result.Should().NotBeNull();
            result.Id.Should().Be(standard.Id);
            result.Name.Should().Be(standard.Name);
            result.Version.Should().Be(standard.Version);
            result.Description.Should().Be(standard.Description);
            result.StandardType.Should().Be(standard.StandardType);
            result.Url.Should().Be(standard.Url);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Process_Exising_Standard(
            Standard standard,
            [Frozen] BuyingCatalogueDbContext dbContext,
            StandardService service)
        {
            dbContext.Standards.Add(standard);
            dbContext.SaveChanges();
            dbContext.ChangeTracker.Clear();

            var toProcess = new StandardCsv()
            {
                Id = standard.Id,
                Name = "modified name",
                Description = "modified description",
                Version = "modified version",
                StandardType = StandardType.Overarching,
                Url = "http://test2.com",
            };

            await service.Process(new List<StandardCsv>() { toProcess });
            dbContext.ChangeTracker.Clear();

            var result = dbContext.Standards
                .First(c => c.Id == standard.Id);

            result.Should().NotBeNull();
            result.Id.Should().Be(standard.Id);
            result.Name.Should().Be("modified name");
            result.Description.Should().Be("modified description");
            result.Version.Should().Be("modified version");
            result.Url.Should().Be("http://test2.com");
            result.StandardType.Should().Be(StandardType.Overarching);
        }
    }
}
