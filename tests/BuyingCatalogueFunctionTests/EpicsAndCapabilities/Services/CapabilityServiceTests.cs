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
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace BuyingCatalogueFunctionTests.EpicsAndCapabilities.Services
{
    public static class CapabilityServiceTests
    {
        [Theory]
        [InMemoryDbAutoData]
        public static async Task Read(CapabilityService service)
        {
            var stream = new MemoryStream(
                Encoding.UTF8.GetBytes(
                    new StringBuilder()
                        .AppendLine("ID,Name,Version,Capability Category,URL,Description,Framework")
                        .AppendLine("C1,NameValue,1.0.2,Citizen services,https://test.com,DescriptionValue,GP IT Futures|Tech Innovation")
                        .ToString())

            );

            var results = await service.Read(stream);
            results.Count.Should().Be(1);
            var result = results.First();

            result.Id.Value.Should().Be(1);
            result.Name.Should().Be("NameValue");
            result.Version.Should().Be("1.0.2");
            result.Category.Should().Be("Citizen services");
            result.Url.Should().Be("https://test.com");
            result.Description.Should().Be("DescriptionValue");
            result.Framework.Should().BeEquivalentTo(new[] { "GP IT Futures", "Tech Innovation" });
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task ProcessNull(CapabilityService service)
        {
            await service.Invoking(async s => await s.Process(null))
                .Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Process_New_Capability_Category_Must_Exist(
            Framework framework,
            CapabilityCategory category,
            Capability capability,
            [Frozen] BuyingCatalogueDbContext dbContext,
            CapabilityService service)
        {
            dbContext.CapabilityCategories.Add(category);
            dbContext.Frameworks.Add(framework);
            dbContext.SaveChanges();

            capability.Category = category;
            var toProcess = new CapabilityCsv()
            {
                Id = new CapabilityIdCsv($"C{capability.Id}"),
                Category = category.Name,
                Name = capability.Name,
                Version = capability.Version,
                Description = capability.Description,
                Url = capability.SourceUrl,
                Framework = new List<string>() { framework.ShortName },
            };

            await service.Process(new List<CapabilityCsv>() { toProcess });
            dbContext.ChangeTracker.Clear();

            var result = dbContext.Capabilities
                .Include(c => c.Category)
                .Include(c => c.FrameworkCapabilities)
                    .ThenInclude(fc => fc.Framework)
                .First(c => c.Id == capability.Id);

            result.Should().NotBeNull();
            result.Id.Should().Be(capability.Id);
            result.Name.Should().Be(capability.Name);
            result.Version.Should().Be(capability.Version);
            result.Description.Should().Be(capability.Description);
            result.SourceUrl.Should().Be(capability.SourceUrl);
            result.Category.Name.Should().Be(capability.Category.Name);
            result.FrameworkCapabilities.Select(f => f.Framework.ShortName).Should().BeEquivalentTo(new[] { framework.ShortName });
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Process_Exising_Capability_Category_Must_Exist(
            Framework framework,
            Framework frameworkToChangeTo,
            Capability capability,
            CapabilityCategory category,
            CapabilityCategory categoryToChangeTo,
            [Frozen] BuyingCatalogueDbContext dbContext,
            CapabilityService service)
        {
            capability.Category = category;
            capability.FrameworkCapabilities.Add(new FrameworkCapability(framework.Id, capability.Id));
            dbContext.CapabilityCategories.Add(category);
            dbContext.CapabilityCategories.Add(categoryToChangeTo);
            dbContext.Frameworks.Add(framework);
            dbContext.Frameworks.Add(frameworkToChangeTo);
            dbContext.Capabilities.Add(capability);
            dbContext.SaveChanges();

            var toProcess = new CapabilityCsv()
            {
                Id = new CapabilityIdCsv($"C{capability.Id}"),
                Category = categoryToChangeTo.Name,
                Name = "modified name",
                Version = "modified version",
                Description = "modified description",
                Url = "modified url",
                Framework = new List<string>() { frameworkToChangeTo.ShortName },
            };

            await service.Process(new List<CapabilityCsv>() { toProcess });
            dbContext.ChangeTracker.Clear();

            var result = dbContext.Capabilities
                .Include(c => c.Category)
                .Include(c => c.FrameworkCapabilities)
                    .ThenInclude(fc => fc.Framework)
                .First(c => c.Id == capability.Id);

            result.Should().NotBeNull();
            result.Id.Should().Be(capability.Id);
            result.Name.Should().Be("modified name");
            result.Version.Should().Be("modified version");
            result.Description.Should().Be("modified description");
            result.SourceUrl.Should().Be("modified url");
            result.Category.Name.Should().Be(categoryToChangeTo.Name);
            result.FrameworkCapabilities.Select(f => f.Framework.ShortName).Should().BeEquivalentTo(new[] { frameworkToChangeTo.ShortName });
        }
    }
}
