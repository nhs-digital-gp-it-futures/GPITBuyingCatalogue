CREATE TABLE [catalogue].[Standards]
(
    [Id] NVARCHAR(5) NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [Url] NVARCHAR(1000) NOT NULL,
    StandardTypeId INT NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_Standards PRIMARY KEY (Id),
    CONSTRAINT FK_Standards_StandardType FOREIGN KEY (StandardTypeId) REFERENCES catalogue.StandardTypes(Id),
    CONSTRAINT FK_Standards_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
);
