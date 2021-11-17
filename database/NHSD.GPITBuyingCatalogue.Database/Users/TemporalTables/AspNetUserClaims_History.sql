CREATE TABLE users.AspNetUserClaims_History
(
     Id int NOT NULL,
     UserId int NOT NULL,
     ClaimType nvarchar(max) NULL,
     ClaimValue nvarchar(max) NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_AspNetUserClaims_History
ON users.AspNetUserClaims_History;
GO

CREATE NONCLUSTERED INDEX IX_AspNetUserClaims_History_Id_Period_Columns
ON users.AspNetUserClaims_History (SysEndTime, SysStartTime, Id);
GO
