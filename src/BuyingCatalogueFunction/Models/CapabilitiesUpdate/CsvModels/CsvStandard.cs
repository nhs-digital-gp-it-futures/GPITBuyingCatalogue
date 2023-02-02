using System;
using CsvHelper.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace BuyingCatalogueFunction.Models.CapabilitiesUpdate.CsvModels;

public class CsvStandard
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Version { get; set; }

    public string Type { get; set; }

    public string Url { get; set; }

    public string Description { get; set; }

    public string Framework { get; set; }

    public StandardType GetStandardType()
    {
        return Type.Trim() switch
        {
            "Capability Specific Standard" => StandardType.Capability,
            "Interop Standard" => StandardType.Interoperability,
            "Overarching Standard" => StandardType.Overarching,
            "Context Specific Standard" => StandardType.ContextSpecific,
            { } val => throw new InvalidOperationException($"Invalid standard type specified: {val}")
        };
    }
}

public sealed class CsvStandardMap : ClassMap<CsvStandard>
{
    public CsvStandardMap()
    {
        Map(x => x.Id).Name("ID");
        Map(x => x.Name);
        Map(x => x.Version);
        Map(x => x.Type);
        Map(x => x.Url).Name("URL");
        Map(x => x.Description);
        Map(x => x.Framework);
    }
}
