CREATE TABLE dbo.AspNetUserRoles
(
     UserId nvarchar(450) NOT NULL,
     RoleId nvarchar(450) NOT NULL,
     CONSTRAINT PK_AspNetUserRoles PRIMARY KEY CLUSTERED (UserId, RoleId),
     CONSTRAINT FK_AspNetUserRoles_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES dbo.AspNetRoles (Id) ON DELETE CASCADE,
     CONSTRAINT FK_AspNetUserRoles_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers (Id) ON DELETE CASCADE,
     INDEX IX_AspNetUserRoles_RoleId NONCLUSTERED (RoleId)
);
