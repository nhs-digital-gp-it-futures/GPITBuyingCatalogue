CREATE TABLE dbo.Framework
(
     Id nvarchar(10) NOT NULL,
     [Name] nvarchar(100) NOT NULL,
     ShortName nvarchar(25) NULL,
     [Description] nvarchar(max) NULL,
     [Owner] nvarchar(100) NULL,
     ActiveDate date NULL,
     ExpiryDate date NULL,
     CONSTRAINT PK_Framework PRIMARY KEY CLUSTERED (Id)
);
