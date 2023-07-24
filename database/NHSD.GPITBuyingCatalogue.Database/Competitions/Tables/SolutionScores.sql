CREATE TABLE [competitions].[SolutionScores]
(
	[Id] INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
    [CompetitionId] INT NOT NULL,
    [SolutionId] NVARCHAR(14) NOT NULL,
    [ScoreType] INT NOT NULL,
    [Score] INT NOT NULL,
    CONSTRAINT FK_SolutionScores_Solution FOREIGN KEY ([CompetitionId], [SolutionId]) REFERENCES competitions.CompetitionSolutions ([CompetitionId], [SolutionId])
)
