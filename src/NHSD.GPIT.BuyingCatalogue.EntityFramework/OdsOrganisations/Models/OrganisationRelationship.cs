namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

public class OrganisationRelationship
{
    public int Id { get; set; }

    public string RelationshipTypeId { get; set; }

    public int OwnerOrganisationId { get; set; }

    public int TargetOrganisationId { get; set; }

    public virtual OdsOrganisation OwnerOrganisation { get; set; }

    public virtual OdsOrganisation TargetOrganisation { get; set; }

    public virtual RelationshipType RelationshipType { get; set; }
}
