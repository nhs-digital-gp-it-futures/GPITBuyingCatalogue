DECLARE @createUser AS nvarchar(4) = '$(CREATE_EA_USER)';
DECLARE @email AS nvarchar(256) = '$(EA_USER_EMAIL)';

IF UPPER(@createUser) = 'TRUE'
AND NOT EXISTS (
    SELECT *
      FROM dbo.AspNetUsers
      WHERE UserName = @email)
BEGIN
    DECLARE @firstName AS nvarchar(50) = '$(EA_USER_FIRST_NAME)';
    DECLARE @lastName AS nvarchar(50) = '$(EA_USER_LAST_NAME)';
    DECLARE @normalizedUserName AS nvarchar(256) = UPPER(@email);
    DECLARE @passwordHash AS nvarchar(max) = '$(EA_USER_PASSWORD_HASH)';
    DECLARE @phoneNumber AS nvarchar(max) = '$(EA_USER_PHONE)';

    DECLARE @executiveAgencyRoleId AS nchar(5) = 'RO116';
    DECLARE @organisationId AS uniqueidentifier = (SELECT OrganisationId FROM dbo.Organisations WHERE PrimaryRoleId = @executiveAgencyRoleId);

    INSERT INTO dbo.AspNetUsers
    (
        Id, UserName, NormalizedUserName, PasswordHash, 
        FirstName, LastName, Email, NormalizedEmail, EmailConfirmed, PhoneNumber, PhoneNumberConfirmed,
        PrimaryOrganisationId, OrganisationFunction, CatalogueAgreementSigned, [Disabled],
        AccessFailedCount, ConcurrencyStamp, LockoutEnabled, SecurityStamp, TwoFactorEnabled
    )
	VALUES
	(CAST(NEWID() AS nchar(36)), @email, @normalizedUserName, @passwordHash,
        @firstName, @lastName, @email, @normalizedUserName, 1, @phoneNumber, 1,
        @organisationId, 'Authority', 1, 0,
        0, NEWID(), 1, NEWID(), 0);
END;
GO
