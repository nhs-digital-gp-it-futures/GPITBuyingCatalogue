DECLARE @ClaimType NVARCHAR(50) = 'permission';
DECLARE @OnboardingRole NVARCHAR(50) = 'Onboarding';

DECLARE @Claims TABLE
(
    ClaimKey NVARCHAR(50) PRIMARY KEY,
    ClaimValue NVARCHAR(100) NOT NULL
);

INSERT INTO @Claims
VALUES
    ('ManageCatalogueSolutions', 'solutions.manage'),
    ('ManageContractingVehicles', 'contracting-vehicles.manage'),
    ('ManageSupplierDefinedEpics', 'supplier-defined-epics.manage'),
    ('ManageCapabilitiesAndEpics', 'capabilities-and-epics.manage'),
    ('ManageInteroperability', 'interoperability.manage'),
    ('ManageBuyerOrganisations', 'buyer-organisations.manage'),
    ('ManageSupplierOrganisations', 'supplier-organisations.manage'),
    ('ManageUsers', 'users.manage'),
    ('ManageAllowedEmailDomains', 'allowed-email-domains.manage'),
    ('ManageAccountCreationRequests', 'account-creation-requests.manage'),
    ('ManageAllOrders', 'orders.manage');

DECLARE @RoleClaimMap TABLE
(
    RoleName NVARCHAR(50),
    ClaimKey NVARCHAR(50)
);

INSERT INTO @RoleClaimMap
SELECT 'Authority', ClaimKey
FROM @Claims;

INSERT INTO @RoleClaimMap
VALUES
    (@OnboardingRole, 'ManageCatalogueSolutions'),
    (@OnboardingRole, 'ManageContractingVehicles'),
    (@OnboardingRole, 'ManageSupplierDefinedEpics'),
    (@OnboardingRole, 'ManageSupplierOrganisations');

INSERT INTO @RoleClaimMap
VALUES
    ('ReadOnly', 'ManageAllOrders');

MERGE INTO users.AspNetRoleClaims AS TARGET
USING (
    SELECT
        r.Id AS RoleId,
        @ClaimType AS ClaimType,
        c.ClaimValue
    FROM @RoleClaimMap rcm
    JOIN @Claims c ON c.ClaimKey = rcm.ClaimKey
    JOIN users.AspNetRoles r ON r.[Name] = rcm.RoleName
) AS SOURCE
ON TARGET.RoleId = SOURCE.RoleId
AND TARGET.ClaimType = SOURCE.ClaimType
AND TARGET.ClaimValue = SOURCE.ClaimValue
WHEN NOT MATCHED BY TARGET THEN
    INSERT (RoleId, ClaimType, ClaimValue)
    VALUES (SOURCE.RoleId, SOURCE.ClaimType, SOURCE.ClaimValue);