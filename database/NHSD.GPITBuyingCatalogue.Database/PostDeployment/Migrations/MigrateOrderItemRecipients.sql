IF NOT EXISTS(SELECT 1 FROM ordering.OrderRecipients) AND NOT EXISTS(SELECT 1 FROM ordering.OrderRecipientItems)
BEGIN
    INSERT INTO [ordering].[OrderRecipients] (OrderId, OdsCode)
    SELECT DISTINCT OrderId, OdsCode FROM ordering.OrderItemRecipients

    INSERT INTO [ordering].[OrderRecipientItems] (OrderId, CatalogueItemId, OdsCode, Quantity, DeliveryDate)
    SELECT OrderId, CatalogueItemId, OdsCode, Quantity, DeliveryDate FROM ordering.OrderItemRecipients
END
