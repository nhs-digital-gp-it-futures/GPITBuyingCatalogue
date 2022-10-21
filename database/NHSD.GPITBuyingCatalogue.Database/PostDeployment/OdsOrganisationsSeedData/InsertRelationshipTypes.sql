IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
    BEGIN
        DECLARE @odsRelationshipTypes AS TABLE
                                         (
                                             [RelTypeId]        NVARCHAR(10)    NOT NULL PRIMARY KEY,
                                             [Description]      NVARCHAR(100)   NOT NULL
                                         );

        INSERT INTO @odsRelationshipTypes ([RelTypeId], [Description])
        VALUES
            ('RE6','IS OPERATED BY'),
            ('RE5','IS LOCATED IN THE GEOGRAPHY OF'),
            ('RE4','IS COMMISSIONED BY'),
            ('RE3','IS DIRECTED BY'),
            ('RE8','IS PARTNER TO'),
            ('RE9','IS NOMINATED PAYEE FOR'),
            ('RE10','IS COVID NOMINATED PAYEE FOR'),
            ('RE2','IS A SUB-DIVISION OF');

        MERGE INTO [ods_organisations].[RelationshipTypes] AS TARGET
        USING @odsRelationshipTypes AS SOURCE
        ON TARGET.[RelTypeId] = SOURCE.[RelTypeId]
        WHEN MATCHED THEN
            UPDATE SET TARGET.[RelTypeId] = SOURCE.[RelTypeId],
                       TARGET.[Description] = SOURCE.[Description]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ([RelTypeId], [Description])
            VALUES (SOURCE.[RelTypeId], SOURCE.[Description]);
    END;
GO
