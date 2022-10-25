IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
    BEGIN
        DECLARE @odsOrganisationRelationships AS TABLE
                                                 (
                                                     [Id]               INT             NOT NULL PRIMARY KEY,
                                                     [RelTypeId]                 NVARCHAR(10)    NOT NULL,
                                                     [TargetOrganisationId]      INT             NOT NULL,
                                                     [OwnerOrganisationId]       INT             NOT NULL
                                                 );

        INSERT INTO @odsOrganisationRelationships ([Id], [RelTypeId], [TargetOrganisationId], [OwnerOrganisationId])
        VALUES
            (671770, 'RE5', 1, 0),
            (673037, 'RE5', 2, 0),
            (673106, 'RE5', 3, 0),
            (668185, 'RE5', 4, 0),
            (667123, 'RE5', 5, 0),
            (667129, 'RE5', 6, 0),
            (667130, 'RE5', 7, 0),
            (671458, 'RE5', 8, 0),
            (671483, 'RE5', 9, 0),
            (667172, 'RE5', 10, 0);

        MERGE INTO [ods_organisations].[OrganisationRelationships] AS TARGET
        USING @odsOrganisationRelationships AS SOURCE
        ON TARGET.[Id] = SOURCE.[Id]
        WHEN MATCHED THEN
            UPDATE SET TARGET.[Id] = SOURCE.[Id],
                       TARGET.[RelTypeId] = SOURCE.[RelTypeId],
                       TARGET.[TargetOrganisationId] = SOURCE.[TargetOrganisationId],
                       TARGET.[OwnerOrganisationId] = SOURCE.[OwnerOrganisationId]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ([Id], [RelTypeId], [TargetOrganisationId], [OwnerOrganisationId])
            VALUES (SOURCE.[Id], SOURCE.[RelTypeId], SOURCE.[TargetOrganisationId], SOURCE.[OwnerOrganisationId]);
    END;
GO
