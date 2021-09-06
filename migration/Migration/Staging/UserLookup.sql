CREATE TABLE dbo.UserLookup
(
    Id int IDENTITY(1, 1) NOT NULL PRIMARY KEY,
    OriginalId uniqueidentifier UNIQUE,
    NormalizedUserName nvarchar(256) NOT NULL,
);
