CREATE TABLE dbo.CompliancyLevel
(
     Id int NOT NULL,
     [Name] nvarchar(16) NOT NULL,
     CONSTRAINT PK_CompliancyLevel PRIMARY KEY CLUSTERED (Id),
     CONSTRAINT IX_CompliancyLevelName UNIQUE NONCLUSTERED ([Name])
);
