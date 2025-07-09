CREATE TABLE [competitions].[ServiceQuantitiesSublocationRecipients]
(
    [CompetitionId] INT NOT NULL,
    [SolutionId] NVARCHAR(14) NOT NULL,
    [ServiceId] NVARCHAR(14) NOT NULL,
    [ParentSublocationOdsCode] NVARCHAR(10) NOT NULL,
    [RecipientOdsCode] NVARCHAR(10) NOT NULL,
    [Quantity] INT NOT NULL,
    CONSTRAINT PK_ServiceQuantitiesSublocationRecipients PRIMARY KEY ([CompetitionId], [SolutionId], [ServiceId], [ParentSublocationOdsCode], [RecipientOdsCode]),
    CONSTRAINT FK_ServiceQuantitiesSublocationRecipients_Competition FOREIGN KEY ([CompetitionId]) REFERENCES competitions.Competitions ([Id]),
    CONSTRAINT FK_ServiceQuantitiesSublocationRecipients_SolutionService FOREIGN KEY ([CompetitionId], [SolutionId], [ServiceId]) REFERENCES competitions.SolutionServices ([CompetitionId], [SolutionId], [ServiceId]) ON DELETE CASCADE,
    CONSTRAINT FK_ServiceQuantitiesSublocationRecipients_SublocationRecipient FOREIGN KEY ([CompetitionId], [ParentSublocationOdsCode], [RecipientOdsCode]) REFERENCES [competitions].[CompetitionSublocationRecipients] ([CompetitionId], [ParentSublocationOdsCode], [RecipientOdsCode])  ON DELETE CASCADE
)