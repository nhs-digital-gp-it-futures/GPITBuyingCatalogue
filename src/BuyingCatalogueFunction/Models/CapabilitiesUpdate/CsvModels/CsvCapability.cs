using CsvHelper.Configuration;

namespace BuyingCatalogueFunction.Models.CapabilitiesUpdate.CsvModels;

public class CsvCapability
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Version { get; set; }

    public string Category { get; set; }

    public string Url { get; set; }

    public string Description { get; set; }

    public string Framework { get; set; }
}

public sealed class CsvCapabilityMap : ClassMap<CsvCapability>
{
    public CsvCapabilityMap()
    {
        Map(x => x.Id).Name("ID");
        Map(x => x.Name);
        Map(x => x.Version);
        Map(x => x.Category).Name("Capability Category");
        Map(x => x.Url).Name("URL");
        Map(x => x.Description);
        Map(x => x.Framework);
    }
}
