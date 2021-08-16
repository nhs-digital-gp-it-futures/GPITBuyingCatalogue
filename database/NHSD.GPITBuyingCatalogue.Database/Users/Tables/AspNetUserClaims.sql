CREATE TABLE users.AspNetUserClaims
(
     Id int IDENTITY(1, 1) NOT NULL,
     UserId int NOT NULL,
     ClaimType nvarchar(max) NULL,
     ClaimValue nvarchar(max) NULL,
     CONSTRAINT PK_AspNetUserClaims PRIMARY KEY (Id),
     CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES users.AspNetUsers (Id) ON DELETE CASCADE,
     INDEX IX_AspNetUserClaims_UserId (UserId),
);
