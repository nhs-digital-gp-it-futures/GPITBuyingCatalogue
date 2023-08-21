CREATE TABLE [competitions].[CompetitionCatalogueItemPriceTiers]
(
	[Id] INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
    [CompetitionItemPriceId] INT NOT NULL,
    [CompetitionId] INT NOT NULL,
    [Price] DECIMAL(18,4) NOT NULL CONSTRAINT OrderItemPriceTiers_PositivePrice CHECK (Price >= 0.00),
    [LowerRange] INT NOT NULL CONSTRAINT OrderItemPriceTiers_PositiveLowerRange CHECK (LowerRange >= 1),
    [UpperRange] INT NULL,
    [ListPrice] DECIMAL(18, 4) NOT NULL, 
    CONSTRAINT FK_CompetitionCatalogueItemPriceTiers_Competition FOREIGN KEY ([CompetitionId]) REFERENCES competitions.Competitions (Id),
    CONSTRAINT FK_CompetitionCatalogueItemPriceTiers_CompetitionItemPriceId FOREIGN KEY ([CompetitionItemPriceId]) REFERENCES competitions.CompetitionCatalogueItemPrices ([Id]) ON DELETE CASCADE,
);
