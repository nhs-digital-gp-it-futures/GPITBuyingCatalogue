BEGIN TRANSACTION

INSERT INTO [competitions].[ServiceQuantitiesSublocationRecipients]
    ([CompetitionId], [SolutionId], [ServiceId],[ParentSublocationOdsCode], [RecipientOdsCode], [Quantity])
SELECT [CompetitionId], [SolutionId], [ServiceId], [OdsCode], [Quantity]
FROM [competitions].[ServiceQuantities] [serq]
    JOIN [competitions].[CompetitionSublocationRecipients] [csr]
    ON [serq].[CompetitionId] = [csr].[CompetitionId] AND [serq].[OdsCode] = [csr].[RecipientOdsCode];

INSERT INTO [competitions].[SolutionQuantitiesSublocationRecipients]
    ([CompetitionId], [SolutionId], [ParentSublocationOdsCode], [RecipientOdsCode], [Quantity])
SELECT [solq].[CompetitionId], [solq].[SolutionId], [solq].[OdsCode] AS [RecipientOdsCode], [solq].[Quantity], [csr].[ParentSublocationOdsCode]
FROM [competitions].[SolutionQuantities] [solq]
    JOIN [competitions].[CompetitionSublocationRecipients] [csr]
    ON [solq].[CompetitionId] = [csr].[CompetitionId] AND [solq].[OdsCode] = [csr].[RecipientOdsCode];

COMMIT TRANSACTION