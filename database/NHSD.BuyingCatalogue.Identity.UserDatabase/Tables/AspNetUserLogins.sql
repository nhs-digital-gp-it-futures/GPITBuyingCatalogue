CREATE TABLE dbo.AspNetUserLogins
(
     LoginProvider nvarchar(128) NOT NULL,
     ProviderKey nvarchar(128) NOT NULL,
     ProviderDisplayName nvarchar(max) NULL,
     UserId nvarchar(450) NOT NULL,
     CONSTRAINT PK_AspNetUserLogins PRIMARY KEY CLUSTERED (LoginProvider, ProviderKey),
     CONSTRAINT FK_AspNetUserLogins_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers (Id) ON DELETE CASCADE,
     INDEX IX_AspNetUserLogins_UserId NONCLUSTERED (UserId)
);
