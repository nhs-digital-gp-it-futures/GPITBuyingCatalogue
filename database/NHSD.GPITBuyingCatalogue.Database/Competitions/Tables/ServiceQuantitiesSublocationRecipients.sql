CREATE TABLE [competitions].[ServiceQuantitiesSublocationRecipients]
(
    [CompetitionId] INT NOT NULL,
    [SolutionId] NVARCHAR(14) NOT NULL,
    [ServiceId] NVARCHAR(14) NOT NULL,
    [OdsCode] NVARCHAR(10) NOT NULL,
    [Quantity] INT NOT NULL,
    CONSTRAINT PK_ServiceQuantitiesSublocationRecipients PRIMARY KEY ([CompetitionId], [SolutionId], [ServiceId], [OdsCode]),
    CONSTRAINT FK_ServiceQuantitiesSublocationRecipients_Competition FOREIGN KEY ([CompetitionId]) REFERENCES competitions.Competitions ([Id]),
    CONSTRAINT FK_ServiceQuantitiesSublocationRecipients_SolutionService FOREIGN KEY ([CompetitionId], [SolutionId], [ServiceId]) REFERENCES competitions.SolutionServices ([CompetitionId], [SolutionId], [ServiceId]) ON DELETE CASCADE,
    CONSTRAINT FK_ServiceQuantitiesSublocationRecipients_SublocationRecipient FOREIGN KEY ([CompetitionId], [OdsCode]) REFERENCES [competitions].[CompetitionSublocationRecipients] ([CompetitionId], [RecipientOdsCode])  ON DELETE CASCADE
)