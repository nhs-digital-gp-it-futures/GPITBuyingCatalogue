DECLARE @capabilityCategories AS TABLE
(
    Id int NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(200) NULL
);

INSERT INTO @capabilityCategories(Id, [Name], [Description])
VALUES
(0, 'Undefined', NULL),
(1, 'Appointments', 'Find solutions that enable staff to efficiently manage time and resources, ensuring all patients have timely access to appropriate medical services.'),
(2, 'Care coordination', 'Find solutions that support patient communication, efficient handling of test results and referrals and help identify anyone needing COVID-19 vaccinations.'),
(3, 'Citizen services', 'Find solutions that enable service users to be more involved in their healthcare, including accessing health records, monitoring conditions and managing their appointments.'),
(4, 'Community-based care', 'Find solutions that help provide healthcare services to care home residents and anyone who needs additional support living in their own home.'),
(5, 'Consultations', 'Find solutions that support the delivery of remote consultations.'),
(6, 'Document and artefact management', 'Find solutions that support the management of electronic documents.'),
(7, 'Health or care organisation management', 'Find solutions that support the efficient running of a health or care organisation.'),
(8, 'Medicines management', 'Find solutions that allow the safe prescribing and dispensing of medicines.'),
(9, 'Patient care management', 'Find solutions that support the maintenance and sharing of patient information.'),
(10, 'Reporting and data analytics', 'Find solutions that allow local data analysis across all aspects of a GP practice.'),
(11, 'Supplier defined Capabilities', 'Find solutions that have met Capabilities defined by a supplier.');

SET IDENTITY_INSERT catalogue.CapabilityCategories ON;

MERGE INTO catalogue.CapabilityCategories AS TARGET
     USING @capabilityCategories AS SOURCE
        ON TARGET.Id = SOURCE.Id
      WHEN MATCHED THEN
UPDATE SET
        TARGET.[Name] = SOURCE.[Name],
        TARGET.[Description] = SOURCE.[Description]
      WHEN NOT MATCHED BY TARGET THEN
      INSERT (Id, [Name], [Description])
      VALUES (SOURCE.Id, SOURCE.[Name], SOURCE.[Description]);

SET IDENTITY_INSERT catalogue.CapabilityCategories OFF;

GO
