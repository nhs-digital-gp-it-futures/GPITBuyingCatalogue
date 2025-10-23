CREATE TABLE [competitions].[CompetitionSolutions]
(
    [Id]                INT             NOT NULL,
    [IsShortlisted]     BIT DEFAULT (0) NOT NULL,
    [IsWinningSolution] BIT DEFAULT 0   NOT NULL,
    [Justification]     NVARCHAR(1000)  NULL,
    CONSTRAINT PK_CompetitionSolutions PRIMARY KEY ([Id]),
    CONSTRAINT FK_CompetitionSolutions_CompetitionCatalogueItem FOREIGN KEY ([Id]) REFERENCES competitions.CompetitionCatalogueItems ([Id]) ON DELETE CASCADE,
)
