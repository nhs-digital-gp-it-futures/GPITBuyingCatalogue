DECLARE @aliceEmail AS nvarchar(50) = N'AliceSmith@email.com';
DECLARE @bobEmail AS nvarchar(50) = N'BobSmith@email.com';
DECLARE @sueEmail AS nvarchar(50) = N'SueSmith@email.com';
DECLARE @daveEmail AS nvarchar(50) = N'DaveSmith@email.com';

IF '$(INSERT_TEST_DATA)' = 'True'
AND NOT EXISTS (
  SELECT *
  FROM users.AspNetUsers
  WHERE UserName IN (@aliceEmail, @bobEmail, @sueEmail, @daveEmail))
BEGIN
    DECLARE @ccgRoleId AS nchar(4) = 'RO98';
    DECLARE @executiveAgencyRoleId AS nchar(5) = 'RO116';
    DECLARE @hullCCGOdsCode AS nchar(3) = '03F';
    DECLARE @northLincsCcgOdsCode AS nchar(3) = '03K';

    DECLARE @aliceOrganisationId AS int = (SELECT TOP (1) Id FROM organisations.Organisations WHERE PrimaryRoleId = @ccgRoleId AND ExternalIdentifier = @northLincsCcgOdsCode);
    DECLARE @aliceOrganisationName AS nvarchar(255) =  (SELECT TOP (1) [Name] FROM organisations.Organisations WHERE PrimaryRoleId = @ccgRoleId AND ExternalIdentifier = @northLincsCcgOdsCode);

    DECLARE @bobOrganisationId AS int = (SELECT Id FROM organisations.Organisations WHERE PrimaryRoleId = @executiveAgencyRoleId);
    DECLARE @bobOrganisationName AS nvarchar(255) =  (SELECT TOP (1) [Name] FROM organisations.Organisations WHERE PrimaryRoleId = @executiveAgencyRoleId);

    DECLARE @sueOrganisationId AS int = (SELECT TOP (1) Id FROM organisations.Organisations WHERE PrimaryRoleId = @ccgRoleId AND ExternalIdentifier = @hullCCGOdsCode);
    DECLARE @sueOrganisationName AS nvarchar(255) =  (SELECT TOP (1) [Name] FROM organisations.Organisations WHERE PrimaryRoleId = @ccgRoleId AND ExternalIdentifier = @hullCCGOdsCode);

    DECLARE @daveOrganisationId AS int = (SELECT TOP (1) Id FROM organisations.Organisations WHERE PrimaryRoleId = @ccgRoleId AND ExternalIdentifier = @hullCCGOdsCode);
    DECLARE @daveOrganisationName AS nvarchar(255) =  (SELECT TOP (1) [Name] FROM organisations.Organisations WHERE PrimaryRoleId = @ccgRoleId AND ExternalIdentifier = @hullCCGOdsCode);

    DECLARE @address AS nchar(108) = N'{ "street_address": "One Hacker Way", "locality": "Heidelberg", "postal_code": 69118, "country": "Germany" }';

    DECLARE @bobId AS int = 2;
    DECLARE @sueId AS int = 3;
    DECLARE @aliceId AS int = 4;
    DECLARE @daveId AS int = 5;

    DECLARE @aliceNormalizedEmail AS nvarchar(50) = UPPER(@aliceEmail);
    DECLARE @bobNormalizedEmail AS nvarchar(50) = UPPER(@bobEmail);
    DECLARE @sueNormalizedEmail AS nvarchar(50) = UPPER(@sueEmail);
    DECLARE @daveNormalizedEmail AS nvarchar(50) = UPPER(@daveEmail);

    DECLARE @phoneNumber AS nvarchar(max) = '01234567890';

    -- 'Pass123$'
    DECLARE @alicePassword AS nvarchar(200) = N'AQAAAAEAACcQAAAAEFSsEthAqGVBLj1P1gF9puxtXm18lKHlmuh9J/Ib0KKBO3GjQvxymJbzpSqy0zuOHg==';

    -- 'Pass123$'
    DECLARE @bobPassword AS nvarchar(200) = N'AQAAAAEAACcQAAAAEOzr1Zwpoo1pKsTa+S65mBZVG4GIy6IYH/IAED6TvBA+FIMg8u/xb0b/cfexV7SHNw==';

    -- 'Pass123$'
    DECLARE @suePassword AS nvarchar(200) =  N'AQAAAAEAACcQAAAAEBRpg4kCDtF5H4UEgv209hSD0TmaRx9JOYorAzNHxzfyZisIDse2AlTA0oF28HlBhQ==';

    -- 'Pass123$'
    DECLARE @davePassword AS nvarchar(200) =  N'AQAAAAEAACcQAAAAEBRpg4kCDtF5H4UEgv209hSD0TmaRx9JOYorAzNHxzfyZisIDse2AlTA0oF28HlBhQ==';

    SET IDENTITY_INSERT users.AspNetUsers ON;

    INSERT INTO users.AspNetUsers
    (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail, AccessFailedCount, ConcurrencyStamp, PhoneNumber,
        EmailConfirmed, LockoutEnabled, PasswordHash, PhoneNumberConfirmed, SecurityStamp, TwoFactorEnabled,
        FirstName, LastName, PrimaryOrganisationId, [Disabled], CatalogueAgreementSigned, AcceptedTermsOfUseDate
    )
    VALUES
    (@aliceId, @aliceEmail, @aliceNormalizedEmail, @aliceEmail, @aliceNormalizedEmail, 0, NEWID(), @phoneNumber, 1, 1, @alicePassword, 0, 'NNJ4SLBPCVUDKXAQXJHCBKQTFEYUAPBC', 0, 'Alice', 'Smith', @aliceOrganisationId, 0, 1, GETUTCDATE()),
    (@bobId, @bobEmail, @bobNormalizedEmail, @bobEmail, @bobNormalizedEmail, 0, NEWID(), @phoneNumber, 1, 1, @bobPassword, 0, 'OBDOPOU5YQ5WQXCR3DITKL6L5IDPYHHJ', 0, 'Bob', 'Smith', @bobOrganisationId, 0, 1, GETUTCDATE()),
    (@sueId, @sueEmail, @sueNormalizedEmail, @sueEmail, @sueNormalizedEmail, 0, NEWID(), @phoneNumber, 1, 1, @suePassword, 0, 'NNJ4SLBPCVUDKXAQXJHCBKQTFEYUAPBC', 0, 'Sue', 'Smith', @sueOrganisationId, 0, 1, GETUTCDATE()),
    (@daveId, @daveEmail, @daveNormalizedEmail, @daveEmail, @daveNormalizedEmail, 0, NEWID(), @phoneNumber, 1, 1, @davePassword, 0, 'NNJ4SLBPCVUDKXAQXJHCBKQTFEYUAPBC', 0, 'Dave', 'Smith', @daveOrganisationId, 0, 1, GETUTCDATE());
    
    DECLARE @BuyerRoleId INT = (SELECT [Id] FROM users.AspNetRoles WHERE [Name] = 'Buyer');
    DECLARE @AdminRoleId INT = (SELECT [Id] FROM users.AspNetRoles WHERE [Name] = 'Authority');
    DECLARE @AccountManagerRoleId INT = (SELECT [Id] FROM users.AspNetRoles WHERE [Name] = 'AccountManager');

    INSERT INTO users.AspNetUserRoles(RoleId, UserId) VALUES (@BuyerRoleId, @aliceId), (@BuyerRoleId, @sueId);
    INSERT INTO users.AspNetUserRoles(RoleId, UserId) VALUES (@AdminRoleId, @bobId);
    INSERT INTO users.AspNetUserRoles(RoleId, UserId) VALUES (@AccountManagerRoleId, @daveId);

    SET IDENTITY_INSERT users.AspNetUsers OFF;

    INSERT INTO users.AspNetUserClaims (ClaimType, ClaimValue, UserId)
    VALUES
    (N'email_verified', N'true', @aliceId),
    (N'website', N'http://alice.com/', @aliceId),
    (N'address', @address, @aliceId),
    (N'primaryOrganisationName', @aliceOrganisationName, @aliceId),
    (N'email_verified', N'true', @bobId),
    (N'location', N'somewhere', @bobId),
    (N'website', N'http://bob.com/', @bobId),
    (N'address', @address, @bobId),
    (N'primaryOrganisationName', @bobOrganisationName, @bobId),
    (N'email_verified', N'true', @sueId),
    (N'location', N'somewhere', @sueId),
    (N'website', N'http://sue.com/', @sueId),
    (N'address', @address, @sueId),
    (N'primaryOrganisationName', @sueOrganisationName, @sueId),
    (N'email_verified', N'true', @daveId),
    (N'location', N'somewhere', @daveId),
    (N'website', N'http://dave.com/', @daveId),
    (N'address', @address, @daveId),
    (N'primaryOrganisationName', @daveOrganisationName, @daveId);
END;
