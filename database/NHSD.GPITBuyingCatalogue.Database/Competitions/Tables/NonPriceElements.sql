CREATE TABLE [competitions].[NonPriceElements]
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [CompetitionId] INT NOT NULL,
    [ImplementationId] INT NULL,
    [ServiceLevelId] INT NULL,
    [LastUpdated] DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    [LastUpdatedBy] INT NULL,
    CONSTRAINT FK_NonPriceElements_Competition FOREIGN KEY ([CompetitionId]) REFERENCES [competitions].[Competitions] ([Id]),
    CONSTRAINT FK_NonPriceElements_ImplementationCriteria FOREIGN KEY ([ImplementationId]) REFERENCES [competitions].[ImplementationCriteria] ([Id]),
    CONSTRAINT FK_NonPriceElements_ServiceLevelCriteria FOREIGN KEY ([ServiceLevelId]) REFERENCES [competitions].[ServiceLevelCriteria] ([Id])
)
