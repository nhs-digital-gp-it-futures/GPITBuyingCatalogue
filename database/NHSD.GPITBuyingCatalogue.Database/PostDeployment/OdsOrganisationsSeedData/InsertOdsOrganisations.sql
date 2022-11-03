﻿IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
    BEGIN
        DECLARE @odsOrganisations AS TABLE
                                     (
                                         [Id]                       NVARCHAR(8)   NOT NULL,
                                         [Name]                     NVARCHAR(255) NOT NULL,
                                         [AddressLine1]             NVARCHAR(255) NULL,
                                         [Postcode]                 NVARCHAR(10)  NULL,
                                         [Country]                  NVARCHAR(100) NULL,
                                         [IsActive]                 BIT NOT NULL
                                     );

        INSERT INTO @odsOrganisations ([Id], [Name], [AddressLine1], [Postcode], [Country], [IsActive])
        VALUES
            ('QWO', 'NHS WEST YORKSHIRE INTEGRATED CARE BOARD', 'WHITE ROSE HOUSE', 'WF1 1LT', 'England', 1),
            ('8JW88', 'TWENTY FOUR SEVEN NURSING', '2 THE GROVE PROMENADE', 'LS29 8AF', 'England', 1),
            ('8D133', 'SHEILDS M D', '2 WENTWORTH STREET', 'WF1 2RX', 'England', 1),
            ('8D696', 'WALLS W D', '98 OUZLEWELL GREEN', 'WF3 3QW', 'England', 1),
            ('8D969', 'DUFFIELD R', '24 LOVE LANE', 'WF8 4AN', 'England', 1),
            ('8AR17', 'CZAJKA CENTRE', 'COLNE ROAD', 'BD20 8PL', 'England', 1),
            ('8AR26', 'ESHTON HALL NH', 'ESHTON HALL', 'BD23 3QQ', 'England', 1),
            ('8AR27', 'CARGRAVE PARK NH', 'WEST STREET', 'BD23 3PH', 'England', 1),
            ('8E138', 'AGGARWAL J L', '7 THE GHYLL', 'HD2 2FE', 'England', 1),
            ('8E241', 'GARDNER L G (DR)', '7 ELMHURST CLOSE', 'LS17 8BD', 'England', 1),
            ('8AR79', 'HILLBRO NH', 'HOLDEN LANE', 'BD17 6RZ', 'England', 1);

        MERGE INTO [ods_organisations].[OdsOrganisations] AS TARGET
        USING @odsOrganisations AS SOURCE
        ON TARGET.[Name] = SOURCE.[Name]
        WHEN MATCHED THEN
            UPDATE SET TARGET.[Name] = SOURCE.[Name],
                       TARGET.[AddressLine1] = SOURCE.[AddressLine1],
                       TARGET.[Postcode] = SOURCE.[Postcode],
                       TARGET.[Country] = SOURCE.[Country],
                       TARGET.[IsActive] = SOURCE.[IsActive]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ([Id], [Name], [AddressLine1], [Postcode], [Country], [IsActive])
            VALUES (SOURCE.[Id], SOURCE.[Name], SOURCE.[AddressLine1], SOURCE.[Postcode], SOURCE.[Country], SOURCE.[IsActive]);
    END;
GO