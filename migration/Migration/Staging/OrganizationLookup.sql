CREATE TABLE dbo.OrganizationLookup
(
    Id int IDENTITY(1, 1) NOT NULL PRIMARY KEY,
    OriginalId uniqueidentifier UNIQUE,
    [Name] nvarchar(255) NOT NULL,
);
