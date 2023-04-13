IF '$(INSERT_TEST_DATA)' = 'True'
AND NOT EXISTS ( SELECT * FROM organisations.RelatedOrganisations )
BEGIN
    DECLARE @relatedOrgId INT = (SELECT Id FROM organisations.Organisations WHERE PrimaryRoleId = 'RO261' AND ExternalIdentifier = 'QOQ');
    DECLARE @orgId INT = (SELECT Id FROM organisations.Organisations WHERE PrimaryRoleId = 'RO261' AND ExternalIdentifier = 'QWO');
    DECLARE @userId INT = (SELECT Id FROM users.AspNetUsers WHERE Email = N'BobSmith@email.com');

    INSERT organisations.RelatedOrganisations ( OrganisationId, RelatedOrganisationId, LastUpdated, LastUpdatedBy )
    SELECT @relatedOrgId, @orgId, GETUTCDATE(), @userId
END;
