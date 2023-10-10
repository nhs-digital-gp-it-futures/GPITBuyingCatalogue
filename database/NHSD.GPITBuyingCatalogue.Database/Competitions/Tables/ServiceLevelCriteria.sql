CREATE TABLE [competitions].[ServiceLevelCriteria]
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [NonPriceElementsId] INT NOT NULL,
    [TimeFrom] DATETIME2(7) NOT NULL,
    [TimeUntil] DATETIME2(7) NOT NULL,
    [ApplicableDays] NVARCHAR(15) NOT NULL,
    [IncludesBankHolidays] BIT DEFAULT(0) NOT NULL,
    CONSTRAINT FK_ServiceLevelCriteria_NonPriceElements FOREIGN KEY ([NonPriceElementsId]) REFERENCES [competitions].[NonPriceElements] ([Id]) ON DELETE CASCADE
)
