IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
    BEGIN
        DECLARE @odsOrganisationRoles AS TABLE
                                         (
                                             [Id]      INT             NOT NULL,
                                             [OrganisationId]    NVARCHAR(8)     NOT NULL,
                                             [RoleId]            NVARCHAR(10)    NOT NULL,
                                             [IsPrimaryRole]     BIT             NOT NULL
                                         );

        INSERT INTO @odsOrganisationRoles ([Id], [OrganisationId], [RoleId], [IsPrimaryRole])
        VALUES
            (300736, 'QWO', 'RO261', 1),
            (300737, '8JW88', 'RO157', 1),
            (300738, '8D133', 'RO157', 1),
            (300739, '8D696', 'RO157', 1),
            (300740, '8D969', 'RO157', 1),
            (300741, '8AR17', 'RO157', 1),
            (300742, '8AR26', 'RO157', 1),
            (300743, '8AR27', 'RO157', 1),
            (300744, '8E138', 'RO157', 1),
            (300745, '8E241', 'RO157', 1),
            (300746, '8AR79', 'RO157', 1),
            (300747, '36J', 'RO98', 1),
            (94087, 'B82007', 'RO177', 1),
            (94088, 'B82053', 'RO177', 1),
            (94089, 'B83002', 'RO177', 1),
            (94090, '03F', 'RO98', 1),
            (94091, 'QOQ', 'RO261', 1),
            (94092, 'B81046', 'RO177', 1),
            (94093, 'B81047', 'RO177', 1),
            (94094, 'B81048', 'RO177', 1);


        MERGE INTO [ods_organisations].[OrganisationRoles] AS TARGET
        USING @odsOrganisationRoles AS SOURCE
        ON TARGET.[Id] = SOURCE.[Id]
        WHEN MATCHED THEN
            UPDATE SET TARGET.[Id] = SOURCE.[Id],
                       TARGET.[OrganisationId] = SOURCE.[OrganisationId],
                       TARGET.[RoleId] = SOURCE.[RoleId],
                       TARGET.[IsPrimaryRole] = SOURCE.[IsPrimaryRole]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ([Id], [OrganisationId], [RoleId], [IsPrimaryRole])
            VALUES (SOURCE.[Id], SOURCE.[OrganisationId], SOURCE.[RoleId], SOURCE.[IsPrimaryRole]);
    END;
GO
