CREATE TABLE catalogue.WorkOffPlans_History
(
    [Id] INT NOT NULL,
    [SolutionId] NVARCHAR(14) NOT NULL,
    [StandardId] NVARCHAR(5) NOT NULL,
    [Details] NVARCHAR(300) NOT NULL,
    [CompletionDate] DATETIME2(7) NOT NULL,
    LastUpdated DATETIME2(7) NOT NULL,
    LastUpdatedBy INT NULL,
    SysStartTime DATETIME2(0) NOT NULL,
    SysEndTime DATETIME2(0) NOT NULL
);
