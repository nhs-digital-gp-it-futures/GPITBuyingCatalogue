CREATE TABLE ordering.Contacts
(
    Id int IDENTITY(1, 1) NOT NULL,
    FirstName nvarchar(100) NULL, 
    LastName nvarchar(100) NULL,
    Email nvarchar(256) NULL, 
    Phone nvarchar(35) NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_Contacts PRIMARY KEY (Id),
    CONSTRAINT FK_Contacts_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ordering.Contacts_History));
