namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

public class OrganisationRole
{
    public OrganisationRole()
    {
    }

    public OrganisationRole(
        string organisationId,
        string roleId)
    {
        OrganisationId = organisationId;
        RoleId = roleId;
    }

    public int Id { get; set; }

    public string OrganisationId { get; set; }

    public string RoleId { get; set; }

    public bool IsPrimaryRole { get; set; }

    public virtual OdsOrganisation Organisation { get; set; }

    public virtual RoleType RoleType { get; set; }
}
