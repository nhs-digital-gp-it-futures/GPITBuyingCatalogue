CREATE TABLE #PricingUnit
(
    Id smallint NOT NULL PRIMARY KEY,
    [Name] nvarchar(20) NOT NULL UNIQUE,
    [RangeDescription] nvarchar(30) NOT NULL,
    [Description] nvarchar(40) NOT NULL
);

INSERT INTO #PricingUnit(Id, [Name], [RangeDescription], [Description])
    VALUES (1, 'patient', 'patients', 'per patient');

IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
BEGIN
    INSERT INTO #PricingUnit(Id, [Name], [RangeDescription], [Description])
    VALUES
        (-1,  'testPatient'         , 'patients'       , 'per patient'),
        (-2,  'testBed'             , 'beds'           , 'per bed'),
        (-3,  'testConsultation'    , 'consultations'  , 'per consultation'),
        (-4,  'testLicence'         , 'licences'       , 'per licence'),
        (-5,  'testSms'             , 'SMS'            , 'per SMS'),
        (-6,  'testHalfDay'         , 'half days'      , 'per half day'),
        (-7,  'testHour'            , 'hours'          , 'per hour'),
        (-8,  'testCourse'          , 'courses'        , 'per course'),

        -- TODO: remove once process set-up to seed test environments with production-like data
        -- (Currently required for ProdLikeData/MergeCataloguePrices)
        (-9,  'testPractice'        , 'practices'      , 'per practice'),
        (-10, 'testDay'             , 'days'           , 'per day'),
        (-11, 'testActiveUser'      , 'active users'   , 'per active user'),
        (-12, 'testConsultationCore', 'consultations'  , 'per consultation – core hours'),
        (-13, 'testConsultationExt' , 'consultations'  , 'per consultation – extended hours'),
        (-14, 'testConsultationOut' , 'consultations'  , 'per consultation – out of hours'),
        (-15, 'testSystmOneUnit'    , 'SystmOne units' , 'per SystmOne unit (test'),
        (-16, 'testItem'            , 'items'          , 'per item'),
        (-17, 'testSmsFragment'     , 'SMS fragments'  , 'per SMS fragment'),
        (-18, 'testHourSession'     , 'hour sessions'  , 'per 1 hour session'),
        (-19, 'testExtract'         , 'extracts'       , 'per extract'),
        (-20, 'testDocument'        , 'documents'      , 'per document'),
        (-21, 'testImplementation'  , 'implementations', 'per implementation'),
        (-22, 'testInstallation'    , 'installations'  , 'per installation'),
        (-23, 'testMigration'       , 'migrations'     , 'per migration'),
        (-24, 'testMergerSplit'     , 'mergers/splits' , 'per practice merge/split'),
        (-25, 'testSeminar'         , 'seminars'       , 'per seminar'),
        (-26, 'testTrainingEnv'     , 'environments'   , 'per training environment'),
        (-27, 'testUnitMerge'       , 'unit merges'    , 'per unit merge'),
        (-28, 'testUnitSplit'       , 'unit splits'    , 'per unit split'),
        (-29, 'testUser'            , 'users'          , 'per user'),
        (2, N'dummy1', N'patients', N'per patient'),
        (3, N'dummy2', N'patients', N'per patient'),
        (4, N'dummy3', N'patients', N'per patient'),
        (5, N'dummy4', N'solutions', N'per solution'),
        (6, N'dummy5', N'recipients', N'per recipient'),
        (7, N'dummy6', N'recipients', N'per recipient'),
        (8, N'dummy7', N'solutions', N'per solution'),
        (9, N'dummy8', N'solutions', N'per solution'),
        (10, N'dummy9', N'recipients', N'per recipient'),
        (11, N'dummy10', N'recipients', N'per recipient'),
        (12, N'dummy11', N'patients', N'per patient'),
        (13, N'dummy12', N'patients', N'per patient'),
        (14, N'dummy13', N'solutions', N'per solution'),
        (15, N'dummy14', N'solutions', N'per solution'),
        (16, N'dummy15', N'recipients', N'per recipient'),
        (17, N'dummy16', N'recipients', N'per recipient'),
        (18, N'dummy17', N'solutions', N'per solution'),
        (19, N'dummy18', N'solutions', N'per solution'),
        (20, N'dummy19', N'recipients', N'per recipient'),
        (21, N'dummy20', N'recipients', N'per recipient'),
        (22, N'dummy21', N'licenses', N'per license'),
        (23, N'dummy22', N'licenses', N'per license');
END;

    SET IDENTITY_INSERT catalogue.PricingUnits ON;

    MERGE INTO catalogue.PricingUnits AS TARGET
    USING #PricingUnit AS SOURCE
    ON TARGET.Id = SOURCE.Id
    WHEN MATCHED THEN
        UPDATE SET TARGET.[Name] = SOURCE.[Name],
                   TARGET.[RangeDescription] = SOURCE.[RangeDescription],
                   TARGET.[Description] = SOURCE.[Description]
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Id, [Name], [RangeDescription], [Description])
        VALUES (SOURCE.Id, SOURCE.[Name], SOURCE.[RangeDescription], SOURCE.[Description]);

    SET IDENTITY_INSERT catalogue.PricingUnits OFF;

    DROP TABLE #PricingUnit;
GO
