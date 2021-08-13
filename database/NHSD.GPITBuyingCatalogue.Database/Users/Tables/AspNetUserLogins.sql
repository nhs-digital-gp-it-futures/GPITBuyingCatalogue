CREATE TABLE users.AspNetUserLogins
(
     LoginProvider nvarchar(128) NOT NULL,
     ProviderKey nvarchar(128) NOT NULL,
     ProviderDisplayName nvarchar(max) NULL,
     UserId int NOT NULL,
     CONSTRAINT PK_AspNetUserLogins PRIMARY KEY CLUSTERED (LoginProvider, ProviderKey),
     CONSTRAINT FK_AspNetUserLogins_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES users.AspNetUsers (Id) ON DELETE CASCADE,
     INDEX IX_AspNetUserLogins_UserId NONCLUSTERED (UserId)
);
