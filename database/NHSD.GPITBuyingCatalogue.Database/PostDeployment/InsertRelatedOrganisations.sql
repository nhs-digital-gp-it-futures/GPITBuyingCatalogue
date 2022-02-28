IF '$(INSERT_TEST_DATA)' = 'True'
AND NOT EXISTS ( SELECT * FROM organisations.RelatedOrganisations )
BEGIN
    DECLARE @northLincsOrgId INT = (SELECT Id FROM organisations.Organisations WHERE PrimaryRoleId = 'RO98' AND ExternalIdentifier = '03K');
    DECLARE @hullOrgId INT = (SELECT Id FROM organisations.Organisations WHERE PrimaryRoleId = 'RO98' AND ExternalIdentifier = '03F');
    DECLARE @userId INT = (SELECT Id FROM users.AspNetUsers WHERE Email = N'BobSmith@email.com');

    INSERT organisations.RelatedOrganisations ( OrganisationId, RelatedOrganisationId, LastUpdated, LastUpdatedBy )
    SELECT @northLincsOrgId, @hullOrgId, GETUTCDATE(), @userId
END;
