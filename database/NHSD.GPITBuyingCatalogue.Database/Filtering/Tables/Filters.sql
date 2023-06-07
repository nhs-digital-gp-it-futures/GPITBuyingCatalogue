CREATE TABLE filtering.Filters
(
    Id int IDENTITY(1, 1) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Description] nvarchar(255) NOT NULL,
    OrganisationId int NOT NULL,
    FrameworkId NVARCHAR(36) NULL,
    Created datetime2(7) CONSTRAINT DF_Filter_Created DEFAULT GETUTCDATE() NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    IsDeleted BIT CONSTRAINT DF_Filters_IsDeleted DEFAULT 0 NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_Filters PRIMARY KEY (Id),
    CONSTRAINT FK_Filters_OrganisationId FOREIGN KEY (OrganisationId) REFERENCES organisations.Organisations(Id),
    CONSTRAINT FK_Filters_FrameworkId FOREIGN KEY (FrameworkId) REFERENCES catalogue.Frameworks(Id),
    CONSTRAINT FK_Filters_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
    INDEX IX_Filters_IsDeleted (IsDeleted)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = filtering.Filters_History));

GO
CREATE NONCLUSTERED INDEX IX_Id_IsDeleted_Revision ON [filtering].[Filters] ([Id], [IsDeleted])
