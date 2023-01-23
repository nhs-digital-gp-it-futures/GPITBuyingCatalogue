using System;
using CsvHelper.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace BuyingCatalogueFunction.Models.CapabilitiesUpdate.CsvModels;

public class CsvEpic
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Status { get; set; }

    public string Level { get; set; }

    public string CapabilityId { get; set; }

    public CompliancyLevel GetCompliancyLevel()
    {
        return Level.Trim().ToUpperInvariant() switch
        {
            "MUST" => CompliancyLevel.Must,
            "SHOULD" => CompliancyLevel.Should,
            "MAY" => CompliancyLevel.May,
            { } val => throw new InvalidOperationException($"Invalid Compliance Level specified {val}")
        };
    }
}

public sealed class CsvEpicMap : ClassMap<CsvEpic>
{
    public CsvEpicMap()
    {
        Map(x => x.Id).Name("Epic ID");
        Map(x => x.Name).Name("Epic Name");
        Map(x => x.Status).Name("Epic Status");
        Map(x => x.Level).Name("Epic Level");
        Map(x => x.CapabilityId).Name("Capability ID");
    }
}
