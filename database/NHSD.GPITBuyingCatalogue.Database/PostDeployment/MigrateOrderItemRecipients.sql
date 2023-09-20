IF NOT EXISTS(SELECT 1 FROM ordering.OrderRecipients)
BEGIN
    INSERT INTO [ordering].[OrderRecipients] (OrderId, OdsCode)
    SELECT DISTINCT OrderId, OdsCode FROM ordering.OrderItemRecipients

    DELETE FROM [ordering].[OrderItemRecipients] WHERE Quantity IS NULL AND DeliveryDate IS NULL
END
