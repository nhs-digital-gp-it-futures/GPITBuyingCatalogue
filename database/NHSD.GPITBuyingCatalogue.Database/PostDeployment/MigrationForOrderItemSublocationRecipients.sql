BEGIN TRANSACTION

INSERT INTO [ordering].[OrderItemSublocationRecipients]
    ([OrderId], [CatalogueItemId], [ParentSublocationOdsCode], [RecipientOdsCode], [Quantity], [DeliveryDate], [LastUpdated], [LastUpdatedBy]
    )
SELECT [oir].[OrderId], [oir].[CatalogueItemId], [oir].[OdsCode] AS [RecipientOdsCode], [oir].[Quantity], [oir].[DeliveryDate], [oir].[LastUpdated], [oir].[LastUpdatedBy], [osr].[ParentSublocationOdsCode]
FROM [ordering].[OrderItemRecipients] [oir]
    JOIN [GPITBuyingCatalogue].[ordering].[OrderSublocationRecipients] [osr]
    ON [oir].[OrderId] = [osr].[OrderId] AND [oir].[OdsCode] = [osr].[RecipientOdsCode];

COMMIT TRANSACTION