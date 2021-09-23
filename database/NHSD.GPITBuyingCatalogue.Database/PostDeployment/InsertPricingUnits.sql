CREATE TABLE #PricingUnit
(
    Id smallint NOT NULL PRIMARY KEY,
    TierName nvarchar(30) NOT NULL,
    [Description] nvarchar(40) NOT NULL
);

INSERT INTO #PricingUnit(Id, TierName, [Description])
    VALUES (1, 'patients', 'per patient');

IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
BEGIN
    INSERT INTO #PricingUnit(Id, TierName, [Description])
    VALUES
        (-1,  'patients'       , 'per patient'),
        (-2,  'beds'           , 'per bed'),
        (-3,  'consultations'  , 'per consultation'),
        (-4,  'licences'       , 'per licence'),
        (-5,  'SMS'            , 'per SMS'),
        (-6,  'half days'      , 'per half day'),
        (-7,  'hours'          , 'per hour'),
        (-8,  'courses'        , 'per course'),

        -- TODO: remove once process set-up to seed test environments with production-like data
        -- (Currently required for ProdLikeData/MergeCataloguePrices)
        (-9,  'practices'      , 'per practice'),
        (-10, 'days'           , 'per day'),
        (-11, 'active users'   , 'per active user'),
        (-12, 'consultations'  , 'per consultation – core hours'),
        (-13, 'consultations'  , 'per consultation – extended hours'),
        (-14, 'consultations'  , 'per consultation – out of hours'),
        (-15, 'SystmOne units' , 'per SystmOne unit (test'),
        (-16, 'items'          , 'per item'),
        (-17, 'SMS fragments'  , 'per SMS fragment'),
        (-18, 'hour sessions'  , 'per 1 hour session'),
        (-19, 'extracts'       , 'per extract'),
        (-20, 'documents'      , 'per document'),
        (-21, 'implementations', 'per implementation'),
        (-22, 'installations'  , 'per installation'),
        (-23, 'migrations'     , 'per migration'),
        (-24, 'mergers/splits' , 'per practice merge/split'),
        (-25, 'seminars'       , 'per seminar'),
        (-26, 'environments'   , 'per training environment'),
        (-27, 'unit merges'    , 'per unit merge'),
        (-28, 'unit splits'    , 'per unit split'),
        (-29, 'users'          , 'per user');
END;

    SET IDENTITY_INSERT catalogue.PricingUnits ON;

    MERGE INTO catalogue.PricingUnits AS TARGET
    USING #PricingUnit AS SOURCE
    ON TARGET.Id = SOURCE.Id
    WHEN MATCHED THEN
        UPDATE SET TARGET.TierName = SOURCE.TierName,
                   TARGET.[Description] = SOURCE.[Description]
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Id, TierName, [Description])
        VALUES (SOURCE.Id, SOURCE.TierName, SOURCE.[Description]);

    SET IDENTITY_INSERT catalogue.PricingUnits OFF;

    DROP TABLE #PricingUnit;
GO
