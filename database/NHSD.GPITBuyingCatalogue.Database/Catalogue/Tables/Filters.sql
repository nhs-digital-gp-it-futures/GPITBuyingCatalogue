CREATE TABLE catalogue.Filters
(
    Id int IDENTITY(1, 1) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Description] nvarchar(255) NOT NULL,
    OrganisationId int NOT NULL,
    FrameworkId NVARCHAR(10) NULL,
    Created datetime2(7) CONSTRAINT DF_Filter_Created DEFAULT GETUTCDATE() NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastPublished datetime2(7) NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_Filters PRIMARY KEY (Id),
    CONSTRAINT FK_Filters_OrganisationId FOREIGN KEY (OrganisationId) REFERENCES organisations.Organisations(Id),
    CONSTRAINT FK_Filters_FrameworkId FOREIGN KEY (FrameworkId) REFERENCES catalogue.Frameworks(Id),
    CONSTRAINT FK_Filters_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.Filters_History));
