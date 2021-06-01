CREATE TABLE dbo.CapabilityCategory
(
     Id int NOT NULL,
     [Name] nvarchar(50) NOT NULL,
     CONSTRAINT PK_CapabilityCategory PRIMARY KEY CLUSTERED (Id),
     CONSTRAINT IX_CapabilityCategoryName UNIQUE NONCLUSTERED ([Name])
);
