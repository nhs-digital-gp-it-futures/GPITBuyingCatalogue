CREATE UNIQUE NONCLUSTERED INDEX RoleNameIndex
ON dbo.AspNetRoles (NormalizedName)
WHERE NormalizedName IS NOT NULL;
