IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
BEGIN

    DECLARE @capabilityCategories AS TABLE
    (
        Id int NOT NULL,
        [Name] nvarchar(50) NOT NULL
    );
    
    INSERT INTO @capabilityCategories(Id, [Name])
    VALUES
    (0, 'Undefined'),
    (1, 'Appointments'),
    (2, 'Care coordination'),
    (3, 'Citizen services'),
    (4, 'Community-based care'),
    (5, 'Consultations'),
    (6, 'Document and artefact management'),
    (7, 'Health or care organisation management'),
    (8, 'Medicines management'),
    (9, 'Patient care management'),
    (10, 'Reporting and data analytics'),
    (11, 'Supplier defined Capabilities');
    
    SET IDENTITY_INSERT catalogue.CapabilityCategories ON;
    
    MERGE INTO catalogue.CapabilityCategories AS TARGET
         USING @capabilityCategories AS SOURCE
            ON TARGET.Id = SOURCE.Id
          WHEN MATCHED THEN
            UPDATE SET
            TARGET.[Name] = SOURCE.[Name]
          WHEN NOT MATCHED BY TARGET THEN
            INSERT (Id, [Name])
            VALUES (SOURCE.Id, SOURCE.[Name]);
    
    SET IDENTITY_INSERT catalogue.CapabilityCategories OFF;
    
END
