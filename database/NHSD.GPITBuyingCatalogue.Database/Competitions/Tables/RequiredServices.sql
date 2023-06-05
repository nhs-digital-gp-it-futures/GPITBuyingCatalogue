CREATE TABLE [dbo].[RequiredServices]
(
	[CompetitionId] INT NOT NULL,
    [SolutionId] NVARCHAR(14) NOT NULL,
    [ServiceId] NVARCHAR(14) NOT NULL,
    CONSTRAINT PK_CompetitionServices PRIMARY KEY ([CompetitionId], [SolutionId], [ServiceId]),
    CONSTRAINT FK_RequiredServices_Competition FOREIGN KEY ([CompetitionId]) REFERENCES competitions.Competitions ([Id]),
    CONSTRAINT FK_RequiredServices_Solution FOREIGN KEY ([CompetitionId], [SolutionId]) REFERENCES competitions.CompetitionSolutions ([CompetitionId], [SolutionId]),
    CONSTRAINT FK_RequiredServices_Service FOREIGN KEY ([ServiceId]) REFERENCES catalogue.AdditionalServices ([CatalogueItemId])
);
