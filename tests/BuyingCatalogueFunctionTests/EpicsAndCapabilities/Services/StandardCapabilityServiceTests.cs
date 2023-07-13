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
    public static class StandardCapabilityServiceTests
    {
        [Theory]
        [InMemoryDbAutoData]
        public static async Task Read(StandardCapabilityService service)
        {
            var stream = new MemoryStream(
                Encoding.UTF8.GetBytes(
                    new StringBuilder()
                        .AppendLine("From,To,FromID,ToID,Type,IsOptional")
                        .AppendLine("111,Interoperability Toolkit (ITK),S33,S48,Mandatory,0")
                        .AppendLine("Appointments Management - Citizen,Clinical Safety,C1,S25,Mandatory,0")
                        .ToString())
            );

            var results = await service.Read(stream);
            results.Count.Should().Be(1);
            var result = results.FirstOrDefault();

            result.Should().NotBeNull();
            result!.FromId.Value.Should().Be(1);
            result.ToId.Should().Be("S25");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task ProcessNull(StandardCapabilityService service)
        {
            await service.Invoking(async s => await s.Process(null))
                .Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Process_New_StandardCapability(
            StandardCapability standardCapability,
            [Frozen] BuyingCatalogueDbContext dbContext,
            StandardCapabilityService service)
        {
            var toProcess = new StandardCapabilityCsv()
            {
                FromId = new CapabilityIdCsv($"C{standardCapability.CapabilityId}"),
                ToId = standardCapability.StandardId,
            };

            await service.Process(new List<StandardCapabilityCsv>() { toProcess });
            dbContext.ChangeTracker.Clear();

            var result = dbContext.StandardCapabilities
                .FirstOrDefault(c => c.StandardId == standardCapability.StandardId && c.CapabilityId == standardCapability.CapabilityId);

            result.Should().NotBeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Process_Existing_StandardCapability(
            StandardCapability standardCapability,
            StandardCapability existingStandardCapability,
            [Frozen] BuyingCatalogueDbContext dbContext,
            StandardCapabilityService service)
        {
            dbContext.StandardCapabilities.Add(existingStandardCapability);
            dbContext.SaveChanges();
            dbContext.ChangeTracker.Clear();

            var toProcess = new StandardCapabilityCsv()
            {
                FromId = new CapabilityIdCsv($"C{standardCapability.CapabilityId}"),
                ToId = standardCapability.StandardId,
            };

            await service.Process(new List<StandardCapabilityCsv>() { toProcess });
            dbContext.ChangeTracker.Clear();

            var existingResult = dbContext.StandardCapabilities
                .FirstOrDefault(c => c.StandardId == existingStandardCapability.StandardId && c.CapabilityId == existingStandardCapability.CapabilityId);
            var addResult = dbContext.StandardCapabilities
                .FirstOrDefault(c => c.StandardId == standardCapability.StandardId && c.CapabilityId == standardCapability.CapabilityId);

            existingResult.Should().BeNull();
            addResult.Should().NotBeNull();
        }
    }
}
