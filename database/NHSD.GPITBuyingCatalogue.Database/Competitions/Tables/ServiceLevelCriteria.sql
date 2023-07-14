CREATE TABLE [competitions].[ServiceLevelCriteria]
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [TimeFrom] DATETIME2(7) NOT NULL,
    [TimeUntil] DATETIME2(7) NOT NULL,
    [ApplicableDays] NVARCHAR(1000) NOT NULL
)
