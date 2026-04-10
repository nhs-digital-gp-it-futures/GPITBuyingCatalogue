BEGIN TRANSACTION

DECLARE @BatchSize INT = 2000;
DECLARE @RowsInserted INT = 1;
SET NOCOUNT ON;

WHILE @RowsInserted > 0
BEGIN
  ;WITH cte AS (
    SELECT TOP (@BatchSize) o.*
    FROM ordering.OrderItems o
    LEFT JOIN ordering.OrderItemsV2 n ON n.OrderId = o.OrderId AND n.CatalogueItemId = o.CatalogueItemId
    WHERE n.Id IS NULL
    ORDER BY o.OrderId, o.CatalogueItemId
  )
  INSERT INTO ordering.OrderItemsV2(OrderId, CatalogueItemId, PriceId, Price, EstimationPeriodId, DefaultDeliveryDate, Created, LastUpdated, LastUpdatedBy, Quantity)
  SELECT 
    c.OrderId, 
    c.CatalogueItemId, 
    c.PriceId,
    c.Price,
    c.EstimationPeriodId,
    c.DefaultDeliveryDate,
    c.Created,
    c.LastUpdated,
    c.LastUpdatedBy,
    c.Quantity
  FROM cte c

  SET @RowsInserted = @@ROWCOUNT;

  IF @RowsInserted > 0
  BEGIN
    PRINT CONCAT('Inserted batch of ', @RowsInserted, ' rows into OrderItemsV2');
    WAITFOR DELAY '00:00:00.050';
  END
END

SET @RowsInserted = 1;

WHILE @RowsInserted > 0
BEGIN
  ;WITH cte AS (
    SELECT TOP (@BatchSize) o.*, oi.Id as orderItemId
    FROM ordering.OrderItemFunding o
    LEFT JOIN ordering.OrderItemsV2 oi ON oi.OrderId = o.OrderId AND oi.CatalogueItemId = o.CatalogueItemId
    ORDER BY o.OrderId, o.CatalogueItemId
  )
  INSERT INTO ordering.OrderItemFundingV2(OrderItemId, OrderItemFundingType, LastUpdated, LastUpdatedBy)
  SELECT 
    c.orderItemId,
    c.OrderItemFundingType,
    c.LastUpdated,
    c.LastUpdatedBy
  FROM cte c
    WHERE NOT EXISTS (
    SELECT 1 FROM ordering.OrderItemFundingV2 oif WHERE oif.OrderItemId = c.orderItemId
    )

  SET @RowsInserted = @@ROWCOUNT

  IF @RowsInserted > 0
  BEGIN
    PRINT CONCAT('Inserted batch of ', @RowsInserted, ' rows into OrderItemFundingV2');
    WAITFOR DELAY '00:00:00.050';
  END
END

SET @RowsInserted = 1

WHILE @RowsInserted > 0
BEGIN
  ;WITH cte AS (
    SELECT TOP (@BatchSize) o.*, oi.Id as orderItemId
    FROM ordering.OrderItemPrices o
    LEFT JOIN ordering.OrderItemsV2 oi ON oi.OrderId = o.OrderId AND oi.CatalogueItemId = o.CatalogueItemId
    ORDER BY o.OrderId, o.CatalogueItemId
  )
  INSERT INTO ordering.OrderItemPricesV2(OrderItemId, CataloguePriceId, BillingPeriodId, ProvisioningTypeId, CataloguePriceTypeId, CataloguePriceCalculationTypeId, 
  CurrencyCode, Description, RangeDescription, LastUpdated, LastUpdatedBy)
  SELECT 
    c.orderItemId,
    c.CataloguePriceId,
    c.BillingPeriodId,
    c.ProvisioningTypeId,
    c.CataloguePriceTypeId,
    c.CataloguePriceCalculationTypeId,
    c.CurrencyCode,
    c.Description,
    c.RangeDescription,
    c.LastUpdated,
    c.LastUpdatedBy
  FROM cte c
  WHERE NOT EXISTS (
    SELECT 1 FROM ordering.OrderItemPricesV2 oip WHERE oip.OrderItemId = c.orderItemId
    )

  SET @RowsInserted = @@ROWCOUNT

  IF @RowsInserted > 0
  BEGIN
    PRINT CONCAT('Inserted batch of ', @RowsInserted, ' rows into OrderItemPricesV2');
    WAITFOR DELAY '00:00:00.050';
  END
