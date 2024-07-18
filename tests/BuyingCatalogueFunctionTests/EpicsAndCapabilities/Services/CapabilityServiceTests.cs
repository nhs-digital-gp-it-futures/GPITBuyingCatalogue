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
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace BuyingCatalogueFunctionTests.EpicsAndCapabilities.Services
{
    public static class CapabilityServiceTests
    {
        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Read(CapabilityService service)
        {
            var stream = new MemoryStream(
                Encoding.UTF8.GetBytes(
                    new StringBuilder()
                        .AppendLine("ID,Name,Capability Category,URL,Description,Framework")
                        .AppendLine("C1,NameValue,Citizen services,https://test.com,DescriptionValue,GP IT Futures|Tech Innovation")
                        .ToString())

            );

            var results = await service.Read(stream);
            results.Count.Should().Be(1);
            var result = results.FirstOrDefault();

            result.Should().NotBeNull();
            result!.Id.Value.Should().Be(1);
            result.Name.Should().Be("NameValue");
            result.Category.Should().Be("Citizen services");
            result.Url.Should().Be("https://test.com");
            result.Description.Should().Be("DescriptionValue");
            result.Framework.Should().BeEquivalentTo(new[] { "GP IT Futures", "Tech Innovation" });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task ProcessNull(CapabilityService service)
        {
            await service.Invoking(async s => await s.Process(null))
                .Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Process_New_Capability_With_Existing_Category_Creates_Capability(
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
            CapabilityCsv toProcess = GetCapabilityDetailsToCreate(capability, framework);

            await service.Process(new List<CapabilityCsv>() { toProcess });
            dbContext.ChangeTracker.Clear();

            Capability? result = GetCapability(capability, dbContext);

            result.Should().NotBeNull();
            result!.Id.Should().Be(capability.Id);
            result.Name.Should().Be(capability.Name);
            result.Description.Should().Be(capability.Description);
            result.SourceUrl.Should().Be(capability.SourceUrl);
            result.Category.Name.Should().Be(capability.Category.Name);
            result.Status.Should().Be(CapabilityStatus.Effective);
            result.FrameworkCapabilities.Select(f => f.Framework.ShortName).Should().BeEquivalentTo(new[] { framework.ShortName });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Process_New_Capability_With_New_Category_Creates_Capability(
            Framework framework,
            CapabilityCategory category,
            Capability capability,
            [Frozen] BuyingCatalogueDbContext dbContext,
            CapabilityService service)
        {
            dbContext.Frameworks.Add(framework);
            dbContext.SaveChanges();

            capability.Category = category;
            CapabilityCsv toProcess = GetCapabilityDetailsToCreate(capability, framework);

            await service.Process(new List<CapabilityCsv>() { toProcess });
            dbContext.ChangeTracker.Clear();

            Capability? result = GetCapability(capability, dbContext);

            result.Should().NotBeNull();
            result!.Id.Should().Be(capability.Id);
            result.Name.Should().Be(capability.Name);
            result.Description.Should().Be(capability.Description);
            result.SourceUrl.Should().Be(capability.SourceUrl);
            result.Category.Name.Should().Be(capability.Category.Name);
            result.Status.Should().Be(CapabilityStatus.Effective);
            result.FrameworkCapabilities.Select(f => f.Framework.ShortName).Should().BeEquivalentTo(new[] { framework.ShortName });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Process_Existing_Capability_With_Existing_Categories_Updates_Capability(
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

            CapabilityCsv toProcess = GetCapabilityDetailsToUpdate(capability, frameworkToChangeTo, categoryToChangeTo);

            await service.Process(new List<CapabilityCsv>() { toProcess });
            dbContext.ChangeTracker.Clear();

            Capability? result = GetCapability(capability, dbContext);

            result.Should().NotBeNull();
            result!.Id.Should().Be(capability.Id);
            result.Name.Should().Be("modified name");
            result.Description.Should().Be("modified description");
            result.SourceUrl.Should().Be("modified url");
            result.Category.Name.Should().Be(categoryToChangeTo.Name);
            result.Status.Should().Be(CapabilityStatus.Effective);
            result.FrameworkCapabilities.Select(f => f.Framework.ShortName).Should().BeEquivalentTo(new[] { frameworkToChangeTo.ShortName });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Process_Existing_Capability_To_New_Category_Updates_Capability(
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
            dbContext.Frameworks.Add(framework);
            dbContext.Frameworks.Add(frameworkToChangeTo);
            dbContext.Capabilities.Add(capability);
            dbContext.SaveChanges();

            CapabilityCsv toProcess = GetCapabilityDetailsToUpdate(capability, frameworkToChangeTo, categoryToChangeTo);

            await service.Process(new List<CapabilityCsv>() { toProcess });
            dbContext.ChangeTracker.Clear();

            Capability? result = GetCapability(capability, dbContext);

            result.Should().NotBeNull();
            result!.Id.Should().Be(capability.Id);
            result.Name.Should().Be("modified name");
            result.Description.Should().Be("modified description");
            result.SourceUrl.Should().Be("modified url");
            result.Category.Name.Should().Be(categoryToChangeTo.Name);
            result.Status.Should().Be(CapabilityStatus.Effective);
            result.FrameworkCapabilities.Select(f => f.Framework.ShortName).Should().BeEquivalentTo(new[] { frameworkToChangeTo.ShortName });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Process_Existing_Capability_Expire(
            Framework framework,
            Capability capability,
            CapabilityCategory category,
            [Frozen] BuyingCatalogueDbContext dbContext,
            CapabilityService service)
        {
            capability.Category = category;
            capability.FrameworkCapabilities.Add(new FrameworkCapability(framework.Id, capability.Id));
            dbContext.CapabilityCategories.Add(category);
            dbContext.Frameworks.Add(framework);
            dbContext.Capabilities.Add(capability);
            dbContext.SaveChanges();

            await service.Process(new List<CapabilityCsv>() { });
            dbContext.ChangeTracker.Clear();

            Capability? result = GetCapability(capability, dbContext);

            result.Should().NotBeNull();
            result!.Status.Should().Be(CapabilityStatus.Expired);
        }

        private static CapabilityCsv GetCapabilityDetailsToUpdate(Capability capability, Framework frameworkToChangeTo, CapabilityCategory categoryToChangeTo)
        {
            return new CapabilityCsv()
            {
                Id = new CapabilityIdCsv($"C{capability.Id}"),
                Category = categoryToChangeTo.Name,
                Name = "modified name",
                Description = "modified description",
                Url = "modified url",
                Framework = new List<string>() { frameworkToChangeTo.ShortName },
            };
        }

        private static CapabilityCsv GetCapabilityDetailsToCreate(Capability capability, Framework framework)
        {
            return new CapabilityCsv()
            {
                Id = new CapabilityIdCsv($"C{capability.Id}"),
                Category = capability.Category.Name,
                Name = capability.Name,
                Description = capability.Description,
                Url = capability.SourceUrl,
                Framework = new List<string>() { framework.ShortName },
            };
        }

        private static Capability? GetCapability(Capability capability, BuyingCatalogueDbContext dbContext)
        {
            return dbContext.Capabilities
                .Include(c => c.Category)
                .Include(c => c.FrameworkCapabilities)
                    .ThenInclude(fc => fc.Framework)
                .FirstOrDefault(c => c.Id == capability.Id);
        }
    }
}
