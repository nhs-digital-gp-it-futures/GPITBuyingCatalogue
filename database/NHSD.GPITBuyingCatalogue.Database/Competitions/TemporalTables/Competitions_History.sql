CREATE TABLE [competitions].[Competitions_History]
(
    [Id] INT NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(250) NOT NULL,
    [FilterId] INT NOT NULL,
    [OrganisationId] INT NOT NULL,
    [Created] DATETIME2(7) NOT NULL,
    [LastUpdated] DATETIME2(7) NOT NULL,
    [LastUpdatedBy] INT NULL,
    [ShortlistAccepted] DATETIME2(7) NULL,
    [Completed] DATETIME2(7) NULL,
    [HasReviewedCriteria] BIT DEFAULT 0 NOT NULL,
    [IsDeleted] BIT NOT NULL,
    [ContractLength] INT NULL,
    [IncludesNonPrice] BIT NULL,
    [SysStartTime] DATETIME2(0) NOT NULL,
    [SysEndTime] DATETIME2(0) NOT NULL,
    [FrameworkId] NVARCHAR(36) NULL
)
