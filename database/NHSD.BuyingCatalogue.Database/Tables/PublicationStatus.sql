CREATE TABLE dbo.PublicationStatus
(
     Id int NOT NULL,
     [Name] nvarchar(16) NOT NULL,
     CONSTRAINT PK_PublicationStatus PRIMARY KEY CLUSTERED (Id),
     CONSTRAINT IX_PublicationStatusName UNIQUE NONCLUSTERED ([Name])
);
