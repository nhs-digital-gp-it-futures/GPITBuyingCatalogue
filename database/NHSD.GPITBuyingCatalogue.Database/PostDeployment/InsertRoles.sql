DECLARE @Roles AS TABLE (RoleName NVARCHAR(50));

INSERT INTO @Roles VALUES ('Buyer'), ('Authority'), ('AccountManager');

MERGE INTO users.AspNetRoles AS TARGET
    USING @Roles AS SOURCE
        ON TARGET.[Name] = SOURCE.RoleName
    WHEN NOT MATCHED BY TARGET THEN
    INSERT ([Name], NormalizedName)
    VALUES (RoleName, UPPER(RoleName));
GO
