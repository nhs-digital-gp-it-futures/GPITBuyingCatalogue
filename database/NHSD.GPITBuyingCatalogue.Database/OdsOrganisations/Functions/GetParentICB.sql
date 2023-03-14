CREATE FUNCTION ods_organisations.GetParentICB (@orderingPartyId int)
RETURNS int AS
BEGIN
	return (select Top(1) o2.Id from organisations.Organisations o 
	inner join [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] ors
	on o.ExternalIdentifier = ors.TargetOrganisationId
	inner join organisations.Organisations o2
	on ors.OwnerOrganisationId = o2.ExternalIdentifier
	where o.Id = @orderingPartyId
	and o.PrimaryRoleId = 'RO98'
	and ors.RelationshipTypeId = 'RE5'
	and o2.PrimaryRoleId = 'RO261')
END;
GO
