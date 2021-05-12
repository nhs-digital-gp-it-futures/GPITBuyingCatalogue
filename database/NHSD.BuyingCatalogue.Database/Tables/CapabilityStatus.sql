CREATE TABLE dbo.CapabilityStatus
(
     Id int NOT NULL,
     [Name] nvarchar(16) NOT NULL,
     CONSTRAINT PK_CapabilityStatus PRIMARY KEY CLUSTERED (Id),
     CONSTRAINT IX_CapabilityStatusName UNIQUE NONCLUSTERED ([Name])
);
