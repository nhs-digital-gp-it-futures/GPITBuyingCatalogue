IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
    BEGIN
        DECLARE @odsRoleTypes AS TABLE
                                 (
                                     [RoleId]        NVARCHAR(10)    NOT NULL PRIMARY KEY,
                                     [Description]   NVARCHAR(100)   NOT NULL
                                 );

        INSERT INTO @odsRoleTypes ([RoleId], [Description])
        VALUES
            ('RO261', 'STRATEGIC PARTNERSHIP');

        MERGE INTO [ods_organisations].[RoleTypes] AS TARGET
        USING @odsRoleTypes AS SOURCE
        ON TARGET.[RoleId] = SOURCE.[RoleId]
        WHEN MATCHED THEN
            UPDATE SET TARGET.[RoleId] = SOURCE.[RoleId],
                       TARGET.[Description] = SOURCE.[Description]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ([RoleId], [Description])
            VALUES (SOURCE.[RoleId], SOURCE.[Description]);
    END;
GO
