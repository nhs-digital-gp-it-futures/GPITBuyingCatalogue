﻿using System.Collections.Generic;

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
}
