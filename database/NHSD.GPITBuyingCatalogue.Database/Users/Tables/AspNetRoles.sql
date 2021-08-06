CREATE TABLE users.AspNetRoles
(
     Id uniqueidentifier NOT NULL,
     [Name] nvarchar(256) NOT NULL,
     NormalizedName nvarchar(256) NOT NULL,
     ConcurrencyStamp nvarchar(max) NULL,
     CONSTRAINT PK_AspNetRoles PRIMARY KEY NONCLUSTERED (Id),
     CONSTRAINT AK_AspNetRoles UNIQUE CLUSTERED (NormalizedName),
);
