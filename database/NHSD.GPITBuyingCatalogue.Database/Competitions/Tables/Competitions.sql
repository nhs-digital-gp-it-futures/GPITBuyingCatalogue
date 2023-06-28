CREATE TABLE [competitions].[Competitions]
(
    [Id] INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(250) NOT NULL,
    [FilterId] INT NOT NULL,
    [OrganisationId] INT NOT NULL,
    [LastUpdated] DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    [LastUpdatedBy] INT NULL,
    [ShortlistAccepted] DATETIME2(7) NULL,
    [Completed] DATETIME2(7) NULL,
    [IsDeleted] BIT DEFAULT 0 NOT NULL,
    [ContractLength] INT NULL,
    [IncludesNonPrice] BIT NULL,
    [SysStartTime] DATETIME2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    [SysEndTime] DATETIME2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT FK_Competitions_Filter FOREIGN KEY (FilterId) REFERENCES filtering.Filters (Id),
    CONSTRAINT FK_Competitions_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers (Id),
    CONSTRAINT FK_Competitions_Organisation FOREIGN KEY (OrganisationId) REFERENCES organisations.Organisations (Id)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [competitions].[Competitions_History]));
