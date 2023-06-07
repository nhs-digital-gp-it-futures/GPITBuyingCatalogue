CREATE TABLE [competitions].[CompetitionSolutions]
(
	[CompetitionId] INT NOT NULL,
    [SolutionId] NVARCHAR(14) NOT NULL,
    [IsShortlisted] BIT DEFAULT 0 NOT NULL,
    [Justification] NVARCHAR(1000) NULL,
    CONSTRAINT PK_CompetitionSolutions PRIMARY KEY ([CompetitionId], [SolutionId]),
    CONSTRAINT FK_CompetitionSolutions_Competition FOREIGN KEY ([CompetitionId]) REFERENCES competitions.Competitions ([Id]),
    CONSTRAINT FK_CompetitionSolutions_Solution FOREIGN KEY ([SolutionId]) REFERENCES catalogue.Solutions ([CatalogueItemId])
);
