CREATE TABLE [competitions].[SolutionQuantitiesSublocationRecipients]
(
    [CompetitionId] INT NOT NULL,
    [SolutionId] NVARCHAR(14) NOT NULL,
    [ParentSublocationOdsCode] NVARCHAR(10) NOT NULL,
    [RecipientOdsCode] NVARCHAR(10) NOT NULL,
    [Quantity] INT NOT NULL,
    CONSTRAINT PK_SolutionQuantitiesSublocationRecipients PRIMARY KEY ([CompetitionId], [SolutionId], [ParentSublocationOdsCode], [RecipientOdsCode]),
    CONSTRAINT FK_SolutionQuantitiesSublocationRecipients_Competition FOREIGN KEY ([CompetitionId]) REFERENCES competitions.Competitions ([Id]),
    CONSTRAINT FK_SolutionQuantitiesSublocationRecipients_Solution FOREIGN KEY ([CompetitionId], [SolutionId]) REFERENCES competitions.CompetitionSolutions ([CompetitionId], [SolutionId]) ON DELETE CASCADE,
    CONSTRAINT FK_SolutionQuantitiesSublocationRecipients_SublocationRecipient FOREIGN KEY ([CompetitionId], [ParentSublocationOdsCode], [RecipientOdsCode]) REFERENCES [competitions].[CompetitionSublocationRecipients] ([CompetitionId], [ParentSublocationOdsCode], [RecipientOdsCode])  ON DELETE CASCADE
)