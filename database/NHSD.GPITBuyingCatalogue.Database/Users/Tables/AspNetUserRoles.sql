﻿CREATE TABLE users.AspNetUserRoles
(
     UserId int NOT NULL,
     RoleId int NOT NULL,
     CONSTRAINT PK_AspNetUserRoles PRIMARY KEY (UserId, RoleId),
     CONSTRAINT FK_AspNetUserRoles_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES users.AspNetRoles (Id) ON DELETE CASCADE,
     CONSTRAINT FK_AspNetUserRoles_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES users.AspNetUsers (Id) ON DELETE CASCADE,
     INDEX IX_AspNetUserRoles_RoleId (RoleId),
);
