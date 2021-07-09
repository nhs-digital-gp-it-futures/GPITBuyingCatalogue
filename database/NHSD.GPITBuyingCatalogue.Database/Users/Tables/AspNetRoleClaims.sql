CREATE TABLE users.AspNetRoleClaims
(
     Id int IDENTITY(1, 1) NOT NULL,
     RoleId nvarchar(450) NOT NULL,
     ClaimType nvarchar(max) NULL,
     ClaimValue nvarchar(max) NULL,
     CONSTRAINT PK_AspNetRoleClaims PRIMARY KEY CLUSTERED (Id),
     CONSTRAINT FK_AspNetRoleClaims_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES users.AspNetRoles (Id) ON DELETE CASCADE,
     INDEX IX_AspNetRoleClaims_RoleId NONCLUSTERED (RoleId)
);
