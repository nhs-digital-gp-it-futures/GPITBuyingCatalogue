IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
    BEGIN
        DECLARE @odsRoleTypes AS TABLE
                                 (
                                     [Id]            NVARCHAR(10)    NOT NULL PRIMARY KEY,
                                     [Description]   NVARCHAR(100)   NOT NULL
                                 );

        INSERT INTO @odsRoleTypes ([Id], [Description])
        VALUES
            ('RO177', 'PRESCRIBING COST CENTRE'),
            ('RO261', 'STRATEGIC PARTNERSHIP'),
            ('RO262', 'SUSTAINABILITY TRANSFORMATION PARTNERSHIP'),
            ('RO318', 'INTEGRATED CARE BOARD'),
            ('RO319', 'SUB ICB LOCATION');

        MERGE INTO [ods_organisations].[RoleTypes] AS TARGET
        USING @odsRoleTypes AS SOURCE
        ON TARGET.[Id] = SOURCE.[Id]
        WHEN MATCHED THEN
            UPDATE SET TARGET.[Id] = SOURCE.[Id],
                       TARGET.[Description] = SOURCE.[Description]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ([Id], [Description])
            VALUES (SOURCE.[Id], SOURCE.[Description]);
    END;
GO
