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
using Microsoft.Extensions.Logging;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace BuyingCatalogueFunction.EpicsAndCapabilities.Services
{
    public class CapabilityService : ICapabilityService
    {
        private readonly ILogger<CapabilityService> logger;
        private readonly BuyingCatalogueDbContext dbContext;

        public CapabilityService(
            ILogger<CapabilityService> logger,
            BuyingCatalogueDbContext dbContext
            )
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        public async Task<List<string>> Process(List<CapabilityCsv> capabilties)
        {
            ArgumentNullException.ThrowIfNull(capabilties);

            var log = new List<string>();

            foreach (var capability in capabilties)
            {
                await Process(dbContext, capability, log);
            }

            var desiredCapabilties = capabilties.Select(s => s.Id.Value).ToList();
            var toExpire = await dbContext
                .Capabilities
                .Where(s => !desiredCapabilties.Any(d => d == s.Id))
                .ToListAsync();

            ProcessExpired(toExpire, log);

            await dbContext.SaveChangesAsync();
            return log;
        }

        public async Task<List<CapabilityCsv>> Read(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            using var streamReader = new StreamReader(stream);
            using var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim,
            });

            var capabilities = new List<CapabilityCsv>();
            await csv.ReadAsync();
            csv.ReadHeader();
            while (await csv.ReadAsync())
            {
                // ID,Name,Version,Capability Category,URL,Description,Framework
                var capability = Map(csv);
                capabilities.Add(capability);
            }

            return capabilities;
        }

        private void ProcessExpired(List<Capability> toExpire, List<string> log)
        {
            foreach (var capability in toExpire)
            {
                capability.Status = CapabilityStatus.Expired;
                var status = dbContext.Entry(capability);
                if (status.State == EntityState.Modified)
                {
                    log.Add($"Expired Capability {capability.Id} {capability.Name} ({Modified(status.Properties)})");
                }
            }
        }

        private static CapabilityCsv Map(CsvReader csv)
        {
            return new CapabilityCsv()
            {
                Id = new CapabilityIdCsv(csv.GetField<string>("ID")),
                Name = csv.GetField<string>("Name"),
                Version = csv.GetField<string>("Version"),
                Category = csv.GetField<string>("Capability Category"),
                Url = csv.GetField<string>("URL"),
                Description = csv.GetField<string>("Description"),
                Framework = ParseFrameworks(csv.GetField<string>("Framework")),
            };
        }

        private async Task Process(BuyingCatalogueDbContext dbContext, CapabilityCsv capability, List<string> log)
        {
            var existing = await dbContext.Capabilities
                .Include(c => c.FrameworkCapabilities)
                .FirstOrDefaultAsync(c => c.Id == capability.Id.Value);

            var capabilityCategory = await dbContext.CapabilityCategories.FirstOrDefaultAsync(x => x.Name == capability.Category)
                ?? throw new InvalidOperationException($"No category found for {capability.Id.Value} using {capability.Category}");

            if (existing == null)
            {
                await Add(dbContext, capability, capabilityCategory, log);
            }
            else
            {
                Update(dbContext, capability, existing, capabilityCategory, log);
            }
        }

        private async Task Add(BuyingCatalogueDbContext dbContext, CapabilityCsv capability, CapabilityCategory capabilityCategory, List<string> log)
        {
            var newCapability = new Capability()
            {
                Id = capability.Id.Value,
                Name = capability.Name,
                Description = capability.Description,
                Version = capability.Version,
                SourceUrl = capability.Url,
                CategoryId = capabilityCategory.Id,
                Status = CapabilityStatus.Effective,
            };

            foreach (var shortName in capability.Framework)
            {
                var frameworkToAdd = dbContext.Frameworks
                    .FirstOrDefault(f => f.ShortName == shortName);

                if (frameworkToAdd == null)
                {
                    throw new InvalidOperationException($"Framework not found {shortName}");
                }

                newCapability.FrameworkCapabilities.Add(new FrameworkCapability(frameworkToAdd.Id, capability.Id.Value));
            }

            await dbContext.Capabilities.AddAsync(newCapability);
            log.Add($"New Capability {newCapability.Id} {newCapability.Name}");
        }

        private void Update(BuyingCatalogueDbContext dbContext, CapabilityCsv capability, Capability existing, CapabilityCategory capabilityCategory, List<string> log)
        {
            existing.Name = capability.Name;
            existing.Description = capability.Description;
            existing.Version = capability.Version;
            existing.SourceUrl = capability.Url;
            existing.CategoryId = capabilityCategory.Id;
            existing.Status = CapabilityStatus.Effective;

            var desiredFrameworkIds = dbContext.Frameworks
                .Where(f => capability.Framework.Contains(f.ShortName))
                .Select(f => f.Id)
                .ToList();

            var existingFrameworkIds = existing.FrameworkCapabilities.Select(f => f.FrameworkId);

            var toAdd = desiredFrameworkIds.Except(existingFrameworkIds);
            var toRemove = existingFrameworkIds.Except(desiredFrameworkIds);

            toAdd.Select(f => new FrameworkCapability(f, existing.Id))
                 .ForEach(f => existing.FrameworkCapabilities.Add(f));

            existing.FrameworkCapabilities
                .Where(f => toRemove.Contains(f.FrameworkId))
                .ForEach(f =>
                {
                    existing.FrameworkCapabilities.Remove(f);
                    dbContext.FrameworkCapabilities.Remove(f);
                });


            var status = dbContext.Entry(existing);
            if (status.State == EntityState.Modified)
            {
                log.Add($"Modified Capability {existing.Id} {existing.Name} ({Modified(status.Properties)})");
            }
        }

        private static string Modified(IEnumerable<PropertyEntry> properties)
        {
            return properties.Where(p => p.IsModified)
                .Aggregate("", (a, b) => $"{a}{b.Metadata.Name}, ");
        }

        private static List<string> ParseFrameworks(string frameworks)
        {
            ArgumentNullException.ThrowIfNull(frameworks);

            return frameworks.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }

}