END

SET @RowsInserted = 1

WHILE @RowsInserted > 0
BEGIN
    UPDATE cbi 
    SET cbi.OrderItemId = oi.Id FROM ordering.ContractBillingItems cbi 
    LEFT JOIN ordering.OrderItemsV2 oi ON cbi.OrderId = oi.OrderId AND cbi.CatalogueItemId = oi.CatalogueItemId
    WHERE cbi.OrderItemId IS NULL

    SET @RowsInserted = @@ROWCOUNT
END

SET @RowsInserted = 1

WHILE @RowsInserted > 0
BEGIN
    UPDATE req
    SET req.OrderItemId = oi.Id FROM ordering.Requirements req
    LEFT JOIN ordering.OrderItemsV2 oi ON req.OrderId = oi.OrderId AND req.CatalogueItemId = oi.CatalogueItemId
    WHERE req.OrderItemId IS NULL

    SET @RowsInserted = @@ROWCOUNT
END

SET @RowsInserted = 1

WHILE @RowsInserted > 0
BEGIN
    UPDATE oipt
    SET oipt.OrderItemPriceId = oip.Id FROM ordering.OrderItemPriceTiers oipt
    LEFT JOIN ordering.OrderItemsV2 oi ON oipt.OrderId = oi.OrderId AND oipt.CatalogueItemId = oi.CatalogueItemId
    LEFT JOIN ordering.OrderItemPricesV2 oip ON oip.OrderItemId = oi.Id
    WHERE oipt.OrderItemPriceId IS NULL AND oip.Id IS NOT NULL;

    SET @RowsInserted = @@ROWCOUNT
END

SET @RowsInserted = 1

WHILE @RowsInserted > 0
BEGIN
  ;WITH cte_raw AS (
    SELECT TOP (@BatchSize) oisr.*, oi.Id as OrderItemId
    FROM ordering.OrderItemSublocationRecipients oisr
    LEFT JOIN ordering.OrderItemsV2 oi ON oisr.OrderId = oi.OrderId AND oisr.CatalogueItemId = oi.CatalogueItemId
    ORDER BY oisr.OrderId, oisr.ParentSublocationOdsCode, oisr.RecipientOdsCode
  ),
  cte AS (
    SELECT DISTINCT OrderItemId, OrderId, ParentSublocationOdsCode, RecipientOdsCode, Quantity, DeliveryDate, LastUpdated, LastUpdatedBy
    FROM cte_raw
  )
  INSERT INTO ordering.OrderItemSublocationRecipientsV2(OrderItemId, OrderId, ParentSublocationOdsCode, RecipientOdsCode, Quantity, DeliveryDate, LastUpdated, LastUpdatedBy)
  SELECT 
    c.OrderItemId,
    c.OrderId,
    c.ParentSublocationOdsCode,
    c.RecipientOdsCode,
    c.Quantity,
    c.DeliveryDate,
    c.LastUpdated,
    c.LastUpdatedBy
  FROM cte c
  WHERE c.OrderItemId IS NOT NULL
  AND NOT EXISTS (
    SELECT 1 FROM ordering.OrderItemSublocationRecipientsV2 t
    WHERE t.OrderItemId = c.OrderItemId
      AND t.OrderId = c.OrderId
      AND t.ParentSublocationOdsCode = c.ParentSublocationOdsCode
      AND t.RecipientOdsCode = c.RecipientOdsCode
  );

  SET @RowsInserted = @@ROWCOUNT

  IF @RowsInserted > 0
  BEGIN
    PRINT CONCAT('Inserted batch of ', @RowsInserted, ' rows into OrderItemSublocationRecipientsV2');
    WAITFOR DELAY '00:00:00.050';
  END
END

ROLLBACK