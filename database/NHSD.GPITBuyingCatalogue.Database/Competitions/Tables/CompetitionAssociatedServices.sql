CREATE TABLE [competitions].[CompetitionAssociatedServices]
(
    [Id]           INT             NOT NULL,
    [ParentItemId] INT             NOT NULL,
    CONSTRAINT PK_CompetitionAssociatedServices PRIMARY KEY ([Id]),
    CONSTRAINT FK_CompetitionAssociatedServices_CompetitionCatalogueItem FOREIGN KEY ([Id]) REFERENCES competitions.CompetitionCatalogueItems ([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_CompetitionAssociatedServices_Parent FOREIGN KEY ([ParentItemId]) REFERENCES competitions.CompetitionCatalogueItems ([Id]) ON DELETE NO ACTION
)

GO
CREATE INDEX IX_CompetitionAssociatedServices_ParentItemId ON competitions.CompetitionAssociatedServices([ParentItemId]);
