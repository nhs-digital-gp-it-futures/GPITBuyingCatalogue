IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
    BEGIN
        DECLARE @odsRelationshipTypes AS TABLE
                                         (
                                             [Id]               NVARCHAR(10)    NOT NULL PRIMARY KEY,
                                             [Description]      NVARCHAR(100)   NOT NULL
                                         );

        INSERT INTO @odsRelationshipTypes ([Id], [Description])
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
        ON TARGET.[Id] = SOURCE.[Id]
        WHEN MATCHED THEN
            UPDATE SET TARGET.[Id] = SOURCE.[Id],
                       TARGET.[Description] = SOURCE.[Description]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ([Id], [Description])
            VALUES (SOURCE.[Id], SOURCE.[Description]);
    END;
GO
