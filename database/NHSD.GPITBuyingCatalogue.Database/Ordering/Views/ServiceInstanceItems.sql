CREATE VIEW ordering.ServiceInstanceItems AS
WITH ServiceInstanceIncrement AS
(
    SELECT r.OrderId, r.CatalogueItemId, r.OdsCode,
           DENSE_RANK() OVER (
               PARTITION BY r.OrderId, r.OdsCode
                   ORDER BY CASE WHEN c.CatalogueItemTypeId = 1 THEN r.CatalogueItemId ELSE a.SolutionId END) AS ServiceInstanceIncrement
      FROM ordering.OrderItemRecipients AS r
           INNER JOIN catalogue.CatalogueItems AS c ON c.CatalogueItemId = r.CatalogueItemId
                   AND c.CatalogueItemTypeId IN (1, 2)
           LEFT OUTER JOIN catalogue.AdditionalServices AS a ON a.CatalogueItemId = r.CatalogueItemId
       WHERE (a.SolutionId IS NULL OR EXISTS (
            SELECT *
              FROM ordering.OrderItemRecipients AS r2
             WHERE r2.OrderId = r.OrderId
               AND r2.OdsCode = r.OdsCode
               AND r2.CatalogueItemId = a.SolutionId))
)
SELECT r.OrderId, r.CatalogueItemId, r.OdsCode,
       'SI' + CAST(s.ServiceInstanceIncrement AS nvarchar(3)) + '-' + r.OdsCode AS ServiceInstanceId
  FROM ordering.OrderItemRecipients AS r
       LEFT OUTER JOIN ServiceInstanceIncrement AS s
               ON s.OrderId = r.OrderId
              AND s.CatalogueItemId = r.CatalogueItemId
              AND s.OdsCode = r.OdsCode;
