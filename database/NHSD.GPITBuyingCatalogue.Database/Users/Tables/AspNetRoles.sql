CREATE TABLE users.AspNetRoles
(
     Id int IDENTITY(1, 1) NOT NULL,
     [Name] nvarchar(256) NOT NULL,
     NormalizedName nvarchar(256) NOT NULL,
     ConcurrencyStamp nvarchar(max) NULL,
     CONSTRAINT PK_AspNetRoles PRIMARY KEY (Id),
     CONSTRAINT AK_AspNetRoles UNIQUE (NormalizedName),
);
