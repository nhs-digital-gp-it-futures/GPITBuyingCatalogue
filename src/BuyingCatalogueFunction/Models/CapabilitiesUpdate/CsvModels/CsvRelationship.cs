using CsvHelper.Configuration;

namespace BuyingCatalogueFunction.Models.CapabilitiesUpdate.CsvModels;

public class CsvRelationship
{
    public string FromId { get; set; }

    public string ToId { get; set; }
}

public sealed class CsvRelationshipMap : ClassMap<CsvRelationship>
{
    public CsvRelationshipMap()
    {
        Map(x => x.FromId).Name("FromID");
        Map(x => x.ToId).Name("ToID");
    }
}
