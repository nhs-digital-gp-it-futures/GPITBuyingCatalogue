CREATE TABLE dbo.SolutionEpicStatus
(
     Id int NOT NULL,
     [Name] nvarchar(16) NOT NULL,
     IsMet bit NOT NULL,
     CONSTRAINT PK_EpicStatus PRIMARY KEY CLUSTERED (Id),
     CONSTRAINT IX_EpicStatusName UNIQUE NONCLUSTERED ([Name])
);
