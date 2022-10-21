namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

public class OrganisationRole
{
    public int UniqueRoleId { get; set; }

    public int OrganisationId { get; set; }

    public string RoleId { get; set; }

    public bool IsPrimaryRole { get; set; }

    public virtual OdsOrganisation Organisation { get; set; }

    public virtual RoleType RoleType { get; set; }
}
