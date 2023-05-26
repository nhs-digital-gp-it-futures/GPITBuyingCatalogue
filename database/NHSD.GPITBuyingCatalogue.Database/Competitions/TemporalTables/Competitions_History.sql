CREATE TABLE [competitions].[Competitions_History]
(
    [Id] INT NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(250) NOT NULL,
    [FilterId] INT NOT NULL,
    [OrganisationId] INT NOT NULL,
    [LastUpdated] DATETIME2(7) NOT NULL,
    [LastUpdatedBy] INT NULL,
    [Completed] DATETIME2(7) NULL,
    [IsDeleted] BIT NOT NULL,
    [SysStartTime] DATETIME2(0) NOT NULL,
    [SysEndTime] DATETIME2(0) NOT NULL
)
