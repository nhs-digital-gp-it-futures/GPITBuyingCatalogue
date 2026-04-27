BEGIN TRANSACTION
DECLARE @SolutionStandards AS TABLE
                              (
                                  [SolutionId] NVARCHAR(14) NOT NULL,
                                  [StandardId] NVARCHAR(5)  NOT NULL,
                                  [Status]     INT          NULL
                              );

INSERT INTO @SolutionStandards
    ([SolutionId], [StandardId])
SELECT SO.CatalogueItemId, ST.Id
FROM catalogue.Solutions AS SO
         JOIN catalogue.Standards AS ST
              ON ST.StandardTypeId = 1
                  AND ST.IsDeleted = 0
UNION
SELECT CI.Id, SC.StandardId
FROM catalogue.CatalogueItemCapabilities AS CIC
         JOIN catalogue.CatalogueItems AS CI
              ON CI.Id = CIC.CatalogueItemId
         JOIN catalogue.StandardsCapabilities AS SC
              ON SC.CapabilityId = CIC.CapabilityId
         JOIN catalogue.Standards AS ST
              ON ST.Id = SC.StandardId
WHERE CI.CatalogueItemTypeId = 1
  AND ST.StandardTypeId <> 1
  AND ST.IsDeleted = 0;

UPDATE S
SET [Status] = IIF(EXISTS(SELECT 1
                          FROM catalogue.InProgressSolutionStandards IPSS
                          WHERE IPSS.SolutionId = SolutionId
                            AND IPSS.StandardId = StandardId), 4, 5)
FROM @SolutionStandards AS S

MERGE INTO catalogue.SolutionStandards AS TARGET
USING @SolutionStandards AS SOURCE
ON TARGET.[StandardId] = SOURCE.[StandardId] AND TARGET.[SolutionId] = SOURCE.[SolutionId]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([StandardId], [SolutionId], [Status])
    VALUES (SOURCE.[StandardId], SOURCE.[SolutionId], SOURCE.[Status])
WHEN MATCHED THEN
    UPDATE
    SET TARGET.[Status] = SOURCE.[Status];

COMMIT TRANSACTION
