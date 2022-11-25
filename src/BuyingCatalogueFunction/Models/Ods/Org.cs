using System.Collections.Generic;

namespace BuyingCatalogueFunction.Models.Ods;

public class Org
{
    public const string Active = "Active";

    public string Name { get; set; }
    public List<Date> Date { get; set; }
    public OrgId OrgId { get; set; }
    public string Status { get; set; }
    public string LastChangeDate { get; set; }
    public string orgRecordClass { get; set; }
    public GeoLoc GeoLoc { get; set; }
    public Contacts Contacts { get; set; }
    public OrgRoles Roles { get; set; }
    public OrgRels Rels { get; set; }
}
