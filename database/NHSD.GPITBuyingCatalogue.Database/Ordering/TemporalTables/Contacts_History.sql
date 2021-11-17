CREATE TABLE ordering.Contacts_History
(
    Id int NOT NULL,
    FirstName nvarchar(100) NULL, 
    LastName nvarchar(100) NULL,
    Email nvarchar(256) NULL, 
    Phone nvarchar(35) NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_Contacts_History
ON ordering.Contacts_History;
GO

CREATE NONCLUSTERED INDEX IX_Contacts_History_Id_Period_Columns
ON ordering.Contacts_History (SysEndTime, SysStartTime, Id);
GO
