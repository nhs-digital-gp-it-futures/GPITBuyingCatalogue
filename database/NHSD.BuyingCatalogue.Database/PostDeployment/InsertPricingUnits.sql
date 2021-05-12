CREATE TABLE #PricingUnit
(
    PricingUnitId uniqueidentifier NOT NULL,
    [Name] nvarchar(20) NOT NULL,
    TierName nvarchar(30) NOT NULL,
    [Description] nvarchar(85) NOT NULL
);

INSERT INTO #PricingUnit(PricingUnitId, [Name], TierName, [Description])
VALUES
    ('F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 'patient'            , 'patients'       , 'per patient'),
    ('D43C661A-0587-45E1-B315-5E5091D6E9D0', 'bed'                , 'beds'           , 'per bed'),
    ('774E5A1D-D15C-4A37-9990-81861BEAE42B', 'consultation'       , 'consultations'  , 'per consultation'),
    ('8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', 'licence'            , 'licences'       , 'per licence'),
    ('90119522-D381-4296-82EE-8FE630593B56', 'sms'                , 'SMS'            , 'per SMS'),
    ('aad2820e-472d-4bac-864e-853f92e9b3bc', 'practice'           , 'practices'      , 'per practice'),
    ('cc6ee39d-41f1-4671-b31a-800485d05752', 'unit'               , 'units'          , 'per unit'),
    ('9d3bade6-f232-4b6e-9809-88a8fbb5c881', 'group'              , 'groups'         , 'per group'),
    ('599a1105-a16a-4861-b54b-d65da84366c9', 'day'                , 'days'           , 'per day'),
    ('121bd710-b73b-48f9-a429-f269a7bb0bf2', 'halfDay'            , 'half days'      , 'per half day'),
    ('823f814d-82c9-4994-94af-4c413ee418dc', 'hour'               , 'hours'          , 'per hour'),
    ('7e4dd1fd-c953-41a8-9e62-64dc306a6307', 'installation'       , 'installations'  , 'per installation'),
    ('701afb98-699e-4eda-9a66-e79a91769614', 'implementation'     , 'implementations', 'per implementation'),
    ('f2bb1b9d-b546-40b3-bfd9-d55221d96880', 'practiceMergerSplit', 'mergers/splits' , 'per practice merge/split'),
    ('6f65c40f-e7cc-4140-85c5-592dcd216132', 'extract'            , 'extracts'       , 'per extract'),
    ('59fa7cad-87b8-4e78-92b7-5689f6b123dc', 'migration'          , 'migrations'     , 'per migration'),
    ('e17fbd0b-208f-453f-938a-9880bab1ec5e', 'course'             , 'courses'        , 'per course'),
    ('1d40c0d1-6bd5-40b3-ba2f-cf433f339787', 'trainingEnvironment', 'environments'   , 'per training environment'),
    ('4b9a4640-a97a-4e30-8ed5-cccae9829616', 'user'               , 'users'          , 'per user'),
    ('66443acc-7e40-4f95-955b-47234016cff1', 'document'           , 'documents'      , 'per document'),
    ('626b43e6-c9a0-4ede-99ed-da3a1ad2d8d3', 'seminar'            , 'seminars'       , 'per seminar'),
    ('60523726-bbaf-4ec3-b29c-dee2f3d3eca8', 'item'               , 'items'          , 'per item'),
    ('8a5e119f-9b33-4017-8cc9-552e86e20898', 'activeUser'         , 'active users'   , 'per active user'),
    ('84c10564-e85f-4f64-843b-528e88b7bf59', 'virtualPC'          , 'virtual PCs'    , 'per virtual PC'),
    ('7d709183-90ad-4b35-b399-010014bb1b9b', 'transaction'        , 'transactions'   , 'per transaction'),
    ('c74e980c-bb59-4b5a-96ce-1e616bdf827c', 'nurseReview'        , 'nurse reviews'  , 'per nurse review'),
    ('34acc3bf-c036-4125-9eab-23fbe39f6352', 'review'             , 'reviews'        , 'per review'),
    --Units in the data that may be amended / consolidated
    ('4b39590d-3f35-4963-83ba-bc7d0bfe988b', 'videoConsultation', 'consultations', 'per video consultation initiated'),
    ('372787ad-041f-4176-93e9-e4a303c39014', 'digitalFirstConsult', 'consultations', 'per digital first consultation'),
    ('a4012e6c-caf3-430c-b8d3-9c45ab9fd0de', 'unitMerge', 'unit merges', 'per unit merge'),
    ('bede8599-7a4e-4753-a928-f419681b7c93', 'unitSplit', 'unit splits', 'per unit split'),
    ('8eea4a69-977d-4fb1-b4d1-2f0971beb04b', 'hourSession', 'hour sessions', 'per 1 hour session'),
    ('A92C1326-4826-48B3-B429-4A368ADB9785', 'na','',''),
    --New Units in confirmed spreadsheet 27/08/2020
    ('60d07eb0-01ef-44e4-bed3-d34ad1352e19', 'consultationCore'    , 'consultations'                , 'per consultation – core hours'),
    ('93931091-8945-43a0-b181-96f2b41ed3c3', 'consultationExtended', 'consultations'                , 'per consultation – extended hours'),
    ('fec28905-5670-4c45-99f3-1f93c8aa156c', 'consultationOut'     , 'consultations'                , 'per consultation – out of hours'),
    ('9f8888de-69fb-4395-83ce-066d4a4dc120', 'systmOneUnit'        , 'SystmOne units'               , 'per SystmOne unit'),
    ('e72500e5-4cb4-4ddf-a8b8-d898187691ca', 'smsFragment'         , 'SMS fragments'                , 'per SMS fragment'),
    ('1ba99da5-44a8-4dc9-97e7-50c450842191', 'usersPerSite_5'      , 'users per site'               , 'up to 5 users per site'),
    ('bf5b9d2c-d690-41d2-9075-7558ad3f3f1a', 'usersPerSite_10'     , 'users per site'               , 'up to 10 users per site'),
    ('8a7fe8b5-83eb-4d12-939b-53e823ecc624', 'usersPerSite_15'     , 'users per site'               , 'up to 15 users per site'),
    ('b53a9db9-697b-4184-8177-28e9d0f66142', 'usersPerSite_16'     , 'users per site'               , '16+ users per site'),
    ('72f164c0-5eeb-4df1-b3ba-68943c0ae86c', 'traineesPerCourse_5' , 'trainees per course'          , 'per 5 trainees per course'),
    ('f5975c2e-a5fd-40a9-9030-cf02227e60b1', 'consultationRequest' , 'requests for consultation'    , 'per request for consultation'),
    ('2ca3b90b-6073-48c6-9162-acc89c6cd459', 'serviceForSites_5'   , 'sites'                        , 'service for up to 5 sites'),
    ('bbd35a61-8baf-43c2-bb93-7942b99bd004', 'serviceForSites_10'  , 'sites'                        , 'service for up to 10 sites'),
    ('f20469f3-7ef5-4dac-ae17-ad1b7c69e9c2', 'serviceForSites_11'  , 'sites'                        , 'service for 11+ sites'),
    ('e6946b09-28a8-4fb5-af57-12ad9247f850', 'callOff'             , 'Call-offs'                    , 'per Call-off'),
    ('720f2d4d-448d-4899-ad40-979b30911ca6', 'carePlan'            , 'care plan/Call-offs'          , 'per custom care plan/Call-off'),
    ('05281ffc-1077-41d5-a253-3077540ef2e9', 'organisation'        , 'organisations'                , 'per organisation'),
    ('a7eb74d3-2615-4fb5-8083-cabd40ca8cba', 'carePlans_1'         , 'care plans'                   , 'per patient for 1 care plan'),
    ('69329f3d-76ac-46f3-88dc-0ea0409975b8', 'carePlans_2'         , 'care plans'                   , 'per patient for 2-5 care plans'),
    ('a973174d-b4b1-4a28-8ab6-6334fb8159bd', 'carePlans_6'         , 'care plans'                   , 'per patient for 6-10 care plans'),
    ('68e39619-d5b1-4355-845d-5b2b20d4d0d3', 'carePlans_11'        , 'care plans'                   , 'per patient for 11-20 care plans'),
    ('af3c90a8-a8c1-46d9-a6de-d97ec3d6087f', 'carePlans_21'        , 'care plans'                   , 'per patient for 21+ care plans'),
    ('9a9cc023-e799-4a46-892f-6e98f462cd0e', 'service'             , 'services'                     , 'per service'),
    ('2cb9d70f-cd40-4f86-aa63-829f030e63dc', 'patients_0-50k'      , 'patients'                     , 'per patient for 0-49,999 patients'),
    ('5fff29ee-a360-4077-b712-73abff3a7f0b', 'patients_50-500k'    , 'patients'                     , 'per patient for 50,000-499,999 patients'),
    ('79b62a1b-8e86-4be5-95e6-c19aa65af4d4', 'patients_500k+'      , 'patients'                     , 'per patient for 500,000+ patients'),
    ('d96142d4-2190-43b4-83a1-1ad8ffc66532', 'additionalPractice'  , 'additional practices'         , 'per additional practice'),
    ('0b8b296e-3d5a-4fd2-8614-fd3df220b394', 'incomingPractice'    , 'incoming practices'           , 'per incoming practice'),
    ('cba9431d-115b-4c62-b0e5-bf11aa82dbd0', 'groupMigration'      , 'group migrations'             , 'per group migration'),
    ('d29a3db3-5426-44f4-9dc6-4569f4561958', 'session'             , 'sessions'                     , 'per session'),
    ('11ecd056-e2ac-45a7-bbf8-a274e0ca8320', 'system'              , 'systems'                      , 'per system'),
    ('fb3b6d1b-78fb-4733-a6cb-6d18582e273e', 'keystoneCapability'  , 'Keystone capabilities'        , 'per Keystone capability'),
    ('8a7683b3-e39a-4f44-b387-1f2f0e33f0d7', 'clinicalUser'        , 'concurrent clinical users'    , 'per concurrent clinical user');

MERGE INTO dbo.PricingUnit AS TARGET
USING #PricingUnit AS SOURCE
ON TARGET.PricingUnitId = SOURCE.PricingUnitId
WHEN MATCHED THEN
    UPDATE SET TARGET.[Name] = SOURCE.[Name],
               TARGET.TierName = SOURCE.TierName,
               TARGET.[Description] = SOURCE.[Description]
WHEN NOT MATCHED BY TARGET THEN
    INSERT (PricingUnitId, [Name], TierName, [Description])
    VALUES (SOURCE.PricingUnitId, SOURCE.[Name], SOURCE.TierName, SOURCE.[Description]);

DROP TABLE #PricingUnit;
GO
