CREATE TABLE [competitions].[CompetitionCatalogueItems]
(
    [Id]                INT          NOT NULL IDENTITY(1, 1),
    [CompetitionId]     INT          NOT NULL,
    [CatalogueItemId]   NVARCHAR(14) NOT NULL,
    [CatalogueItemType] INT          NOT NULL,
    CONSTRAINT PK_CompetitionCatalogueItems PRIMARY KEY (Id),
    CONSTRAINT FK_CompetitionCatalogueItems_Competition FOREIGN KEY ([CompetitionId]) REFERENCES competitions.Competitions ([Id]),
    CONSTRAINT FK_CompetitionCatalogueItems_CatalogueItem FOREIGN KEY ([CatalogueItemId]) REFERENCES catalogue.CatalogueItems ([Id]),
    CONSTRAINT FK_CompetitionCatalogueItems_CatalogueItemType FOREIGN KEY ([CatalogueItemType]) REFERENCES catalogue.CatalogueItemTypes ([Id])
);
