using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuyingCatalogueFunction.EpicsAndCapabilities.Interfaces;
using BuyingCatalogueFunction.EpicsAndCapabilities.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace BuyingCatalogueFunction.EpicsAndCapabilities.Services
{
    public class EpicService : IEpicService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public EpicService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<string>> Process(List<EpicCsv> epics)
        {
            ArgumentNullException.ThrowIfNull(epics);

            var log = new List<string>();
            foreach (var epic in epics)
            {
                await Process(dbContext, epic, log);
            }

            var desiredEpics = epics.Select(s => s.Id).ToList();
            var toExpire = await dbContext
                .Epics
                .Where(s => !s.SupplierDefined && !desiredEpics.Any(d => d == s.Id))
                .ToListAsync();

            ProcessExpired(toExpire, log);

            await dbContext.SaveChangesAsync();
            return log;
        }

        public async Task<List<EpicCsv>> Read(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);

            using var streamReader = new StreamReader(stream);
            using var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim,
            });

            var epics = new Dictionary<string, EpicCsv>();
            await csv.ReadAsync();
            csv.ReadHeader();
            while (await csv.ReadAsync())
            {
                // Epic ID,Epic Name,Epic Level ,Capability ,Capability ID
                ReadLine(csv, epics);
            }

            return epics.Values.ToList();
        }

        private static void ReadLine(CsvReader csv, Dictionary<string, EpicCsv> epics)
        {
            var Id = csv.GetField<string>("Epic ID")
                ?? throw new InvalidOperationException("Epic ID is required");

            if (epics.TryGetValue(Id, out var existingEpic))
            {
                existingEpic.Capabilities.Add(new CapabilityEpicCsv()
                {
                    CapabilityId = new(csv.GetField<string>("Capability ID")),
                    Level = ParseLevel(csv.GetField<string>("Epic Level")),
                });
            }
            else
            {
                EpicCsv newEpic = Map(csv, Id);
                epics.Add(newEpic.Id, newEpic);
            }
        }

        private void ProcessExpired(List<Epic> toExpire, List<string> log)
        {
            foreach (var epic in toExpire)
            {
                epic.IsActive = false;
                var status = dbContext.Entry(epic);
                if (status.State == EntityState.Modified)
                {
                    log.Add($"Expired Epic {epic.Id} {epic.Name} ({Modified(status.Properties)})");
                }
            }
        }

        private static EpicCsv Map(CsvReader csv, string Id)
        {
            var newEpic = new EpicCsv()
            {
                Id = Id,
                Name = csv.GetField<string>("Epic Name"),
            };
            newEpic.Capabilities.Add(new CapabilityEpicCsv()
            {
                CapabilityId = new CapabilityIdCsv(csv.GetField<string>("Capability ID")),
                Level = ParseLevel(csv.GetField<string>("Epic Level")),
            });
            return newEpic;
        }

        private static async Task Process(BuyingCatalogueDbContext dbContext, EpicCsv epic, List<string> log)
        {
            var existing = await dbContext.Epics
                .Include(c => c.Capabilities)
                .FirstOrDefaultAsync(c => c.Id == epic.Id);

            if (existing == null)
            {
                await Add(dbContext, epic, log);
            }
            else
            {
                Update(dbContext, epic, existing, log);
            }
        }

        private static void Update(BuyingCatalogueDbContext dbContext, EpicCsv desiredEpic, Epic existingEpic, List<string> log)
        {
            existingEpic.Name = desiredEpic.Name;
            existingEpic.IsActive = true;

            var desiredCapabilityEpics = desiredEpic.Capabilities.ToList();

            foreach (var capabilityEpic in existingEpic.CapabilityEpics.ToList())
            {
                var toUpdate = desiredCapabilityEpics
                    .Where(ce => ce.CapabilityId.Value == capabilityEpic.CapabilityId)
                    .FirstOrDefault();

                if (toUpdate == null)
                {
                    desiredCapabilityEpics.Remove(toUpdate);
                    existingEpic.CapabilityEpics.Remove(capabilityEpic);
                }
                else
                {
                    desiredCapabilityEpics.Remove(toUpdate);
                    capabilityEpic.CompliancyLevel = toUpdate.Level;
                }
            }

            desiredCapabilityEpics
                .ForEach(ce => existingEpic.CapabilityEpics.Add(new CapabilityEpic()
                {
                    CapabilityId = ce.CapabilityId.Value,
                    EpicId = existingEpic.Id,
                    CompliancyLevel = ce.Level
                }));

            var status = dbContext.Entry(existingEpic);
            if (status.State == EntityState.Modified)
            {
                log.Add($"Modified Epic {existingEpic.Id} {existingEpic.Name} ({Modified(status.Properties)})");
            }
        }

        private static string Modified(IEnumerable<PropertyEntry> properties)
        {
            return properties.Where(p => p.IsModified)
                .Aggregate("", (a, b) => $"{a}{b.Metadata.Name}, ");
        }

        private static async Task Add(BuyingCatalogueDbContext dbContext, EpicCsv epic, List<string> log)
        {
            var newEpic = new Epic()
            {
                Id = epic.Id,
                Name = epic.Name,
                IsActive = true,
            };

            epic.Capabilities
                .Select(c =>
                    new CapabilityEpic()
                    {
                        CapabilityId = c.CapabilityId.Value,
                        EpicId = epic.Id,
                        CompliancyLevel = c.Level
                    })
                .ForEach(ce => newEpic.CapabilityEpics.Add(ce));

            await dbContext.Epics.AddAsync(newEpic);
            log.Add($"New Epic {newEpic.Id} {newEpic.Name}");
        }

        private static CompliancyLevel ParseLevel(string? level)
        {
            ArgumentNullException.ThrowIfNull(level);

            return level.Trim().ToUpperInvariant() switch
            {
                "MUST" => CompliancyLevel.Must,
                "SHOULD" => CompliancyLevel.Should,
                "MAY" => CompliancyLevel.May,
                { } val => throw new InvalidOperationException($"Invalid Compliance Level specified {val}")
            };
        }
    }
}
