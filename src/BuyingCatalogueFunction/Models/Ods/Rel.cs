using System.Collections.Generic;

namespace BuyingCatalogueFunction.Models.Ods;

public class Rel
{
    public List<Date> Date { get; set; }
    public string Status { get; set; }
    public Target Target { get; set; }
    public string id { get; set; }
    public int uniqueRelId { get; set; }
}