CREATE TABLE [competitions].[CompetitionCatalogueItems]
(
    [Id]                INT             NOT NULL IDENTITY (1, 1),
    [CompetitionId]     INT             NOT NULL,
    [CatalogueItemId]   NVARCHAR(14)    NOT NULL,
    [CatalogueItemType] INT             NOT NULL,
    [IsShortlisted]     BIT DEFAULT (0) NULL,
    [IsWinningSolution] BIT DEFAULT 0   NULL,
    [Justification]     NVARCHAR(1000)  NULL,
    [ParentItemId]      INT             NULL,
    [IsRequired]        BIT DEFAULT (0) NULL,
    [Quantity]          INT             NULL,
    CONSTRAINT PK_CompetitionCatalogueItems PRIMARY KEY (Id),
    CONSTRAINT FK_CompetitionCatalogueItems_Competition FOREIGN KEY ([CompetitionId]) REFERENCES competitions.Competitions ([Id]),
    CONSTRAINT FK_CompetitionCatalogueItems_CatalogueItem FOREIGN KEY ([CatalogueItemId]) REFERENCES catalogue.CatalogueItems ([Id]),
    CONSTRAINT FK_CompetitionCatalogueItems_CatalogueItemType FOREIGN KEY ([CatalogueItemType]) REFERENCES catalogue.CatalogueItemTypes ([Id]),
    CONSTRAINT FK_CompetitionCatalogueItems_Parent FOREIGN KEY ([ParentItemId]) REFERENCES competitions.CompetitionCatalogueItems ([Id]) ON DELETE NO ACTION
);
