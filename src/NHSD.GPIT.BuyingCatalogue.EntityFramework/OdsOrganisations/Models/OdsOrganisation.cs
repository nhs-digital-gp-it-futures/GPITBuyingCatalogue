using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

public partial class OdsOrganisation
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string AddressLine1 { get; set; }

    public string AddressLine2 { get; set; }

    public string AddressLine3 { get; set; }

    public string Town { get; set; }

    public string County { get; set; }

    public string Postcode { get; set; }

    public string Country { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<OrganisationRelationship> Related { get; set; } =
        new HashSet<OrganisationRelationship>();

    public virtual ICollection<OrganisationRelationship> Parents { get; set; } =
        new HashSet<OrganisationRelationship>();

    public virtual ICollection<OrganisationRole> Roles { get; set; } = new HashSet<OrganisationRole>();

    public void UpdateFrom(OdsOrganisation source)
    {
        if (source == null)
        {
            return;
        }

        Name = source.Name;
        IsActive = source.IsActive;
        AddressLine1 = source.AddressLine1;
        AddressLine2 = source.AddressLine2;
        AddressLine3 = source.AddressLine3;
        Town = source.Town;
        County = source.County;
        Postcode = source.Postcode;
        Country = source.Country;
    }
}
