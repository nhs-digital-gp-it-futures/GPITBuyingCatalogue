using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

public class OdsOrganisation
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string AddressLine1 { get; set; }

    public string AddressLine2 { get; set; }

    public string AddressLine3 { get; set; }

    public string AddressLine4 { get; set; }

    public string AddressLine5 { get; set; }

    public string Town { get; set; }

    public string County { get; set; }

    public string Postcode { get; set; }

    public string Country { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<OrganisationRelationship> Related { get; set; }

    public virtual OrganisationRelationship Parent { get; set; }
}
