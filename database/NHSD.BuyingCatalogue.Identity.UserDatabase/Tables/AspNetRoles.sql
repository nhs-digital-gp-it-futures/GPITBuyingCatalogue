CREATE TABLE dbo.AspNetRoles
(
     Id nvarchar(450) NOT NULL,
     [Name] nvarchar(256) NULL,
     NormalizedName nvarchar(256) NULL,
     ConcurrencyStamp nvarchar(max) NULL,
     CONSTRAINT PK_AspNetRoles PRIMARY KEY CLUSTERED (Id)
);
