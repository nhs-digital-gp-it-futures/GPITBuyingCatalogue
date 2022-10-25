IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
    BEGIN
        DECLARE @odsOrganisationRoles AS TABLE
                                         (
                                             [Id]      INT             NOT NULL,
                                             [OrganisationId]    INT             NOT NULL,
                                             [RoleId]            NVARCHAR(10)    NOT NULL,
                                             [IsPrimaryRole]     BIT             NOT NULL
                                         );

        INSERT INTO @odsOrganisationRoles ([Id], [OrganisationId], [RoleId], [IsPrimaryRole])
        VALUES
            (300736, 0, 'RO261', 1);


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
