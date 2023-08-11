using System.Collections.Generic;

namespace BuyingCatalogueFunction.IncrementalUpdate.Models.Ods;

public class OrgRole
{
    public string id { get; set; }
    public int uniqueRoleId { get; set; }
    public List<Date> Date { get; set; }
    public string Status { get; set; }
    public bool? primaryRole { get; set; }
}
