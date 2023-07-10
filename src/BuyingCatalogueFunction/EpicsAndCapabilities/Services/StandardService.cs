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
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace BuyingCatalogueFunction.EpicsAndCapabilities.Services
{
    public class StandardService : IStandardService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public StandardService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<string>> Process(List<StandardCsv> standards)
        {
            ArgumentNullException.ThrowIfNull(standards);

            var log = new List<string>();

            foreach (var capability in standards)
            {
                await Process(dbContext, capability, log);
            }

            var desiredStandardIds = standards.Select(s => s.Id).ToList();
            var toDelete = await dbContext
                .Standards
                .Where(s => !desiredStandardIds.Any(d => d == s.Id))
                .ToListAsync();

            ProcessDeletions(toDelete, log);

            await dbContext.SaveChangesAsync();
            return log;
        }

        public async Task<List<StandardCsv>> Read(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            using var streamReader = new StreamReader(stream);
            using var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim,
            });

            var standards = new List<StandardCsv>();
            await csv.ReadAsync();
            csv.ReadHeader();
            while (await csv.ReadAsync())
            {
                // ID,Name,Version,Type,URL,Description,Framework
                var standard = Map(csv);
                standards.Add(standard);
            }

            return standards;
        }

        private void ProcessDeletions(List<Standard> toDelete, List<string> log)
        {
            foreach (var standard in toDelete)
            {
                standard.IsDeleted = true;
                var status = dbContext.Entry(standard);
                if (status.State == EntityState.Modified)
                {
                    log.Add($"Soft Deleted Standard {standard.Id} {standard.Name} ({Modified(status.Properties)})");
                }
            }
        }

        private static StandardCsv Map(CsvReader csv)
        {
            return new StandardCsv()
            {
                Id = csv.GetField<string>("ID"),
                Name = csv.GetField<string>("Name"),
                Version = csv.GetField<string>("Version"),
                StandardType = ParseStandardType(csv.GetField<string>("Type")),
                Url = csv.GetField<string>("URL"),
                Description = csv.GetField<string>("Description"),
            };
        }

        private static async Task Process(BuyingCatalogueDbContext dbContext, StandardCsv standard, List<string> log)
        {
            var existing = await dbContext.Standards
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == standard.Id);

            if (existing == null)
            {
                await Add(dbContext, standard, log);
            }
            else
            {
                Update(dbContext, standard, existing, log);
            }
        }

        private static async Task Add(BuyingCatalogueDbContext dbContext, StandardCsv standard, List<string> log)
        {
            var newStandard = new Standard()
            {
                Id = standard.Id,
                Name = standard.Name,
                Description = standard.Description,
                Version = standard.Version,
                Url = standard.Url,
                StandardType = standard.StandardType,
                IsDeleted = false,
            };

            await dbContext.Standards.AddAsync(newStandard);
            log.Add($"New Standard {newStandard.Id} {newStandard.Name}");
        }

        private static void Update(BuyingCatalogueDbContext dbContext, StandardCsv standard, Standard existing, List<string> log)
        {
            existing.Name = standard.Name;
            existing.Description = standard.Description;
            existing.Version = standard.Version;
            existing.Url = standard.Url;
            existing.StandardType = standard.StandardType;
            existing.IsDeleted = false;

            var status = dbContext.Entry(existing);
            if (status.State == EntityState.Modified)
            {
                log.Add($"Modified Standard {existing.Id} {existing.Name} ({Modified(status.Properties)})");
            }
        }

        private static string Modified(IEnumerable<PropertyEntry> properties)
        {
            return properties.Where(p => p.IsModified)
                .Aggregate("", (a, b) => $"{a}{b.Metadata.Name}, ");
        }

        private static StandardType ParseStandardType(string value)
        {
            return value.Trim() switch
            {
                "Capability Specific Standard" => StandardType.Capability,
                "Interop Standard" => StandardType.Interoperability,
                "Overarching Standard" => StandardType.Overarching,
                "Context Specific Standard" => StandardType.ContextSpecific,
                { } val => throw new InvalidOperationException($"Invalid standard type specified: {val}")
            };
        }
    }
}
