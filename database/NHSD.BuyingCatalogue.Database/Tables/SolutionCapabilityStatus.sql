CREATE TABLE dbo.SolutionCapabilityStatus
(
     Id int NOT NULL,
     [Name] nvarchar(16) NOT NULL,
     Pass bit NOT NULL,
     CONSTRAINT PK_SolutionCapabilityStatus PRIMARY KEY CLUSTERED (Id),
     CONSTRAINT IX_SolutionCapabilityStatusName UNIQUE NONCLUSTERED ([Name])
);
