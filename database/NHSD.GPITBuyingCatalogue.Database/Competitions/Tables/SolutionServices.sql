CREATE TABLE [competitions].[SolutionServices]
(
    [CompetitionId] INT NOT NULL,
    [SolutionId] NVARCHAR(14) NOT NULL,
    [ServiceId] NVARCHAR(14) NOT NULL,
    [IsRequired] BIT DEFAULT 0 NOT NULL,
    [CompetitionItemPriceId] INT NULL,
    [Quantity] INT NULL,
    CONSTRAINT PK_CompetitionServices PRIMARY KEY ([CompetitionId], [SolutionId], [ServiceId]),
    CONSTRAINT FK_SolutionServices_Competition FOREIGN KEY ([CompetitionId]) REFERENCES competitions.Competitions ([Id]),
    CONSTRAINT FK_SolutionServices_Solution FOREIGN KEY ([CompetitionId], [SolutionId]) REFERENCES competitions.CompetitionSolutions ([CompetitionId], [SolutionId]),
    CONSTRAINT FK_SolutionServices_Service FOREIGN KEY ([ServiceId]) REFERENCES catalogue.CatalogueItems ([Id]),
    CONSTRAINT FK_SolutionServices_CompetitionItemPrice FOREIGN KEY ([CompetitionItemPriceId]) REFERENCES competitions.CompetitionCatalogueItemPrices ([Id])
);
