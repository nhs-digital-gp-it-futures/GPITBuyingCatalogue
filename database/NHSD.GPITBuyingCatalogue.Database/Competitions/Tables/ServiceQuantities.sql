CREATE TABLE [competitions].[ServiceQuantities]
(
    [CompetitionId] INT NOT NULL,
    [SolutionId] NVARCHAR(14) NOT NULL,
    [ServiceId] NVARCHAR(14) NOT NULL,
    [OdsCode] NVARCHAR(10) NOT NULL,
    [Quantity] INT NOT NULL,
    CONSTRAINT PK_ServiceQuantities PRIMARY KEY ([CompetitionId], [SolutionId], [ServiceId], [OdsCode]),
    CONSTRAINT FK_ServiceQuantities_Competition FOREIGN KEY ([CompetitionId]) REFERENCES competitions.Competitions ([Id]),
    CONSTRAINT FK_ServiceQuantities_SolutionService FOREIGN KEY ([CompetitionId], [SolutionId], [ServiceId]) REFERENCES competitions.SolutionServices ([CompetitionId], [SolutionId], [ServiceId]) ON DELETE CASCADE,
    CONSTRAINT FK_ServiceQuantities_Recipient FOREIGN KEY ([CompetitionId], [OdsCode]) REFERENCES [competitions].[CompetitionRecipients] ([CompetitionId], [OdsCode])  ON DELETE CASCADE
)
