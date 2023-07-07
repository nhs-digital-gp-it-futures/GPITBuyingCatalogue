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
    public static class EpicServiceTests
    {
        [Theory]
        [InMemoryDbAutoData]
        public static async Task Read(EpicService service)
        {
            var stream = new MemoryStream(
                Encoding.UTF8.GetBytes(
                    new StringBuilder()
                        .AppendLine("Epic ID,Epic Name,Epic Status ,Epic Level ,Capability ,Capability ID")
                        .AppendLine("E00109,EpicName,Active,MUST,Appointments Management - Citizen,C1")
                        .AppendLine("E00109,EpicName,Active,MAY,Communicate With Practice - Citizen,C2")
                        .ToString())
            );

            var results = await service.Read(stream);
            results.Count.Should().Be(1);
            var result = results.FirstOrDefault();

            result.Should().NotBeNull();
            result!.Id.Should().Be("E00109");
            result.Name.Should().Be("EpicName");
            result.IsActive.Should().BeTrue();
            result.Capabilities.Should().BeEquivalentTo(new[]
            {
                new CapabilityEpicCsv()
                {
                    CapabilityId = new("C1"),
                    Level = CompliancyLevel.Must
                },
                new CapabilityEpicCsv()
                {
                    CapabilityId = new("C2"),
                    Level = CompliancyLevel.May
                }
            });
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task ProcessNull(EpicService service)
        {
            await service.Invoking(async s => await s.Process(null))
                .Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Process_New_Epic(
            Capability capability,
            Epic epic,
            [Frozen] BuyingCatalogueDbContext dbContext,
            EpicService service)
        {
            dbContext.Capabilities.Add(capability);
            dbContext.SaveChanges();

            var toProcess = new EpicCsv()
            {
                Id = epic.Id,
                Name = epic.Name,
                IsActive = epic.IsActive,
                Capabilities = new List<CapabilityEpicCsv>() {
                    new CapabilityEpicCsv()
                    {
                       CapabilityId = new($"C{capability.Id}"),
                       Level = CompliancyLevel.Must
                    }
                },
            };

            await service.Process(new List<EpicCsv>() { toProcess });
            dbContext.ChangeTracker.Clear();

            var result = dbContext.Epics
                .Include(c => c.Capabilities)
                .FirstOrDefault(c => c.Id == epic.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(epic.Id);
            result.Name.Should().Be(epic.Name);
            result.IsActive.Should().Be(epic.IsActive);
            result.Capabilities.Select(f => f.Id).Should().BeEquivalentTo(new[] { capability.Id });
            result.CapabilityEpics.Should().BeEquivalentTo(new[]
                {
                    new CapabilityEpic()
                    {
                        CapabilityId = capability.Id,
                        CompliancyLevel = CompliancyLevel.Must,
                        EpicId = epic.Id,
                    }
                },
                opt => opt.Excluding(c => c.Epic));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Process_Exising_Epic(
            Capability capability,
            Capability capabilityToChangeTo,
            Epic epic,
            [Frozen] BuyingCatalogueDbContext dbContext,
            EpicService service)
        {
            epic.CapabilityEpics.Add(new CapabilityEpic()
            {
                CapabilityId = capability.Id,
                EpicId = epic.Id,
                CompliancyLevel = CompliancyLevel.Must,
            });
            dbContext.Capabilities.Add(capability);
            dbContext.Capabilities.Add(capabilityToChangeTo);
            dbContext.Epics.Add(epic);
            dbContext.SaveChanges();
            dbContext.ChangeTracker.Clear();

            var toProcess = new EpicCsv()
            {
                Id = epic.Id,
                Name = "modified name",
                IsActive = !epic.IsActive,
                Capabilities = new List<CapabilityEpicCsv>() {
                    new CapabilityEpicCsv()
                    {
                       CapabilityId = new($"C{capabilityToChangeTo.Id}"),
                       Level = CompliancyLevel.Must
                    }
                },
            };

            await service.Process(new List<EpicCsv>() { toProcess });
            dbContext.ChangeTracker.Clear();

            var result = dbContext.Epics
                .Include(c => c.Capabilities)
                .FirstOrDefault(c => c.Id == epic.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(epic.Id);
            result.IsActive.Should().Be(!epic.IsActive);
            result.Name.Should().Be("modified name");
            result.Capabilities.Select(f => f.Id).Should().BeEquivalentTo(new[] { capabilityToChangeTo.Id });
            result.CapabilityEpics.Should().BeEquivalentTo(new[]
                {
                    new CapabilityEpic()
                    {
                        CapabilityId = capabilityToChangeTo.Id,
                        CompliancyLevel = CompliancyLevel.Must,
                        EpicId = epic.Id,
                    }
                },
                opt => opt.Excluding(c => c.Epic));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Process_Exising_Epic_Change_Level(
            Capability capability,
            Epic epic,
            [Frozen] BuyingCatalogueDbContext dbContext,
            EpicService service)
        {
            epic.CapabilityEpics.Add(new CapabilityEpic()
            {
                CapabilityId = capability.Id,
                EpicId = epic.Id,
                CompliancyLevel = CompliancyLevel.Must,
            });
            dbContext.Capabilities.Add(capability);
            dbContext.Epics.Add(epic);
            dbContext.SaveChanges();
            dbContext.ChangeTracker.Clear();

            var toProcess = new EpicCsv()
            {
                Id = epic.Id,
                Name = "modified name",
                IsActive = !epic.IsActive,
                Capabilities = new List<CapabilityEpicCsv>() {
                    new CapabilityEpicCsv()
                    {
                       CapabilityId = new($"C{capability.Id}"),
                       Level = CompliancyLevel.May
                    }
                },
            };

            await service.Process(new List<EpicCsv>() { toProcess });
            dbContext.ChangeTracker.Clear();

            var result = dbContext.Epics
                .Include(c => c.Capabilities)
                .FirstOrDefault(c => c.Id == epic.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(epic.Id);
            result.IsActive.Should().Be(!epic.IsActive);
            result.Name.Should().Be("modified name");
            result.Capabilities.Select(f => f.Id).Should().BeEquivalentTo(new[] { capability.Id });
            result.CapabilityEpics.Should().BeEquivalentTo(new[]
                {
                    new CapabilityEpic()
                    {
                        CapabilityId = capability.Id,
                        CompliancyLevel = CompliancyLevel.May,
                        EpicId = epic.Id,
                    }
                },
                opt => opt.Excluding(c => c.Epic));
        }
    }
}
