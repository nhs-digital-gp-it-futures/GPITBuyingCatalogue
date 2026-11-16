DECLARE @AccountManagerRoleId INT = (SELECT Id FROM users.AspNetRoles WHERE [Name] = 'AccountManager');
DECLARE @BuyerRoleId INT = (SELECT Id FROM users.AspNetRoles WHERE [Name] = 'Buyer');

UPDATE users.AspNetUserRoles
SET RoleId = @BuyerRoleId
WHERE RoleId = @AccountManagerRoleId
    AND UserId NOT IN
    (
        SELECT UserId
        FROM users.AspNetUserRoles
        WHERE RoleId = @BuyerRoleId
    );

DELETE FROM users.AspNetRoles WHERE [Name] = 'AccountManager';
