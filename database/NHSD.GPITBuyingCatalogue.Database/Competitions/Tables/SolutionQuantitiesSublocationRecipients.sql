CREATE TABLE [competitions].[SolutionQuantitiesSublocationRecipients]
(
    [CompetitionId] INT NOT NULL,
    [SolutionId] NVARCHAR(14) NOT NULL,
    [OdsCode] NVARCHAR(10) NOT NULL,
    [Quantity] INT NOT NULL,
    CONSTRAINT PK_SolutionQuantitiesSublocationRecipients PRIMARY KEY ([CompetitionId], [SolutionId], [OdsCode]),
    CONSTRAINT FK_SolutionQuantitiesSublocationRecipients_Competition FOREIGN KEY ([CompetitionId]) REFERENCES competitions.Competitions ([Id]),
    CONSTRAINT FK_SolutionQuantitiesSublocationRecipients_Solution FOREIGN KEY ([CompetitionId], [SolutionId]) REFERENCES competitions.CompetitionSolutions ([CompetitionId], [SolutionId]) ON DELETE CASCADE,
    CONSTRAINT FK_SolutionQuantitiesSublocationRecipients_SublocationRecipient FOREIGN KEY ([CompetitionId], [OdsCode]) REFERENCES [competitions].[CompetitionSublocationRecipients] ([CompetitionId], [RecipientOdsCode])  ON DELETE CASCADE
)