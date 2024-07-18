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
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly ILogger<CapabilityService> logger;

        public CapabilityService(
            BuyingCatalogueDbContext dbContext,
            ILogger<CapabilityService> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
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
                // ID,Name,Capability Category,URL,Description,Framework
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

            CapabilityCategory capabilityCategory = await EnsureCategroy(dbContext, capability.Category, log);
            List<Framework> desiredFrameworks = ValidateFrameworks(dbContext, capability);

            if (existing == null)
            {
                await Add(dbContext, capability, capabilityCategory, desiredFrameworks, log);
            }
            else
            {
                Update(dbContext, capability, existing, capabilityCategory, desiredFrameworks, log);
            }
        }

        private List<Framework> ValidateFrameworks(BuyingCatalogueDbContext dbContext, CapabilityCsv capability)
        {
            var desiredFrameworks = dbContext.Frameworks
                .Where(f => capability.Framework.Contains(f.ShortName))
                .ToList();

            var missing = capability.Framework.Except(desiredFrameworks.Select(f => f.ShortName));

            if (missing.Any())
            {
                logger.LogWarning(
                    "Missing framework(s) {frameworks} for capability {capability}",
                    missing.Aggregate((a, b) => $"{a},{b}"),
                    capability.Id.Value);
            }

            return desiredFrameworks;
        }

        private static async Task<CapabilityCategory> EnsureCategroy(BuyingCatalogueDbContext dbContext, string categoryName, List<string> log)
        {
            var category = await dbContext.CapabilityCategories.FirstOrDefaultAsync(x => x.Name == categoryName);
            if (category == null)
            {
                category = new CapabilityCategory()
                {
                    Name = categoryName,
                };
                await dbContext.CapabilityCategories.AddAsync(category);
                await dbContext.SaveChangesAsync();
                log.Add($"New Capability Category {category.Id} {categoryName}");
            }
            return category;
        }

        private async Task Add(
            BuyingCatalogueDbContext dbContext,
            CapabilityCsv capability,
            CapabilityCategory capabilityCategory,
            List<Framework> desiredFrameworks,
            List<string> log)
        {
            var newCapability = new Capability()
            {
                Id = capability.Id.Value,
                Name = capability.Name,
                Description = capability.Description,
                SourceUrl = capability.Url,
                CategoryId = capabilityCategory.Id,
                Status = CapabilityStatus.Effective,
            };

            foreach (var frameworkToAdd in desiredFrameworks)
            {
                newCapability.FrameworkCapabilities.Add(new FrameworkCapability(frameworkToAdd.Id, capability.Id.Value));
            }

            await dbContext.Capabilities.AddAsync(newCapability);
            log.Add($"New Capability {newCapability.Id} {newCapability.Name}");
        }

        private void Update(
            BuyingCatalogueDbContext dbContext,
            CapabilityCsv capability,
            Capability existing,
            CapabilityCategory capabilityCategory,
            List<Framework> desiredFrameworks,
            List<string> log)
        {
            existing.Name = capability.Name;
            existing.Description = capability.Description;
            existing.SourceUrl = capability.Url;
            existing.CategoryId = capabilityCategory.Id;
            existing.Status = CapabilityStatus.Effective;

            var desiredFrameworkIds = desiredFrameworks.Select(f => f.Id);

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
