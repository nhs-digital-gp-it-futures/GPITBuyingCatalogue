CREATE TABLE [competitions].[Weightings]
(
    [CompetitionId] INT NOT NULL PRIMARY KEY,
    [Price] INT NULL,
    [NonPrice] INT NULL
    CONSTRAINT FK_Weightings_Competition FOREIGN KEY ([CompetitionId]) REFERENCES [competitions].[Competitions] ([Id])
)
