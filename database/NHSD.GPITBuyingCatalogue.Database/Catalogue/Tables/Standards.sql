CREATE TABLE [catalogue].[Standards]
(
    [Id] NVARCHAR(5) NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [Url] NVARCHAR(1000) NOT NULL,
    [Version] NVARCHAR(10) NULL, --TODO Remove column in future release
    StandardTypeId INT NOT NULL,
  	IsDeleted bit NOT NULL CONSTRAINT DF_Standards_IsDeleted DEFAULT(0),
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_Standards PRIMARY KEY (Id),
    CONSTRAINT FK_Standards_StandardType FOREIGN KEY (StandardTypeId) REFERENCES catalogue.StandardTypes(Id),
    CONSTRAINT FK_Standards_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.Standards_History));
