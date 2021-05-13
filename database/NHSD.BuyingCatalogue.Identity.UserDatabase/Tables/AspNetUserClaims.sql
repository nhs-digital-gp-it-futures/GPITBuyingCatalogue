CREATE TABLE dbo.AspNetUserClaims
(
     Id int IDENTITY(1, 1) NOT NULL,
     UserId nvarchar(450) NOT NULL,
     ClaimType nvarchar(max) NULL,
     ClaimValue nvarchar(max) NULL,
     CONSTRAINT PK_AspNetUserClaims PRIMARY KEY CLUSTERED (Id),
     CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers (Id) ON DELETE CASCADE,
     INDEX IX_AspNetUserClaims_UserId NONCLUSTERED (UserId)
);
