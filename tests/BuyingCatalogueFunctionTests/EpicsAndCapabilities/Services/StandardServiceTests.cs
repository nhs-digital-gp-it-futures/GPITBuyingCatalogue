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
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace BuyingCatalogueFunctionTests.EpicsAndCapabilities.Services
{
    public static class StandardServiceTests
    {
        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Read(StandardService service)
        {
            var stream = new MemoryStream(
                Encoding.UTF8.GetBytes(
                    new StringBuilder()
                        .AppendLine("ID,Name,Type,URL,Description,Framework")
                        .AppendLine("S49,StandardName,Context Specific Standard,https://test.com,\"Description\",GP IT Futures|Tech Innovation")
                        .ToString())
            );

            var results = await service.Read(stream);
            results.Count.Should().Be(1);
            var result = results.FirstOrDefault();

            result.Should().NotBeNull();
            result!.Id.Should().Be("S49");
            result.Name.Should().Be("StandardName");
            result.StandardType.Should().Be(StandardType.ContextSpecific);
            result.Url.Should().Be("https://test.com");
            result.Description.Should().Be("Description");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task ProcessNull(StandardService service)
        {
            await service.Invoking(async s => await s.Process(null))
                .Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Process_New_Standard(
            Standard standard,
            [Frozen] BuyingCatalogueDbContext dbContext,
            StandardService service)
        {
            var toProcess = new StandardCsv()
            {
                Id = standard.Id,
                Name = standard.Name,
                Description = standard.Description,
                StandardType = standard.StandardType,
                Url = standard.Url,
            };

            await service.Process(new List<StandardCsv>() { toProcess });
            dbContext.ChangeTracker.Clear();

            var result = dbContext.Standards
                .FirstOrDefault(c => c.Id == standard.Id);

            result.Should().NotBeNull();

            result!.Id.Should().Be(standard.Id);
            result.Name.Should().Be(standard.Name);
            result.Description.Should().Be(standard.Description);
            result.StandardType.Should().Be(standard.StandardType);
            result.Url.Should().Be(standard.Url);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Process_Existing_Standard(
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
                StandardType = StandardType.Overarching,
                Url = "http://test2.com",
            };

            await service.Process(new List<StandardCsv>() { toProcess });
            dbContext.ChangeTracker.Clear();

            var result = dbContext.Standards
                .FirstOrDefault(c => c.Id == standard.Id);

            result.Should().NotBeNull();

            result!.Id.Should().Be(standard.Id);
            result.Name.Should().Be("modified name");
            result.Description.Should().Be("modified description");
            result.Url.Should().Be("http://test2.com");
            result.StandardType.Should().Be(StandardType.Overarching);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Process_Existing_Standard_Delete(
            Standard standard,
            [Frozen] BuyingCatalogueDbContext dbContext,
            StandardService service)
        {
            dbContext.Standards.Add(standard);
            dbContext.SaveChanges();
            dbContext.ChangeTracker.Clear();

            await service.Process(new List<StandardCsv>() { });
            dbContext.ChangeTracker.Clear();

            var result = dbContext.Standards
                .FirstOrDefault(c => c.Id == standard.Id);

            result.Should().BeNull();

            var resultDeleted = dbContext.Standards
                .IgnoreQueryFilters()
                .FirstOrDefault(c => c.Id == standard.Id);

            resultDeleted.Should().NotBeNull();
            resultDeleted!.IsDeleted.Should().BeTrue();
        }
    }
}
