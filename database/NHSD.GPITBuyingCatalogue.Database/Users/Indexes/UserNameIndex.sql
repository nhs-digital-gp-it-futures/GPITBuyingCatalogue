CREATE UNIQUE NONCLUSTERED INDEX UserNameIndex
ON users.AspNetUsers (NormalizedUserName)
WHERE NormalizedUserName IS NOT NULL;
