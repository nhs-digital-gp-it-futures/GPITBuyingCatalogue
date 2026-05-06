SET XACT_ABORT ON;
SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @RowsInserted INT;

    MERGE INTO ordering.OrderItemsV2 AS TARGET
    USING (
        SELECT
          o.OrderId,
          o.CatalogueItemId,
          o.EstimationPeriodId,
          o.Created,
          o.LastUpdated,
          o.LastUpdatedBy
        FROM ordering.OrderItems o
    ) AS SOURCE
      ON TARGET.OrderId = SOURCE.OrderId
     AND TARGET.CatalogueItemId = SOURCE.CatalogueItemId
    WHEN NOT MATCHED BY TARGET THEN
      INSERT (
        OrderId,
        CatalogueItemId,
        EstimationPeriodId,
        Created,
        LastUpdated,
        LastUpdatedBy
      )
      VALUES (
        SOURCE.OrderId,
        SOURCE.CatalogueItemId,
        SOURCE.EstimationPeriodId,
        SOURCE.Created,
        SOURCE.LastUpdated,
        SOURCE.LastUpdatedBy
      );

    SET @RowsInserted = @@ROWCOUNT;
    PRINT CONCAT('OrderItemsV2 rows affected: ', @RowsInserted);

    MERGE INTO ordering.OrderItemFundingV2 AS TARGET
    USING (
        SELECT
          oi.Id AS OrderItemId,
          o.OrderItemFundingType,
          o.LastUpdated,
          o.LastUpdatedBy
        FROM ordering.OrderItemFunding o
        INNER JOIN ordering.OrderItemsV2 oi
          ON oi.OrderId = o.OrderId
         AND oi.CatalogueItemId = o.CatalogueItemId
        WHERE NOT EXISTS (
          SELECT 1
          FROM ordering.OrderItemFundingV2 oif
          WHERE oif.OrderItemId = oi.Id
        )
    ) AS SOURCE
      ON TARGET.OrderItemId = SOURCE.OrderItemId
    WHEN NOT MATCHED BY TARGET THEN
      INSERT (
        OrderItemId,
        OrderItemFundingType,
        LastUpdated,
        LastUpdatedBy
      )
      VALUES (
        SOURCE.OrderItemId,
        SOURCE.OrderItemFundingType,
        SOURCE.LastUpdated,
        SOURCE.LastUpdatedBy
      );

    SET @RowsInserted = @@ROWCOUNT;
    PRINT CONCAT('OrderItemFundingV2 rows affected: ', @RowsInserted);

    MERGE INTO ordering.OrderItemPricesV2 AS TARGET
    USING (
        SELECT
          oi.Id AS OrderItemId,
          o.CataloguePriceId,
          o.BillingPeriodId,
          o.ProvisioningTypeId,
          o.CataloguePriceTypeId,
          o.CataloguePriceCalculationTypeId,
          o.CurrencyCode,
          o.Description,
          o.RangeDescription,
          o.LastUpdated,
          o.LastUpdatedBy
        FROM ordering.OrderItemPrices o
        INNER JOIN ordering.OrderItemsV2 oi
          ON oi.OrderId = o.OrderId
         AND oi.CatalogueItemId = o.CatalogueItemId
        WHERE NOT EXISTS (
          SELECT 1
          FROM ordering.OrderItemPricesV2 oip
          WHERE oip.OrderItemId = oi.Id
        )
    ) AS SOURCE
      ON TARGET.OrderItemId = SOURCE.OrderItemId
    WHEN NOT MATCHED BY TARGET THEN
      INSERT (
        OrderItemId,
        CataloguePriceId,
        BillingPeriodId,
        ProvisioningTypeId,
        CataloguePriceTypeId,
        CataloguePriceCalculationTypeId,
        CurrencyCode,
        Description,
        RangeDescription,
        LastUpdated,
        LastUpdatedBy
      )
      VALUES (
        SOURCE.OrderItemId,
        SOURCE.CataloguePriceId,
        SOURCE.BillingPeriodId,
        SOURCE.ProvisioningTypeId,
        SOURCE.CataloguePriceTypeId,
        SOURCE.CataloguePriceCalculationTypeId,
        SOURCE.CurrencyCode,
        SOURCE.Description,
        SOURCE.RangeDescription,
        SOURCE.LastUpdated,
        SOURCE.LastUpdatedBy
      );

    SET @RowsInserted = @@ROWCOUNT;
    PRINT CONCAT('OrderItemPricesV2 rows affected: ', @RowsInserted);

    UPDATE cbi
    SET cbi.OrderItemId = oi.Id
    FROM ordering.ContractBillingItems cbi
    INNER JOIN ordering.OrderItemsV2 oi
      ON cbi.OrderId = oi.OrderId
     AND cbi.CatalogueItemId = oi.CatalogueItemId
    WHERE cbi.OrderItemId IS NULL;

    PRINT CONCAT('ContractBillingItems updated: ', @@ROWCOUNT);

    UPDATE req
    SET req.OrderItemId = oi.Id
    FROM ordering.Requirements req
    INNER JOIN ordering.OrderItemsV2 oi
      ON req.OrderId = oi.OrderId
     AND req.CatalogueItemId = oi.CatalogueItemId
    WHERE req.OrderItemId IS NULL;

    PRINT CONCAT('Requirements updated: ', @@ROWCOUNT);

    UPDATE oipt
    SET oipt.OrderItemPriceId = oip.Id
    FROM ordering.OrderItemPriceTiers oipt
    INNER JOIN ordering.OrderItemsV2 oi
      ON oipt.OrderId = oi.OrderId
     AND oipt.CatalogueItemId = oi.CatalogueItemId
    INNER JOIN ordering.OrderItemPricesV2 oip
      ON oip.OrderItemId = oi.Id
    WHERE oipt.OrderItemPriceId IS NULL;

    PRINT CONCAT('OrderItemPriceTiers updated: ', @@ROWCOUNT);

    MERGE INTO ordering.OrderItemSublocationRecipientsV2 AS TARGET
    USING (
        SELECT
          oi.Id AS OrderItemId,
          oisr.OrderId,
          oisr.ParentSublocationOdsCode,
          oisr.RecipientOdsCode,
          oisr.Quantity,
          oisr.DeliveryDate,
          oisr.LastUpdated,
          oisr.LastUpdatedBy
        FROM ordering.OrderItemSublocationRecipients oisr
        INNER JOIN ordering.OrderItemsV2 oi
          ON oi.OrderId = oisr.OrderId
         AND oi.CatalogueItemId = oisr.CatalogueItemId
    ) AS SOURCE
      ON TARGET.OrderItemId = SOURCE.OrderItemId
     AND TARGET.OrderId = SOURCE.OrderId
     AND TARGET.ParentSublocationOdsCode = SOURCE.ParentSublocationOdsCode
     AND TARGET.RecipientOdsCode = SOURCE.RecipientOdsCode
    WHEN NOT MATCHED BY TARGET THEN
      INSERT (
        OrderItemId,
        OrderId,
        ParentSublocationOdsCode,
        RecipientOdsCode,
        Quantity,
        DeliveryDate,
        LastUpdated,
        LastUpdatedBy
      )
      VALUES (
        SOURCE.OrderItemId,
        SOURCE.OrderId,
        SOURCE.ParentSublocationOdsCode,
        SOURCE.RecipientOdsCode,
        SOURCE.Quantity,
        SOURCE.DeliveryDate,
        SOURCE.LastUpdated,
        SOURCE.LastUpdatedBy
      );

    SET @RowsInserted = @@ROWCOUNT;
    PRINT CONCAT('OrderItemSublocationRecipientsV2 rows affected: ', @RowsInserted);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;