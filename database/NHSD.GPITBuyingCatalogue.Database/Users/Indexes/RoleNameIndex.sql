CREATE UNIQUE NONCLUSTERED INDEX RoleNameIndex
ON users.AspNetRoles (NormalizedName)
WHERE NormalizedName IS NOT NULL;
