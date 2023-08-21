CREATE TABLE [competitions].[NonPriceElements]
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [CompetitionId] INT NOT NULL,
    CONSTRAINT FK_NonPriceElements_Competition FOREIGN KEY ([CompetitionId]) REFERENCES [competitions].[Competitions] ([Id]),
)
