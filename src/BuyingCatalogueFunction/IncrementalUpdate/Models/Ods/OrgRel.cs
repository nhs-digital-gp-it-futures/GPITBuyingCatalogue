using System.Collections.Generic;

namespace BuyingCatalogueFunction.IncrementalUpdate.Models.Ods;

public class OrgRel
{
    public List<Date> Date { get; set; }
    public string Status { get; set; }
    public Target Target { get; set; }
    public string id { get; set; }
    public int uniqueRelId { get; set; }
}
