IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
    BEGIN
        DECLARE @odsOrganisations AS TABLE
                                     (
                                         [OrganisationId]        INT NOT NULL PRIMARY KEY,
                                         [Name]                  NVARCHAR(255) NOT NULL,
                                         [AddressLine1]          NVARCHAR(255) NULL,
                                         [Postcode]              NVARCHAR(10)  NULL,
                                         [Country]               NVARCHAR(100) NULL,
                                         [Status] NVARCHAR(100) NOT NULL
                                     );

        INSERT INTO @odsOrganisations ([OrganisationId], [Name], [AddressLine1], [Postcode], [Country], [Status])
        VALUES
            (0, 'NHS WEST YORKSHIRE INTEGRATED CARE BOARD', 'WHITE ROSE HOUSE', 'WF1 1LT', 'England', 'Active'),
            (1, 'TWENTY FOUR SEVEN NURSING', '2 THE GROVE PROMENADE', 'LS29 8AF', 'England', 'Active'),
            (2, 'SHEILDS M D', '2 WENTWORTH STREET', 'WF1 2RX', 'England', 'Active'),
            (3, 'WALLS W D', '98 OUZLEWELL GREEN', 'WF3 3QW', 'England', 'Active'),
            (4, 'DUFFIELD', '24 LOVE LANE', 'WF8 4AN', 'England', 'Active'),
            (5, 'CZAJKA CENTRE', 'COLNE ROAD', 'BD20 8PL', 'England', 'Active'),
            (6, 'ESHTON HALL NH', 'ESHTON HALL', 'BD23 3QQ', 'England', 'Active'),
            (7, 'CARGRAVE PARK NH', 'WEST STREET', 'BD23 3PH', 'England', 'Active'),
            (8, 'AGGARWAL J L', '7 THE GHYLL', 'HD2 2FE', 'England', 'Active'),
            (9, 'GARDNER L G (DR)', '7 ELMHURST CLOSE', 'LS17 8BD', 'England', 'Active'),
            (10, 'HILLBRO NH', 'HOLDEN LANE', 'BD17 6RZ', 'England', 'Active');

        MERGE INTO [ods_organisations].[OdsOrganisations] AS TARGET
        USING @odsOrganisations AS SOURCE
        ON TARGET.[Name] = SOURCE.[Name]
        WHEN MATCHED THEN
            UPDATE SET TARGET.[Name] = SOURCE.[Name],
                       TARGET.[AddressLine1] = SOURCE.[AddressLine1],
                       TARGET.[Postcode] = SOURCE.[Postcode],
                       TARGET.[Country] = SOURCE.[Country],
                       TARGET.[Status] = SOURCE.[Status]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ([OrganisationId], [Name], [AddressLine1], [Postcode], [Country], [Status])
            VALUES (SOURCE.[OrganisationId], SOURCE.[Name], SOURCE.[AddressLine1], SOURCE.[Postcode], SOURCE.[Country], SOURCE.[Status]);
    END;
GO
