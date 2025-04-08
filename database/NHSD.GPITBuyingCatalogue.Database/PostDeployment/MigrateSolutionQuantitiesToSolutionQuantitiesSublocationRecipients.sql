INSERT INTO [competitions].[SolutionQuantitiesSublocationRecipients]
    ([CompetitionId], [SolutionId], [OdsCode], [Quantity])
SELECT [CompetitionId], [SolutionId], [OdsCode], [Quantity]
FROM [competitions].[SolutionQuantities];
