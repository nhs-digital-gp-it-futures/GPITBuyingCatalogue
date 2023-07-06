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
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace BuyingCatalogueFunction.EpicsAndCapabilities.Services
{
    public class StandardCapabilityService : IStandardCapabilityService
    {
        private readonly ILogger<StandardCapabilityService> logger;
        private readonly BuyingCatalogueDbContext dbContext;

        public StandardCapabilityService(
            ILogger<StandardCapabilityService> logger,
            BuyingCatalogueDbContext dbContext
            )
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        public async Task<List<string>> Process(List<StandardCapabilityCsv> standardCapabilities)
        {
            ArgumentNullException.ThrowIfNull(standardCapabilities);

            var log = new List<string>();

            var existing = await dbContext.StandardCapabilities.ToListAsync();
            var desired = standardCapabilities
                .Select(x => new StandardCapability()
                {
                    CapabilityId = x.FromId.Value,
                    StandardId = x.ToId
                })
                .ToList();

            var toAdd = desired
                .Where(d => !existing.Any(e => e.CapabilityId == d.CapabilityId && e.StandardId == d.StandardId))
                .ToList();

            var toRemove = existing
                .Where(e => !desired.Any(d => d.CapabilityId == e.CapabilityId && d.StandardId == e.StandardId))
                .ToList();

            dbContext.StandardCapabilities.RemoveRange(toRemove);
            dbContext.StandardCapabilities.AddRange(toAdd);

            toAdd.ForEach(a => log.Add($"Adding StandardCapability {a.CapabilityId} {a.StandardId}"));
            toRemove.ForEach(a => log.Add($"Removing StandardCapability {a.CapabilityId} {a.StandardId}"));

            await dbContext.SaveChangesAsync();
            return log;
        }

        public async Task<List<StandardCapabilityCsv>> Read(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            using var streamReader = new StreamReader(stream);
            using var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim,
            });

            var standardCapabilities = new List<StandardCapabilityCsv>();
            await csv.ReadAsync();
            csv.ReadHeader();
            while (await csv.ReadAsync())
            {
                if (csv.GetField<string>("FromID").StartsWith("C"))
                {
                    // From,To,FromID,ToID,Type,IsOptional
                    standardCapabilities.Add(Map(csv));
                }
            }

            return standardCapabilities;
        }

        private static StandardCapabilityCsv Map(CsvReader csv)
        {
            return new StandardCapabilityCsv()
            {
                FromId = new CapabilityIdCsv(csv.GetField<string>("FromID")),
                ToId = csv.GetField<string>("ToID"),
            };
        }
    }
}
