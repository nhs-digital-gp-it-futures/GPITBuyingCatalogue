namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

public class OrganisationRelationship
{
    public int Id { get; set; }

    public string RelationshipTypeId { get; set; }

    public string OwnerOrganisationId { get; set; }

    public string TargetOrganisationId { get; set; }

    public virtual OdsOrganisation OwnerOrganisation { get; set; }

    public virtual OdsOrganisation TargetOrganisation { get; set; }

    public virtual RelationshipType RelationshipType { get; set; }
}
