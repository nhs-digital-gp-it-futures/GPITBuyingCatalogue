CREATE TABLE dbo.AspNetUserTokens
(
     UserId nvarchar(450) NOT NULL,
     LoginProvider nvarchar(128) NOT NULL,
     [Name] nvarchar(128) NOT NULL,
     [Value] nvarchar(max) NULL,
     CONSTRAINT PK_AspNetUserTokens PRIMARY KEY CLUSTERED (UserId, LoginProvider, [Name]),
     CONSTRAINT FK_AspNetUserTokens_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers (Id) ON DELETE CASCADE
);
