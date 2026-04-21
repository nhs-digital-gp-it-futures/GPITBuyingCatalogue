SET XACT_ABORT ON;
SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @BatchSize INT = 2000;
    DECLARE @RowsInserted INT = 1;

    WHILE @RowsInserted > 0
    BEGIN
      MERGE INTO ordering.OrderItemsV2 AS TARGET
      USING (
        SELECT TOP (@BatchSize)
          o.OrderId,
          o.CatalogueItemId,
          o.PriceId,
          o.Price,
          o.EstimationPeriodId,
          o.DefaultDeliveryDate,
          o.Created,
          o.LastUpdated,
          o.LastUpdatedBy,
          o.Quantity
        FROM ordering.OrderItems o
        ORDER BY o.OrderId, o.CatalogueItemId
      ) AS SOURCE
        ON TARGET.OrderId = SOURCE.OrderId
       AND TARGET.CatalogueItemId = SOURCE.CatalogueItemId
      WHEN NOT MATCHED BY TARGET THEN
        INSERT (
          OrderId,
          CatalogueItemId,
          PriceId,
          Price,
          EstimationPeriodId,
          DefaultDeliveryDate,
          Created,
          LastUpdated,
          LastUpdatedBy,
          Quantity
        )
        VALUES (
          SOURCE.OrderId,
          SOURCE.CatalogueItemId,
          SOURCE.PriceId,
          SOURCE.Price,
          SOURCE.EstimationPeriodId,
          SOURCE.DefaultDeliveryDate,
          SOURCE.Created,
          SOURCE.LastUpdated,
          SOURCE.LastUpdatedBy,
          SOURCE.Quantity
        );

      SET @RowsInserted = @@ROWCOUNT;

      IF @RowsInserted > 0
      BEGIN
        PRINT CONCAT('Inserted batch of ', @RowsInserted, ' rows into OrderItemsV2');
        WAITFOR DELAY '00:00:00.050';
      END
    END;

    SET @RowsInserted = 1;

    WHILE @RowsInserted > 0
    BEGIN
      MERGE INTO ordering.OrderItemFundingV2 AS TARGET
      USING (
        SELECT TOP (@BatchSize)
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
        ORDER BY o.OrderId, o.CatalogueItemId
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

      IF @RowsInserted > 0
      BEGIN
        PRINT CONCAT('Inserted batch of ', @RowsInserted, ' rows into OrderItemFundingV2');
        WAITFOR DELAY '00:00:00.050';
      END
    END;

    SET @RowsInserted = 1;

    WHILE @RowsInserted > 0
    BEGIN
      MERGE INTO ordering.OrderItemPricesV2 AS TARGET
      USING (
        SELECT TOP (@BatchSize)
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
        ORDER BY o.OrderId, o.CatalogueItemId
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

      IF @RowsInserted > 0
      BEGIN
        PRINT CONCAT('Inserted batch of ', @RowsInserted, ' rows into OrderItemPricesV2');
        WAITFOR DELAY '00:00:00.050';
      END
    END;

    SET @RowsInserted = 1;

    WHILE @RowsInserted > 0
    BEGIN
      UPDATE cbi
      SET cbi.OrderItemId = oi.Id
      FROM ordering.ContractBillingItems cbi
      INNER JOIN ordering.OrderItemsV2 oi
        ON cbi.OrderId = oi.OrderId
       AND cbi.CatalogueItemId = oi.CatalogueItemId
      WHERE cbi.OrderItemId IS NULL;

      SET @RowsInserted = @@ROWCOUNT;
    END;

    SET @RowsInserted = 1;

    WHILE @RowsInserted > 0
    BEGIN
      UPDATE req
      SET req.OrderItemId = oi.Id
      FROM ordering.Requirements req
      INNER JOIN ordering.OrderItemsV2 oi
        ON req.OrderId = oi.OrderId
       AND req.CatalogueItemId = oi.CatalogueItemId
      WHERE req.OrderItemId IS NULL;

      SET @RowsInserted = @@ROWCOUNT;
    END;

    SET @RowsInserted = 1;

    WHILE @RowsInserted > 0
    BEGIN
      UPDATE oipt
      SET oipt.OrderItemPriceId = oip.Id
      FROM ordering.OrderItemPriceTiers oipt
      INNER JOIN ordering.OrderItemsV2 oi
        ON oipt.OrderId = oi.OrderId
       AND oipt.CatalogueItemId = oi.CatalogueItemId
      INNER JOIN ordering.OrderItemPricesV2 oip
        ON oip.OrderItemId = oi.Id
      WHERE oipt.OrderItemPriceId IS NULL;

      SET @RowsInserted = @@ROWCOUNT;
    END;

    SET @RowsInserted = 1;

    WHILE @RowsInserted > 0
    BEGIN
      MERGE INTO ordering.OrderItemSublocationRecipientsV2 AS TARGET
      USING (
        SELECT TOP (@BatchSize)
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
        ORDER BY oisr.OrderId, oisr.ParentSublocationOdsCode, oisr.RecipientOdsCode
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

      IF @RowsInserted > 0
      BEGIN
        PRINT CONCAT('Inserted batch of ', @RowsInserted, ' rows into OrderItemSublocationRecipientsV2');
        WAITFOR DELAY '00:00:00.050';
      END
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;