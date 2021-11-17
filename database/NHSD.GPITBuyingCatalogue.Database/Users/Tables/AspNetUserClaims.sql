CREATE TABLE users.AspNetUserClaims
(
     Id int IDENTITY(1, 1) NOT NULL,
     UserId int NOT NULL,
     ClaimType nvarchar(max) NULL,
     ClaimValue nvarchar(max) NULL,
     LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_AspNetUserClaims PRIMARY KEY (Id),
     CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES users.AspNetUsers (Id) ON DELETE CASCADE,
     CONSTRAINT FK_AspNetUserClaims_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
     INDEX IX_AspNetUserClaims_UserId (UserId),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = users.AspNetUserClaims_History));
