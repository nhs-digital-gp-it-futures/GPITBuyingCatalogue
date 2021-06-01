CREATE UNIQUE NONCLUSTERED INDEX UserNameIndex
ON dbo.AspNetUsers (NormalizedUserName)
WHERE NormalizedUserName IS NOT NULL;
