IF UPPER('$(INSERT_TEST_DATA)') = 'FALSE'
BEGIN
    BEGIN TRANSACTION

    BEGIN TRY

    -- Manually insert outlier CSU which doesn't fit data structure
    INSERT INTO [ordering].[OrderSublocations]
        ([OrderId], [SublocationOdsCode], [OwnerOdsCode])
    SELECT 10571, '0CX', '0CX'
        WHERE NOT EXISTS (
            SELECT 1
                FROM [ordering].[OrderSublocations]
                WHERE [OrderId] = 10571
                  AND [SublocationOdsCode] = '0CX'
                  AND [OwnerOdsCode] = '0CX');

    -- Insert missing relationships
    DECLARE @odsOrganisationRelationships
        AS TABLE
           (
               [Id]                   INT          NOT NULL PRIMARY KEY,
               [RelationshipTypeId]   NVARCHAR(10) NOT NULL,
               [TargetOrganisationId] NVARCHAR(8)  NOT NULL,
               [OwnerOrganisationId]  NVARCHAR(8)  NOT NULL,
               [IsActive]             BIT          NOT NULL DEFAULT (1)
           );

    INSERT INTO @odsOrganisationRelationships
               ([Id],      [RelationshipTypeId], [TargetOrganisationId], [OwnerOrganisationId], [IsActive])
        VALUES (999999000, 'RE4',                'K84049',               '10Q',                 1         ),
               (999999001, 'RE4',                'L84073',               '11M',                 1         ),
               (999999002, 'RE4',                'G82017',               '91Q',                 1         ),
               (999999003, 'RE4',                'M92002',               'D2P2L',               1         ),
               (999999004, 'RE4',                'Y06325',               '14L',                 1         ),
               (999999005, 'RE4',                'Y00260',               'M1J4Y',               1         ),
               (999999006, 'RE4',                'C82105',               '04C',                 1         ),
               (999999007, 'RE4',                'A86007',               '13T',                 1         ),
               (999999008, 'RE4',                'G82123',               '91Q',                 1         ),
               (999999009, 'RE4',                'G81084',               '97R',                 1         ),
               (999999010, 'RE4',                'Y05652',               '00V',                 1         ),
               (999999011, 'RE4',                'G85124',               '72Q',                 1         ),
               (999999012, 'RE4',                'G81680',               '09D',                 1         ),
               (999999013, 'RE4',                'H81037',               '92A',                 1         ),
               (999999014, 'RE4',                'C81661',               '15M',                 1         ),
               (999999015, 'RE4',                'Y02658',               '13T',                 1         ),
               (999999016, 'RE4',                'P85028',               '00Y',                 1         ),
               (999999017, 'RE4',                'N83015',               '01J',                 1         ),
               (999999018, 'RE4',                'K84025',               '10Q',                 1         ),
               (999999019, 'RE4',                'F85043',               '93C',                 1         ),
               (999999020, 'RE4',                'M85671',               '15E',                 1         ),
               (999999021, 'RE4',                'E81064',               'M1J4Y',               1         ),
               (999999022, 'RE4',                'J82135',               'D4U1Y',               1         ),
               (999999023, 'RE4',                'A87023',               '99C',                 1         ),
               (999999024, 'RE4',                'A86035',               '13T',                 1         ),
               (999999025, 'RE4',                'G82706',               '91Q',                 1         ),
               (999999026, 'RE4',                'H83625',               '36L',                 1         ),
               (999999027, 'RE4',                'L84071',               '11M',                 1         ),
               (999999028, 'RE4',                'G83037',               '72Q',                 1         ),
               (999999029, 'RE4',                'K81638',               '15A',                 1         ),
               (999999030, 'RE4',                'Y02414',               '05Q',                 1         ),
               (999999031, 'RE4',                'P88034',               '01W',                 1         ),
               (999999032, 'RE4',                'L82009',               '11N',                 1         ),
               (999999033, 'RE4',                'F86642',               'A3A8R',               1         ),
               (999999034, 'RE4',                'J82180',               'D9Y0V',               1         ),
               (999999035, 'RE4',                'E82089',               '06K',                 1         ),
               (999999036, 'RE4',                'A84032',               '00L',                 1         ),
               (999999037, 'RE4',                'E85625',               'W2U3Z',               1         ),
               (999999038, 'RE4',                'B81002',               '03F',                 1         ),
               (999999039, 'RE4',                'B81095',               '03F',                 1         ),
               (999999040, 'RE4',                'C82122',               '04C',                 1         ),
               (999999041, 'RE4',                'Y03296',               '72Q',                 1         ),
               (999999042, 'RE4',                'L81036',               '15C',                 1         ),
               (999999043, 'RE4',                'P84652',               '14L',                 1         ),
               (999999044, 'RE4',                'F84657',               'A3A8R',               1         ),
               (999999045, 'RE4',                'E83037',               '93C',                 1         ),
               (999999046, 'RE4',                'G82088',               '91Q',                 1         ),
               (999999047, 'RE4',                'F82621',               'A3A8R',               1         ),
               (999999048, 'RE4',                'F81024',               '06Q',                 1         ),
               (999999049, 'RE4',                'M92014',               'D2P2L',               1         ),
               (999999050, 'RE4',                'Y00912',               '01W',                 1         ),
               (999999051, 'RE4',                'E81001',               'M1J4Y',               1         ),
               (999999052, 'RE4',                'B86058',               '15F',                 1         ),
               (999999053, 'RE4',                'B86001',               '15F',                 1         ),
               (999999054, 'RE4',                'E85018',               'W2U3Z',               1         ),
               (999999055, 'RE4',                'K83607',               '78H',                 1         ),
               (999999056, 'RE4',                'B83617',               '36J',                 1         ),
               (999999057, 'RE4',                'F83027',               '93C',                 1         ),
               (999999058, 'RE4',                'Y02962',               '36L',                 1         ),
               (999999059, 'RE4',                'B82062',               '42D',                 1         ),
               (999999060, 'RE4',                'F84673',               'A3A8R',               1         ),
               (999999061, 'RE4',                'P85008',               '00Y',                 1         ),
               (999999062, 'RE4',                'Y04968',               '18C',                 1         ),
               (999999063, 'RE4',                'A84609',               '00L',                 1         ),
               (999999064, 'RE4',                'B81060',               '02Y',                 1         ),
               (999999065, 'RE4',                'A81630',               '16C',                 1         ),
               (999999066, 'RE4',                'A81633',               '16C',                 1         ),
               (999999067, 'RE4',                'L84078',               '11M',                 1         ),
               (999999068, 'RE4',                'M81062',               '15E',                 1         ),
               (999999069, 'RE4',                'A82019',               '01H',                 1         ),
               (999999070, 'RE4',                'E83650',               '93C',                 1         ),
               (999999071, 'RE4',                'Y00328',               'M1J4Y',               1         ),
               (999999072, 'RE4',                'J82006',               'D9Y0V',               1         ),
               (999999073, 'RE4',                'A85620',               '13T',                 1         ),
               (999999074, 'RE4',                'P81738',               '01A',                 1         ),
               (999999075, 'RE4',                'Y00589',               '06Q',                 1         ),
               (999999076, 'RE4',                'P91633',               '02A',                 1         ),
               (999999077, 'RE4',                'G82198',               '91Q',                 1         ),
               (999999078, 'RE4',                'G81703',               '97R',                 1         ),
               (999999079, 'RE4',                'C88096',               '03N',                 1         ),
               (999999080, 'RE4',                'C81660',               '15M',                 1         ),
               (999999081, 'RE4',                'C81087',               '15M',                 1         ),
               (999999082, 'RE4',                'C82623',               '04C',                 1         ),
               (999999083, 'RE4',                'L84070',               '11M',                 1         ),
               (999999084, 'RE4',                'P92616',               '02H',                 1         ),
               (999999085, 'RE4',                'M82031',               'M2L0M',               1         ),
               (999999086, 'RE4',                'G82049',               '91Q',                 1         ),
               (999999087, 'RE4',                'K84048',               '10Q',                 1         ),
               (999999088, 'RE4',                'B83017',               '36J',                 1         ),
               (999999089, 'RE4',                'B81664',               '03H',                 1         ),
               (999999090, 'RE4',                'C81090',               '15M',                 1         ),
               (999999091, 'RE4',                'G82671',               '91Q',                 1         ),
               (999999092, 'RE4',                'K82074',               'M1J4Y',               1         ),
               (999999093, 'RE4',                'E86003',               'W2U3Z',               1         ),
               (999999094, 'RE4',                'Y03656',               '00X',                 1         ),
               (999999095, 'RE4',                'J81052',               '11J',                 1         ),
               (999999096, 'RE4',                'A82077',               '01K',                 1         ),
               (999999097, 'RE4',                'L84032',               '11M',                 1         ),
               (999999098, 'RE4',                'P84616',               '14L',                 1         ),
               (999999099, 'RE4',                'G82639',               '91Q',                 1         ),
               (999999100, 'RE4',                'K82018',               '14Y',                 1         ),
               (999999101, 'RE4',                'L82618',               '11N',                 1         ),
               (999999102, 'RE4',                'A85012',               '13T',                 1         ),
               (999999103, 'RE4',                'G81007',               '97R',                 1         ),
               (999999104, 'RE4',                'P83020',               '00V',                 1         ),
               (999999105, 'RE4',                'H83040',               '36L',                 1         ),
               (999999106, 'RE4',                'H81083',               '92A',                 1         ),
               (999999107, 'RE4',                'J83031',               '92G',                 1         ),
               (999999108, 'RE4',                'N85040',               '12F',                 1         ),
               (999999109, 'RE4',                'J82183',               'D9Y0V',               1         ),
               (999999110, 'RE4',                'K82060',               'M1J4Y',               1         ),
               (999999111, 'RE4',                'C83071',               '71E',                 1         ),
               (999999112, 'RE4',                'L84615',               '11M',                 1         ),
               (999999113, 'RE4',                'J82114',               '10R',                 1         ),
               (999999114, 'RE4',                'F81684',               '99G',                 1         ),
               (999999115, 'RE4',                'Y02344',               '03F',                 1         ),
               (999999116, 'RE4',                'B86654',               '15F',                 1         ),
               (999999117, 'RE4',                'E81077',               'M1J4Y',               1         ),
               (999999118, 'RE4',                'C81642',               '15M',                 1         ),
               (999999119, 'RE4',                'C82626',               '04C',                 1         ),
               (999999120, 'RE4',                'Y01165',               '06N',                 1         ),
               (999999121, 'RE4',                'Y06507',               '52R',                 1         ),
               (999999122, 'RE4',                'Y02157',               '00R',                 1         ),
               (999999123, 'RE4',                'A87003',               '99C',                 1         ),
               (999999124, 'RE4',                'B81070',               '02Y',                 1         ),
               (999999125, 'RE4',                'L84616',               '11M',                 1         ),
               (999999126, 'RE4',                'A82058',               '01H',                 1         ),
               (999999127, 'RE4',                'K81089',               'D4U1Y',               1         ),
               (999999128, 'RE4',                'F82666',               'A3A8R',               1         ),
               (999999129, 'RE4',                'L83627',               '15N',                 1         ),
               (999999130, 'RE4',                'C84127',               '52R',                 1         ),
               (999999131, 'RE4',                'C84717',               '52R',                 1         ),
               (999999132, 'RE4',                'J82633',               'D9Y0V',               1         ),
               (999999133, 'RE4',                'L83053',               '15N',                 1         ),
               (999999134, 'RE4',                'B84005',               '02T',                 1         ),
               (999999135, 'RE4',                'F84711',               'A3A8R',               1         ),
               (999999136, 'RE4',                'H81007',               '92A',                 1         ),
               (999999137, 'RE4',                'H85035',               '36L',                 1         ),
               (999999138, 'RE4',                'A85003',               '13T',                 1         ),
               (999999139, 'RE4',                'B81603',               '03H',                 1         ),
               (999999140, 'RE4',                'B86056',               '15F',                 1         ),
               (999999141, 'RE4',                'P92653',               '02H',                 1         ),
               (999999142, 'RE4',                'Y00561',               'M1J4Y',               1         ),
               (999999143, 'RE4',                'C84114',               '52R',                 1         ),
               (999999144, 'RE4',                'M83623',               '05W',                 1         ),
               (999999145, 'RE4',                'L81101',               '92G',                 1         ),
               (999999146, 'RE4',                'G85005',               '72Q',                 1         ),
               (999999147, 'RE4',                'F84041',               'A3A8R',               1         ),
               (999999148, 'RE4',                'Y03671',               '92G',                 1         ),
               (999999149, 'RE4',                'L85028',               '11X',                 1         ),
               (999999150, 'RE4',                'F85052',               '93C',                 1         ),
               (999999151, 'RE4',                'G81667',               '09D',                 1         ),
               (999999152, 'RE4',                'Y02442',               '15M',                 1         ),
               (999999153, 'RE4',                'F84714',               'A3A8R',               1         ),
               (999999154, 'RE4',                'C88656',               '03N',                 1         ),
               (999999155, 'RE4',                'M83706',               '05Q',                 1         ),
               (999999156, 'RE4',                'J82090',               '10R',                 1         ),
               (999999157, 'RE4',                'P85621',               '00Y',                 1         ),
               (999999158, 'RE4',                'Y00372',               '97R',                 1         ),
               (999999159, 'RE4',                'M91020',               'D2P2L',               1         ),
               (999999160, 'RE4',                'B81058',               '03F',                 1         ),
               (999999161, 'RE4',                'L85619',               '11X',                 1         ),
               (999999162, 'RE4',                'L81064',               '92G',                 1         ),
               (999999163, 'RE4',                'K81039',               'D4U1Y',               1         ),
               (999999164, 'RE4',                'B81023',               '03H',                 1         ),
               (999999165, 'RE4',                'G82704',               '91Q',                 1         ),
               (999999166, 'RE4',                'G82605',               '91Q',                 1         ),
               (999999167, 'RE4',                'N83635',               '01X',                 1         ),
               (999999168, 'RE4',                'J81613',               '11J',                 1         ),
               (999999169, 'RE4',                'Y01124',               '00Y',                 1         ),
               (999999170, 'RE4',                'F84685',               'A3A8R',               1         ),
               (999999171, 'RE4',                'E84713',               'W2U3Z',               1         ),
               (999999172, 'RE4',                'K81059',               'D4U1Y',               1         ),
               (999999173, 'RE4',                'F81218',               '07G',                 1         ),
               (999999174, 'RE4',                'M83082',               '05W',                 1         ),
               (999999175, 'RE4',                'E82072',               '06H',                 1         ),
               (999999176, 'RE4',                'E83657',               '93C',                 1         ),
               (999999177, 'RE4',                'N84618',               '01V',                 1         ),
               (999999178, 'RE4',                'M91011',               'D2P2L',               1         ),
               (999999179, 'RE4',                'D83014',               '07K',                 1         ),
               (999999180, 'RE4',                'B81055',               '03H',                 1         ),
               (999999181, 'RE4',                'E85740',               'W2U3Z',               1         ),
               (999999182, 'RE4',                'A81043',               '16C',                 1         ),
               (999999183, 'RE4',                'E82130',               '06K',                 1         ),
               (999999184, 'RE4',                'L81094',               '15C',                 1         ),
               (999999185, 'RE4',                'G82225',               '91Q',                 1         ),
               (999999186, 'RE4',                'J81644',               '11J',                 1         ),
               (999999187, 'RE4',                'P81737',               '02M',                 1         ),
               (999999188, 'RE4',                'E85714',               'W2U3Z',               1         ),
               (999999189, 'RE4',                'M85008',               '15E',                 1         ),
               (999999190, 'RE4',                'J81637',               '11J',                 1         ),
               (999999191, 'RE4',                'F83025',               '93C',                 1         ),
               (999999192, 'RE4',                'E87694',               'W2U3Z',               1         ),
               (999999193, 'RE4',                'K81006',               'D4U1Y',               1         ),
               (999999194, 'RE4',                'E82104',               '06K',                 1         ),
               (999999195, 'RE4',                'C83631',               '71E',                 1         ),
               (999999196, 'RE4',                'J82041',               'D9Y0V',               1         ),
               (999999197, 'RE4',                'F84676',               'A3A8R',               1         ),
               (999999198, 'RE4',                'F84742',               'A3A8R',               1         ),
               (999999199, 'RE4',                'Y02222',               '72Q',                 1         ),
               (999999200, 'RE4',                'Y00522',               'M1J4Y',               1         ),
               (999999201, 'RE4',                'Y02656',               '02Y',                 1         ),
               (999999202, 'RE4',                'E82048',               '06N',                 1         ),
               (999999203, 'RE4',                'B83030',               '36J',                 1         ),
               (999999204, 'RE4',                'N85014',               '12F',                 1         ),
               (999999205, 'RE4',                'H81060',               '92A',                 1         ),
               (999999206, 'RE4',                'N83055',               '01J',                 1         ),
               (999999207, 'RE4',                'H85005',               '36L',                 1         ),
               (999999208, 'RE4',                'M83705',               '05Q',                 1         ),
               (999999209, 'RE4',                'Y04942',               '71E',                 1         ),
               (999999210, 'RE4',                'M85782',               '15E',                 1         ),
               (999999211, 'RE4',                'G82762',               '91Q',                 1         ),
               (999999212, 'RE4',                'H84629',               '36L',                 1         ),
               (999999213, 'RE4',                'P87618',               '01G',                 1         ),
               (999999214, 'RE4',                'H85665',               '36L',                 1         ),
               (999999215, 'RE4',                'M92044',               'D2P2L',               1         ),
               (999999216, 'RE4',                'A88007',               '00N',                 1         ),
               (999999217, 'RE4',                'E85692',               'W2U3Z',               1         ),
               (999999218, 'RE4',                'M88044',               'D2P2L',               1         ),
               (999999219, 'RE4',                'J82008',               'D9Y0V',               1         ),
               (999999220, 'RE4',                'M92042',               'D2P2L',               1         ),
               (999999221, 'RE4',                'Y00110',               '99A',                 1         ),
               (999999222, 'RE4',                'E85630',               'W2U3Z',               1         ),
               (999999223, 'RE4',                'M81003',               '18C',                 1         ),
               (999999224, 'RE4',                'B86625',               '15F',                 1         ),
               (999999225, 'RE4',                'K81622',               '15A',                 1         ),
               (999999226, 'RE4',                'C82029',               '04C',                 1         ),
               (999999227, 'RE4',                'J81003',               '11J',                 1         ),
               (999999228, 'RE4',                'B81077',               '03H',                 1         ),
               (999999229, 'RE4',                'Y00542',               '72Q',                 1         ),
               (999999230, 'RE4',                '00L',                  '00L',                 1         ),
               (999999231, 'RE4',                '00T',                  '00T',                 1         ),
               (999999232, 'RE4',                '00V',                  '00V',                 1         ),
               (999999233, 'RE4',                '01G',                  '01G',                 1         ),
               (999999234, 'RE4',                '01W',                  '01W',                 1         ),
               (999999235, 'RE4',                '01Y',                  '01Y',                 1         ),
               (999999236, 'RE4',                '02E',                  '02E',                 1         ),
               (999999237, 'RE4',                '02P',                  '02P',                 1         ),
               (999999238, 'RE4',                '02T',                  '02T',                 1         ),
               (999999239, 'RE4',                '02X',                  '02X',                 1         ),
               (999999240, 'RE4',                '02Y',                  '02Y',                 1         ),
               (999999241, 'RE4',                '03N',                  '03N',                 1         ),
               (999999242, 'RE4',                '03Q',                  '03Q',                 1         ),
               (999999243, 'RE4',                '03R',                  '03R',                 1         ),
               (999999244, 'RE4',                '03W',                  '03W',                 1         ),
               (999999245, 'RE4',                '04Y',                  '04Y',                 1         ),
               (999999246, 'RE4',                '05D',                  '05D',                 1         ),
               (999999247, 'RE4',                '05G',                  '05G',                 1         ),
               (999999248, 'RE4',                '05Q',                  '05Q',                 1         ),
               (999999249, 'RE4',                '05V',                  '05V',                 1         ),
               (999999250, 'RE4',                '05W',                  '05W',                 1         ),
               (999999251, 'RE4',                '06L',                  '06L',                 1         ),
               (999999252, 'RE4',                '06N',                  '06N',                 1         ),
               (999999253, 'RE4',                '07G',                  '07G',                 1         ),
               (999999254, 'RE4',                '09D',                  '09D',                 1         ),
               (999999255, 'RE4',                '0CX',                  '0CX',                 1         ),
               (999999256, 'RE4',                '11M',                  '11M',                 1         ),
               (999999257, 'RE4',                '11N',                  '11N',                 1         ),
               (999999258, 'RE4',                '12F',                  '12F',                 1         ),
               (999999259, 'RE4',                '13T',                  '13T',                 1         ),
               (999999260, 'RE4',                '15E',                  '15E',                 1         ),
               (999999261, 'RE4',                '15F',                  '15F',                 1         ),
               (999999262, 'RE4',                '15M',                  '15M',                 1         ),
               (999999263, 'RE4',                '15N',                  '15N',                 1         ),
               (999999264, 'RE4',                '16C',                  '16C',                 1         ),
               (999999265, 'RE4',                '26A',                  '26A',                 1         ),
               (999999266, 'RE4',                '36L',                  '36L',                 1         ),
               (999999267, 'RE4',                '42D',                  '42D',                 1         ),
               (999999268, 'RE4',                '52R',                  '52R',                 1         ),
               (999999269, 'RE4',                '70F',                  '70F',                 1         ),
               (999999270, 'RE4',                '71E',                  '71E',                 1         ),
               (999999271, 'RE4',                '72Q',                  '72Q',                 1         ),
               (999999272, 'RE4',                '78H',                  '78H',                 1         ),
               (999999273, 'RE4',                '84H',                  '84H',                 1         ),
               (999999274, 'RE4',                '92G',                  '92G',                 1         ),
               (999999275, 'RE4',                '93C',                  '93C',                 1         ),
               (999999276, 'RE4',                '97R',                  '97R',                 1         ),
               (999999277, 'RE4',                '99C',                  '99C',                 1         ),
               (999999278, 'RE4',                'A3A8R',                'A3A8R',               1         ),
               (999999279, 'RE4',                'B2M3M',                'B2M3M',               1         ),
               (999999280, 'RE4',                'D9Y0V',                'D9Y0V',               1         ),
               (999999281, 'RE4',                'M1J4Y',                'M1J4Y',               1         )

    MERGE INTO [ods_organisations].[OrganisationRelationships] AS TARGET
    USING @odsOrganisationRelationships AS SOURCE
    ON TARGET.[Id] = SOURCE.[Id]
    WHEN MATCHED THEN
        UPDATE
        SET TARGET.[RelationshipTypeId]   = SOURCE.[RelationshipTypeId],
            TARGET.[TargetOrganisationId] = SOURCE.[TargetOrganisationId],
            TARGET.[OwnerOrganisationId]  = SOURCE.[OwnerOrganisationId],
            TARGET.[IsActive]             = SOURCE.[IsActive]
    WHEN NOT MATCHED BY TARGET THEN
        INSERT ([Id], [RelationshipTypeId], [TargetOrganisationId], [OwnerOrganisationId], [IsActive])
            VALUES (SOURCE.[Id], SOURCE.[RelationshipTypeId], SOURCE.[TargetOrganisationId], SOURCE.[OwnerOrganisationId],
                    SOURCE.[IsActive]);

    -- Migrate sublocations, sublocation recipients, and order items
    DECLARE @OrderSublocationIsCommissionedBy VARCHAR(3)
    DECLARE @OrderSublocationIsLocatedInTheGeographyOf VARCHAR(3)
    DECLARE @IcbSublocationRole VARCHAR(10)
    SET @OrderSublocationIsCommissionedBy = 'RE4'
    SET @OrderSublocationIsLocatedInTheGeographyOf = 'RE5'
    SET @IcbSublocationRole = 'RO319'

    -- Migrate sublocations based on relationship
    INSERT INTO [ordering].[OrderSublocations]
        ([OrderId], [SublocationOdsCode], [OwnerOdsCode])
    SELECT DISTINCT [or].[OrderId], [rel].[OwnerOrganisationId] AS [SublocationOdsCode],
                    [rel2].[OwnerOrganisationId] AS [OwnerOdsCode]
        FROM [ordering].[OrderRecipients] [or]
                 JOIN [ods_organisations].[OrganisationRelationships] [rel]
                      ON [or].[OdsCode] = [rel].[TargetOrganisationId]
                 JOIN [ods_organisations].[OrganisationRelationships] [rel2]
                      ON [rel].[OwnerOrganisationId] = [rel2].[TargetOrganisationId]
        WHERE [rel].[RelationshipTypeId] = @OrderSublocationIsCommissionedBy
          AND [rel2].[RelationshipTypeId] = @OrderSublocationIsLocatedInTheGeographyOf
          AND [rel].[IsActive] = 1
          AND [rel2].[IsActive] = 1
          AND NOT EXISTS (
            SELECT 1
                FROM [ordering].[OrderSublocations] [os]
                WHERE [os].[OrderId] = [or].[OrderId]
                  AND [os].[SublocationOdsCode] = [rel].[OwnerOrganisationId]
                  AND [os].[OwnerOdsCode] = [rel2].[OwnerOrganisationId]);

    -- Migrate sublocations based on role (sublocation as recipient outliers)
    INSERT INTO [ordering].[OrderSublocations]
        ([OrderId], [SublocationOdsCode], [OwnerOdsCode])
    SELECT DISTINCT [or].[OrderId], [or].[OdsCode], [rel2].[OwnerOrganisationId]
        FROM [ordering].[OrderRecipients] [or]
                 JOIN [ods_organisations].[OrganisationRoles] [role]
                      ON [or].[OdsCode] = [role].[OrganisationId]
                 JOIN [ods_organisations].[OrganisationRelationships] [rel2]
                      ON [or].[OdsCode] = [rel2].[TargetOrganisationId]
        WHERE [role].[RoleId] = @IcbSublocationRole
          AND [rel2].[RelationshipTypeId] = @OrderSublocationIsLocatedInTheGeographyOf
          AND [role].[IsActive] = 1
          AND [rel2].[IsActive] = 1
          AND NOT EXISTS (
            SELECT 1
                FROM [ordering].[OrderSublocations] [os]
                WHERE [os].[OrderId] = [or].[OrderId]
                  AND [os].[SublocationOdsCode] = [or].[OdsCode]
                  AND [os].[OwnerOdsCode] = [rel2].[OwnerOrganisationId]);

    -- Migrate recipients based on sublocation parent relationship
    INSERT INTO [ordering].[OrderSublocationRecipients]
        ([OrderId], [ParentSublocationOdsCode], [RecipientOdsCode])
    SELECT [or].[OrderId], [rel].[OwnerOrganisationId] AS [ParentSublocationOdsCode], [or].[OdsCode] AS [RecipientOdsCode]
        FROM [ordering].[OrderRecipients] [or]
                 JOIN [ods_organisations].[OrganisationRelationships] [rel]
                      ON [or].[OdsCode] = [rel].[TargetOrganisationId]
        WHERE [rel].[RelationshipTypeId] = @OrderSublocationIsCommissionedBy
          AND [rel].[IsActive] = 1
          AND NOT EXISTS (
            SELECT 1
                FROM [ordering].[OrderSublocationRecipients] [osr]
                WHERE [osr].[OrderId] = [or].[OrderId]
                  AND [osr].[ParentSublocationOdsCode] = [rel].[OwnerOrganisationId]
                  AND [osr].[RecipientOdsCode] = [or].[OdsCode]);

    INSERT INTO [ordering].[OrderItemSublocationRecipients]
    ([OrderId], [CatalogueItemId], [ParentSublocationOdsCode], [RecipientOdsCode], [Quantity], [DeliveryDate],
     [LastUpdated], [LastUpdatedBy])
    SELECT [oir].[OrderId], [oir].[CatalogueItemId], [osr].[ParentSublocationOdsCode], [osr].[RecipientOdsCode],
           [oir].[Quantity], [oir].[DeliveryDate], [oir].[LastUpdated], [oir].[LastUpdatedBy]
        FROM [ordering].[OrderItemRecipients] [oir]
                 JOIN [ordering].[OrderSublocationRecipients] [osr]
                      ON [oir].[OrderId] = [osr].[OrderId] AND [oir].[OdsCode] = [osr].[RecipientOdsCode]
        WHERE NOT EXISTS (
            SELECT 1
                FROM [ordering].[OrderItemSublocationRecipients] [oisr]
                WHERE [oisr].[OrderId] = [oir].[OrderId]
                  AND [oisr].[CatalogueItemId] = [oir].[CatalogueItemId]
                  AND [oisr].[ParentSublocationOdsCode] = [osr].[ParentSublocationOdsCode]
                  AND [oisr].[RecipientOdsCode] = [osr].[RecipientOdsCode]);

    -- Remove temp relationships
    DELETE
        FROM [ods_organisations].[OrganisationRelationships]
        WHERE [Id] IN (
            SELECT [Id]
                FROM @odsOrganisationRelationships);

    COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK;
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT @ErrorMessage = ERROR_MESSAGE(),
               @ErrorSeverity = ERROR_SEVERITY(),
               @ErrorState = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END;
