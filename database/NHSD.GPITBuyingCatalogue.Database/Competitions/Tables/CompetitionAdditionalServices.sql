CREATE TABLE [competitions].[CompetitionAdditionalServices]
(
    [Id]           INT             NOT NULL,
    [ParentItemId] INT             NOT NULL,
    [IsRequired]   BIT DEFAULT (0) NOT NULL,
    CONSTRAINT PK_CompetitionAdditionalServices PRIMARY KEY ([Id]),
    CONSTRAINT FK_CompetitionAdditionalServices_CompetitionCatalogueItem FOREIGN KEY ([Id]) REFERENCES competitions.CompetitionCatalogueItems ([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_CompetitionAdditionalServices_Parent FOREIGN KEY ([ParentItemId]) REFERENCES competitions.CompetitionCatalogueItems ([Id]) ON DELETE NO ACTION
)

GO
CREATE INDEX IX_CompetitionAdditionalServices_ParentItemId ON competitions.CompetitionAdditionalServices([ParentItemId]);
