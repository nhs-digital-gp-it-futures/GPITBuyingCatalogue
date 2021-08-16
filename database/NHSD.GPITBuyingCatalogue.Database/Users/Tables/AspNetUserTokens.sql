﻿CREATE TABLE users.AspNetUserTokens
(
     UserId int NOT NULL,
     LoginProvider nvarchar(128) NOT NULL,
     [Name] nvarchar(128) NOT NULL,
     [Value] nvarchar(max) NULL,
     CONSTRAINT PK_AspNetUserTokens PRIMARY KEY (UserId, LoginProvider, [Name]),
     CONSTRAINT FK_AspNetUserTokens_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES users.AspNetUsers (Id) ON DELETE CASCADE,
);
