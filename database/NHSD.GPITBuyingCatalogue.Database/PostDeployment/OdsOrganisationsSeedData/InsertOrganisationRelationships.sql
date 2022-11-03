﻿IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
    BEGIN
        DECLARE @odsOrganisationRelationships AS TABLE
                                                 (
                                                     [Id]                           INT             NOT NULL PRIMARY KEY,
                                                     [RelationshipTypeId]           NVARCHAR(10)    NOT NULL,
                                                     [TargetOrganisationId]         NVARCHAR(8)     NOT NULL,
                                                     [OwnerOrganisationId]          NVARCHAR(8)     NOT NULL
                                                 );

        INSERT INTO @odsOrganisationRelationships ([Id], [RelationshipTypeId], [TargetOrganisationId], [OwnerOrganisationId])
        VALUES
            (671770, 'RE5', '8JW88', 'QWO'),
            (673037, 'RE5', '8D133', 'QWO'),
            (673106, 'RE5', '8D696', 'QWO'),
            (668185, 'RE5', '8D969', 'QWO'),
            (667123, 'RE5', '8AR17', 'QWO'),
            (667129, 'RE5', '8AR26', 'QWO'),
            (667130, 'RE5', '8AR27', 'QWO'),
            (671458, 'RE5', '8E138', 'QWO'),
            (671483, 'RE5', '8E241', 'QWO'),
            (667172, 'RE5', '8AR79', 'QWO');

        MERGE INTO [ods_organisations].[OrganisationRelationships] AS TARGET
        USING @odsOrganisationRelationships AS SOURCE
        ON TARGET.[Id] = SOURCE.[Id]
        WHEN MATCHED THEN
            UPDATE SET TARGET.[Id] = SOURCE.[Id],
                       TARGET.[RelationshipTypeId] = SOURCE.[RelationshipTypeId],
                       TARGET.[TargetOrganisationId] = SOURCE.[TargetOrganisationId],
                       TARGET.[OwnerOrganisationId] = SOURCE.[OwnerOrganisationId]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ([Id], [RelationshipTypeId], [TargetOrganisationId], [OwnerOrganisationId])
            VALUES (SOURCE.[Id], SOURCE.[RelationshipTypeId], SOURCE.[TargetOrganisationId], SOURCE.[OwnerOrganisationId]);
    END;
GO
