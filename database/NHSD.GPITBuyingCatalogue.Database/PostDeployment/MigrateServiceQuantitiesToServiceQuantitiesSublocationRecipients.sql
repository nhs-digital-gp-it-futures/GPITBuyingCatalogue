INSERT INTO [competitions].[ServiceQuantitiesSublocationRecipients]
    ([CompetitionId], [SolutionId], [ServiceId], [OdsCode], [Quantity])
SELECT [CompetitionId], [SolutionId], [ServiceId], [OdsCode], [Quantity]
FROM [competitions].[ServiceQuantities];
