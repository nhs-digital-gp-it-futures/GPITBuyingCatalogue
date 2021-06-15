----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      IMPORTANT - please do not manually edit this file.  It is generated automatically on build.  Any changes made will be lost.
----------------------------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertCapabilityStatuses.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM dbo.CapabilityStatus)
    INSERT INTO dbo.CapabilityStatus(Id, [Name])
    VALUES (1, 'Effective');
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertCapabilityCategories.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @capabilityCategories AS TABLE
(
    Id int NOT NULL,
    [Name] nvarchar(50) NOT NULL
);

INSERT INTO @capabilityCategories(Id, [Name])
VALUES
(0, 'Undefined'),
(1, 'GP IT Futures'),
(2, 'Covid-19 Vaccination'),
(3, 'DFOCVC');

MERGE INTO dbo.CapabilityCategory AS TARGET
     USING @capabilityCategories AS SOURCE
        ON TARGET.Id = SOURCE.Id
      WHEN MATCHED THEN
UPDATE SET TARGET.[Name] = SOURCE.[Name]
      WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, [Name])
    VALUES (SOURCE.Id, SOURCE.[Name]);
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertCompliancyLevels.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM dbo.CompliancyLevel)
    INSERT INTO dbo.CompliancyLevel(Id, [Name])
    VALUES
    (1, 'MUST'),
    (2, 'SHOULD'),
    (3, 'MAY');
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertPublicationStatuses.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM dbo.PublicationStatus)
    INSERT INTO dbo.PublicationStatus(Id, [Name])
    VALUES
    (1, 'Draft'),
    (2, 'Unpublished'),
    (3, 'Published'),
    (4, 'Withdrawn');
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertSolutionCapabilityStatuses.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM dbo.SolutionCapabilityStatus)
    INSERT INTO dbo.SolutionCapabilityStatus(Id, [Name], Pass)
    VALUES
    (1, 'Passed – Full', 1),
    (2, 'Passed – Partial', 1),
    (3, 'Failed', 0);
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertSolutionEpicStatuses.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM dbo.SolutionEpicStatus)
    INSERT INTO dbo.SolutionEpicStatus(Id, [Name], IsMet)
    VALUES
    (1, 'Passed', 1),
    (2, 'Not Evidenced', 0),
    (3, 'Failed', 0);
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertCatalogueItemTypes.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM dbo.CatalogueItemType)
    INSERT INTO dbo.CatalogueItemType(CatalogueItemTypeId, [Name])
    VALUES
    (1, 'Solution'),
    (2, 'Additional Service'),
    (3, 'Associated Service');
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertFrameworks.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @frameworks AS TABLE
(
    Id nvarchar(10) NOT NULL PRIMARY KEY,
    [Name] nvarchar(100) NOT NULL,
    ShortName nvarchar(25) NULL,
    [Owner] nvarchar(100) NULL
);

INSERT INTO @frameworks (Id, [Name], ShortName, [Owner])
VALUES
('NHSDGP001', 'NHS Digital GP IT Futures Framework 1', 'GP IT Futures', 'NHS Digital'),
('DFOCVC001', 'Digital First Online Consultation and Video Consultation Framework 1', 'DFOCVC', 'NHS England');

MERGE INTO dbo.Framework AS TARGET
     USING @frameworks AS SOURCE ON TARGET.Id = SOURCE.Id
      WHEN MATCHED THEN
           UPDATE SET TARGET.[Name] = SOURCE.[Name]
      WHEN NOT MATCHED BY TARGET THEN
           INSERT (Id, [Name], ShortName, [Owner])
           VALUES (SOURCE.Id, SOURCE.[Name], SOURCE.ShortName, SOURCE.[Owner]);
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertCapabilities.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @capabilities AS TABLE
(
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    CapabilityRef nvarchar(10) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    SourceUrl nvarchar(1000) NOT NULL,
    IsFoundation bit DEFAULT 0 NOT NULL,
    [Version] nvarchar(10) DEFAULT '1.0.1' NULL,
    EffectiveDate date DEFAULT '2019-12-31' NOT NULL,
    CategoryId int DEFAULT 1 NOT NULL,
    FrameworkId nvarchar(20) DEFAULT 'NHSDGP001' NOT NULL
);

DECLARE @gpitFuturesBaseUrl AS char(55) = 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/';

INSERT INTO @capabilities(Id, CapabilityRef, [Name], [Description], SourceUrl, IsFoundation)
VALUES
('21AE013D-42A4-4748-B435-73D5887944C2', 'C1',  'Appointments Management – Citizen', 'Enables Citizens to manage their Appointments online. Supports the use of Appointment slots that have been configured in Appointments Management – GP.', @gpitFuturesBaseUrl + '1391134205/Appointments+Management+-+Citize', 0),
('4F09E77B-E3A3-4A25-8EC1-815921F83628', 'C2',  'Communicate With Practice – Citizen', 'Supports secure and trusted electronic communications between Citizens and the Practice. Integrates with Patient Information Maintenance.', @gpitFuturesBaseUrl + '1391134188/Communicate+With+Practice+-+Citize', 0),
('60C2F5B0-B950-44C8-A246-099335A1C816', 'C3',  'Prescription Ordering – Citizen', 'Enables Citizens to request medication online and manage nominated and preferred Pharmacies for Patients.', @gpitFuturesBaseUrl + '1391134214/Prescription+Ordering+-+Citizen', 0),
('64E5986D-1EBF-4DF0-8219-C150C082CA7B', 'C4',  'View Record – Citizen', 'Enables Citizens to view their Patient Record online.', @gpitFuturesBaseUrl + '1391134197/View+Record+-+Citize', 0),
('EFD93D25-447B-4CA3-9D78-108D42AFEAE0', 'C5',  'Appointments Management – GP', 'Supports the administration, scheduling, resourcing and reporting of appointments.', @gpitFuturesBaseUrl + '1391134029/Appointments+Management+-+GP', 1),
('A71F2BE1-6395-4DB7-828C-D4733B42B5B5', 'C6',  'Clinical Decision Support', 'Supports clinical decision-making to improve Patient safety at the point of care.', @gpitFuturesBaseUrl + '1391134150/Clinical+Decision+Support', 0),
('0A372F63-ADD4-4529-A6CD-4437C6EF115B', 'C7',  'Communication Management', 'Supports the delivery and management of communications to Citizens and Practice staff.', @gpitFuturesBaseUrl + '1391134087/Communication+Management', 0),
('4518D3F7-F56D-48F0-9FBE-7FA943F4673B', 'C8',  'Digital Diagnostics', 'Supports electronic requesting with other healthcare organisations. Test results can be received, reviewed and stored against the Patient record.', @gpitFuturesBaseUrl + '1391133770/Digital+Diagnostics', 0),
('19002612-8D53-4472-82FC-2753B253434C', 'C9',  'Document Management', 'Supports the secure management and classification of all forms unstructured electronic documents including those created by scanning paper documents. Also enables processing of documents and matching documents with Patients.', @gpitFuturesBaseUrl + '1391134166/Document+Management', 0),
('9D805AAD-D43A-480E-9BC0-41A755BAFE2F', 'C10', 'GP Extracts Verification', 'Supports Practice staff in ensuring accuracy of the data that is used with the Calculating Quality Reporting Service (CQRS).', @gpitFuturesBaseUrl + '1391133797/GP+Extracts+Verificatio', 0),
('20B09859-6FC2-404C-B7A4-3830790E63AB', 'C11', 'Referral Management', 'Supports recording, reviewing, sending, and reporting of Patient Referrals. Enables Referral information to be included in the Patient Record.', @gpitFuturesBaseUrl + '1391133614/Referral+Management', 1),
('E3E4CF8A-22D3-4056-BB5D-10F8E26B9B5E', 'C12', 'Resource Management', 'Supports the management and reporting of Practice information, resources, Staff Members and related organisations. Also enables management of Staff Member availability and inactivity.', @gpitFuturesBaseUrl + '1391133939/Resource+Management', 1),
('8C384983-774A-45BD-9D4E-6B3C7D3B7323', 'C13', 'Patient Information Maintenance', 'Supports the registration of Patients and the maintenance of all Patient personal information. Supports the organisation and presentation of a comprehensive Patient Record. Also supports the management of related persons and configuring access to Citizen Services.', @gpitFuturesBaseUrl + '1391134180/Patient+Information+Maintenance', 1),
('B3F89711-6BD7-42D7-BE5B-BAE2F239EBDD', 'C14', 'Prescribing', 'Supports the effective and safe prescribing of medical products and appliances to Patients. Information to support prescribing will be available.', @gpitFuturesBaseUrl + '1391134158/Prescribing', 1),
('9442DCC4-22DF-494B-8672-B7B4DD077496', 'C15', 'Recording Consultations', 'Supports the standardised recording of Consultations and other General Practice activities. Also supports the extraction of Female Genital Mutilation (FGM) data for the FGM data set.', @gpitFuturesBaseUrl + '1391134389/Recording+Consultations', 1),
('DD649CC4-A710-4472-98B3-663D9D12A8B7', 'C16', 'Reporting', 'Enables reporting and analysis of data from other Capabilities in the Practice Solution to support clinical care and Practice management.', @gpitFuturesBaseUrl + '1391133718/Reporting', 0),
('E5521A71-A28E-4BC9-BDDF-599F0A90719D', 'C17', 'Scanning', 'Support the con[Version] of paper documentation into digital format preserving the document quality and structure.', @gpitFuturesBaseUrl + '1391134270/Scanning', 0),
('385E00F9-3DE6-4A72-B662-E0405BCECFC8', 'C18', 'Telehealth', 'Enables Citizens and Patients that use health monitoring solutions to share monitoring data with health and care professionals to support remote delivery of care and increase self-care outside of clinical settings.', @gpitFuturesBaseUrl + '1391134248/Telehealth', 0),
('1E82CC7C-87C7-4379-B86F-CF36C59D1A46', 'C19', 'Unstructured Data Extraction', 'Enables automated and manual interpretation and extraction of structured data from paper documents and unstructured electronic documents to support their classification and matching with Patient Records.', @gpitFuturesBaseUrl + '1391133668/Unstructured+Data+Extractio', 0),
('9D325DEC-6E5B-44E4-876B-EACF6CD41B3E', 'C20', 'Workflow', 'Supports manual and automated management of work in the Practice. Also supports effective planning, tracking, monitoring and reporting.', @gpitFuturesBaseUrl + '1391134020/Workflow', 0),
('1C552148-6EA8-4D82-84EB-E660622A1741', 'C21', 'Care Homes', 'Enables a record of the Resident''s health and care needs to be maintained and shared with parties who are involved in providing care, to support decision making and the effective planning and delivery of care.', @gpitFuturesBaseUrl + '1391133439/Care+Homes', 0),
('12B3AD26-487E-43B1-9D58-264C3C359BC6', 'C22', 'Caseload Management', 'Supports the allocation of appropriate Health and Care Professionals to Patients/Service Users in need of support, ensuring balanced workloads and the efficient use of staff and other resources.', @gpitFuturesBaseUrl + '1391133457/Caseload+Management', 0),
('7547E181-C897-4A01-86D9-09B76AB1C906', 'C23', 'Cross-organisation Appointment Booking', 'Enables appointments to be made available and booked across Organisational boundaries, creating flexibility for Health and Care Professionals and Patients/Service Users.', @gpitFuturesBaseUrl + '1391135407/Cross-organisation+Appointment+Booking', 0),
('890AF628-5B84-4176-B3D1-A4ADC65710FE', 'C24', 'Cross-organisation Workflow Tools', 'Supports and automates clinical and business processes across Organisational boundaries to make processes and communication more efficient.', @gpitFuturesBaseUrl + '1391133492/Cross-organisation+Workflow+Tools', 0),
('7E8A8D7A-F8CE-4AA5-A3EF-31BBBD39DF40', 'C25', 'Cross-organisation Workforce Management', 'Supports the efficient planning and scheduling of the health and care workforce to ensure that services can be delivered effectively by the right staff.', @gpitFuturesBaseUrl + '1391135659/Cross-organisation+Workforce+Management', 0),
('5DB79FF4-FA9C-4DA2-BBFC-8CA40FEC0B43', 'C26', 'Data Analytics for Integrated and Federated Care', 'Supports the analysis of multiple and complex datasets and presentation of the output to enable decision-making, service design and performance management.', @gpitFuturesBaseUrl + '1391135590/Data+Analytics+for+Integrated+and+Federated+Care', 0),
('A66765F0-7EB6-400B-8319-FE7FBD86AB47', 'C27', 'Domiciliary Care', 'Enables Service Providers to effectively plan and manage Domiciliary Care services to ensure care needs are met and that Care Workers can manage their schedule.', @gpitFuturesBaseUrl + '1391133451/Domiciliary+Care', 0),
('7BE309D9-696F-4B90-A65E-EB16DD5AC4ED', 'C29', 'e-Consultations (Professional to Professional)', 'Enables the communication and sharing of specialist knowledge and advice between Health and Care Professionals to support better care decisions and professional development.', @gpitFuturesBaseUrl + '1391135495/e-Consultations+Professional+to+Professional', 0),
('8BEE1FF3-84D4-430B-A678-336F57C57387', 'C30', 'Medicines Optimisation', 'Supports clinicians and pharmacists in reviewing a Patient''s medication and requesting changes to medication to ensure the Patient is taking the best combination of medicines.', @gpitFuturesBaseUrl + '1391133405/Medicines+Optimisatio', 0),
('0766FCF3-79B1-4B2F-A79E-9B09C0249034', 'C32', 'Personal Health Budget', 'Enables a Patient/Service User to set up and manage a Personal Health Budget giving them more choice and control over the management of their identified healthcare and well-being needs.', @gpitFuturesBaseUrl + '1391133426/Personal+Health+Budget', 0),
('E5E3BE58-E5EC-4423-85DD-61D88640C22A', 'C33', 'Personal Health Record', 'Enables a Patient/Service User to manage and maintain their own Electronic Health Record and to share that information with relevant Health and Care Professionals.', @gpitFuturesBaseUrl + '1391135480/Personal+Health+Record', 0),
('2271B113-5D5D-4899-B259-3046CAEA76ED', 'C34', 'Population Health Management', 'Enables Organisations to accumulate, analyse and report on Patient healthcare data to identify improvement in care and identify and track Patient outcomes.', @gpitFuturesBaseUrl + '1391135469/Population+Health+Management', 0),
('12C6A61C-013C-475F-BB0C-2DA5D414C03B', 'C35', 'Risk Stratification', 'Supports Health and Care Professionals by providing trusted models to predict future Patient events, informing interventions to achieve better Patient outcomes.', @gpitFuturesBaseUrl + '1391133445/Risk+Stratificatio', 0),
('D1532CA0-EF0C-457C-9CFC-AFFA0FBDF134', 'C36', 'Shared Care Plans', 'Enables the maintenance of a single, shared care plan across multiple Organisations to ensure more co-ordinated working and more efficient management of activities relating to the Patient/Service User''s health and care.', @gpitFuturesBaseUrl + '1391134486/Shared+Care+Plans', 0),
('1D1B92A4-BD48-4C55-8301-9D1830BCD729', 'C37', 'Social Prescribing', 'Supports the referral of Patients/Service Users to non-clinical services to help address their health and well-being needs.', @gpitFuturesBaseUrl + '1391135572/Social+Prescribing', 0),
('188F67DB-49D9-4808-810F-27D9E7703DF6', 'C38', 'Telecare', 'Supports the monitoring of Patients/Service Users or their environment to ensure quick identification and response to any adverse event.', @gpitFuturesBaseUrl + '1391135549/Telecare', 0),
('59696227-602A-421D-A883-29E88997AC17', 'C39', 'Unified Care Record', 'Provides a consolidated view to Health and Care Professionals of a Patient/Service User''s complete and up-to-date records, sourced from various health and care settings.', @gpitFuturesBaseUrl + '1391134504/Unified+Care+Record', 0),
('4CFB2E12-9B05-4F48-AD25-5E8A4A06C6E7', 'C40', 'Medicines Verification', 'Supports compliance with the Falsified Medicines Directive and minimise the risk that falsified medicinal products are supplied to the public.', @gpitFuturesBaseUrl + '1391135093/Medicines+Verificatio', 0),
('6E77147D-D2AF-46BD-A2F2-BB4F235DAF3A', 'C41', 'Productivity', 'Supports Patients/Service Users and Health and Care Professionals by delivering improved efficiency or experience related outcomes.', @gpitFuturesBaseUrl + '1391135618/Productivity', 0),
('D314DC27-BC65-4ABD-97C5-F9BE478D8A10', 'C42', 'Dispensing', 'Supports the timely and effective dispensing of medical products and appliances to Patients.', @gpitFuturesBaseUrl + '1391133465/Dispensing', 0);

DECLARE @covidVaccinationBaseUrl AS char(55) = 'https://gpitbjss.atlassian.net/wiki/spaces/CVPDR/pages/';

INSERT INTO @capabilities(Id, CapabilityRef, [Name], [Description], SourceUrl, [Version], EffectiveDate, CategoryId)
VALUES
('151CA7DF-5227-4EBF-9200-93258EFA3735', 'C45', 'Cohort Identification', 'The Cohort Identification Capability enables the identification of Patient cohorts by identifying Patients that require a COVID-19 vaccination based on nationally defined criteria.', @covidVaccinationBaseUrl + '7918551305/Cohort+Identification', '1.0.2', '2021-01-25', 2),
('53255CB2-C5F6-427B-A18A-D3055E310FD6', 'C46', 'Appointments Management – COVID-19 Vaccinations', 'The Appointments Management – COVID-19 Vaccinations Capability enables the administration and scheduling of COVID-19 vaccination appointments for Patients.', @covidVaccinationBaseUrl + '7918551324/Appointments+Management+-+COVID-19+Vaccinations', '1.0.0', '2020-12-09', 2),
('EC26D316-4F3D-45B1-BDF5-717D24DAB360', 'C47', 'Vaccination and Adverse Reaction Recording', 'The Vaccination and Adverse Reaction Recording Capability enables the recording of COVID-19 vaccination and adverse reaction data at the point of care. The Capability also supports the delivery of this data to the Patient’s registered GP Practice Foundation Solution and to NHS Digital.', @covidVaccinationBaseUrl + '7918551342/Vaccination+and+Adverse+Reaction+Recording', '4.0.0', '2021-03-29', 2);

INSERT INTO @capabilities(Id, CapabilityRef, [Name], [Description], SourceUrl, [Version], EffectiveDate, CategoryId, FrameworkId)
VALUES
('EE71409B-F570-4581-B082-2B13DAC6CE6D', 'C43', 'Online Consultation', 'The Online Consultation Capability allows Patients/Service Users/Proxies to request and receive support relating to healthcare concerns, at a time and place convenient for them.', 'https://gpitbjss.atlassian.net/wiki/spaces/DFOCVCL/pages/5132484695/Online+Consultation', '1.0.1', '2021-03-11', 3, 'DFOCVC001'),
('4E07C901-1DA1-43EB-BE45-C89B7DBD9D66', 'C44', 'Video Consultation', 'The Video Consultation Capability allows Health or Care Professionals to conduct secure live remote video consultations with individual or groups of Patients/Service Users/Proxies ensuring they can receive support relating to healthcare concerns when a Video Consultation is most appropriate.', 'https://gpitbjss.atlassian.net/wiki/spaces/DFOCVCL/pages/5132484947/Video+Consultation', '1.0.0', '2021-03-11', 3, 'DFOCVC001');

-- The code below should not need to be changed

MERGE INTO dbo.Capability AS TARGET
     USING @capabilities AS SOURCE
        ON TARGET.Id = SOURCE.Id
      WHEN MATCHED THEN
UPDATE SET TARGET.[Version] = SOURCE.[Version],
           TARGET.[Name] = SOURCE.[Name],
           TARGET.[Description] = SOURCE.[Description],
           TARGET.SourceUrl = SOURCE.SourceUrl,
           TARGET.EffectiveDate = SOURCE.EffectiveDate,
           TARGET.CategoryId = SOURCE.CategoryId
      WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, CapabilityRef, [Version], StatusId, [Name], [Description], SourceUrl, EffectiveDate, CategoryId)
    VALUES (SOURCE.Id, SOURCE.CapabilityRef, SOURCE.[Version], 1, SOURCE.[Name], SOURCE.[Description], SOURCE.SourceUrl, SOURCE.EffectiveDate, SOURCE.CategoryId);

MERGE INTO dbo.FrameworkCapabilities AS TARGET
     USING @capabilities AS SOURCE
        ON TARGET.CapabilityId = SOURCE.Id
      WHEN MATCHED THEN
UPDATE SET TARGET.IsFoundation = SOURCE.IsFoundation
      WHEN NOT MATCHED BY TARGET THEN
    INSERT (FrameworkId, CapabilityId, IsFoundation)
    VALUES (SOURCE.FrameworkId, SOURCE.Id, SOURCE.IsFoundation);
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertCataloguePriceTypes.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM dbo.CataloguePriceType)
    INSERT INTO dbo.CataloguePriceType(CataloguePriceTypeId, [Name])
    VALUES
    (1, 'Flat'),
    (2, 'Tiered');
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertEpics.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE TABLE #Epics
(
    Id nvarchar(10) NOT NULL PRIMARY KEY,
    [Name] nvarchar(150) NOT NULL,
    CapabilityId uniqueidentifier NOT NULL,
    CompliancyLevelId int NULL,
    Active bit NOT NULL
);

DECLARE @capabilityId AS uniqueidentifier = (SELECT Id FROM Capability WHERE CapabilityRef = 'C1');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C1E1', 'Manage Appointments', @capabilityId, 1, 1),
('C1E2', 'Manage Appointments by Proxy', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C2');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C2E1', 'Manage communications – Patient', @capabilityId, 1, 1),
('C2E2', 'Manage communications – Proxy', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C3');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C3E1', 'Manage Repeat Medications – Patient', @capabilityId, 1, 1),
('C3E2', 'Manage my nominated EPS pharmacy', @capabilityId, 1, 1),
('C3E3', 'Manage my Preferred PharmacyAs a Patient', @capabilityId, 3, 1),
('C3E4', 'Manage Acute Medications', @capabilityId, 3, 1),
('C3E5', 'View medication information', @capabilityId, 3, 1),
('C3E6', 'Manage Repeat Medications as a Proxy', @capabilityId, 3, 1),
('C3E7', 'Manage nominated EPS pharmacy as a Proxy', @capabilityId, 3, 1),
('C3E8', 'Manage Preferred Pharmacy as a Proxy', @capabilityId, 3, 1),
('C3E9', 'Manage Acute Medications as a Proxy', @capabilityId, 3, 1),
('C3E10', 'View medication information as a Proxy', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C4');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C4E1', 'View Patient Record – Patient', @capabilityId, 1, 1),
('C4E2', 'View Patient Record – Proxy', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C5');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C5E1', 'Manage Session templates', @capabilityId, 1, 1),
('C5E2', 'Manage Sessions', @capabilityId, 1, 1),
('C5E3', 'Configure Appointments', @capabilityId, 1, 1),
('C5E4', 'Practice configuration', @capabilityId, 1, 1),
('C5E5', 'Manage Appointments', @capabilityId, 1, 1),
('C5E6', 'View Appointment reports', @capabilityId, 1, 1),
('C5E7', 'Access Patient Record', @capabilityId, 1, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C6');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C6E1', 'Access to Clinical Decision Support', @capabilityId, 1, 1),
('C6E2', 'Local configuration for Clinical Decision Support triggering', @capabilityId, 1, 1),
('C6E3', 'View Clinical Decision Support reports', @capabilityId, 1, 1),
('C6E4', 'Configuration for custom Clinical Decision Support', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C7');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C7E1', 'Manage communication consents for a Patient', @capabilityId, 1, 1),
('C7E2', 'Manage communication preferences for a Patient', @capabilityId, 1, 1),
('C7E3', 'Manage communication templates', @capabilityId, 1, 1),
('C7E4', 'Create communications', @capabilityId, 1, 1),
('C7E5', 'Manage automated communications', @capabilityId, 1, 1),
('C7E6', 'View communication reports', @capabilityId, 1, 1),
('C7E7', 'Access Patient Record', @capabilityId, 1, 1),
('C7E8', 'Manage communication consents for a Proxy', @capabilityId, 3, 1),
('C7E9', 'Manage communication preferences for a Proxy', @capabilityId, 3, 1),
('C7E10', 'Manage incoming communications', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C8');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C8E1', 'Manage Requests for Investigations', @capabilityId, 1, 1),
('C8E2', 'View Requests for Investigations reports', @capabilityId, 1, 1),
('C8E3', 'Create a Request for Investigation for multiple Patients', @capabilityId, 3, 1),
('C8E4', 'Receive external Request for Investigation information', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C9');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C9E1', 'Manage document classifications', @capabilityId, 1, 1),
('C9E2', 'Manage document properties', @capabilityId, 1, 1),
('C9E3', 'Manage document attributes', @capabilityId, 1, 1),
('C9E4', 'Manage document coded entries', @capabilityId, 1, 1),
('C9E5', 'Document workflows', @capabilityId, 1, 1),
('C9E6', 'Manage document annotation', @capabilityId, 1, 1),
('C9E7', 'Search for documents', @capabilityId, 1, 1),
('C9E8', 'Search document content', @capabilityId, 1, 1),
('C9E9', 'Document and Patient matching', @capabilityId, 1, 1),
('C9E10', 'Visually compare multiple documents', @capabilityId, 1, 1),
('C9E11', 'View any version of a document', @capabilityId, 1, 1),
('C9E12', 'Print documents', @capabilityId, 1, 1),
('C9E13', 'Export documents to new formats', @capabilityId, 1, 1),
('C9E14', 'Document reports', @capabilityId, 1, 1),
('C9E15', 'Receipt of electronic documents', @capabilityId, 1, 1),
('C9E16', 'Access Patient Record', @capabilityId, 1, 1),
('C9E17', 'Search for documents using document content', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C10');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C10E1', 'View GPES payment extract reports', @capabilityId, 1, 1),
('C10E2', 'View national GPES non-payment extract reports', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C11');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C11E1', 'Manage Referrals', @capabilityId, 1, 1),
('C11E2', 'View Referral reports', @capabilityId, 1, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C12');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C12E1', 'Manage General Practice and Branch site information', @capabilityId, 1, 1),
('C12E2', 'Manage General Practice Staff Members', @capabilityId, 1, 1),
('C12E3', 'Manage Staff Member inactivity periods', @capabilityId, 1, 1),
('C12E4', 'Manage Staff Member Groups', @capabilityId, 1, 1),
('C12E5', 'Manage Related Organisations information', @capabilityId, 1, 1),
('C12E6', 'Manage Related Organisation Staff Members', @capabilityId, 1, 1),
('C12E7', 'Manage Non-human Resources', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C13');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C13E1', 'Manage Patients ', @capabilityId, 1, 1),
('C13E2', 'Access Patient Record', @capabilityId, 1, 1),
('C13E3', 'Manage Patient Related Persons', @capabilityId, 1, 1),
('C13E4', 'Manage Patients for Citizen Services', @capabilityId, 1, 1),
('C13E5', 'Manage Patient Communications', @capabilityId, 1, 1),
('C13E6', 'Configure Patient notifications', @capabilityId, 1, 1),
('C13E7', 'Manage Practice notifications – Patient', @capabilityId, 1, 1),
('C13E8', 'Search for Patient Records', @capabilityId, 1, 1),
('C13E9', 'View Patient Reports', @capabilityId, 1, 1),
('C13E10', 'Configure Citizen service access for the Practice', @capabilityId, 1, 1),
('C13E11', 'Identify Patients outside of Catchment Area', @capabilityId, 1, 1),
('C13E12', 'Manage Patient Cohorts from Search Results', @capabilityId, 1, 1),
('C13E13', 'View Subject Access Request reports', @capabilityId, 3, 1),
('C13E14', 'Manage Acute Prescription Request Service', @capabilityId, 3, 1),
('C13E15', 'Notify the Patient of changes', @capabilityId, 3, 1),
('C13E16', 'Manage Subject Access Request (SAR) requests', @capabilityId, 3, 1),
('C13E17', 'Notify the Proxy of changes', @capabilityId, 3, 1),
('C13E18', 'Manage Practice notifications – Proxy', @capabilityId, 3, 1),
('C13E19', 'Configure Proxy notifications', @capabilityId, 3, 1),
('C13E20', 'Manage Proxy Communications', @capabilityId, 3, 1),
('C13E21', 'Manage Proxys for Citizen Services', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C14');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C14E1', 'Access prescribable items', @capabilityId, 1, 1),
('C14E2', 'Manage Formularies', @capabilityId, 1, 1),
('C14E3', 'Manage shared Formularies', @capabilityId, 1, 1),
('C14E4', 'Set default Formulary for Practice Users', @capabilityId, 1, 1),
('C14E5', 'Manage prescribed medication', @capabilityId, 1, 1),
('C14E6', 'Manage prescriptions', @capabilityId, 1, 1),
('C14E7', 'Manage Patient''s Preferred Pharmacy', @capabilityId, 1, 1),
('C14E8', 'Manage Patient medication reviews', @capabilityId, 1, 1),
('C14E9', 'View prescribed medication reports', @capabilityId, 1, 1),
('C14E10', 'Manage Repeat Medication requests', @capabilityId, 1, 1),
('C14E11', 'Manage Acute Medication requests', @capabilityId, 1, 1),
('C14E12', 'Manage Authorising Prescribers', @capabilityId, 1, 1),
('C14E13', 'Access Patient Record', @capabilityId, 1, 1),
('C14E14', 'View EPS Nominated Pharmacy changes', @capabilityId, 3, 1),
('C14E15', 'Configure warnings for prescribable items', @capabilityId, 3, 1),
('C14E16', 'Medications are linked to diagnoses', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C15');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C15E1', 'Record Consultation information', @capabilityId, 1, 1),
('C15E2', 'View report on calls and recalls', @capabilityId, 1, 1),
('C15E3', 'View report of Consultations', @capabilityId, 1, 1),
('C15E4', 'Access Patient Record', @capabilityId, 1, 1),
('C15E5', 'Manage Consultation form templates', @capabilityId, 1, 1),
('C15E6', 'Share Consultation form templates', @capabilityId, 1, 1),
('C15E7', 'Use supplier implemented national Consultation form templates', @capabilityId, 1, 1),
('C15E8', 'Extract Female Genital Mutilation data', @capabilityId, 1, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C16');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C16E1', 'Report data from other Capabilities', @capabilityId, 1, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C17');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C17E1', 'Scan documents', @capabilityId, 1, 1),
('C17E2', 'Image enhancement', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C18');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C18E1', 'Share monitoring data with my General Practice', @capabilityId, 1, 1),
('C18E2', 'Configure Telehealth for the Practice', @capabilityId, 1, 1),
('C18E3', 'Configure Telehealth for the Patient', @capabilityId, 1, 1),
('C18E4', 'Manage incoming Telehealth data', @capabilityId, 1, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C19');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C19E1', 'Document classification', @capabilityId, 1, 1),
('C19E2', 'Manage Document Classification rules', @capabilityId, 1, 1),
('C19E3', 'Document and Patient matching', @capabilityId, 1, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C20');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C20E1', 'Manage Task templates', @capabilityId, 1, 1),
('C20E2', 'Manage Workflow templates', @capabilityId, 1, 1),
('C20E3', 'Configure Task rules', @capabilityId, 1, 1),
('C20E4', 'Configure Workflow rules', @capabilityId, 1, 1),
('C20E5', 'Manage Tasks', @capabilityId, 1, 1),
('C20E6', 'Manage Workflows', @capabilityId, 1, 1),
('C20E7', 'Manage Task List configurations', @capabilityId, 1, 1),
('C20E8', 'Manage Workflows List configurations', @capabilityId, 1, 1),
('C20E9', 'View Task reports', @capabilityId, 1, 1),
('C20E10', 'View Workflow reports', @capabilityId, 1, 1),
('C20E11', 'Access Patient Record', @capabilityId, 1, 1),
('C20E12', 'Share Task List configuration', @capabilityId, 3, 1),
('C20E13', 'Share Workflow List configuration', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C21');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C21E1', 'Maintain Resident''s Care Home Record', @capabilityId, 1, 1),
('C21E2', 'Maintain Resident Proxy preferences', @capabilityId, 3, 1),
('C21E3', 'View and maintain End of Life Care Plans', @capabilityId, 3, 1),
('C21E4', 'Record incident and adverse events', @capabilityId, 3, 1),
('C21E5', 'Maintain Staff Records', @capabilityId, 3, 1),
('C21E6', 'Maintain Staff Task schedules', @capabilityId, 3, 1),
('C21E7', 'Manage Tasks', @capabilityId, 3, 1),
('C21E8', 'Reporting', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C22');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C22E1', 'Manage Cases', @capabilityId, 1, 1),
('C22E2', 'Maintain Caseloads', @capabilityId, 1, 1),
('C22E3', 'Generate and manage contact schedules', @capabilityId, 1, 1),
('C22E4', 'Update Case details', @capabilityId, 1, 1),
('C22E5', 'Review and comment on Caseload', @capabilityId, 3, 1),
('C22E6', 'Review and comment on contact schedule', @capabilityId, 3, 1),
('C22E7', 'View and update Patient/Service User''s Health or Care Record', @capabilityId, 3, 1),
('C22E8', 'Reporting', @capabilityId, 3, 1),
('C22E9', 'Care Pathway templates', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C23');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C23E1', 'Make Appointments available to external organisations', @capabilityId, 1, 1),
('C23E2', 'Search externally bookable Appointment slots', @capabilityId, 1, 1),
('C23E3', 'Book externally bookable Appointment slots', @capabilityId, 1, 1),
('C23E4', 'Maintain Appointments', @capabilityId, 1, 1),
('C23E5', 'Notifications', @capabilityId, 3, 1),
('C23E6', 'Manage Appointment Requests', @capabilityId, 3, 1),
('C23E7', 'Booking approval', @capabilityId, 3, 1),
('C23E8', 'Report on usage of Cross-Organisation Appointments', @capabilityId, 3, 1),
('C23E9', 'Manage Cross-Organisation Appointment Booking templates', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C24');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C24E1', 'Use Workflow to run a Cross-organisational Process', @capabilityId, 1, 1),
('C24E2', 'Maintain cross-organisational workflows', @capabilityId, 1, 1),
('C24E3', 'Maintain cross-organisational workflow templates', @capabilityId, 3, 1),
('C24E4', 'Share workflow templates', @capabilityId, 3, 1),
('C24E5', 'Manage automated notifications and reminders', @capabilityId, 3, 1),
('C24E6', 'Manage ad-hoc notifications', @capabilityId, 3, 1),
('C24E7', 'Report on Cross-organisational Workflows', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C25');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C25E1', 'Maintain service schedule', @capabilityId, 1, 1),
('C25E2', 'Share service schedule', @capabilityId, 1, 1),
('C25E3', 'Workforce management reporting', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C26');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C26E1', 'Analyse data across multiple organisations within the Integrated/Federated Care Setting (Federation)', @capabilityId, 1, 1),
('C26E2', 'Analyse data across different datasets', @capabilityId, 1, 1),
('C26E3', 'Create new or update existing reports ', @capabilityId, 1, 1),
('C26E4', 'Run existing reports', @capabilityId, 1, 1),
('C26E5', 'Present output', @capabilityId, 1, 1),
('C26E6', 'Define selection rules on reports', @capabilityId, 1, 1),
('C26E7', 'Create and run performance-based reports', @capabilityId, 3, 1),
('C26E8', 'Drill down to detailed information', @capabilityId, 3, 1),
('C26E9', 'Forecasting', @capabilityId, 3, 1),
('C26E10', 'Enable reporting at different levels', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C27');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C27E1', 'Maintain Domiciliary Care schedules', @capabilityId, 1, 1),
('C27E2', 'Share Domiciliary Care schedules', @capabilityId, 1, 1),
('C27E3', 'Manage Appointments', @capabilityId, 1, 1),
('C27E4', 'Service User manages their schedule for Domiciliary Care', @capabilityId, 3, 1),
('C27E5', 'Manage Care Plans for Service Users', @capabilityId, 3, 1),
('C27E6', 'Remote access to Domiciliary Care schedule', @capabilityId, 3, 1),
('C27E7', 'Receive notifications relating to Service User', @capabilityId, 3, 1),
('C27E8', 'Reports', @capabilityId, 3, 1),
('C27E9', 'Nominated individuals to view Domiciliary Care schedule', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C29');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C29E1', 'Health or Care Professional requests support', @capabilityId, 1, 1),
('C29E2', 'Respond to request for support from another Health or Care Professional', @capabilityId, 1, 1),
('C29E3', 'Link additional information to a request for support', @capabilityId, 3, 1),
('C29E4', 'Live Consultation: Health and Care Professionals', @capabilityId, 3, 1),
('C29E5', 'Link Consultation to Patient/Service User''s Record', @capabilityId, 3, 1),
('C29E6', 'Reports', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C30');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C30E1', 'Single unified medication view', @capabilityId, 1, 1),
('C30E2', 'Request medication changes', @capabilityId, 1, 1),
('C30E3', 'Identify Patients requiring medicines review', @capabilityId, 3, 1),
('C30E4', 'Maintain medicines review', @capabilityId, 3, 1),
('C30E5', 'Notify Patient and Proxies of medication changes', @capabilityId, 3, 1),
('C30E6', 'Notify other interested parties of medication changes', @capabilityId, 3, 1),
('C30E7', 'Configure medication substitutions', @capabilityId, 3, 1),
('C30E8', 'Use pre-configured medication substitutions', @capabilityId, 3, 1),
('C30E9', 'Maintain prescribed medication', @capabilityId, 3, 1),
('C30E10', 'Access national or local Medicines Optimisation guidance', @capabilityId, 3, 1),
('C30E11', 'Prescribing decision support', @capabilityId, 3, 1),
('C30E12', 'Medicines Optimisation reporting', @capabilityId, 3, 1),
('C30E13', 'Configure notifications for required Medicines Reviews', @capabilityId, 3, 1),
('C30E14', 'Receive notification for required medicines reviews', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C32');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C32E1', 'Manage Personal Health Budget', @capabilityId, 1, 1),
('C32E2', 'Record Personal Health Budget purchases', @capabilityId, 1, 1),
('C32E3', 'Assess Personal Health Budgets', @capabilityId, 1, 1),
('C32E4', 'Link Personal Health Budget with care plan', @capabilityId, 3, 1),
('C32E5', 'Support different models for management of Personal Health Budgets', @capabilityId, 3, 1),
('C32E6', 'Apply criteria for the use of Personal Health Budgets', @capabilityId, 3, 1),
('C32E7', 'Payments under Personal Health Budgets', @capabilityId, 3, 1),
('C32E8', 'Maintain directory of equipment, treatments and services', @capabilityId, 3, 1),
('C32E9', 'Search a directory of equipment, treatments and services', @capabilityId, 3, 1),
('C32E10', 'Manage multiple budgets', @capabilityId, 3, 1),
('C32E11', 'Link to Patient Record', @capabilityId, 3, 1),
('C32E12', 'Link to Workflow', @capabilityId, 3, 1),
('C32E13', 'Provider view', @capabilityId, 3, 1),
('C32E14', 'Management Information', @capabilityId, 3, 1),
('C32E15', 'Identify candidates for Personal Health Budgets', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C33');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C33E1', 'Maintain Personal Health Record content', @capabilityId, 1, 1),
('C33E2', 'Organise Personal Health Record', @capabilityId, 3, 1),
('C33E3', 'Manage access to Personal Health Record', @capabilityId, 3, 1),
('C33E4', 'Manage data coming into Personal Health Record', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C34');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C34E1', 'Access healthcare data', @capabilityId, 1, 1),
('C34E2', 'Maintain cohorts', @capabilityId, 1, 1),
('C34E3', 'Stratify population by risk', @capabilityId, 1, 1),
('C34E4', 'Data analysis and reporting', @capabilityId, 1, 1),
('C34E5', 'Outcomes', @capabilityId, 1, 1),
('C34E6', 'Dashboard', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C35');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C35E1', 'Run Risk Stratification algorithms', @capabilityId, 1, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C36');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C36E1', 'Create Shared Care Plan', @capabilityId, 1, 1),
('C36E2', 'View Shared Care Plan', @capabilityId, 1, 1),
('C36E3', 'Amend Shared Care Plan', @capabilityId, 1, 1),
('C36E4', 'Close Shared Care Plan', @capabilityId, 1, 1),
('C36E5', 'Assign Shared Care Plan actions', @capabilityId, 3, 1),
('C36E6', 'Access Shared Care Plans remotely', @capabilityId, 3, 1),
('C36E7', 'Search and view Shared Care Plans', @capabilityId, 3, 1),
('C36E8', 'Real-time access to Shared Care Plans', @capabilityId, 3, 1),
('C36E9', 'Notifications', @capabilityId, 3, 1),
('C36E10', 'Reports', @capabilityId, 3, 1),
('C36E11', 'Manage Shared Care Plan templates', @capabilityId, 3, 1),
('C36E12', 'Manage care schedules', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C37');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C37E1', 'Assess wellness or well-being of the Patient or Service User', @capabilityId, 1, 1),
('C37E2', 'Search the directory', @capabilityId, 1, 1),
('C37E3', 'Refer Patient/Service User to service(s)', @capabilityId, 1, 1),
('C37E4', 'Maintain referral record', @capabilityId, 1, 1),
('C37E5', 'Link to national or local directory of services', @capabilityId, 1, 1),
('C37E6', 'Maintain directory of services', @capabilityId, 1, 1),
('C37E7', 'Maintain service criteria', @capabilityId, 1, 1),
('C37E8', 'Refer Patient/Service User to Link Worker', @capabilityId, 3, 1),
('C37E9', 'Capture Patient/Service User consent', @capabilityId, 3, 1),
('C37E10', 'Patient self-referral', @capabilityId, 3, 1),
('C37E11', 'Integrate Social Prescribing Referral Record with Clinical Record', @capabilityId, 3, 1),
('C37E12', 'Receive notification of an Appointment', @capabilityId, 3, 1),
('C37E13', 'Remind Patients/Service Users of Appointments', @capabilityId, 3, 1),
('C37E14', 'Provide service feedback', @capabilityId, 3, 1),
('C37E15', 'View service feedback', @capabilityId, 3, 1),
('C37E16', 'Obtain Management Information (MI) on Social Prescribing', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C38');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C38E1', 'Define response to event', @capabilityId, 1, 1),
('C38E2', 'Monitor and alert', @capabilityId, 1, 1),
('C38E3', 'Receive alerts', @capabilityId, 1, 1),
('C38E4', 'Meet availability targets', @capabilityId, 1, 1),
('C38E5', 'Ease of use', @capabilityId, 3, 1),
('C38E6', 'Patient/Service Users with sensory impairment(s)', @capabilityId, 3, 1),
('C38E7', 'Obtain Management Information (MI) on Telecare', @capabilityId, 3, 1),
('C38E8', 'Enable 2-way communication with Patient/Service User', @capabilityId, 3, 1),
('C38E9', 'Remote testing of Telecare device', @capabilityId, 3, 1),
('C38E10', 'Manual testing of Telecare device', @capabilityId, 3, 1),
('C38E11', 'Sustainability of Telecare device', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C39');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C39E1', 'View Unified Care Record', @capabilityId, 1, 1),
('C39E2', 'Patient/Service User views the Unified Care Record', @capabilityId, 3, 1),
('C39E3', 'Default Views', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C40');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C40E1', 'Verify Medicinal Product Unique Identifiers', @capabilityId, 1, 1),
('C40E2', 'Decommission Medicinal Products', @capabilityId, 1, 1),
('C40E3', 'Record the integrity of Anti-tampering Devices', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C42');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C42E1', 'Manage Stock in a Dispensary', @capabilityId, 1, 1),
('C42E2', 'Manage Stock Orders', @capabilityId, 1, 1),
('C42E3', 'Manage Dispensing tasks for a Dispensary', @capabilityId, 1, 1),
('C42E4', 'Dispense Medication', @capabilityId, 1, 1),
('C42E5', 'Manage Dispensaries', @capabilityId, 1, 1),
('C42E6', 'Manage Endorsements', @capabilityId, 1, 1),
('C42E7', 'Manage Supplier Accounts', @capabilityId, 1, 1),
('C42E8', 'View Stock reports', @capabilityId, 1, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C45');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C45E1', 'Identify COVID-19 vaccination cohorts', @capabilityId, 1, 1),
('C45E2', 'Verify Patient information using Personal Demographics Service (PDS)', @capabilityId, 1, 1),
('C45E3', 'Import or consume COVID-19 vaccination data for Patients', @capabilityId, 1, 1),
('C45E4', 'Extract COVID-19 vaccination cohorts data in .CSV file format', @capabilityId, 3, 1),
('C45E5', 'Bulk send SMS messages for COVID-19 vaccination invite communications', @capabilityId, 3, 1),
('C45E6', 'Bulk create letters for COVID-19 vaccination invite communications', @capabilityId, 3, 1),
('C45E7', 'Bulk send email for COVID-19 vaccination invite communications', @capabilityId, 3, 1),
('C45E8', 'Automatically record which Patients have had COVID-19 vaccination invites created', @capabilityId, 3, 1),
('C45E9', 'View whether Patients have had a COVID-19 vaccination invite communication created', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C46');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C46E1', 'Define appointment availability for COVID-19 vaccination site', @capabilityId, 1, 1),
('C46E2', 'Book COVID-19 vaccination appointments for eligible Patients registered across different GP Practices', @capabilityId, 1, 1),
('C46E3', 'Record that a COVID-19 vaccination appointment for a Patient has been completed', @capabilityId, 1, 1),
('C46E4', 'Extract COVID-19 vaccination appointments data for NHS Digital', @capabilityId, 1, 1),
('C46E5', 'Import COVID-19 vaccination Patient cohorts data via .CSV file', @capabilityId, 3, 1),
('C46E6', 'Verify Patient information using Personal Demographics Service (PDS)', @capabilityId, 3, 1),
('C46E7', 'Bulk send SMS messages for COVID-19 vaccination invite communications', @capabilityId, 3, 1),
('C46E8', 'Bulk create letters for COVID-19 vaccination invite communications', @capabilityId, 3, 1),
('C46E9', 'Bulk send email for COVID-19 vaccination invite communications', @capabilityId, 3, 1),
('C46E10', 'Automatically record which Patients have had COVID-19 vaccination invites created', @capabilityId, 3, 1),
('C46E11', 'View whether Patients have had a COVID-19 vaccination invite communication created', @capabilityId, 3, 1),
('C46E12', 'Automatically bulk send booking reminders to Patients via SMS messages for COVID-19 vaccination invites', @capabilityId, 3, 1),
('C46E13', 'Automatically bulk create booking reminders to Patients as letters for COVID-19 vaccination invites', @capabilityId, 3, 1),
('C46E14', 'Automatically bulk send booking reminders to Patients via email for COVID-19 vaccination invites', @capabilityId, 3, 1),
('C46E15', 'Book Appointments across Solutions using GP Connect Appointments Management', @capabilityId, 3, 1),
('C46E16', 'Patients can book their own COVID-19 vaccination appointments', @capabilityId, 3, 1),
('C46E17', 'Patients can re-schedule their own future COVID-19 vaccination appointment', @capabilityId, 3, 1),
('C46E18', 'Patients can cancel their own future COVID-19 vaccination appointment', @capabilityId, 3, 1),
('C46E19', 'Automatically send booking notifications to Patients via SMS messages for COVID-19 vaccination appointments', @capabilityId, 3, 1),
('C46E20', 'Automatically create booking notifications to Patients as letters for COVID-19 vaccination appointments', @capabilityId, 3, 1),
('C46E21', 'Automatically send booking notifications to Patients via email for COVID-19 vaccination appointments', @capabilityId, 3, 1),
('C46E22', 'Create ad-hoc booking notifications to Patients for COVID-19 vaccination appointments', @capabilityId, 3, 1),
('C46E23', 'Automatically bulk send appointment reminders to Patients via SMS messages for COVID-19 vaccination appointments', @capabilityId, 3, 1),
('C46E24', 'Automatically bulk create booking reminders to Patients as letters for COVID-19 vaccination appointments', @capabilityId, 3, 1),
('C46E25', 'Automatically bulk send appointment reminders to Patients via email for COVID-19 vaccination appointments', @capabilityId, 3, 1),
('C46E26', 'Send ad-hoc appointment reminders to Patients for COVID-19 vaccination appointments', @capabilityId, 3, 1),
('C46E27', 'View all booked COVID-19 vaccination appointments for a specified time period', @capabilityId, 3, 1),
('C46E28', 'Export all booked COVID-19 vaccination appointments for a specified time period', @capabilityId, 3, 1),
('C46E29', 'Cancel booked COVID-19 vaccination appointments for Patients', @capabilityId, 3, 1),
('C46E30', 'Re-schedule booked COVID-19 vaccination appointments for Patients', @capabilityId, 3, 1),
('C46E31', 'Automatically send appointment cancellation notifications to Patients via SMS messages for COVID-19 appointments', @capabilityId, 3, 1),
('C46E32', 'Automatically create appointment cancellation notifications to Patients as letters for COVID-19 appointments', @capabilityId, 3, 1),
('C46E33', 'Automatically send appointment cancellation notifications to Patients via email for COVID-19 appointments', @capabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C47');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('C47E1', 'Record structured COVID-19 vaccination data at the point of care for Patients registered at different GP Practices', @capabilityId, 1, 1),
('C47E2', 'Record structured COVID-19 adverse reaction data at the point of care for Patients registered at different GP Practices', @capabilityId, 1, 1),
('C47E3', 'Extract COVID-19 vaccination data for NHS Digital Daily Clinical Vaccination Extract', @capabilityId, 1, 1),
('C47E4', 'Extract COVID-19 adverse reaction data for NHS Digital Daily Clinical Adverse Reaction Extract', @capabilityId, 1, 1),
('C47E5', 'Automatically send vaccination data to Patient’s registered GP Practice Foundation Solution using Digital Medicines FHIR messages', @capabilityId, 1, 1),
('C47E6', 'Automatically send COVID-19 adverse reaction data to Patient’s registered GP Practice Foundation Solution using Digital Medicines FHIR messages', @capabilityId, 1, 1),
('C47E7', 'Automatically send COVID-19 vaccination data to the NHS Business Services Authority (NHSBSA)', @capabilityId, 1, 1),
('C47E8', 'View information from the GP Patient Record using GP Connect Access Record HTML', @capabilityId, 3, 1),
('C47E9', 'View information from the GP Patient Record held by the same Solution', @capabilityId, 3, 1),
('C47E10', 'View Summary Care Record (SCR) for a Patient', @capabilityId, 3, 1),
('C47E11', 'Scanning of a GS1 barcode when recording vaccination data', @capabilityId, 3, 1),
('C47E12', 'Record structured COVID-19 vaccination data at the point of care directly into GP Patient Record', @capabilityId, 3, 1),
('C47E13', 'Record structured COVID-19 adverse reaction data at the point of care directly into GP Patient Record', @capabilityId, 3, 1),
('C47E14', 'Verify Patient information using Personal Demographics Service (PDS)', @capabilityId, 1, 1),
('C47E15', 'Latest COVID-19 Clinical Screening Questions at the point of care for Patients registered at different GP Practices', @capabilityId, 1, 1),
('C47E16', 'Record structured COVID-19 vaccination data at the point of care for Patients using pre-configured vaccine batches', @capabilityId, 1, 1),
('C47E17', 'View vaccination information for a Patient held by the National Immunisation Management Service (NIMS) at point of care', @capabilityId, 1, 1),
('C47E18', 'Update previously recorded structured COVID-19 vaccination and adverse reaction data for Patients', @capabilityId, 1, 1),
('C47E19', 'Extract COVID-19 Extended Attributes data for NHS Digital Extended Attributes Extract', @capabilityId, 1, 1),
('C47E20', 'View reports on COVID-19 vaccination data', @capabilityId, 1, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C43');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('E00001', 'conduct Online Consultation', @CapabilityId, 1, 1),
('E00002', 'conduct Online Consultation with a Proxy', @CapabilityId, 3, 1),
('E00003', 'Patient/Service User requests for Online Consultation support and provides information', @CapabilityId, 3, 1),
('E00004', 'Proxy requests for Online Consultation support and provides information', @CapabilityId, 3, 1),
('E00005', 'respond to Online Consultation requests for support from Patients/Service Users', @CapabilityId, 3, 1),
('E00006', 'respond to Online Consultation requests for support from Proxies', @CapabilityId, 3, 1),
('E00007', 'include attachments in Online Consultation requests', @CapabilityId, 3, 1),
('E00008', 'include attachments in Online Consultation requests from a Proxy', @CapabilityId, 3, 1),
('E00009', 'automated response to Online Consultation requests for support from Patients/Service Users', @CapabilityId, 3, 1),
('E00010', 'automated response to Online Consultation requests for support from Proxies', @CapabilityId, 3, 1),
('E00011', 'Patient/Service User makes an administrative request', @CapabilityId, 3, 1),
('E00012', 'Proxy makes an administrative request', @CapabilityId, 3, 1),
('E00013', 'respond to administrative requests for support from Patients/Service Users', @CapabilityId, 3, 1),
('E00014', 'respond to administrative requests for support from Proxies', @CapabilityId, 3, 1),
('E00015', 'automated responses to administrative requests from Patients/Service Users', @CapabilityId, 3, 1),
('E00016', 'automated responses to administrative requests from Proxies', @CapabilityId, 3, 1),
('E00017', 'link Online Consultation requests for support and responses', @CapabilityId, 3, 1),
('E00018', 'link Online Consultation requests for support from a Proxy and responses', @CapabilityId, 3, 1),
('E00019', 'self-help and signposting', @CapabilityId, 3, 1),
('E00020', 'Proxy supporting self-help and signposting', @CapabilityId, 3, 1),
('E00021', 'symptom checking', @CapabilityId, 3, 1),
('E00022', 'symptom checking by a Proxy', @CapabilityId, 3, 1),
('E00023', 'Direct Messaging', @CapabilityId, 3, 1),
('E00024', 'Direct Messaging by a Proxy', @CapabilityId, 3, 1),
('E00025', 'view the Patient Record during Online Consultation', @CapabilityId, 3, 1),
('E00026', 'electronically share files during Direct Messaging', @CapabilityId, 3, 1),
('E00027', 'electronically share files during Direct Messaging with a Proxy', @CapabilityId, 3, 1),
('E00028', 'customisation of report', @CapabilityId, 3, 1),
('E00029', 'report on utilisation of Online Consultation requests for support', @CapabilityId, 3, 1),
('E00030', 'report on outcomes or dispositions provided to the Patient/Service User', @CapabilityId, 3, 1),
('E00031', 'report on the status of Online Consultations', @CapabilityId, 3, 1),
('E00032', 'report on Patient demographics using Online Consultation', @CapabilityId, 3, 1),
('E00033', 'manually prioritise Online Consultation requests for support', @CapabilityId, 3, 1),
('E00034', 'assign Online Consultation requests to a Health or Care Professional manually', @CapabilityId, 3, 1),
('E00035', 'categorise outcome of Online Consultation requests', @CapabilityId, 3, 1),
('E00037', 'automatically prioritise Online Consultation requests', @CapabilityId, 3, 1),
('E00038', 'assign Online Consultation requests to Health or Care Professional automatically', @CapabilityId, 3, 1),
('E00056', 'disable and enable Direct Messaging for a Healthcare Organisation', @CapabilityId, 3, 1),
('E00057', 'disable and enable Direct Messaging for a Patient/Service User', @CapabilityId, 3, 1),
('E00058', 'disable and enable electronic file sharing during Direct Messaging for a Healthcare Organisation', @CapabilityId, 3, 1),
('E00075', 'Patient/Service User feedback for Online Consultation', @CapabilityId, 3, 1),
('E00076', 'record Online Consultation outcome to the Patient Record', @CapabilityId, 3, 1),
('E00077', 'retain attachments (file and images) in the Patient Record', @CapabilityId, 3, 1),
('E00078', 'Verify Patient/Service User details against Personal Demographics Service (PDS)', @CapabilityId, 3, 1),
('E00079', 'SNOMED code Online Consultation', @CapabilityId, 3, 1),
('E00080', 'customisation of the question sets for Patients/Service Users requesting Online Consultation support', @CapabilityId, 3, 1),
('E00081', 'accessibility options for Online Consultation', @CapabilityId, 3, 1),
('E00082', 'notification to Patients/Service Users', @CapabilityId, 3, 1),
('E00083', 'customisation of instructions to Patients/Service Users using Online Consultation Solution', @CapabilityId, 3, 1),
('E00084', 'categorise administration requests', @CapabilityId, 3, 1),
('E00085', 'disable and enable Direct Messaging for an Online Consultation request for support', @CapabilityId, 3, 1),
('E00086', 'configuration of the triage process', @CapabilityId, 3, 1),
('E00089', 'save the complete record of an Online Consultation to the Patient Record', @CapabilityId, 3, 1),
('E00090', 'Health or Care Professional initiates an Online Consultations request', @CapabilityId, 3, 1),
('E00091', 'Proxy Verification', @CapabilityId, 3, 1);

SET @capabilityId = (SELECT Id FROM Capability WHERE CapabilityRef = 'C44');

INSERT INTO #Epics(Id, [Name], CapabilityId, CompliancyLevelId, Active)
VALUES
('E00039', 'conduct Video Consultation', @CapabilityId, 1, 1),
('E00040', 'conduct Video Consultation with a Proxy', @CapabilityId, 3, 1),
('E00041', 'conduct Video Consultation with the Patient/Service User without registration', @CapabilityId, 3, 1),
('E00042', 'conduct Video Consultation with a Proxy without registration', @CapabilityId, 3, 1),
('E00043', 'end Video Consultation with a Patient/Service User', @CapabilityId, 3, 1),
('E00045', 'Direct Messaging during a Video Consultation', @CapabilityId, 3, 1),
('E00047', 'view the Patient Record during Video Consultation', @CapabilityId, 3, 1),
('E00048', 'conduct group Video Consultations', @CapabilityId, 3, 1),
('E00051', 'electronically share files during a Video Consultation', @CapabilityId, 3, 1),
('E00053', 'Health or Care Professional can share their screen during a Video Consultation', @CapabilityId, 3, 1),
('E00055', 'record Video Consultation outcome to the Patient record ', @CapabilityId, 3, 1),
('E00059', 'Health or Care Professional can record a Video Consultation', @CapabilityId, 3, 1),
('E00060', 'Patient/Service User can record a Video Consultation', @CapabilityId, 3, 1),
('E00061', 'accessibility options', @CapabilityId, 3, 1),
('E00062', 'waiting room', @CapabilityId, 3, 1),
('E00063', 'disable and enable Direct Messaging during a Video Consultation for the Patient/Service User', @CapabilityId, 3, 1),
('E00064', 'record Direct Messages to the Patient Record', @CapabilityId, 3, 1),
('E00065', 'Patient/Service User name is not automatically visible in a group Video Consultation', @CapabilityId, 3, 1),
('E00066', 'invite new participants to an existing Video Consultation with a Patient/Service User', @CapabilityId, 3, 1),
('E00067', 'disable and enable electronic file sharing during a Video Consultation', @CapabilityId, 3, 1),
('E00068', 'disable and enable screen sharing during a Video Consultation', @CapabilityId, 3, 1),
('E00069', 'Patient/Service User feedback on Video Consultations', @CapabilityId, 3, 1),
('E00070', 'test the Video Consultation settings', @CapabilityId, 3, 1),
('E00071', 'consecutive consultations with multiple Patients/Service Users via a single Video Consultation', @CapabilityId, 3, 1),
('E00072', 'reminder of upcoming or scheduled Video Consultation', @CapabilityId, 3, 1),
('E00073', 'disable and enable audio during a group Video Consultation', @CapabilityId, 3, 1),
('E00074', 'disable and enable video during a group Video Consultation', @CapabilityId, 3, 1),
('E00087', 'retain attachments (file and images) received during Video Consultation in the Patient Record', @CapabilityId, 3, 1),
('E00088', 'SNOMED code Video Consultation', @CapabilityId, 3, 1);


MERGE INTO dbo.Epic AS TARGET
USING #Epics AS SOURCE
ON TARGET.Id = SOURCE.Id
WHEN MATCHED THEN
    UPDATE SET TARGET.[Name] = SOURCE.[Name],
               TARGET.CompliancyLevelId = SOURCE.CompliancyLevelId,
               TARGET.Active = SOURCE.Active
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, [Name], CapabilityId, CompliancyLevelId, Active)
    VALUES (SOURCE.Id, SOURCE.[Name], SOURCE.CapabilityId, SOURCE.CompliancyLevelId, Active);

DROP TABLE #Epics;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertPricingUnits.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE TABLE #PricingUnit
(
    PricingUnitId uniqueidentifier NOT NULL,
    [Name] nvarchar(20) NOT NULL,
    TierName nvarchar(30) NOT NULL,
    [Description] nvarchar(40) NOT NULL
);

INSERT INTO #PricingUnit(PricingUnitId, [Name], TierName, [Description])
VALUES
    ('F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 'patient'            , 'patients'       , 'per patient'),
    ('D43C661A-0587-45E1-B315-5E5091D6E9D0', 'bed'                , 'beds'           , 'per bed'),
    ('774E5A1D-D15C-4A37-9990-81861BEAE42B', 'consultation'       , 'consultations'  , 'per consultation'),
    ('8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', 'licence'            , 'licences'       , 'per licence'),
    ('90119522-D381-4296-82EE-8FE630593B56', 'sms'                , 'SMS'            , 'per SMS'),
    ('aad2820e-472d-4bac-864e-853f92e9b3bc', 'practice'           , 'practices'      , 'per practice'),
    ('cc6ee39d-41f1-4671-b31a-800485d05752', 'unit'               , 'units'          , 'per unit'),
    ('9d3bade6-f232-4b6e-9809-88a8fbb5c881', 'group'              , 'groups'         , 'per group'),
    ('599a1105-a16a-4861-b54b-d65da84366c9', 'day'                , 'days'           , 'per day'),
    ('121bd710-b73b-48f9-a429-f269a7bb0bf2', 'halfDay'            , 'half days'      , 'per half day'),
    ('823f814d-82c9-4994-94af-4c413ee418dc', 'hour'               , 'hours'          , 'per hour'),
    ('7e4dd1fd-c953-41a8-9e62-64dc306a6307', 'installation'       , 'installations'  , 'per installation'),
    ('701afb98-699e-4eda-9a66-e79a91769614', 'implementation'     , 'implementations', 'per implementation'),
    ('f2bb1b9d-b546-40b3-bfd9-d55221d96880', 'practiceMergerSplit', 'mergers/splits' , 'per practice merge/split'),
    ('6f65c40f-e7cc-4140-85c5-592dcd216132', 'extract'            , 'extracts'       , 'per extract'),
    ('59fa7cad-87b8-4e78-92b7-5689f6b123dc', 'migration'          , 'migrations'     , 'per migration'),
    ('e17fbd0b-208f-453f-938a-9880bab1ec5e', 'course'             , 'courses'        , 'per course'),
    ('1d40c0d1-6bd5-40b3-ba2f-cf433f339787', 'trainingEnvironment', 'environments'   , 'per training environment'),
    ('4b9a4640-a97a-4e30-8ed5-cccae9829616', 'user'               , 'users'          , 'per user'),
    ('66443acc-7e40-4f95-955b-47234016cff1', 'document'           , 'documents'      , 'per document'),
    ('626b43e6-c9a0-4ede-99ed-da3a1ad2d8d3', 'seminar'            , 'seminars'       , 'per seminar'),
    ('60523726-bbaf-4ec3-b29c-dee2f3d3eca8', 'item'               , 'items'          , 'per item'),
    ('8a5e119f-9b33-4017-8cc9-552e86e20898', 'activeUser'         , 'active users'   , 'per active user'),
    ('84c10564-e85f-4f64-843b-528e88b7bf59', 'virtualPC'          , 'virtual PCs'    , 'per virtual PC'),
    ('7d709183-90ad-4b35-b399-010014bb1b9b', 'transaction'        , 'transactions'   , 'per transaction'),
    ('c74e980c-bb59-4b5a-96ce-1e616bdf827c', 'nurseReview'        , 'nurse reviews'  , 'per nurse review'),
    ('34acc3bf-c036-4125-9eab-23fbe39f6352', 'review'             , 'reviews'        , 'per review'),
    --Units in the data that may be amended / consolidated
    ('4b39590d-3f35-4963-83ba-bc7d0bfe988b', 'videoConsultation', 'consultations', 'per video consultation initiated'),
    ('372787ad-041f-4176-93e9-e4a303c39014', 'digitalFirstConsult', 'consultations', 'per digital first consultation'),
    ('a4012e6c-caf3-430c-b8d3-9c45ab9fd0de', 'unitMerge', 'unit merges', 'per unit merge'),
    ('bede8599-7a4e-4753-a928-f419681b7c93', 'unitSplit', 'unit splits', 'per unit split'),
    ('8eea4a69-977d-4fb1-b4d1-2f0971beb04b', 'hourSession', 'hour sessions', 'per 1 hour session'),
    ('A92C1326-4826-48B3-B429-4A368ADB9785', 'na','',''),
    --New Units in confirmed spreadsheet 27/08/2020
    ('60d07eb0-01ef-44e4-bed3-d34ad1352e19', 'consultationCore'    , 'consultations'                , 'per consultation – core hours'),
    ('93931091-8945-43a0-b181-96f2b41ed3c3', 'consultationExtended', 'consultations'                , 'per consultation – extended hours'),
    ('fec28905-5670-4c45-99f3-1f93c8aa156c', 'consultationOut'     , 'consultations'                , 'per consultation – out of hours'),
    ('9f8888de-69fb-4395-83ce-066d4a4dc120', 'systmOneUnit'        , 'SystmOne units'               , 'per SystmOne unit'),
    ('e72500e5-4cb4-4ddf-a8b8-d898187691ca', 'smsFragment'         , 'SMS fragments'                , 'per SMS fragment'),
    ('1ba99da5-44a8-4dc9-97e7-50c450842191', 'usersPerSite_5'      , 'users per site'               , 'up to 5 users per site'),
    ('bf5b9d2c-d690-41d2-9075-7558ad3f3f1a', 'usersPerSite_10'     , 'users per site'               , 'up to 10 users per site'),
    ('8a7fe8b5-83eb-4d12-939b-53e823ecc624', 'usersPerSite_15'     , 'users per site'               , 'up to 15 users per site'),
    ('b53a9db9-697b-4184-8177-28e9d0f66142', 'usersPerSite_16'     , 'users per site'               , '16+ users per site'),
    ('72f164c0-5eeb-4df1-b3ba-68943c0ae86c', 'traineesPerCourse_5' , 'trainees per course'          , 'per 5 trainees per course'),
    ('f5975c2e-a5fd-40a9-9030-cf02227e60b1', 'consultationRequest' , 'requests for consultation'    , 'per request for consultation'),
    ('2ca3b90b-6073-48c6-9162-acc89c6cd459', 'serviceForSites_5'   , 'sites'                        , 'service for up to 5 sites'),
    ('bbd35a61-8baf-43c2-bb93-7942b99bd004', 'serviceForSites_10'  , 'sites'                        , 'service for up to 10 sites'),
    ('f20469f3-7ef5-4dac-ae17-ad1b7c69e9c2', 'serviceForSites_11'  , 'sites'                        , 'service for 11+ sites'),
    ('e6946b09-28a8-4fb5-af57-12ad9247f850', 'callOff'             , 'Call-offs'                    , 'per Call-off'),
    ('720f2d4d-448d-4899-ad40-979b30911ca6', 'carePlan'            , 'care plan/Call-offs'          , 'per custom care plan/Call-off'),
    ('05281ffc-1077-41d5-a253-3077540ef2e9', 'organisation'        , 'organisations'                , 'per organisation'),
    ('a7eb74d3-2615-4fb5-8083-cabd40ca8cba', 'carePlans_1'         , 'care plans'                   , 'per patient for 1 care plan'),
    ('69329f3d-76ac-46f3-88dc-0ea0409975b8', 'carePlans_2'         , 'care plans'                   , 'per patient for 2–5 care plans'),
    ('a973174d-b4b1-4a28-8ab6-6334fb8159bd', 'carePlans_6'         , 'care plans'                   , 'per patient for 6–10 care plans'),
    ('68e39619-d5b1-4355-845d-5b2b20d4d0d3', 'carePlans_11'        , 'care plans'                   , 'per patient for 11–20 care plans'),
    ('af3c90a8-a8c1-46d9-a6de-d97ec3d6087f', 'carePlans_21'        , 'care plans'                   , 'per patient for 21+ care plans'),
    ('9a9cc023-e799-4a46-892f-6e98f462cd0e', 'service'             , 'services'                     , 'per service'),
    ('2cb9d70f-cd40-4f86-aa63-829f030e63dc', 'patients_0–50k'      , 'patients'                     , 'per patient for 0–49,999 patients'),
    ('5fff29ee-a360-4077-b712-73abff3a7f0b', 'patients_50–500k'    , 'patients'                     , 'per patient for 50,000–499,999 patients'),
    ('79b62a1b-8e86-4be5-95e6-c19aa65af4d4', 'patients_500k+'      , 'patients'                     , 'per patient for 500,000+ patients'),
    ('d96142d4-2190-43b4-83a1-1ad8ffc66532', 'additionalPractice'  , 'additional practices'         , 'per additional practice'),
    ('0b8b296e-3d5a-4fd2-8614-fd3df220b394', 'incomingPractice'    , 'incoming practices'           , 'per incoming practice'),
    ('cba9431d-115b-4c62-b0e5-bf11aa82dbd0', 'groupMigration'      , 'group migrations'             , 'per group migration'),
    ('d29a3db3-5426-44f4-9dc6-4569f4561958', 'session'             , 'sessions'                     , 'per session'),
    ('11ecd056-e2ac-45a7-bbf8-a274e0ca8320', 'system'              , 'systems'                      , 'per system'),
    ('fb3b6d1b-78fb-4733-a6cb-6d18582e273e', 'keystoneCapability'  , 'Keystone capabilities'        , 'per Keystone capability'),
    ('8a7683b3-e39a-4f44-b387-1f2f0e33f0d7', 'clinicalUser'        , 'concurrent clinical users'    , 'per concurrent clinical user'),        
    ('aef635b7-1f26-4c4d-b99c-bcf97bae5b55', 'letter'              , 'letters'                      , 'per letter'),
    ('aa6499fd-755c-4872-b1ff-f3db8deb1e14', 'agentPhoneCall'      , 'agent phone calls'            , 'per agent phone call'),
    ('8a8ee619-9980-458c-979d-9f6d968e8806', 'email'               , 'e-mails'                      , 'per e-mail'),
    ('12b53ce9-fdc0-48b5-a07d-d95bdd7220c7', 'smsWebEmail_500'     , 'responses per site'           , '500 SMS/web/email responses per site'),
    ('02438278-ab93-4689-b123-7ac4e78f59fe', 'smsWebEmail_1000'    , 'responses per site'           , '1000 SMS/web/email responses per site'),
    ('67e1c174-8443-4883-b51f-43c297fa9c08', 'smsWebEmail_1500'    , 'responses per site'           , '1500 SMS/web/email responses per site'),
    ('1a36f980-76a4-4b3d-b36d-46afc6655124', 'dataMigration'       , 'data migrations'              , 'per data migration'),
    ('8413d5e1-2995-4885-9ffa-ec961d82aa6d', 'sharedService'       , 'shared services'              , 'per shared service'),
    ('f24afc45-971d-41fa-9beb-6f577e987c6d', 'patients_1–20k'      , 'patients'                     , 'per patient for 1–19,999 patients'),
    ('0fb82859-aa6c-4db5-b513-c776c7278aca', 'patients_20–150k'    , 'patients'                     , 'per patient 20,000–149,999 patients'),
    ('7b2c46d4-e5c1-4315-a05e-021e43d485ee', 'patients_150–250k'   , 'patients'                     , 'per patient 150,000–249,999 patients'),
    ('d37f63e0-0bb2-4703-9f86-f22057231a00', 'patients_250k'       , 'patients'                     , 'per patient 250,000 patients & above'),
    ('950365df-456d-4c72-bc6f-74a0f7535728', 'patientRegistered'   , 'patients'                     , 'per registered patient'),
    ('2c25c251-168d-4f14-a637-3d09fe7440b7', 'onlineConsultationGP', 'consultations'                , 'per online consultation non-GP practices'),
    ('edb80481-d99a-4d25-8d58-da6881b453f7', 'videoConsultationGP' , 'consultations'                , 'per video consultation non-GP practices'),
    ('a2441010-cfd0-4a0a-bbf7-e73e279b4298', 'referralEps_1–5k'    , 'referral episodes'            , 'per referral episode 1–5000 episodes'),
    ('c4bfbaf0-f311-47d2-b02c-7ee76b9f779f', 'referralEps_5–20k'   , 'referral episodes'            , 'per referral 5001–20,000 episodes'),
    ('2f59e518-e211-44bb-b9b3-0fc518f4f46c', 'referralEps_20–50k'  , 'referral episodes'            , 'per referral 20,001–50,000 episodes'),
    ('5c863dc7-b17f-43aa-997a-e5d1226525d8', 'referralEps_50k'     , 'referral episodes'            , 'per referral 50,0001 or more episodes'),
    ('3d49fdd4-a949-40ab-adea-f541a6dbe034', 'trainingSession_360' , 'training sessions'            , 'per 4 x 90 min online training sessions'),
    ('035c6c00-773b-47f7-af61-4eb60a1ce639', 'language'            , 'languages'                    , 'per language'),
    ('2231d5e7-c9a8-4c77-ae35-ca9d5cc5d79a', 'solution'            , 'solutions'                    , 'per solution'),
    ('1783eb6d-dbc0-4a00-bc4f-e9ce9331632c', 'pcn'                 , 'pcn'                          , 'per PCN'),
    ('088655c4-83cb-40b6-a996-1f3eb9056fcd', 'call_1min'           , 'calls'                        , 'per 1 minute/part of a 1 min of call'),
    ('4c791800-02a4-4e34-a6a5-575c08f37ad2', 'trainingSession_90'  , 'training sessions'            , 'per 90 min online training session'),
    ('d88a3485-e2bc-4dc5-bb75-1ffb6df8a31d', 'smsSent'             , 'sms'                          , 'per SMS sent'),
    ('4a202b19-2cd0-4650-b4f7-b89e00762168', 'smsText'             , 'sms'                          , 'per SMS text'),
    ('ccd04c97-e688-4a3b-804b-1a1dbbb4f343', 'practiceSite'        , 'practices/sites'              , 'per practice/site'),
    ('caaa4d47-58fe-4a82-8cfd-fa90a5f7dc30', 'patients_50–150k'    , 'patients'                     , 'per patient for 50,000–149,999 patients'),
    ('03259d19-4ccf-4bfe-8c90-884d32f351bb', 'patients_150–500k'   , 'patients'                     , 'per patient for 150,000–499,999 patients'),
    ('53ed1ded-6a01-4e84-a02a-d90608948082', 'patients_1–90k'      , 'patients'                     , 'per patient for 1–89,999 patients'),
    ('bbb6622c-fd88-459f-9015-34c8bbca76ba', 'patients_90–900k'    , 'patients'                     , 'per patient 90,000–899,999 patients'),
    ('0d592388-3d61-4f9d-9e11-6b0f5858acc0', 'patients_899k'       , 'patients'                     , 'per patient 899,000 patients and above'),
    ('2891ed7e-64bb-4ed6-8108-95c1fad9e2ee', 'patientUser'         , 'patient users'                , 'per patient user'),
    ('a10ae1df-b8a1-4f8d-aeb2-cd636e24d966', 'minuteUser'          , 'minutes/users'                , 'per minute per end user'),
    ('8175143c-3350-4186-954c-d98172a7dfb0', 'gigabyte'            , 'gigabytes'                    , 'per gigabyte'),
    ('d9a7efe6-11a6-474a-99a1-03cd5909408c', 'pack_10k'            , 'packs'                        , 'per pack (10,000 mins/texts)'),
    ('a55a2ce5-a5a9-4ddf-a4a0-2cfa3d89d021', 'patients_oc'         , 'patients'                     , 'per patient for OC'),
    ('87bb2f80-6b4c-412b-996e-77ac04c2abbf', 'patients_vc'         , 'patients'                     , 'per patient for VC'),
    ('62803784-227c-442b-8e90-2d47034de45e', 'singleCharge_1'      , 'single charges'               , 'single charge for band 1'),
    ('ada6a7c3-2989-4cb1-b413-6232aff92e99', 'singleCharge_2'      , 'single charges'               , 'single charge for band 2'),
    ('2945185f-9a30-42c9-a2e6-d61f8ade1cc0', 'singleCharge_3'      , 'single charges'               , 'single charge for band 3'),
    ('163f1b70-f45d-4b2f-ad89-575bc1def629', 'singleCharge_4'      , 'single charges'               , 'single charge for band 4'),
    ('8e360df0-deed-4546-a022-c101e7f31a2b', 'singleCharge_5'      , 'single charges'               , 'single charge for band 5'),
    ('7ae52815-8697-4c5c-8d0a-dd8e18b71dfc', 'activeUsers_100'     , 'active users'                 , 'Up to 100 active users'),
    ('99f37c97-0add-488f-8a24-8f5f97389c91', 'activeUsers_250'     , 'active users'                 , 'Up to 250 active users'),
    ('701d3650-1334-4c61-b4db-03d351b6a49c', 'activeUsers_500'     , 'active users'                 , 'Up to 500 active users'),
    ('644bbe2f-9ce6-4f8f-a53b-e1ea43096f88', 'activeUsers_1k'      , 'active users'                 , 'Up to 1000 active users'),
    ('5a7098f5-db63-4d95-82b1-570035251c18', 'activeUsers_2k'      , 'active users'                 , 'Up to 2000 active users');

MERGE INTO dbo.PricingUnit AS TARGET
USING #PricingUnit AS SOURCE
ON TARGET.PricingUnitId = SOURCE.PricingUnitId
WHEN MATCHED THEN
    UPDATE SET TARGET.[Name] = SOURCE.[Name],
               TARGET.TierName = SOURCE.TierName,
               TARGET.[Description] = SOURCE.[Description]
WHEN NOT MATCHED BY TARGET THEN
    INSERT (PricingUnitId, [Name], TierName, [Description])
    VALUES (SOURCE.PricingUnitId, SOURCE.[Name], SOURCE.TierName, SOURCE.[Description]);

DROP TABLE #PricingUnit;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertProvisioningTypes.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM dbo.ProvisioningType)
    INSERT INTO dbo.ProvisioningType(ProvisioningTypeId, [Name])
    VALUES
    (1, 'Patient'),
    (2, 'Declarative'),
    (3, 'OnDemand');
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertSuppliers.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @emptyGuid AS uniqueidentifier = '00000000-0000-0000-0000-000000000000';
DECLARE @now AS datetime = GETUTCDATE();

IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND NOT EXISTS (SELECT * FROM dbo.Supplier)
BEGIN
    INSERT INTO dbo.Supplier(Id, [Name], LegalName, Summary, [Address], LastUpdated, LastUpdatedBy)
    VALUES (
        '100000',
        'Really Kool Corporation',
        'Really Kool Corporation',
        'Really Kool Corporation is a fictious UK based IT company but that''s not going to stop us making Really Kool products!',
        '{"line1": "The Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',
        @now,
        @emptyGuid);

    INSERT INTO dbo.Supplier(Id, [Name], LegalName, Summary, [Address], LastUpdated, LastUpdatedBy)
    VALUES
    ('100001', 'Remedical Software', 'Remedical Limited', 'The Remedical Software', '{"line1": "Remedical Software Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100002', 'CareShare', 'CareShare Limited', 'The CareShare', '{"line1": "CareShare Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100003', 'Avatar Solutions', 'Avatar Solutions Plc', 'Avatar Solutions', '{"line1": "Avatar Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100004', 'Catterpillar Medworks', 'Catterpillar Medworks Ltd', 'Catterpillar Medworks', '{"line1": "Medworks Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100005', 'Curtis Systems', 'Curtis Systems Ltd', 'Curtis Systems', '{"line1": "Curtis Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100006', 'Clinical Raptor', 'Clinical Raptor Ltd', 'Clinical Raptor', '{"line1": "Raptor Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100007', 'Doc Lightning', 'Doc Lightning Ltd', 'Doc Lightning', '{"line1": "Doc Lightning Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100008', 'Docability Software', 'Docability Ltd', 'Docability Software', '{"line1": "Docability Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100009', 'Empire Softworks',  'Empire Softworks Plc', 'Empire Softworks', '{"line1": "Empire Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100010', 'Cure Forward', 'Cure Forward Ltd', 'Cure Forward', '{"line1": "Cure Forward Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100011', 'Hansa Healthcare', 'Hansa Healthcare Plc', 'Hansa Healthcare', '{"line1": "Hansa Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100012', 'Moonlight Intercare', 'Moonlight Intercare', 'Moonlight Intercare', '{"line1": "Moonlight Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100013', 'eHealth Development', 'eHealth Development', 'eHealth Development', '{"line1": "eHealth Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100014', 'Dr. Nick', 'Dr. Nick', 'Dr. Nick', '{"line1": "Simpson Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100015', 'Testproof Technology',  'Testproof Technology', 'Testproof Technology', '{"line1": "Testproof Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100016', 'Hojo Health', 'Hojo Health Ltd', 'Hojo Health', '{"line1": "Hojo Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100017', 'Jericho Healthcare', 'Jericho Ltd', 'Jericho Healthcare', '{"line1": "Jericho Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100018', 'Mana Systems', 'Mana Systems', 'Mana Systems', '{"line1": "Mana Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100019', 'Sunhealth Nanosystems', 'Sunhealth Nanosystems', 'Sunhealth Nanosystems', '{"line1": "Sunhealth Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100020', 'Oakwood', 'Oakwood Ltd', 'Oakwood', '{"line1": "Oakwood Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid);

    INSERT INTO dbo.Supplier(Id, [Name], LegalName, Summary, SupplierUrl, [Address], LastUpdated, LastUpdatedBy)
    VALUES
    (
        '99999',
        'NotEmis Health',
        'NotEgton Medical Information Systems',
        'We are the UK leader in connected healthcare software & services. Through innovative IT we help healthcare professionals access the information they need to provide better, faster and more cost effective patient care.

    Our clinical software is used in all major healthcare settings from GP surgeries to pharmacies, community, hospitals, and specialist services. By providing innovative, integrated solutions, we’re working to break the boundaries of system integration & interoperability.

    We also specialise in supplying IT infrastructure, software and engineering services, and through our technical support teams we have the skills and knowledge to enhance your IT systems.

    Patient (www.patient.info) is the UK’s leading health website. Designed to help patients play a key role in their own care, it provides access to clinically authored health information leaflets, videos, health check and assessment tools and patient forums.

    TRUNCATED FOR DEMO',
        'www.emishealth.com',
        '{"line1": "NotEmis Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',
        @now,
        @emptyGuid),
    (
        '99998',
        'NotTPP',
        'NotThe Phoenix Partnership',
        'TPP is a digital health company, committed to delivering world-class healthcare software around the world. Its EHR product, SystmOne, is used by over 7,000 NHS organisations in over 25 different care settings. This includes significant deployments in Acute Hospitals, Emergency Departments, Mental Health services, Social Care services and General Practice. In recent years, TPP has increased its international presence, with live deployments in China and across the Middle East.',
        'https://www.tpp-uk.com/',
        '{"line1": "NotTPP Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',
        @now,
        @emptyGuid);


    INSERT INTO dbo.SupplierContact (Id, SupplierId, FirstName, LastName, Email, PhoneNumber, LastUpdated, LastUpdatedBy)
    VALUES
    (NEWID(),'100000', 'Tim',      'Teabag',    'timtea@test.test',       '01234 567891',     @now, @emptyGuid),
    (NEWID(),'100001', 'Kim',      'Samosa',    'kimsam@test.test',       '01234 567891',     @now, @emptyGuid),
    (NEWID(),'100002', 'Boston',   'Sponge',    'bosponge@test.test',     '01234 567891',     @now, @emptyGuid),
    (NEWID(),'100003', 'Betty',    'Sponge',    'betsponge@test.test',    '01234 567891',     @now, @emptyGuid),
    (NEWID(),'100004', 'Eduardo',  'Eggbert',   'eduegg@test.test',       '01234 567891',     @now, @emptyGuid),
    (NEWID(),'100005', 'Sam',      'Samosa',    'sammosa@test.test',      '01234 567891',     @now, @emptyGuid),
    (NEWID(),'100006', 'Harry',    'Samosa',    'harsam@test.test',       '01234 567891',     @now, @emptyGuid),
    (NEWID(),'100007', 'Agent',    'Banjo',     'agbanj@test.test',       '01234 567891',     @now, @emptyGuid),
    (NEWID(),'100008', 'Pat',      'Sponge',    'patsponge@sponge.test',  '+44(0)1234 567891',@now, @emptyGuid),
    (NEWID(),'100009', 'Richard',  'Teabag',    'richtbag@teabag.test',   '01234 567891',     @now, @emptyGuid),
    (NEWID(),'100010', 'Harry',    'Banjo',     'harbanjo@banjo.test',    '01234 567891',     @now, @emptyGuid),
    (NEWID(),'100011', 'Timothy',  'Sponge',    'timsponge@test.test',    '01234 567891',     @now, @emptyGuid),
    (NEWID(),'100012', 'Oscar',    'Banjo',     'oscarrrds@banjo.test',   '+44(0)1234 567891',@now, @emptyGuid),
    (NEWID(),'100013', 'Victoria', 'Teabag',    'victea@test.test',       '+44 1234 567891',  @now, @emptyGuid),
    (NEWID(),'100014', 'Boston',   'Banjo',     'bosbanjo@test.test',     '+44 1234 567891',  @now, @emptyGuid),
    (NEWID(),'100015', 'Betty',    'Eggbert',   'betegg@test.test',       '+44 1234 567891',  @now, @emptyGuid),
    (NEWID(),'100016', 'Eduardo',  'Butcher',   'edu.butcher@test.test',  '+44 1234 567891',  @now, @emptyGuid),
    (NEWID(),'100017', 'Harry',    'Teabag',    'hartea@test.test',       '+44 1234 567891',  @now, @emptyGuid),
    (NEWID(),'100018', 'Richard',  'Samosa',    'richsam@test.test',      '+44 1234 567891',  @now, @emptyGuid),
    (NEWID(),'100019', 'Timothy',  'Teabag',    'timothy@test.test',      '+44 1234 567891',  @now, @emptyGuid),
    (NEWID(),'100020', 'Victoria', 'Sponge',    'victsponge@test.test',   '+44 1234 567891',  @now, @emptyGuid),
    (NEWID(),'99998',  'Richard',  'Eggbert',   'ricegg@test.test',       '+44 1234 567891',  @now, @emptyGuid),
    (NEWID(),'99999',  'Agent',    'Teabag',    'teagen@test.test',       '+44 1234 567891',  @now, @emptyGuid);
END;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertTimeUnits.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM dbo.TimeUnit)
    INSERT INTO dbo.TimeUnit(TimeUnitId, [Name], [Description])
    VALUES
    (1, 'month', 'per month'),
    (2, 'year', 'per year');
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertSolutions.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @citizenAppointmentManagementCapabilityId AS uniqueidentifier = '21ae013d-42a4-4748-b435-73d5887944c2';
DECLARE @citizenCommunicateCapabilityId AS uniqueidentifier = '4f09e77b-e3a3-4a25-8ec1-815921f83628';
DECLARE @citizenViewRecordCapabilityId AS uniqueidentifier = '64e5986d-1ebf-4df0-8219-c150c082ca7b';
DECLARE @clinicalDecisionSupportCapabilityId AS uniqueidentifier = 'a71f2be1-6395-4db7-828c-d4733b42b5b5';
DECLARE @communicationManagementCapabilityId AS uniqueidentifier = '0a372f63-add4-4529-a6cd-4437c6ef115b';
DECLARE @dataAnalyticsCapabilityId AS uniqueidentifier = '5db79ff4-fa9c-4da2-bbfc-8ca40fec0b43';
DECLARE @gpAppointmentManagementCapabilityId AS uniqueidentifier = 'efd93d25-447b-4ca3-9d78-108d42afeae0';
DECLARE @gpExtractVerificationCapabilityId AS uniqueidentifier = '9d805aad-d43a-480e-9bc0-41a755bafe2f';
DECLARE @medicineOptimizationCapabilityId AS uniqueidentifier = '8bee1ff3-84d4-430b-a678-336f57c57387';
DECLARE @patientInformationMaintenanceCapabilityId AS uniqueidentifier = '8c384983-774a-45bd-9d4e-6b3c7d3b7323';
DECLARE @prescribingCapabilityId AS uniqueidentifier = 'b3f89711-6bd7-42d7-be5b-bae2f239ebdd';
DECLARE @presciptionOrderingCapabilityId AS uniqueidentifier = '60c2f5b0-b950-44c8-a246-099335a1c816';
DECLARE @recordingConsultationCapabilityId AS uniqueidentifier = '9442dcc4-22df-494b-8672-b7b4dd077496';
DECLARE @productivityCapabilityId AS uniqueidentifier = '6e77147d-d2af-46bd-a2f2-bb4f235daf3a';
DECLARE @referralManagementCapabilityId AS uniqueidentifier = '20b09859-6fc2-404c-b7a4-3830790e63ab';
DECLARE @reportingCapabilityId AS uniqueidentifier = 'dd649cc4-a710-4472-98b3-663d9d12a8b7';
DECLARE @resourceManagementCapabilityId AS uniqueidentifier = 'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e';
DECLARE @scanningCapabilityId AS uniqueidentifier = 'e5521a71-a28e-4bc9-bddf-599f0a90719d';
DECLARE @sharedCarePlansCapabilityId AS uniqueidentifier = 'd1532ca0-ef0c-457c-9cfc-affa0fbdf134';
DECLARE @unifiedCareRecordCapabilityId AS uniqueidentifier = '59696227-602a-421d-a883-29e88997ac17';
DECLARE @workflowCapabilityId AS uniqueidentifier = '9d325dec-6e5b-44e4-876b-eacf6cd41b3e';

DECLARE @gpitframeworkId AS nvarchar(10) = 'NHSDGP001';
DECLARE @dfocvcframeworkId AS nvarchar(10) = 'DFOCVC001';
DECLARE @emptyGuid AS uniqueidentifier = '00000000-0000-0000-0000-000000000000';
DECLARE @now AS datetime = GETUTCDATE();

DECLARE @publishedStatus AS int = 3;
DECLARE @solutionItemType AS int = 1;
DECLARE @version1 AS nvarchar(10) = '1.0.0';

DECLARE @purchaseModelId AS uniqueIdentifier;
DECLARE @solutionId AS nvarchar(14);

IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND NOT EXISTS (SELECT * FROM dbo.Solution)
BEGIN
    SET @solutionId = '100000-001';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Write on Time', '100000', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Flexible Pricing", "Lightweight interface designed for maximum usability", "DNA tracking and automatic improvement suggestions", "Web-based", "Remotely accessible"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.writeontime.com/about',
            'Write on Time is a Citizen-facing Appointments Management system specifically designed to reduce the number of DNAs in your practice.',
            'FULL DESCRIPTION – Write on Time is a Citizen-facing Appointments Management system specifically designed to reduce the number of DNAs in your practice.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Pat', 'Butcher', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef = 'C1';

        INSERT INTO dbo.FrameworkSolutions(FrameworkId ,SolutionId ,IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@gpitframeworkId, @solutionId , 1, @now, @emptyGuid);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100001-001';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Appointment Gateway', '100001', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Tested and approved by hundred''s of GPs", "99.9% service availability guaranteed", "Appointment forwarding & referral integration", "Fully interopable with all major GP IT solutions", "Compliant with all relevant ISO standards"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.appointmentgateway.com/about',
            'Appointment Gateway is a complete appointment management suite that has been fully integrated with all major clinical systems throughout both the UK and Europe.',
            'FULL DESCRIPTION – Appointment Gateway is a complete appointment management suite that has been fully integrated with all major clinical systems throughout both the UK and Europe.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Sam', 'Samosa', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef IN ('C1', 'C5');

        INSERT INTO dbo.FrameworkSolutions(FrameworkId, SolutionId, IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@gpitframeworkId, @solutionId, 1, @now, @emptyGuid);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100002-001';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Zen Guidance', '100002', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Advanced AI functionality", "MESH & FHIR compliant", "Remotely accessible ", "Cloud-hosted", "24/7 customer support"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.zenguidance.com/about',
            'Zen Guidance utilizes an advanced AI framework to provide clinicians with highly accurate data to support sound decision-making.',
            'FULL DESCRIPTION – Zen Guidance utilizes an advanced AI framework to provide clinicians with highly accurate data to support sound decision-making.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Victoria', 'Sponge', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability (SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef = 'C6';
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100003-001';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Intellidoc Comms', '100003', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Efficient instant & scheduled messaging", "Web-based interface", "Compliant with all relevant ISO standards", "Wide range of add-ons available", "Cloud-hosted"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.intellidoc.com/about',
            'Intellidoc Comms empowers all practice staff to record & send communications in an extremely streamlined and time-efficient manner.',
            'FULL DESCRIPTION – Intellidoc Comms empowers all practice staff to record & send communications in an extremely streamlined and time-efficient manner.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Richard', 'Burton', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef IN ('C7', 'C15');
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100004-001';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Diagnostics XYZ', '100004', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Seamless integration with a wide range of diagnostic hardware", "Demo & free trial available", "FHIR compliant", "Plug and play – minimal deployment activity required", "Optimized for touchscreen & desktop use"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.diagnostics.xyz/about',
            'Diagnostics XYZ introduces new diagnostic tools not currently provided by the leading clinical software suppliers.',
            'FULL DESCRIPTION – Diagnostics XYZ introduces new diagnostic tools not currently provided by the leading clinical software suppliers.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Harry', 'Houdini', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef = 'C8';

        INSERT INTO dbo.FrameworkSolutions(FrameworkId ,SolutionId ,IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@gpitframeworkId, @solutionId , 1, @now, @emptyGuid);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100005-001';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Document Wizard', '100005', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Industry-leading data extraction & scanning accuracy", "Fully interopable with all major GP IT solutions", "24/7 customer support", "Fully Compliant with all relevant ISO standards", "Modular architecture to enhance compatibility and customisation"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.documentwizard.com/about',
            'Document Wizard is the UK industry-leader for clinical document management software due to our patented lightweight interface and interoperability.',
            'FULL DESCRIPTION – Document Wizard is the UK industry-leader for clinical document management software due to our patented lightweight interface and interoperability.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Agent', 'M', '01234 567891', 'm@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef IN ('C9', 'C19', 'C41');
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100006-001';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Paperlite', '100006', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Revolutionary optical character recognition technology", "Can be deployed quickly at low-cost", "Web-based interface", "Cloud-hosted", "Wide range of add-ons available"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.paperlite.com/about',
            'Paperlite utilises new OCR technology to seamlessly transfer written notes to digital patient records.',
            'FULL DESCRIPTION – Paperlite utilises new OCR technology to seamlessly transfer written notes to digital patient records.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES ('100006-001', 'Timothy', 'Teabag', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT '100006-001', Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef IN ('C9', 'C17');
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100007-001';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Medsort', '100007', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Fully adaptable to suit your practice''s needs", "Integrates with Spine", "FHIR compliant", "Flexible Pricing", "24/7 customer support"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.medsort.com/about',
            'Medsort enhances your medicine optimisation process and introduces new, more customisable tools that can be adapted to your local environment.',
            'FULL DESCRIPTION – Medsort enhances your medicine optimisation process and introduces new, more customisable tools that can be adapted to your local environment.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Betty', 'Banjo', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef = 'C30';
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100007-002';
    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'BostonDynamics', '100007', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Fully adaptable to suit your practice''s needs", "Integrates with Spine", "FHIR compliant", "Flexible Pricing", "24/7 customer support"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.bostondynamics.com/about',
            'BostonDynamics enhances your medicine optimisation process and introduces new, more customisable tools that can be adapted to your local environment.',
            'FULL DESCRIPTION – BostonDynamics enhances your medicine optimisation process and introduces new, more customisable tools that can be adapted to your local environment.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Boston', 'Rocks', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef = 'C30';
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '99999-89';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'NotEmis Web GP', '99999', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version],
                    Features, ClientApplication, Hosting, ImplementationDetail, RoadMap,
                    IntegrationsUrl, Summary, FullDescription,
                    LastUpdated, LastUpdatedBy)
        VALUES
        (
            @solutionId,
            @version1,
            '["Access to real-time patient data that can be shared between locations and healthcare organisations","One-click access to patient summary information","Quick data entry using protocols, templates and concepts tailored to your practice requirements","Integrated clinical safety alerts graded to highlight severity","Automatic notification of linked pre-existing conditions when recording a new acute problem","Integrated patient recall system to target specific lists of patients for specific clinics","Intelligent alerts and auto-templates to capture outstanding QOF information","Integrated QOF-finder to identify patients where you are losing QOF points","Seamless data exchange with over 100 partners including Graphnet, Cerner and Optum","Integration with Patient Access enables patients to book appointments and order prescriptions"]',
            '{"ClientApplicationTypes":["browser-based","native-mobile","native-desktop"],"BrowsersSupported":["Google Chrome","Chromium","Internet Explorer 11","Internet Explorer 10"],"MobileResponsive":true,"Plugins":{"Required":false,"AdditionalInformation":""},"MinimumConnectionSpeed":"2Mbps","MinimumDesktopResolution":"16:9 – 1366 x 768","HardwareRequirements":"The browser activities are only supported in relation the native desktop client therefore mirror the native desktop client hardware requirements detailed below.","NativeMobileHardwareRequirements":"Any device capable of supporting the listed supported operating systems is compliant.","NativeDesktopHardwareRequirements":"The spoke server is an important part of the solution. It provides a patch distribution system for client updates and acts as a local cache. \r\nEMIS Health recommends that your spoke is a dedicated device; however, if you use your spoke to perform other functions, such as act as a domain controller, store business documents or host other applications, then a Windows server class operating system will be required, along with an appropriate specification of server hardware.","AdditionalInformation":"","MobileFirstDesign":false,"NativeMobileFirstDesign":false,"MobileOperatingSystems":{"OperatingSystems":["Apple IOS","Android","Other"],"OperatingSystemsDescription":"•\tiOS v 10.3.3.3 and above\r\n•\tAndroid v 6 and above\r\n•\tWindows 10 (Build 14393)"},"MobileConnectionDetails":{"MinimumConnectionSpeed":"","ConnectionType":["GPRS","3G","LTE","4G","5G","Wifi"],"Description":"The mobile application only requires internet connectivity to synchronize therefore there is no minimum connection speed required."},"MobileMemoryAndStorage":{"MinimumMemoryRequirement":"2GB","Description":"All compliant devices have a minimum 16GB storage"},"MobileThirdParty":{"ThirdPartyComponents":"","DeviceCapabilities":"The device should have access to the relevant App Store to enable the installation of the respective application although deployment via mobile device management solutions is supported."},"NativeMobileAdditionalInformation":"Apple have recently announced that a new operating system, designed specifically for iPad devices.\r\nWe have tested this and can confirm that EMIS Mobile is fully compatible.","NativeDesktopOperatingSystemsDescription":"Microsoft Windows 7 (x86 x64)\r\nMicrosoft Windows 8.1 (x86 x64)\r\nMicrosoft Windows 10 (x86 x64)","NativeDesktopMinimumConnectionSpeed":"2Mbps","NativeDesktopThirdParty":{"ThirdPartyComponents":".NET framework 4","DeviceCapabilities":"The application requires connectivity to the EMIS Data Centre."},"NativeDesktopMemoryAndStorage":{"MinimumMemoryRequirement":"4GB","StorageRequirementsDescription":"10GB free disk space","MinimumCpu":"Intel Core i3 equivalent or higher","RecommendedResolution":"16:9 – 1366 x 768"},"NativeDesktopAdditionalInformation":"The minimum connection speed is dependent on the number of clients that need to be supported.\r\n\r\nEMISHealth do not support the use of on-screen keyboards for 2 in 1 devices."}',
            '{"PublicCloud":{"Summary":"This service is not available on public cloud","Link":""},"PrivateCloud":{"Summary":"EMIS Web is hosted in EMIS’ own data centres and the solution is provided as Software as a Service","Link":"","HostingModel":"Model complies with GPIT Futures requirements for hosting","RequiresHSCN":"End user devices must be connected to HSCN/N3"}}',
            'The typical timescales for the implementation of EMIS Web are around 12 weeks depending on the patient list size.

An implementation plan is provided and agreed at the beginning of the process outlining all key dates and activities, where required these dates are negotiable to fit the needs of the customer.

Key activities:

•	Customer supplied with high level implementation plan and welcome pack

•	Engineer visit to perform install of client software and check connectivity

•	Customer supplied test data loaded into a test system and made available

•	Learning needs analysis performed & agreed training plan for go live

•	On site visit to train the customer how to check and cleanse their data

•	Any defects and corrections completed on migrated data

•	Practice sign off of test data

•	Agreed training provide pre and post go live in line with results from LNA

•	Go live day, trainers and engineer onsite to support a smooth transition',
'The following roadmap details all IT Futures managed capacity items. EMIS Health is committed to delivering against the effective date. The roadmap provides visibility on which items have been completed, are scheduled and are in the pipeline to be scheduled.',
'https://www.emishealth.com/products/partner-products/',
        'EMIS Web is the most widely used GP clinical system in the UK. Created by clinicians for clinicians, it helps run efficient practices, whilst delivering the best possible patient care. With patient safety at its core, EMIS Web enables you to deliver safe and informed on demand care across locations.',
        'EMIS Web GP is a comprehensive, flexible and powerful clinical management tool for healthcare organisations. It is intuitive, easy to learn and was ranked the number one clinical system in the UK for interoperability (KLAS report).

EMIS Web GP is fully accredited to securely exchange data with the NHS SPINE and incorporates functionality such as GP2GP, Electronic Prescription Service (EPS), Personal Demographics Service (PDS), NHS e-Referral Service (e-RS) , Summary Care Record (SCR), eMED3 (Fitnotes), SNOMED CT and Female genital mutilation (FGM).

Using EMIS Web, healthcare professionals can provide the best possible patient care with patient safety at its core. We safely and securely hold more patient records than any other supplier and work with clinicians and pharmacists to ensure the highest possible standards of patient safety are upheld. The system provides secure access to all the information they need to make the right decisions for their patients.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, Email, PhoneNumber, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Eduardo', 'Eggbert', 'eddie@eggs.test', '01234 567891', 'Internal Sales Team', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability (SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
        VALUES
        (@solutionId, @gpAppointmentManagementCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @referralManagementCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @gpExtractVerificationCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @scanningCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @reportingCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @productivityCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @clinicalDecisionSupportCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 1, @now, @emptyGuid);

        INSERT INTO dbo.FrameworkSolutions(FrameworkId ,SolutionId ,IsFoundation, LastUpdated ,LastUpdatedBy)
             VALUES (@gpitframeworkId, @solutionId , 1, @now, @emptyGuid);

        INSERT INTO dbo.SolutionEpic (SolutionId, CapabilityId, EpicId, StatusId, LastUpdated, LastUpdatedBy)
        VALUES
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E1', 1, @now, @emptyGuid),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E2', 1, @now, @emptyGuid),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E3', 1, @now, @emptyGuid),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E4', 1, @now, @emptyGuid),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E5', 1, @now, @emptyGuid),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E6', 1, @now, @emptyGuid),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E7', 1, @now, @emptyGuid),
        (@solutionId, @clinicalDecisionSupportCapabilityId, 'C6E1', 1, @now, @emptyGuid),
        (@solutionId, @clinicalDecisionSupportCapabilityId, 'C6E2', 3, @now, @emptyGuid),
        (@solutionId, @clinicalDecisionSupportCapabilityId, 'C6E3', 2, @now, @emptyGuid),
        (@solutionId, @clinicalDecisionSupportCapabilityId, 'C6E4', 2, @now, @emptyGuid),
        (@solutionId, @gpExtractVerificationCapabilityId, 'C10E1', 1, @now, @emptyGuid),
        (@solutionId, @gpExtractVerificationCapabilityId, 'C10E2', 2, @now, @emptyGuid),
        (@solutionId, @referralManagementCapabilityId, 'C11E1', 1, @now, @emptyGuid),
        (@solutionId, @referralManagementCapabilityId, 'C11E2', 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 'C12E1', 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 'C12E2', 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 'C12E3', 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 'C12E4', 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 'C12E5', 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 'C12E6', 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 'C12E7', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E1', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E2', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E3', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E4', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E5', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E6', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E7', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E8', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E9', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E10', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E11', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E12', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E13', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E14', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E15', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E16', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E17', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E18', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E19', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E20', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E21', 2, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E1', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E2', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E3', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E4', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E5', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E6', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E7', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E8', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E9', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E10', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E11', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E12', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E13', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E14', 2, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E15', 2, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E16', 2, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E1', 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E2', 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E3', 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E4', 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E5', 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E6', 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E7', 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E8', 1, @now, @emptyGuid),
        (@solutionId, @reportingCapabilityId, 'C16E1', 1, @now, @emptyGuid),
        (@solutionId, @scanningCapabilityId, 'C17E1', 1, @now, @emptyGuid),
        (@solutionId, @scanningCapabilityId, 'C17E2', 2, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E1', 2, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E2', 2, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E3', 1, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E4', 2, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E5', 1, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E6', 2, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E7', 1, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E8', 2, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E9', 1, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E10', 2, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E11', 1, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E12', 2, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E13', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E1', 1, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E2', 1, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E3', 1, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E4', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E5', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E6', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E7', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E8', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E9', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E10', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E11', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E12', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E13', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E14', 2, @now, @emptyGuid);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '99998-98';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'NotSystmOne', '99998', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version],
                    Features, ClientApplication, Hosting, ImplementationDetail,
                    IntegrationsUrl, AboutUrl, Summary, FullDescription,
                    LastUpdated, LastUpdatedBy)
        VALUES
        (
            @solutionId,
            @version1,
            '["Full Spine Compliance – EPS, PDS, SCR, eRS, GP2GP","Standards – SNOMED CT, HL7 V2, V3, FHIR, GP Connect","Appointments – Configurable Clinics, Dedicated Appointments, Visits Screens, SMS Integration","Prescribing – Acute, Repeat, Formularies, Action Groups, Decision Support","Complete Electronic Health Record (EHR)","Comprehensive consultations – Recalls, Referrals, Structured Data","Clinical Development Kit – Data Entry Templates, Views, Questionnaires, Integrated Word Letters","Full Workflow Support including Automatic Consultations","Analytics – Customisable Reports, Batch Reports, Bulk Actions, QOF Tools, Automatic Submissions","Patient Online Services – Appointment Booking, Medication Requests, Record Access, Proxy Access"]',
            '{"ClientApplicationTypes":["native-desktop"],"BrowsersSupported":[],"NativeMobileHardwareRequirements":"The OS system drive must have a drive letter of C.","NativeDesktopHardwareRequirements":"The OS system drive must have a drive letter of C.","NativeMobileFirstDesign":false,"MobileOperatingSystems":{"OperatingSystems":["Other"],"OperatingSystemsDescription":"Windows"},"MobileConnectionDetails":{"MinimumConnectionSpeed":"1Mbps","ConnectionType":["3G","4G","Wifi"],"Description":"CPU of 1 GHz or faster 32-bit or 64-bit processor"},"MobileMemoryAndStorage":{"MinimumMemoryRequirement":"1GB","Description":"4GB of free space on the C drive"},"MobileThirdParty":{"ThirdPartyComponents":"","DeviceCapabilities":"Minimum screen resolution of 1024 x 720 pixels."},"NativeDesktopOperatingSystemsDescription":"TPP supports all versions of Windows for desktops that are currently supported by Microsoft. Following verification of the configuration by TPP, installation of Windows to a virtual environment is supported to the products and versions including Virtual VMware View 5+, Citrix Xen Desktop 6+ and Microsoft Server 2012+.\r\nInstallation of the SystmOne client to any Server Operating System is not licensed by TPP. It should also be noted that both 32-bit and 64-bit versions of Microsoft Windows are supported unless otherwise stated. Windows RT is not supported.","NativeDesktopMinimumConnectionSpeed":"0.5Mbps","NativeDesktopThirdParty":{"ThirdPartyComponents":"Windows 7 requires 1GB and Office 2010 requires 256 MB. Other third party applications, shared graphics or peripherals (such as attached printers) should also be taken into account. These will all increase the amount of memory required for the computer to run smoothly.","DeviceCapabilities":"A minimum screen resolution of 1024 x 768 pixels with 16-bit colours is required. TPP recommends a minimum of a 17” TFT flat screen monitor with a resolution of 1280 x 1024 and 32-bit colours."},"NativeDesktopMemoryAndStorage":{"MinimumMemoryRequirement":"512MB","StorageRequirementsDescription":"4GB of free space on the C drive. Where a SystmOne Gateway client is used, 100GB of free space on the C drive is recommended.","MinimumCpu":"A minimum of a 2.0 GHz Pentium 4 series CPI is required.","RecommendedResolution":"5:4 – 1280 x 1024"},"NativeDesktopAdditionalInformation":"Applications that can open/view rich text file (.rtf) and comma separated (.csv) documents are required. To perform letter writing, Microsoft Word is also required. TPP only supports versions of Office that are supported by Microsoft which currently includes Office 2010, 2013, 2016 and 2019."}',
            '{"PrivateCloud":{"Summary":"The SystmOne Solution requires the following key items to be in place for smooth operation:\r\n-UDP Ports 2120-2130 and TCP Ports 2130-2140 should be opened to 20.146.120.128/25 and 20.146.248.128/25. TCP port 443 is also required for SystmOnline and Mobile Working to systmonline.tpp-uk.com. TPP also recommend allowing ICMP traffic for diagnostic purposes.\r\n\r\nA full list of requirements can be found in the SystmOne WES.","Link":"","HostingModel":"TPP provide a centralised solution with all server hardware hosted in TPP''s private cloud infrastructure. All server patching, security updates and feature releases are managed by TPP. The solution is hosted within 2 geographically separated private cloud instances with data replicated between the sites in real time in order to provide a high level of resiliency.\r\n\r\nTPP use a number of tools to monitor capacity, analyse usage trends and log the utilisation of the system. This ensures the solution scales to demand and new functionality / business requirements.","RequiresHSCN":"End user devices must be connected to HSCN/N3"}}',
            'If a greenfield unit is required, the turn-around time to receive the Live unit can be as quick as two weeks, once a signed contract is in place and all staff have received the required training.
TPP will assess the request and set up the unit as specified in the order details. Once the Live system is ready to use, TPP will be in touch with the contact who requested the unit.

When transitioning from a previous system that has a mature adapter in place (EMIS Web, Vision, Microtest), implementation is a quick rollout of 8 weeks, including data migration of any existing patient records.

The main phases for this implementation are:
•	Initial data production
•	Data checking
•	Training
•	Data reload & sign-off
•	Final data production
•	Go Live

If transitioning from any other system, an additional 8-week adapter build period would be required.

TPP maintain close contact with staff at the unit throughout these phases to ensure an efficient and accurate implementation.',
            'https://www.tpp-uk.com/partners',
            'https://www.tpp-uk.com/products/systmone',
            'SystmOne GP has been evolving for over 20 years, with continuous clinical input. It is one of the most advanced clinical systems in the world and is used by more than 2,700 GP practices nationwide. SystmOne GP is the ideal solution to meet the ever-changing needs of modern General Practice.',
            'SystmOne GP has been in use across UK General Practice for over 20 years. It is the system of choice for over 2,700 practices and is used by over 75,000 staff members. SystmOne GP is an advanced solution that goes far beyond the main functionality required for running a GP practice. It contains complete workflow support, a full analytics module, QOF tracking, and a comprehensive clinical development kit. Improving the quality of care across settings is core to TPP’s vision. The GP product is ideal for cross-organisational working and fully supports the requirements of Primary Care Networks.
            It enables true integrated care between GP, hospital, mental health and social care settings. TPP GP is Spine-accredited, providing access to the latest versions of GP2GP, EPS, and eRS. The system is fully compliant with SNOMED CT. SystmOne GP is leading on national interoperability programmes, compliant with national open FHIR standards for access to GP data and for transfer of care documentation.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.SolutionCapability (SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
        VALUES
        (@solutionId, @presciptionOrderingCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @gpAppointmentManagementCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @unifiedCareRecordCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @referralManagementCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @communicationManagementCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @reportingCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @citizenAppointmentManagementCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @citizenCommunicateCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @dataAnalyticsCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @sharedCarePlansCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @productivityCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @citizenViewRecordCapabilityId, 1, @now, @emptyGuid);

        INSERT INTO dbo.FrameworkSolutions(FrameworkId ,SolutionId ,IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@gpitframeworkId, @solutionId , 1, @now, @emptyGuid);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '99999-01';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'DFOCVC Online Consultation', '99999', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Digital Online Consultation","Video Consultation", "Fully interopable with all major GP IT solutions", "Compliant with all relevant ISO standards"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.dfocvctest.com/about',
            'Summary - DFOCVC.',
            'FULL DESCRIPTION – Digital First, Online Consultation and Video Consultation Solution.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Sam', 'Samosa', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef IN ('C43');

        INSERT INTO dbo.FrameworkSolutions(FrameworkId, SolutionId, IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@dfocvcframeworkId, @solutionId, 0, @now, @emptyGuid);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '99999-02';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'GPIT DFOCVC Online Consultation', '99999', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Digital Online Consultation","Video Consultation", "Fully interopable with all major GP IT solutions", "Compliant with all relevant ISO standards"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.gpitdfocvctest.com/about',
            'Summary - GPIT DFOCVC.',
            'FULL DESCRIPTION – GPIT Futures, Digital First Online Consultation and Video Consultation Solution.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Sam', 'Samosa', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef IN ('C44');

        INSERT INTO dbo.FrameworkSolutions(FrameworkId, SolutionId, IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@gpitframeworkId, @solutionId, 1, @now, @emptyGuid),
                    (@dfocvcframeworkId, @solutionId, 0, @now, @emptyGuid);
    END;

    DECLARE @flatPriceType AS int = 1;
    DECLARE @tieredPriceType AS int = 2;

    DECLARE @patientProvisioningType AS int = 1;
    DECLARE @declarativeProvisioningType AS int = 2;
    DECLARE @onDemandProvisioningType AS int = 3;

    DECLARE @monthTimeUnit AS int = 1;
    DECLARE @yearTimeUnit AS int = 2;

    /* Insert prices */
    IF NOT EXISTS (SELECT * FROM dbo.CataloguePrice)
    BEGIN
     INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price)
          VALUES ('100000-001', @patientProvisioningType, @flatPriceType, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', @yearTimeUnit, 'GBP', @now, 99.99),
                 ('100000-001', @patientProvisioningType, @tieredPriceType, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', @yearTimeUnit, 'GBP', @now, NULL),
                 ('100000-001', @onDemandProvisioningType, @flatPriceType, '774E5A1D-D15C-4A37-9990-81861BEAE42B', NULL, 'GBP', @now, 1001.010),
                 ('100001-001', @onDemandProvisioningType, @flatPriceType, '8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', NULL, 'GBP', @now, 3.142),
                 ('100002-001', @declarativeProvisioningType, @flatPriceType, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', @monthTimeUnit, 'GBP', @now, 4.85),
                 ('100002-001', @declarativeProvisioningType, @tieredPriceType, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', @monthTimeUnit, 'GBP', @now, NULL),
                 ('100003-001', @declarativeProvisioningType, @flatPriceType, 'D43C661A-0587-45E1-B315-5E5091D6E9D0', @monthTimeUnit, 'GBP', @now, 19.987),
                 ('100004-001', @declarativeProvisioningType, @flatPriceType, '8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', @monthTimeUnit, 'GBP', @now, 10101.65),
                 ('100005-001', @onDemandProvisioningType, @flatPriceType, '8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', NULL, 'GBP', @now, 456),
                 ('100006-001', @declarativeProvisioningType, @flatPriceType, '90119522-D381-4296-82EE-8FE630593B56', @monthTimeUnit, 'GBP', @now, 7),
                 ('100007-001', @onDemandProvisioningType, @flatPriceType, '90119522-D381-4296-82EE-8FE630593B56', NULL, 'GBP', @now, 0.15),
                 ('100007-002', @onDemandProvisioningType, @tieredPriceType, '90119522-D381-4296-82EE-8FE630593B56', NULL, 'GBP', @now, NULL),
                 ('99998-98', @patientProvisioningType, @flatPriceType, '8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', @yearTimeUnit, 'GBP', @now, 30000),
                 ('99998-98', @patientProvisioningType, @tieredPriceType, '8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', @yearTimeUnit, 'GBP', @now, NULL),
                 ('99999-01', @patientProvisioningType, @flatPriceType, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', @yearTimeUnit, 'GBP', @now, 1.25),
                 ('99999-02', @patientProvisioningType, @flatPriceType, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', @yearTimeUnit, 'GBP', @now, 1.55),
                 ('99999-89', @patientProvisioningType, @flatPriceType, '8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', @yearTimeUnit, 'GBP', @now, 500.49),
                 ('99999-89', @patientProvisioningType, @tieredPriceType, '8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', @yearTimeUnit, 'GBP', @now, NULL);

          -- Tiered price IDs
          DECLARE @priceId1000001 AS int = (SELECT CataloguePriceId from dbo.CataloguePrice WHERE CatalogueItemId = '100000-001' AND CataloguePriceTypeId = @tieredPriceType);
          DECLARE @priceId1000021 AS int = (SELECT CataloguePriceId from dbo.CataloguePrice WHERE CatalogueItemId = '100002-001' AND CataloguePriceTypeId = @tieredPriceType);
          DECLARE @priceId1000072 AS int = (SELECT CataloguePriceId from dbo.CataloguePrice WHERE CatalogueItemId = '100007-002' AND CataloguePriceTypeId = @tieredPriceType);
          DECLARE @priceId9999898 AS int = (SELECT CataloguePriceId from dbo.CataloguePrice WHERE CatalogueItemId = '99998-98' AND CataloguePriceTypeId = @tieredPriceType);
          DECLARE @priceId9999989 AS int = (SELECT CataloguePriceId from dbo.CataloguePrice WHERE CatalogueItemId = '99999-89' AND CataloguePriceTypeId = @tieredPriceType);

          INSERT INTO dbo.CataloguePriceTier(CataloguePriceId, BandStart, BandEnd, Price)
               VALUES (@priceId1000001, 1, 999, 123.45),
                      (@priceId1000001, 1000, 1999, 49.99),
                      (@priceId1000001, 2000, NULL, 19.99),
                      (@priceId1000021, 1, 10, 200),
                      (@priceId1000021, 11, 99, 150.15),
                      (@priceId1000021, 100, NULL, 99.99),
                      (@priceId9999898, 1, 10000, 500),
                      (@priceId9999898, 10001, NULL, 499.99),
                      (@priceId9999989, 1, 8, 42.42),
                      (@priceId9999989, 9, 33,33.33),
                      (@priceId9999989, 34, 1004, 50),
                      (@priceId9999989, 1005, NULL, 0.02),
                      (@priceId1000072, 1, 10, 20),
                      (@priceId1000072, 11, 99, 30.15),
                      (@priceId1000072, 100, NULL, 40.99);
     END;
END;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertAdditionalServices.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @cataloguePriceId AS int = 0;
DECLARE @publishedStatus AS int = 3;
DECLARE @solutionItemType AS int = 1;
DECLARE @now AS datetime = GETUTCDATE();
DECLARE @additionalServiceItemType AS int = 2;
DECLARE @solutionId AS nvarchar(14);
DECLARE @additionalServiceId AS nvarchar(14);
DECLARE @additionalServiceId2 AS nvarchar(14);
DECLARE @emptyGuid AS uniqueidentifier = '00000000-0000-0000-0000-000000000000';

IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND EXISTS (SELECT * FROM dbo.Solution)
BEGIN
    SET @solutionId = '100000-001';
    SET @additionalServiceId = '100000-001-A01'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Write on Time additional service', '100000', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Write on Time', 'Write on time Addttion Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 1, 1, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', @now, 199.99);
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100001-001';
    SET @additionalServiceId = '100001-001-A01'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Appointment Gateway additional service', '100001', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Appointment Gateway', 'Appointment Gateway Addition Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 2, 1, 'D43C661A-0587-45E1-B315-5E5091D6E9D0', 2, 'GBP', @now, 299.99);
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100002-001';
    SET @additionalServiceId = '100002-001-A01'
    SET @additionalServiceId2 = '100002-001-A02'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Zen Guidance additional service', '100002', @publishedStatus, @now),
                        (@additionalServiceId2, @additionalServiceItemType, 'Zen Guidance additional service 2', '100002', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Zen Guidance', 'Zen Guidance Addition Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 2, 1, 'D43C661A-0587-45E1-B315-5E5091D6E9D0', 2, 'GBP', @now, 399.99),
                        (@additionalServiceId2, 2, 1, '774E5A1D-D15C-4A37-9990-81861BEAE42B', 2, 'GBP', @now, 389.99);
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100004-001';
    SET @additionalServiceId = '100004-001-A01'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Diagnostics XYZ additional service', '100004', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Diagnostics XYZ', 'Diagnostics XYZ Addition Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 2, 2, 'D43C661A-0587-45E1-B315-5E5091D6E9D0', 2, 'GBP', @now, 499.99);

            SET @cataloguePriceId = (SELECT CataloguePriceId FROM dbo.CataloguePrice WHERE CatalogueItemId = @additionalServiceId);

            INSERT INTO dbo.CataloguePriceTier(CataloguePriceId, BandStart, BandEnd, Price)
                 VALUES (@cataloguePriceId, 1, 999, 123.45),
                        (@cataloguePriceId, 1000, 1999, 49.99),
                        (@cataloguePriceId, 2000, NULL, 19.99);
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100005-001';
    SET @additionalServiceId = '100005-001-A01'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Document Wizard additional service', '100005', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Document Wizard', 'Document Wizard Addition Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 1, 2, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 1, 'GBP', @now, 499.99);

            SET @cataloguePriceId = (SELECT CataloguePriceId FROM dbo.CataloguePrice WHERE CatalogueItemId = @additionalServiceId);

            INSERT INTO dbo.CataloguePriceTier(CataloguePriceId, BandStart, BandEnd, Price)
                 VALUES (@cataloguePriceId, 1, 9, 100.45),
                        (@cataloguePriceId, 10, 199, 200.99),
                        (@cataloguePriceId, 200, NULL,300.99);
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100006-001';
    SET @additionalServiceId = '100006-001-A01'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Paperlite additional service', '100006', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Paperlite', 'Paperlite Addition Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 3, 1, '8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', null, 'GBP', @now, 499.99);
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100007-001';
    SET @additionalServiceId = '100007-001-A01'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Medsort additional service', '100007', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Medsort', 'Medsort Addition Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 1, 1, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', @now, 599.99);
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100007-002';
    SET @additionalServiceId = '100007-002-A01'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Boston Dynamics additional service', '100007', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Boston Dynamics', 'Boston Dynamics Addition Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 1, 1, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', @now, 599.99);
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '99999-89';
    SET @additionalServiceId = '99999-89-A01'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'NotEmis Web GP additional service', '99999', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to NotEmis Web GP', 'NotEmis Web GP Addition Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 2, 1, 'D43C661A-0587-45E1-B315-5E5091D6E9D0', 1, 'GBP', @now, 699.99);
        END;
    END;
END;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./InsertAssociatedServices.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @emptyGuid AS uniqueidentifier = '00000000-0000-0000-0000-000000000000';
DECLARE @now AS datetime = GETUTCDATE();

DECLARE @declarative AS int = (SELECT ProvisioningTypeId FROM dbo.ProvisioningType WHERE [Name] = 'Declarative');
DECLARE @onDemand AS int = (SELECT ProvisioningTypeId FROM dbo.ProvisioningType WHERE [Name] = 'OnDemand');

DECLARE @flat AS int = (SELECT CataloguePriceTypeId FROM dbo.CataloguePriceType WHERE [Name] = 'Flat');
DECLARE @tiered AS int = (SELECT CataloguePriceTypeId FROM dbo.CataloguePriceType WHERE [Name] = 'Tiered');

DECLARE @hour AS uniqueidentifier = (SELECT PricingUnitId FROM dbo.PricingUnit WHERE [Name] = 'hour');
DECLARE @course AS uniqueidentifier = (SELECT PricingUnitId FROM dbo.PricingUnit WHERE [Name] = 'course');
DECLARE @halfDay AS uniqueidentifier = (SELECT PricingUnitId FROM dbo.PricingUnit WHERE [Name] = 'halfDay');

DECLARE @associatedServiceItemType AS int = (SELECT CatalogueItemTypeId FROM dbo.CatalogueItemType WHERE [Name] = 'Associated Service');
DECLARE @publishedStatus AS int = (SELECT Id FROM dbo.PublicationStatus WHERE [Name] = 'Published');

DECLARE @gbp AS char(3) = 'GBP';

DECLARE @associatedServiceId AS nvarchar(14);

IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND NOT EXISTS (SELECT * FROM dbo.AssociatedService)
BEGIN
    SET @associatedServiceId = '100000-S-001';

    INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
         VALUES (@associatedServiceId, @associatedServiceItemType, 'Really Kool associated service', '100000', @publishedStatus, @now);

    INSERT INTO dbo.AssociatedService (AssociatedServiceId, [Description], OrderGuidance, LastUpdated, LastUpdatedBy) 
         VALUES (@associatedServiceId, 'Really Kool associated service', NULL, @now, @emptyGuid);

    INSERT INTO dbo.CataloguePrice (CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price)
         VALUES (@associatedServiceId, @declarative, @flat, @course, NULL, @gbp, @now, 99.99),
                (@associatedServiceId, @onDemand, @flat, @halfDay, NULL, @gbp, @now, 150.00);

    SET @associatedServiceId = '100000-S-002';

    INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
         VALUES (@associatedServiceId, @associatedServiceItemType, 'Really Kool tiered associated service', '100000', @publishedStatus, @now);

    INSERT INTO dbo.AssociatedService (AssociatedServiceId, [Description], OrderGuidance, LastUpdated, LastUpdatedBy) 
         VALUES (@associatedServiceId, 'Really Kool tiered associated service', NULL, @now, @emptyGuid);

    INSERT INTO dbo.CataloguePrice (CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price)
         VALUES (@associatedServiceId, @onDemand, @tiered, @hour, NULL, @gbp, @now, NULL);

    DECLARE @tieredPriceId AS int = (SELECT CataloguePriceId FROM dbo.CataloguePrice WHERE CatalogueItemId = @associatedServiceId AND CataloguePriceTypeId = @tiered);

    INSERT INTO dbo.CataloguePriceTier (CataloguePriceId, BandStart, BandEnd, Price)
         VALUES (@tieredPriceId, 1, 9, 100),
                (@tieredPriceId, 10, NULL, 49.99);
END;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./CreateClinicalCommissioningGroups.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @ccgRoleId AS nchar(4) = 'RO98';

IF NOT EXISTS (SELECT * FROM dbo.Organisations WHERE PrimaryRoleId = @ccgRoleId)
	INSERT INTO dbo.Organisations (OrganisationId, [Name], [Address], OdsCode, PrimaryRoleId)
	VALUES
	('B7EE5261-43E7-4589-907B-5EEF5E98C085', 'Cheshire and Merseyside Commissioning Hub', '{"line1":"C/O NHS ENGLAND, 1W09, 1ST FLOOR","line2":"QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '13Y', @ccgRoleId),
	('43CC8258-6E92-4D39-9296-EDC5BB000563', 'Cumbria and North East Commissioning Hub', '{"line1":"C/O NHS ENGLAND, 1W09, 1ST FLOOR","line2":"QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '13X', @ccgRoleId),
	('A98F01D6-3F73-4B59-B213-E2BD3397C16F', 'Cumbria and North East – H&J Commissioning Hub', '{"line1":"C/O NHS ENGLAND","line2":"1W09, 1ST FLOOR, QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '14P', @ccgRoleId),
	('BB21DD07-58DF-457A-A9CB-6B035684692B', 'East Commissioning Hub', '{"line1":"C/O NHS ENGLAND, 1W09, 1ST FLOOR","line2":"QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '14E', @ccgRoleId),
	('6D37D7FB-DF06-403C-9C46-D649919F9158', 'East – H&J Commissioning Hub', '{"line1":"C/O NHS ENGLAND","line2":"1W09, 1ST FLOOR, QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '14R', @ccgRoleId),
	('D795A598-26F0-46FB-8A6E-F22CD912D7DC', 'Greater Manchester Commissioning Hub', '{"line1":"4TH FLOOR","line2":"PICCADILLY PLACE","line3":"LONDON ROAD","town":"MANCHESTER","postcode":"M1 3BN","country":"ENGLAND"}', '14J', @ccgRoleId),
	('E99EFE1F-273A-4DE8-9B18-C466C15D7560', 'Hampshire, Isle of Wight and Thames Valley Commissioning Hub', '{"line1":"NHS ENGLAND","line2":"QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '15J', @ccgRoleId),
	('DFF57334-4055-48CF-A569-DA490CCDC66A', 'Hampshire, Isle of Wight and Thames Valley – H&J Commissioning Hub', '{"line1":"C/O NHS ENGLAND","line2":"1W09, 1ST FLOOR, QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '14W', @ccgRoleId),
	('145CDD34-0BB9-4122-A0BF-1B6FCB0219BB', 'Kent, Surrey and Sussex Commissioning Hub', '{"line1":"NHS ENGLAND","line2":"QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '15K', @ccgRoleId),
	('B1E85A4D-364C-4ADC-A981-EB21B58A2CC0', 'Kent, Surrey and Sussex – H&J Commissioning Hub', '{"line1":"C/O NHS ENGLAND","line2":"1W09, 1ST FLOOR, QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '14V', @ccgRoleId),
	('44EDA119-4B2A-4A9A-B3A6-CE3A527BBE38', 'Lancashire and Greater Manchester Commissioning Hub', '{"line1":"NHS ENGLAND, 1W09, 1ST FLOOR","line2":"QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '13W', @ccgRoleId),
	('1F418AD5-8867-4613-AA21-B66A49C4A3D7', 'Lancashire Commissioning Hub', '{"line1":"C/O NHS ENGLAND","line2":"1W09 1ST FLOOR QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '14K', @ccgRoleId),
	('7EB95DD7-07FB-404B-82D8-9FFE01B5EFCF', 'Lancashire – H&J Commissioning Hub', '{"line1":"C/O NHS ENGLAND","line2":"1W09, 1ST FLOOR, QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '14X', @ccgRoleId),
	('4530CC04-23D1-4C93-95F3-6B025D79580B', 'London Commissioning Hub', '{"line1":"C/O NHS COMMISIONING BOARD","line2":"QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '13R', @ccgRoleId),
	('E1AEC969-5379-4906-891F-379B9CAADEF8', 'London – H&J Commissioning Hub', '{"line1":"C/O NHS COMMISSIONING BOARD","line2":"QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '14M', @ccgRoleId),
	('5D4A328D-8ECD-452E-9C11-091AD0F4F187', 'National Commissioning Hub 1', '{"line1":"C/O NHS ENGLAND","line2":"1W09, IST FLOOR, QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '13Q', @ccgRoleId),
	('E83C3B63-BDEA-4AF3-8721-E6AE2199A941', 'National Commissioning Hub 2', '{"line1":"C/O NHS ENGLAND","line2":"1W09, IST FLOOR, QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '15L', @ccgRoleId),
	('72BCBAF4-5033-45ED-B6FD-F4B0DD72CF70', 'NHS Airedale, Wharfedale and Craven CCG', '{"line1":"DOUGLAS MILLS","line2":"BOWLING OLD LANE","town":"BRADFORD","county":"WEST YORKSHIRE","postcode":"BD5 7JR","country":"ENGLAND"}', '02N', @ccgRoleId),
	('DA31DD92-8DBF-495A-B4D2-F3842549DF14', 'NHS Ashford CCG', '{"line1":"INCA HOUSE","line2":"TRINITY ROAD","line3":"EUREKA SCIENCE PARK","town":"ASHFORD","county":"KENT","postcode":"TN25 4AB","country":"ENGLAND"}', '09C', @ccgRoleId),
	('4222B058-383F-40CD-A63C-9C9F7EB10889', 'NHS Aylesbury Vale CCG', '{"line1":"VERNEY HOUSE","line2":"GATESHEAD ROAD","town":"AYLESBURY","county":"BUCKINGHAMSHIRE","postcode":"HP19 8ET","country":"ENGLAND"}', '10Y', @ccgRoleId),
	('D78F32FE-1A14-4316-A0F3-C9D041FC379C', 'NHS Barking and Dagenham CCG', '{"line1":"6TH FLOOR","line2":"NORTH HOUSE","line3":"ST. EDWARDS WAY","town":"ROMFORD","postcode":"RM1 3AE","country":"ENGLAND"}', '07L', @ccgRoleId),
	('1BFCB301-AC95-49FF-84D1-8BCE861E86A9', 'NHS Barnet CCG', '{"line1":"GROUND FLOOR, BUILDING 2","line2":"NORTH LONDON BUSINESS PARK","line3":"OAKLEIGH ROAD SOUTH, NEW SOUTHGATE","town":"LONDON","county":"GREATER LONDON","postcode":"N11 1NP","country":"ENGLAND"}', '07M', @ccgRoleId),
	('7FDEE94C-9B0B-4D2C-8AD6-6E3844D334C4', 'NHS Barnsley CCG', '{"line1":"HILLDER HOUSE","line2":"49-51 GAWBER ROAD","town":"BARNSLEY","postcode":"S75 2PY","country":"ENGLAND"}', '02P', @ccgRoleId),
	('CE795DD9-F9F9-48D8-BBB5-A60777FE5D82', 'NHS Basildon and Brentwood CCG', '{"line1":"PHOENIX HOUSE","line2":"CHRISTOPHER MARTIN ROAD","town":"BASILDON","postcode":"SS14 3HG","country":"ENGLAND"}', '99E', @ccgRoleId),
	('7048F4C0-F437-4000-8448-EFB00091C772', 'NHS Bassetlaw CCG', '{"line1":"RETFORD HOSPITAL","line2":"NORTH ROAD","town":"RETFORD","county":"NOTTINGHAMSHIRE","postcode":"DN22 7XF","country":"ENGLAND"}', '02Q', @ccgRoleId),
	('9598753A-FE70-4276-BD77-8EC22010B602', 'NHS Bath and North East Somerset CCG', '{"line1":"ST MARTINS HOSPITAL","line2":"CLARA CROSS LANE","town":"BATH","county":"AVON","postcode":"BA2 5RP","country":"ENGLAND"}', '11E', @ccgRoleId),
	('DE4219B4-B37D-464A-A36D-3769B941A237', 'NHS Bath and North East Somerset, Swindon and Wiltshire CCG', '{"line1":"ST. MARTINS HOSPITAL","line2":"CLARA CROSS LANE","town":"BATH","postcode":"BA2 5RP","country":"ENGLAND"}', '92G', @ccgRoleId),
	('63CAFE29-AB1A-47CA-82CB-AFCD4104782D', 'NHS Bedfordshire CCG', '{"line1":"CAPABILITY HOUSE","line2":"WREST PARK","line3":"SILSOE","town":"BEDFORD","county":"BEDFORDSHIRE","postcode":"MK45 4HR","country":"ENGLAND"}', '06F', @ccgRoleId),
	('D292D668-F2E3-42ED-A93A-190A00D945E7', 'NHS Berkshire West CCG', '{"line1":"57-59 BATH ROAD","town":"READING","postcode":"RG30 2BA","country":"ENGLAND"}', '15A', @ccgRoleId),
	('785FC4D1-7CBC-4B0A-8890-6440549BDBE4', 'NHS Bexley CCG', '{"line1":"CIVIC OFFICES","line2":"2 WATLING STREET","town":"BEXLEYHEATH","county":"KENT","postcode":"DA6 7AT","country":"ENGLAND"}', '07N', @ccgRoleId),
	('EF432A5B-45DB-4D44-92F2-DCD4B2A12310', 'NHS Birmingham and Solihull CCG', '{"line1":"FIRST FLOOR","line2":"WESLEYAN","line3":"COLMORE CIRCUS QUEENSWAY","town":"BIRMINGHAM","postcode":"B4 6AR","country":"ENGLAND"}', '15E', @ccgRoleId),
	('5D7A3DB7-F176-47DC-8B0C-139CFAF788DB', 'NHS Birmingham Crosscity CCG', '{"line1":"BARTHOLOMEW HOUSE","line2":"142 HAGLEY ROAD","town":"BIRMINGHAM","county":"WEST MIDLANDS","postcode":"B16 9PA","country":"ENGLAND"}', '13P', @ccgRoleId),
	('8EBDBC1A-EA0C-4948-BE23-468F8E3DCE2F', 'NHS Birmingham South and Central CCG', '{"line1":"BARTHOLOMEW HOUSE","line2":"142 HAGLEY ROAD","town":"BIRMINGHAM","county":"WEST MIDLANDS","postcode":"B16 9PA","country":"ENGLAND"}', '04X', @ccgRoleId),
	('E56A100B-077B-4AD7-8D39-2D2481B53A26', 'NHS Blackburn With Darwen CCG', '{"line1":"FUSION HOUSE","line2":"EVOLUTION PARK","line3":"HASLINGDEN ROAD","town":"BLACKBURN","county":"LANCASHIRE","postcode":"BB1 2FD","country":"ENGLAND"}', '00Q', @ccgRoleId),
	('8AC9BAA3-8958-4AC8-82A2-8CFAAAFF1DB5', 'NHS Blackpool CCG', '{"line1":"THE STADIUM","line2":"SEASIDERS WAY","town":"BLACKPOOL","postcode":"FY1 6JX","country":"ENGLAND"}', '00R', @ccgRoleId),
	('6B417452-88B8-4A15-8E23-036D897C318A', 'NHS Bolton CCG', '{"line1":"ST PETERS HOUSE","line2":"SILVERWELL STREET","town":"BOLTON","postcode":"BL1 1PP","country":"ENGLAND"}', '00T', @ccgRoleId),
	('8C6C36FC-E0AA-4AC9-8D9A-5238246902EC', 'NHS Bracknell and Ascot CCG', '{"line1":"KING EDWARD VII HOSPITAL","line2":"ST. LEONARDS ROAD","town":"WINDSOR","county":"BERKSHIRE","postcode":"SL4 3DP","country":"ENGLAND"}', '10G', @ccgRoleId),
	('B091CD2C-EACC-4E89-B5D5-E44D073C00CB', 'NHS Bradford City CCG', '{"line1":"DOUGLAS MILLS","line2":"BOWLING OLD LANE","town":"BRADFORD","county":"WEST YORKSHIRE","postcode":"BD5 7JR","country":"ENGLAND"}', '02W', @ccgRoleId),
	('302FA724-C9C8-4EDF-9094-5D8E85695E41', 'NHS Bradford District and Craven CCG', '{"line1":"SCOREX HOUSE WEST","line2":"1 BOLTON ROAD","town":"BRADFORD","county":"WEST YORKSHIRE","postcode":"BD1 4AS","country":"ENGLAND"}', '36J', @ccgRoleId),
	('76026F74-FDB0-41EE-B04C-3415BA389A60', 'NHS Bradford Districts CCG', '{"line1":"DOUGLAS MILLS","line2":"BOWLING OLD LANE","town":"BRADFORD","county":"WEST YORKSHIRE","postcode":"BD5 7JR","country":"ENGLAND"}', '02R', @ccgRoleId),
	('03A8B2F5-44F7-43B8-9EEA-96193987B5CB', 'NHS Brent CCG', '{"line1":"WEMBLEY CENTRE FOR HEALTH & CARE","line2":"CHAPLIN ROAD","town":"WEMBLEY","postcode":"HA0 4UZ","country":"ENGLAND"}', '07P', @ccgRoleId),
	('B94E36AB-2FF6-4E46-9E91-1429CFEF56DC', 'NHS Brighton and Hove CCG', '{"line1":"HOVE TOWN HALL","line2":"NORTON ROAD","town":"HOVE","postcode":"BN3 4AH","country":"ENGLAND"}', '09D', @ccgRoleId),
	('9A87BEDD-8986-4155-9FA3-52E4A1A3ED52', 'NHS Bristol CCG', '{"line1":"SOUTH PLAZA","line2":"MARLBOROUGH STREET","town":"BRISTOL","county":"AVON","postcode":"BS1 3NX","country":"ENGLAND"}', '11H', @ccgRoleId),
	('399E5347-6F54-48B6-B4AF-FC7C72C69338', 'NHS Bristol, North Somerset and South Gloucestershire CCG', '{"line1":"SOUTH PLAZA","line2":"MARLBOROUGH STREET","town":"BRISTOL","postcode":"BS1 3NX","country":"ENGLAND"}', '15C', @ccgRoleId),
	('F01126C9-890E-4B88-BFB5-4DE93E3EA9B6', 'NHS Bromley CCG', '{"line1":"BECKENHAM BEACON","line2":"379 CROYDON ROAD","town":"BECKENHAM","county":"KENT","postcode":"BR3 3QL","country":"ENGLAND"}', '07Q', @ccgRoleId),
	('9B159AED-EEF1-4569-B33B-7103B93DE6E6', 'NHS Buckinghamshire CCG', '{"line1":"SECOND FLOOR","line2":"THE GATEWAY","line3":"GATEHOUSE ROAD","town":"AYLESBURY","postcode":"HP19 8FF","country":"ENGLAND"}', '14Y', @ccgRoleId),
	('8FDACC5F-F315-4F6D-A075-6FF3BA5F98CC', 'NHS Bury CCG', '{"line1":"TOWNSIDE PRIMARY CARE CENTRE","line2":"1 KNOWSLEY PLACE","line3":"KNOWSLEY STREET","town":"BURY","county":"LANCASHIRE","postcode":"BL9 0SN","country":"ENGLAND"}', '00V', @ccgRoleId),
	('37CB8F12-1148-4D7D-999A-69FECC5C5380', 'NHS Calderdale CCG', '{"line1":"F MILL","line2":"DEAN CLOUGH MILLS","town":"HALIFAX","postcode":"HX3 5AX","country":"ENGLAND"}', '02T', @ccgRoleId),
	('EE02D603-30A6-477C-98AE-B40BFF9F4784', 'NHS Cambridgeshire and Peterborough CCG', '{"line1":"LOCKTON HOUSE","line2":"CLARENDON ROAD","town":"CAMBRIDGE","postcode":"CB2 8FH","country":"ENGLAND"}', '06H', @ccgRoleId),
	('3DC9775B-3902-410B-97EF-BA04F6EF1B1D', 'NHS Camden CCG', '{"line1":"EUSTON TOWER","line2":"286 EUSTON ROAD","town":"LONDON","postcode":"NW1 3DP","country":"ENGLAND"}', '07R', @ccgRoleId),
	('868F556E-1E79-4F11-B8E3-EC80C4142109', 'NHS Cannock Chase CCG', '{"line1":"BLOCK D","line2":"BEECROFT COURT","line3":"BEECROFT ROAD","town":"CANNOCK","county":"STAFFORDSHIRE","postcode":"WS11 1JP","country":"ENGLAND"}', '04Y', @ccgRoleId),
	('33194979-FA76-4CF7-A50F-99C4E216A690', 'NHS Canterbury and Coastal CCG', '{"line1":"COUNCIL OFFICES","line2":"MILITARY ROAD","town":"CANTERBURY","county":"KENT","postcode":"CT1 1YW","country":"ENGLAND"}', '09E', @ccgRoleId),
	('6432C737-CD5A-459D-9C3C-C1044DD97868', 'NHS Castle Point and Rochford CCG', '{"line1":"PEARL HOUSE","line2":"12 CASTLE ROAD","town":"RAYLEIGH","postcode":"SS6 7QF","country":"ENGLAND"}', '99F', @ccgRoleId),
	('1B4778D7-331A-4BB5-BDEF-9B9CE64333F5', 'NHS Central London (Westminster) CCG', '{"line1":"15 MARYLEBONE ROAD","town":"LONDON","postcode":"NW1 5JD","country":"ENGLAND"}', '09A', @ccgRoleId),
	('A8FF90D2-2775-4A01-A6D1-B1A765DFE05D', 'NHS Central Manchester CCG', '{"line1":"PARKWAY ONE","line2":"PARKWAY BUSINESS CENTRE","line3":"PRINCESS ROAD","town":"MANCHESTER","county":"GREATER MANCHESTER","postcode":"M14 7LU","country":"ENGLAND"}', '00W', @ccgRoleId),
	('19B2AE3B-D380-4109-ADED-5C125C1F2709', 'NHS Cheshire CCG', '{"line1":"BEVAN HOUSE","line2":"BARONY COURT","town":"NANTWICH","county":"CHESHIRE","postcode":"CW5 5RD","country":"ENGLAND"}', '27D', @ccgRoleId),
	('C80A4F4B-0608-4DF0-B9B3-CD8F6D6775BD', 'NHS Chiltern CCG', '{"line1":"ROOM XR37, 2ND FLOOR","line2":"AMERSHAM HOSPITAL","line3":"WHIELDEN STREET","town":"AMERSHAM","county":"BUCKINGHAMSHIRE","postcode":"HP7 0JD","country":"ENGLAND"}', '10H', @ccgRoleId),
	('C9231001-981B-4EE5-8D13-F50841ECBB93', 'NHS Chorley and South Ribble CCG', '{"line1":"CHORLEY HOUSE","line2":"LANCASHIRE ENTERPRISE BUSINESS PARK","line3":"CENTURION WAY","town":"LEYLAND","county":"LANCASHIRE","postcode":"PR26 6TT","country":"ENGLAND"}', '00X', @ccgRoleId),
	('C8601CB9-C7AA-42F4-A5C0-FD690AE8A7AF', 'NHS City and Hackney CCG', '{"line1":"3RD FLOOR A BLOCK","line2":"ST LEONARDS HOSPITAL","line3":"NUTTALL STREET","town":"LONDON","county":"GREATER LONDON","postcode":"N1 5LZ","country":"ENGLAND"}', '07T', @ccgRoleId),
	('A99314C0-87BE-4B56-8829-B90E23987055', 'NHS Coastal West Sussex CCG', '{"line1":"1 THE CAUSEWAY","line2":"GORING-BY-SEA","town":"WORTHING","county":"WEST SUSSEX","postcode":"BN12 6BT","country":"ENGLAND"}', '09G', @ccgRoleId),
	('13D56925-CD12-4F6B-ADED-39990AABA1B1', 'NHS Corby CCG', '{"line1":"WILLOWBROOK HEALTH CENTRE","line2":"COTTINGHAM ROAD","town":"CORBY","county":"NORTHAMPTONSHIRE","postcode":"NN17 2UR","country":"ENGLAND"}', '03V', @ccgRoleId),
	('0F806919-CCA3-46B4-91AA-E2409A683026', 'NHS County Durham CCG', '{"line1":"SALTERS LANE","line2":"SEDGEFIELD","town":"STOCKTON-ON-TEES","postcode":"TS21 3EE","country":"ENGLAND"}', '84H', @ccgRoleId),
	('D5C41704-5254-4322-A092-5A8368061E2C', 'NHS Coventry and Rugby CCG', '{"line1":"PARKSIDE HOUSE","line2":"QUINTON ROAD","town":"COVENTRY","county":"WEST MIDLANDS","postcode":"CV1 2NJ","country":"ENGLAND"}', '05A', @ccgRoleId),
	('18A677BF-8E61-45B9-8265-6E6C4CC2628B', 'NHS Crawley CCG', '{"line1":"LOWER GROUND FLOOR, RED WING","line2":"CRAWLEY HOSPITAL","line3":"WEST GREEN DRIVE","town":"CRAWLEY","county":"WEST SUSSEX","postcode":"RH11 7DH","country":"ENGLAND"}', '09H', @ccgRoleId),
	('01B7FED2-73CB-41FD-A8AF-6EE847663236', 'NHS Croydon CCG', '{"line1":"LEON HOUSE","line2":"233 HIGH STREET","town":"CROYDON","county":"SURREY","postcode":"CR0 9XT","country":"ENGLAND"}', '07V', @ccgRoleId),
	('6F6F7D0D-01E9-488F-B7CD-C2E889C4080B', 'NHS Darlington CCG', '{"line1":"C/O BILLINGHAM HEALTH CENTRE","line2":"QUEENSWAY","town":"BILLINGHAM","county":"CLEVELAND","postcode":"TS23 2LA","country":"ENGLAND"}', '00C', @ccgRoleId),
	('24C66FCA-F1A7-4867-84B4-611429776885', 'NHS Dartford, Gravesham and Swanley CCG', '{"line1":"GRAVESHAM CIVIC CENTRE","line2":"WINDMILL STREET","town":"GRAVESEND","county":"KENT","postcode":"DA12 1AU","country":"ENGLAND"}', '09J', @ccgRoleId),
	('C874EE2E-E836-4873-83EA-8832B0D97327', 'NHS Derby and Derbyshire CCG', '{"line1":"CARDINAL SQUARE","line2":"10 NOTTINGHAM ROAD","town":"DERBY","county":"DERBYSHIRE","postcode":"DE1 3QT","country":"ENGLAND"}', '15M', @ccgRoleId),
	('4CF635B3-78F1-4713-BA17-1C15CE35A436', 'NHS Devon CCG', '{"line1":"COUNTY HALL","line2":"TOPSHAM ROAD","town":"EXETER","county":"DEVON","postcode":"EX2 4QL","country":"ENGLAND"}', '15N', @ccgRoleId),
	('AAC42B08-8070-4C3B-A5CF-009B389C89C8', 'NHS Doncaster CCG', '{"line1":"SOVEREIGN HOUSE","line2":"HEAVENS WALK","town":"DONCASTER","postcode":"DN4 5HZ","country":"ENGLAND"}', '02X', @ccgRoleId),
	('5818FDD0-5437-4049-B290-0802473AD859', 'NHS Dorset CCG', '{"line1":"VESPASIAN HOUSE","line2":"BRIDPORT ROAD","town":"DORCHESTER","county":"DORSET","postcode":"DT1 1TG","country":"ENGLAND"}', '11J', @ccgRoleId),
	('550F8961-E3AC-4671-B8F5-807D7C692BAB', 'NHS Dudley CCG', '{"line1":"BRIERLEY HILL HEALTH & SOCIAL CARE","line2":"VENTURE WAY","town":"BRIERLEY HILL","postcode":"DY5 1RU","country":"ENGLAND"}', '05C', @ccgRoleId),
	('FDE31259-15CE-4554-9C28-CB26F5373C0E', 'NHS Durham Dales, Easington and Sedgefield CCG', '{"line1":"SEDGEFIELD COMMUNITY HOSPITAL","line2":"SALTERS LANE","line3":"SEDGEFIELD","town":"STOCKTON-ON-TEES","county":"CLEVELAND","postcode":"TS21 3EE","country":"ENGLAND"}', '00D', @ccgRoleId),
	('EDA5FC03-3BEB-4CFB-A621-0D4948F6A411', 'NHS Ealing CCG', '{"line1":"PERCEVAL HOUSE","line2":"14-16 UXBRIDGE ROAD","town":"LONDON","postcode":"W5 2HL","country":"ENGLAND"}', '07W', @ccgRoleId),
	('0F9798E5-2637-4FA5-9274-90D2C437EBDF', 'NHS East and North Hertfordshire CCG', '{"line1":"CHARTER HOUSE","line2":"PARKWAY","town":"WELWYN GARDEN CITY","county":"HERTFORDSHIRE","postcode":"AL8 6JL","country":"ENGLAND"}', '06K', @ccgRoleId),
	('C89B431A-0077-4A7D-8BF2-C04C1DA1419E', 'NHS East Berkshire CCG', '{"line1":"KING EDWARD VII HOSPITAL","line2":"ST LEONARDS ROAD","town":"WINDSOR","postcode":"SL4 3DP","country":"ENGLAND"}', '15D', @ccgRoleId),
	('3EDE91E9-95A5-4931-B67A-7E312B89883C', 'NHS Eastbourne, Hailsham and Seaford CCG', '{"line1":"36-38 FRIARS WALK","town":"LEWES","county":"EAST SUSSEX","postcode":"BN7 2PB","country":"ENGLAND"}', '09F', @ccgRoleId),
	('03852F69-D27D-49A6-8472-6AB2D3E1FC17', 'NHS Eastern Cheshire CCG', '{"line1":"1ST FLOOR","line2":"WEST WING","line3":"NEW ALDERLERY HOUSE","town":"VICTORIA ROAD MACCLESFIELD","county":"CHESHIRE","postcode":"SK10 3BL","country":"ENGLAND"}', '01C', @ccgRoleId),
	('E34CD785-DEBE-4EDA-BC6E-17D8AE67FA95', 'NHS East Lancashire CCG', '{"line1":"WALSHAW HOUSE","line2":"REGENT STREET","town":"NELSON","postcode":"BB9 8AS","country":"ENGLAND"}', '01A', @ccgRoleId),
	('A0B43660-1D13-41A7-BDC3-2863CAA93EEB', 'NHS East Leicestershire and Rutland CCG', '{"line1":"LEICESTERSHIRE COUNTY COUNCIL","line2":"ROOM 30 PEN LLOYD BUILDING","line3":"LEICESTER ROAD, GLENFIELD","town":"LEICESTER","county":"LEICESTERSHIRE","postcode":"LE3 8TB","country":"ENGLAND"}', '03W', @ccgRoleId),
	('3B2F3790-EA61-42E5-9E17-BA6020C0692D', 'NHS East Riding of Yorkshire CCG', '{"line1":"HEALTH HOUSE","line2":"GRANGE PARK LANE","line3":"WILLERBY","town":"HULL","postcode":"HU10 6DT","country":"ENGLAND"}', '02Y', @ccgRoleId),
	('0E3F25C9-3D2D-454B-ACEF-266A9F3AC86D', 'NHS East Staffordshire CCG', '{"line1":"EDWIN HOUSE","line2":"SECOND AVENUE","line3":"CENTRUM ONE HUNDRED","town":"BURTON-ON-TRENT","county":"STAFFORDSHIRE","postcode":"DE14 2WF","country":"ENGLAND"}', '05D', @ccgRoleId),
	('4A6005C4-6D53-4352-9DF2-2DBA11096224', 'NHS East Surrey CCG', '{"line1":"TANDRIDGE DISTRICT COUNCIL","line2":"COUNCIL OFFICES","line3":"8 STATION ROAD EAST","town":"OXTED","postcode":"RH8 0BT","country":"ENGLAND"}', '09L', @ccgRoleId),
	('DCB28F60-C251-460A-96B4-ECFC002C57B4', 'NHS East Sussex CCG', '{"line1":"36-38 FRIARS WALK","town":"LEWES","postcode":"BN7 2PB","country":"ENGLAND"}', '97R', @ccgRoleId),
	('5D3C56F0-4EFB-454B-AF15-AD087448BC64', 'NHS Enfield CCG', '{"line1":"HOLBROOK HOUSE","line2":"COCKFOSTERS ROAD","town":"BARNET","county":"HERTFORDSHIRE","postcode":"EN4 0DY","country":"ENGLAND"}', '07X', @ccgRoleId),
	('035079BA-9373-459B-A8AA-FC2FCF849528', 'NHS Erewash CCG', '{"line1":"TOLL BAR HOUSE","line2":"1 DERBY ROAD","town":"ILKESTON","county":"DERBYSHIRE","postcode":"DE7 5FH","country":"ENGLAND"}', '03X', @ccgRoleId),
	('18B95224-6669-477E-8092-DE5A2573E4C7', 'NHS Fareham and Gosport CCG', '{"line1":"COMMCEN BUILDING","line2":"FORT SOUTHWICK","line3":"JAMES CALLAGHAN DRIVE","town":"FAREHAM","postcode":"PO17 6AR","country":"ENGLAND"}', '10K', @ccgRoleId),
	('23A0806B-2FF8-46F6-818D-87B96F6E554B', 'NHS Fylde and Wyre CCG', '{"line1":"DERBY ROAD","line2":"WESHAM","town":"PRESTON","postcode":"PR4 3AL","country":"ENGLAND"}', '02M', @ccgRoleId),
	('E80CA348-EF3E-47DD-8621-F2300C5AEEAB', 'NHS Gateshead CCG', '{"line1":"RIVERSIDE HOUSE","line2":"GOLDCREST WAY","line3":"NEWBURN RIVERSIDE","town":"NEWCASTLE UPON TYNE","county":"TYNE AND WEAR","postcode":"NE15 8NY","country":"ENGLAND"}', '00F', @ccgRoleId),
	('57634F78-BEFC-41E9-A76F-78DC59E6A2ED', 'NHS Gloucestershire CCG', '{"line1":"SANGER HOUSE","line2":"5220 VALIANT COURT","line3":"GLOUCESTER BUSINESS PARK, BROCKWORT","town":"GLOUCESTER","postcode":"GL3 4FE","country":"ENGLAND"}', '11M', @ccgRoleId),
	('0079FFDE-C6D2-4418-BDC5-F70901DF0315', 'NHS Greater Huddersfield CCG', '{"line1":"BROAD LEA HOUSE","line2":"DYSON WOOD WAY","line3":"BRADLEY","town":"HUDDERSFIELD","postcode":"HD2 1GZ","country":"ENGLAND"}', '03A', @ccgRoleId),
	('56BBCB8A-179F-4306-8236-58B1BE8619A8', 'NHS Greater Preston CCG', '{"line1":"CHORLEY HOUSE","line2":"LANCASHIRE ENTERPRISE BUSINESS PARK","line3":"CENTURION WAY","town":"LEYLAND","county":"LANCASHIRE","postcode":"PR26 6TT","country":"ENGLAND"}', '01E', @ccgRoleId),
	('417D0DB4-008B-40A7-805E-4CCBFE3872B4', 'NHS Great Yarmouth and Waveney CCG', '{"line1":"BECCLES HOUSE","line2":"1 COMMON LANE NORTH","town":"BECCLES","postcode":"NR34 9BN","country":"ENGLAND"}', '06M', @ccgRoleId),
	('89A925AF-3D28-4E0E-BA07-A50ED87637AF', 'NHS Greenwich CCG', '{"line1":"THE WOOLWICH CENTRE","line2":"35 WELLINGTON STREET","line3":"WOOLWICH","town":"LONDON","county":"GREATER LONDON","postcode":"SE18 6ND","country":"ENGLAND"}', '08A', @ccgRoleId),
	('2071580C-71D3-4BC0-8944-959DE9DB8D07', 'NHS Guildford and Waverley CCG', '{"line1":"DOMINION HOUSE","line2":"WOODBRIDGE ROAD","town":"GUILDFORD","postcode":"GU1 4PU","country":"ENGLAND"}', '09N', @ccgRoleId),
	('AAE7471F-34B1-41B7-B1ED-807D1F9F1D0B', 'NHS Halton CCG', '{"line1":"FIRST FLOOR","line2":"RUNCORN TOWN HALL","line3":"HEATH ROAD","town":"RUNCORN","county":"CHESHIRE","postcode":"WA7 5TD","country":"ENGLAND"}', '01F', @ccgRoleId),
	('37DB4B3D-DE3D-4C35-81FE-583E176AD329', 'NHS Hambleton, Richmondshire and Whitby CCG', '{"line1":"HAMBLETON DISTRICT COUNCIL","line2":"CIVIC CENTRE","line3":"STONECROSS","town":"NORTHALLERTON","county":"NORTH YORKSHIRE","postcode":"DL6 2UU","country":"ENGLAND"}', '03D', @ccgRoleId),
	('19FC60DE-8E67-4352-9B91-14C3C2D51B51', 'NHS Hammersmith and Fulham CCG', '{"line1":"15 MARYLEBONE ROAD","town":"LONDON","postcode":"NW1 5JD","country":"ENGLAND"}', '08C', @ccgRoleId),
	('B42AE3E6-8FA4-4040-83D5-7A9505961C70', 'NHS Hardwick CCG', '{"line1":"SCARSDALE HOSPITAL","line2":"NIGHTINGALE CLOSE","line3":"OFF NEWBOLD ROAD","town":"CHESTERFIELD","county":"DERBYSHIRE","postcode":"S41 7PF","country":"ENGLAND"}', '03Y', @ccgRoleId),
	('9DC98391-C70C-4C60-9046-122021EE8481', 'NHS Haringey CCG', '{"line1":"RIVER PARK HOUSE","line2":"225 HIGH ROAD","line3":"WOOD GREEN","town":"LONDON","county":"GREATER LONDON","postcode":"N22 8HQ","country":"ENGLAND"}', '08D', @ccgRoleId),
	('326B2A91-5CFB-48E6-B2F6-818A74FEE119', 'NHS Harrogate and Rural District CCG', '{"line1":"1 GRIMBALD CRAG COURT","line2":"ST JAMES BUSINESS PARK","town":"KNARESBOROUGH","county":"NORTH YORKSHIRE","postcode":"HG5 8QB","country":"ENGLAND"}', '03E', @ccgRoleId),
	('34B4ECD9-177A-401A-8831-0039005EEDB2', 'NHS Harrow CCG', '{"line1":"4TH FLOOR","line2":"THE HEIGHTS","line3":"59-65 LOWLANDS ROAD","town":"HARROW","postcode":"HA1 3AW","country":"ENGLAND"}', '08E', @ccgRoleId),
	('3121E281-4024-4121-9F3A-FFB9FD86E216', 'NHS Hartlepool and Stockton–On–Tees CCG', '{"line1":"BILLINGHAM HEALTH CENTRE","line2":"QUEENSWAY","town":"BILLINGHAM","county":"CLEVELAND","postcode":"TS23 2LA","country":"ENGLAND"}', '00K', @ccgRoleId),
	('93A515EE-2541-4BF3-B90A-CAB6E4B4062B', 'NHS Hastings and Rother CCG', '{"line1":"BEXHILL HOSPITAL","line2":"HOLLIERS HILL","town":"BEXHILL-ON-SEA","county":"EAST SUSSEX","postcode":"TN40 2DZ","country":"ENGLAND"}', '09P', @ccgRoleId),
	('6897FF95-3225-4844-9B5D-83961E477442', 'NHS Havering CCG', '{"line1":"6TH FLOOR","line2":"NORTH HOUSE","line3":"ST EDWARDS WAY","town":"ROMFORD","postcode":"RM1 3AE","country":"ENGLAND"}', '08F', @ccgRoleId),
	('A254254A-DE9E-4D90-8DE4-419475264B3B', 'NHS Herefordshire and Worcestershire CCG', '{"line1":"THE COACH HOUSE","line2":"JOHN COMYN DRIVE","town":"WORCESTER","postcode":"WR3 7NS","country":"ENGLAND"}', '18C', @ccgRoleId),
	('FBEE3F72-EB91-4867-8DCC-C6FFF50FD914', 'NHS Herefordshire CCG', '{"line1":"ST. OWENS CHAMBERS","line2":"22 ST. OWEN STREET","town":"HEREFORD","county":"HEREFORDSHIRE","postcode":"HR1 2PL","country":"ENGLAND"}', '05F', @ccgRoleId),
	('44157E81-0F1A-46BD-933B-A2AA3B0E6888', 'NHS Herts Valleys CCG', '{"line1":"THE FORUM","line2":"MARLOWES","town":"HEMEL HEMPSTEAD","postcode":"HP1 1DN","country":"ENGLAND"}', '06N', @ccgRoleId),
	('C5869F03-CE66-445F-94A8-C25B4851AB28', 'NHS Heywood, Middleton and Rochdale CCG', '{"line1":"3RD FLOOR","line2":"NUMBER ONE RIVERSIDE","line3":"SMITH STREET","town":"ROCHDALE","postcode":"OL16 1XU","country":"ENGLAND"}', '01D', @ccgRoleId),
	('56E1F6F7-991F-407D-80EE-B8A96DB8DDBB', 'NHS High Weald Lewes Havens CCG', '{"line1":"36-38 FRIARS WALK","town":"LEWES","county":"EAST SUSSEX","postcode":"BN7 2PB","country":"ENGLAND"}', '99K', @ccgRoleId),
	('4BE80E5D-20F1-4AC5-AED8-DDF8E429243E', 'NHS Hillingdon CCG', '{"line1":"BOUNDARY HOUSE","line2":"CRICKET FIELD ROAD","town":"UXBRIDGE","county":"GREATER LONDON","postcode":"UB8 1QG","country":"ENGLAND"}', '08G', @ccgRoleId),
	('2F785B6C-9AF3-4397-85D5-039BB4297950', 'NHS Horsham and Mid Sussex CCG', '{"line1":"LOWER GROUND FLOOR, RED WING","line2":"CRAWLEY HOSPITAL","line3":"WEST GREEN DRIVE","town":"CRAWLEY","county":"WEST SUSSEX","postcode":"RH11 7DH","country":"ENGLAND"}', '09X', @ccgRoleId),
	('4F868F88-DE1F-4783-83B3-DC43B8EA5AB0', 'NHS Hounslow CCG', '{"line1":"HOUNSLOW HOUSE","line2":"5TH FLOOR","line3":"7 BATH ROAD","town":"HOUNSLOW","postcode":"TW3 3EB","country":"ENGLAND"}', '07Y', @ccgRoleId),
	('9173B946-793A-4B28-906D-270DB18DD82A', 'NHS Hull CCG', '{"line1":"WILBERFORCE COURT","line2":"ALFRED GELDER STREET","town":"HULL","postcode":"HU1 1UY","country":"ENGLAND"}', '03F', @ccgRoleId),
	('8543186E-44FA-4EC5-8BC8-961773A76AA3', 'NHS Ipswich and East Suffolk CCG', '{"line1":"ENDEAVOUR HOUSE","line2":"RUSSELL ROAD","town":"IPSWICH","postcode":"IP1 2BX","country":"ENGLAND"}', '06L', @ccgRoleId),
	('E9964B99-5863-46C9-9DE1-1EB22EEEDF0A', 'NHS Isle of Wight CCG', '{"line1":"UNIT A","line2":"THE APEX","line3":"ST CROSS BUSINESS PARK","town":"NEWPORT","postcode":"PO30 5XW","country":"ENGLAND"}', '10L', @ccgRoleId),
	('6412844B-D3DE-45DE-B71E-CD07B2069FAB', 'NHS Islington CCG', '{"line1":"2ND FLOOR","line2":"LAYCOCK PROFESSIONAL DEVELOPMENT","line3":"LAYCOCK STREET","town":"LONDON","county":"GREATER LONDON","postcode":"N1 1TH","country":"ENGLAND"}', '08H', @ccgRoleId),
	('046ACA25-40E6-44F2-BB7D-F4C57734C7BE', 'NHS Kent and Medway CCG', '{"line1":"WHARF HOUSE","line2":"MEDWAY WHARF ROAD","town":"TONBRIDGE","postcode":"TN9 1RE","country":"ENGLAND"}', '91Q', @ccgRoleId),
	('AC9825AF-0FD6-4E35-9A51-D50BAAD4FCA8', 'NHS Kernow CCG', '{"line1":"SEDGEMOOR CENTRE","line2":"PRIORY ROAD","town":"ST. AUSTELL","postcode":"PL25 5AS","country":"ENGLAND"}', '11N', @ccgRoleId),
	('1C22BD01-F6C3-4F3B-B347-668AD95B7A63', 'NHS Kingston CCG', '{"line1":"3RD FLOOR, GUILDHALL 1","line2":"HIGH STREET","town":"KINGSTON UPON THAMES","county":"SURREY","postcode":"KT1 1EU","country":"ENGLAND"}', '08J', @ccgRoleId),
	('8BCA69C1-7E40-4DEC-929E-FCD28A0C3B72', 'NHS Knowsley CCG', '{"line1":"NUTGROVE VILLA","line2":"WESTMORLAND ROAD","line3":"HUYTON","town":"LIVERPOOL","postcode":"L36 6GA","country":"ENGLAND"}', '01J', @ccgRoleId),
	('2AB67B87-3232-41FD-9065-D771B274ABE1', 'NHS Lambeth CCG', '{"line1":"1 LOWER MARSH","line2":"WATERLOO","town":"LONDON","county":"GREATER LONDON","postcode":"SE1 7NT","country":"ENGLAND"}', '08K', @ccgRoleId),
	('CFD987AC-9FAB-4E6E-BC66-3620B81EC192', 'NHS Leeds CCG', '{"line1":"SUITES 2 - 4 WIRA HOUSE","line2":"RING ROAD","line3":"WEST PARK","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS16 6EB","country":"ENGLAND"}', '15F', @ccgRoleId),
	('B908F25B-1ED6-41BE-AA32-D60B1A6141E0', 'NHS Leeds North CCG', '{"line1":"LEAFIELD HOUSE","line2":"107-109 KING LANE","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS17 5BP","country":"ENGLAND"}', '02V', @ccgRoleId),
	('FA20BA0F-1FAC-4B8C-9FBF-4B4AF5C9FEBA', 'NHS Leeds South and East CCG', '{"line1":"3200 CENTURY WAY","line2":"THORPE PARK","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS15 8ZB","country":"ENGLAND"}', '03G', @ccgRoleId),
	('F5DF8A2C-5E22-4368-8400-239DD141C0BE', 'NHS Leeds West CCG', '{"line1":"UNITS 2-4 WIRA BUSINESS PARK","line2":"RING ROAD","line3":"WEST PARK","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS16 6EB","country":"ENGLAND"}', '03C', @ccgRoleId),
	('2566656F-3E08-4022-8D1C-E2204F458C27', 'NHS Leicester City CCG', '{"line1":"ST JOHNS HOUSE","line2":"30 EAST STREET","town":"LEICESTER","postcode":"LE1 6NB","country":"ENGLAND"}', '04C', @ccgRoleId),
	('1C0B8B11-2935-4192-9849-0AD207450687', 'NHS Lewisham CCG', '{"line1":"CANTILEVER HOUSE","line2":"ELTHAM ROAD","line3":"LEE","town":"LONDON","county":"GREATER LONDON","postcode":"SE12 8RN","country":"ENGLAND"}', '08L', @ccgRoleId),
	('3F6C40C8-1EB5-48D5-93EF-9387DE56FAD1', 'NHS Lincolnshire CCG', '{"line1":"BRIDGE HOUSE","line2":"THE POINT","line3":"LIONS WAY","town":"SLEAFORD","postcode":"NG34 8GG","country":"ENGLAND"}', '71E', @ccgRoleId),
	('C5539B0A-28F6-44BC-AF88-CAEDAE5C6B4A', 'NHS Lincolnshire East CCG', '{"line1":"CROSS O CLIFF","line2":"BRACEBRIDGE HEATH","town":"LINCOLN","postcode":"LN4 2HN","country":"ENGLAND"}', '03T', @ccgRoleId),
	('9518471B-59EF-40A3-AAEE-FB11669AE8AF', 'NHS Lincolnshire West CCG', '{"line1":"CROSS O CLIFF","line2":"BRACEBRIDGE HEATH","town":"LINCOLN","postcode":"LN4 2HN","country":"ENGLAND"}', '04D', @ccgRoleId),
	('B08BAF2C-2B79-4858-A6A5-25E71FB6CFEA', 'NHS Liverpool CCG', '{"line1":"2 RENSHAW STREET","town":"LIVERPOOL","postcode":"L1 2SA","country":"ENGLAND"}', '99A', @ccgRoleId),
	('C0AF0368-FDFD-4486-8580-506F0E1C15E5', 'NHS Luton CCG', '{"line1":"3RD FLOOR","line2":"ARNDALE HOUSE","line3":"THE MALL","town":"LUTON","postcode":"LU1 2LJ","country":"ENGLAND"}', '06P', @ccgRoleId),
	('238149C5-4EEE-4F5A-8B03-5D9ABE6A2C98', 'NHS Manchester CCG', '{"line1":"PARKWAY 3","line2":"PARKWAY BUSINESS CENTRE","line3":"PRINCESS ROAD","town":"MANCHESTER","county":"GREATER MANCHESTER","postcode":"M14 7LU","country":"ENGLAND"}', '14L', @ccgRoleId),
	('3249061B-968D-4070-B475-B91DCDC651B2', 'NHS Mansfield and Ashfield CCG', '{"line1":"HAWTHORN HOUSE, RANSOM WOOD B PARK","line2":"SOUTHWELL ROAD WEST","line3":"RAINWORTH","town":"MANSFIELD","county":"NOTTINGHAMSHIRE","postcode":"NG21 0HJ","country":"ENGLAND"}', '04E', @ccgRoleId),
	('AC9B5AE4-4D3A-487D-89BE-8F562F2ABF93', 'NHS Medway CCG', '{"line1":"50 PEMBROKE COURT","line2":"CHATHAM MARITIME","town":"CHATHAM","county":"KENT","postcode":"ME4 4EL","country":"ENGLAND"}', '09W', @ccgRoleId),
	('D716C751-67E2-4456-8E56-6E18BD39B95E', 'NHS Merton CCG', '{"line1":"120 THE BROADWAY","line2":"WIMBLEDON","town":"LONDON","county":"GREATER LONDON","postcode":"SW19 1RH","country":"ENGLAND"}', '08R', @ccgRoleId),
	('EC228CD8-CB46-41DA-88B8-3BFB203C4835', 'NHS Mid Essex CCG', '{"line1":"WREN HOUSE","line2":"HEDGEROWS BUSINESS PARK","line3":"COLCHESTER ROAD","town":"CHELMSFORD","postcode":"CM2 5PF","country":"ENGLAND"}', '06Q', @ccgRoleId),
	('DE9F3E4F-CDBB-43BE-AF2A-1A4A54377784', 'NHS Milton Keynes CCG', '{"line1":"SHERWOOD PLACE","line2":"155 SHERWOOD DRIVE","line3":"BLETCHLEY","town":"MILTON KEYNES","postcode":"MK3 6RT","country":"ENGLAND"}', '04F', @ccgRoleId),
	('BA2C7710-91AE-4822-AEE6-92C262040C28', 'NHS Morecambe Bay CCG', '{"line1":"MOOR LANE MILLS","line2":"MOOR LANE","town":"LANCASTER","postcode":"LA1 1QD","country":"ENGLAND"}', '01K', @ccgRoleId),
	('D49BAEB1-1EDA-4308-9D69-A01C5435C425', 'NHS Nene CCG', '{"line1":"FRANCIS CRICK HOUSE","line2":"SUMMERHOUSE ROAD","line3":"MOULTON PARK INDUSTRIAL ESTATE","town":"NORTHAMPTON","county":"NORTHAMPTONSHIRE","postcode":"NN3 6BF","country":"ENGLAND"}', '04G', @ccgRoleId),
	('DE8642FA-B710-46C8-8E70-1196B6BECADA', 'NHS Newark and Sherwood CCG', '{"line1":"BALDERTON PRIMARY CARE CENTRE","line2":"LOWFIELD LANE","line3":"BALDERTON","town":"NEWARK","county":"NOTTINGHAMSHIRE","postcode":"NG24 3HJ","country":"ENGLAND"}', '04H', @ccgRoleId),
	('FAB36081-DA95-4F92-B62A-5A9C0EBF747F', 'NHS Newbury and District CCG', '{"line1":"23A KINGFISHER COURT","line2":"HAMBRIDGE ROAD","town":"NEWBURY","county":"BERKSHIRE","postcode":"RG14 5SJ","country":"ENGLAND"}', '10M', @ccgRoleId),
	('FE7F0C71-781D-4B7C-9214-9C67F6A368C1', 'NHS Newcastle Gateshead CCG', '{"line1":"RIVERSIDE HOUSE","line2":"GOLDCREST WAY","town":"NEWCASTLE UPON TYNE","postcode":"NE15 8NY","country":"ENGLAND"}', '13T', @ccgRoleId),
	('6AE1F969-04C3-4C32-AC30-A3B920F27ACC', 'NHS Newcastle North and East CCG', '{"line1":"RIVERSIDE HOUSE","line2":"GOLDCREST WAY","line3":"NEWBURN RIVERSIDE","town":"NEWCASTLE UPON TYNE","county":"TYNE AND WEAR","postcode":"NE15 8NY","country":"ENGLAND"}', '00G', @ccgRoleId),
	('0F69ED74-992F-43BA-B2F8-7765E787F555', 'NHS Newcastle West CCG', '{"line1":"RIVERSIDE HOUSE","line2":"GOLDCREST WAY","line3":"NEWBURN RIVERSIDE","town":"NEWCASTLE UPON TYNE","county":"TYNE AND WEAR","postcode":"NE15 8NY","country":"ENGLAND"}', '00H', @ccgRoleId),
	('44DC7B02-ACA2-45C7-A24F-FE1887B9DB74', 'NHS Newham CCG', '{"line1":"4TH FLOOR","line2":"UNEX TOWER","line3":"5 STATION STREET","town":"LONDON","postcode":"E15 1DA","country":"ENGLAND"}', '08M', @ccgRoleId),
	('A71CCDFD-EBBF-4BAB-97D1-E3743492F366', 'NHS Norfolk and Waveney CCG', '{"line1":"LAKESIDE 400","line2":"OLD CHAPEL WAY","line3":"BROADLAND BUSINESS PARK","town":"NORWICH","postcode":"NR7 0WG","country":"ENGLAND"}', '26A', @ccgRoleId),
	('C2573C16-2AE2-4772-B336-6DAE04047E8C', 'NHS Northamptonshire CCG', '{"line1":"FRANCIS CRICK HOUSE","line2":"6 SUMMERHOUSE ROAD","line3":"MOULTON PARK INDUSTRIAL ESTATE","town":"NORTHAMPTON","county":"NORTHAMPTONSHIRE","postcode":"NN3 6BF","country":"ENGLAND"}', '78H', @ccgRoleId),
	('5D0D3A93-6FF5-4FF7-ADC4-E772C99F8131', 'NHS North and West Reading CCG', '{"line1":"57-59 BATH ROAD","town":"READING","county":"BERKSHIRE","postcode":"RG30 2BA","country":"ENGLAND"}', '10N', @ccgRoleId),
	('C3A0B00C-C4FD-4132-8C40-D9B7D16A0605', 'NHS North Central London CCG', '{"line1":"2ND FLOOR","line2":"LAYCOCK STREET","town":"LONDON","county":"GREATER LONDON","postcode":"N1 1TH","country":"ENGLAND"}', '93C', @ccgRoleId),
	('118D3953-7B72-47B7-B855-272E35B09FD4', 'NHS North Cumbria CCG', '{"line1":"4 WAVELL DRIVE","line2":"ROSEHILL INDUSTRIAL ESTATE","town":"CARLISLE","postcode":"CA1 2SE","country":"ENGLAND"}', '01H', @ccgRoleId),
	('1D67D429-BF56-4B35-9A4B-9C6052EE46FF', 'NHS North Derbyshire CCG', '{"line1":"SCARSDALE HOSPITAL","line2":"NIGHTINGALE CLOSE","line3":"OFF NEWBOLD ROAD","town":"CHESTERFIELD","county":"DERBYSHIRE","postcode":"S41 7PF","country":"ENGLAND"}', '04J', @ccgRoleId),
	('B5F2F760-6048-429B-B385-D761B75F9EBB', 'NHS North Durham CCG', '{"line1":"SEDGEFIELD COMMUNITY HOSPITAL","line2":"SALTERS LANE","line3":"SEDGEFIELD","town":"STOCKTON-ON-TEES","county":"CLEVELAND","postcode":"TS21 3EE","country":"ENGLAND"}', '00J', @ccgRoleId),
	('56D51912-2ED4-4CBB-95CA-F844893BA305', 'NHS North East Essex CCG', '{"line1":"ASPEN HOUSE","line2":"STEPHENSON ROAD","line3":"SEVERALLS BUSINESS PARK","town":"COLCHESTER","county":"ESSEX","postcode":"CO4 9QR","country":"ENGLAND"}', '06T', @ccgRoleId),
	('B898D45C-3A69-4A76-B176-BABFAD70D56E', 'NHS North East Hampshire and Farnham CCG', '{"line1":"ALDERSHOT CENTRE FOR HEALTH","line2":"HOSPITAL HILL","town":"ALDERSHOT","postcode":"GU11 1AY","country":"ENGLAND"}', '99M', @ccgRoleId),
	('3806EE7F-1C5E-4431-AB28-F7610C12CE0A', 'NHS North East Lincolnshire CCG', '{"line1":"MUNICIPAL OFFICES","line2":"TOWN HALL SQUARE","town":"GRIMSBY","postcode":"DN31 1HU","country":"ENGLAND"}', '03H', @ccgRoleId),
	('1A1F2424-5AA5-4E71-83AA-F13AAA18D16E', 'NHS Northern, Eastern and Western Devon CCG', '{"line1":"NEWCOURT HOUSE","line2":"OLD RYDON LANE","town":"EXETER","county":"DEVON","postcode":"EX2 7JU","country":"ENGLAND"}', '99P', @ccgRoleId),
	('CE4D11C9-0AC7-4AD8-A703-8D9F4CBD6784', 'NHS North Hampshire CCG', '{"line1":"CENTRAL 40 LIME TREE WAY","line2":"CROCKFORD LANE","line3":"CHINEHAM BUSINESS PARK, CHINEHAM","town":"BASINGSTOKE","county":"HAMPSHIRE","postcode":"RG24 8GU","country":"ENGLAND"}', '10J', @ccgRoleId),
	('B2C3FA32-EAF9-4C60-B690-F283E040153C', 'NHS North Kirklees CCG', '{"line1":"4TH FLOOR","line2":"EMPIRE HOUSE","line3":"WAKEFIELD ROAD","town":"DEWSBURY","county":"WEST YORKSHIRE","postcode":"WF12 8DJ","country":"ENGLAND"}', '03J', @ccgRoleId),
	('B8948CCE-F73B-479D-AC23-3EBA3F284184', 'NHS North Lincolnshire CCG', '{"line1":"THE HEALTH PLACE","line2":"WRAWBY ROAD","town":"BRIGG","postcode":"DN20 8GS","country":"ENGLAND"}', '03K', @ccgRoleId),
	('6714FB68-DD79-4FCF-B0C1-F4395462391F', 'NHS North Manchester CCG', '{"line1":"2ND FLOOR SILK MILL","line2":"HOLYOAK STREET","line3":"NEWTON HEATH","town":"MANCHESTER","county":"GREATER MANCHESTER","postcode":"M40 1HA","country":"ENGLAND"}', '01M', @ccgRoleId),
	('32ECDC34-E917-4EFD-87C1-2271897BDB74', 'NHS North Norfolk CCG', '{"line1":"LAKESIDE 400","line2":"OLD CHAPEL WAY","line3":"BROADLAND BUSINESS PARK","town":"NORWICH","postcode":"NR7 0WG","country":"ENGLAND"}', '06V', @ccgRoleId),
	('795C7795-D2F9-414C-967E-D58659349E1A', 'NHS North Somerset CCG', '{"line1":"CASTLEWOOD","line2":"TICKENHAM ROAD","town":"CLEVEDON","county":"AVON","postcode":"BS21 6FW","country":"ENGLAND"}', '11T', @ccgRoleId),
	('F5C31F2C-3BCC-4446-B0F1-FBE1FE913613', 'NHS North Staffordshire CCG', '{"line1":"MORSTON HOUSE","line2":"THE MIDWAY","town":"NEWCASTLE","county":"STAFFORDSHIRE","postcode":"ST5 1QG","country":"ENGLAND"}', '05G', @ccgRoleId),
	('68219CAC-1647-4877-8A6F-337552D88DCA', 'NHS North Tyneside CCG', '{"line1":"12 HEDLEY COURT","line2":"ORION BUSINESS PARK","town":"NORTH SHIELDS","postcode":"NE29 7ST","country":"ENGLAND"}', '99C', @ccgRoleId),
	('0BAE8F4C-6428-4751-8050-59EF35A7FE80', 'NHS Northumberland CCG', '{"line1":"COUNTY HALL","town":"MORPETH","postcode":"NE61 2EF","country":"ENGLAND"}', '00L', @ccgRoleId),
	('6E9527B9-F634-440B-A279-9E6CD68D99CE', 'NHS North West Surrey CCG', '{"line1":"58 CHURCH STREET","town":"WEYBRIDGE","county":"SURREY","postcode":"KT13 8DP","country":"ENGLAND"}', '09Y', @ccgRoleId),
	('81D3F90D-CE84-484D-A15D-428225CE9A5B', 'NHS North Yorkshire CCG', '{"line1":"1 GRIMBALD CRAG COURT","town":"KNARESBOROUGH","postcode":"HG5 8QB","country":"ENGLAND"}', '42D', @ccgRoleId),
	('8957F91F-ADDE-40ED-BFC3-8183E9C09B70', 'NHS Norwich CCG', '{"line1":"ROOM 202","line2":"NORWICH CITY HALL","line3":"ST PETERS STREET","town":"NORWICH","postcode":"NR2 1NH","country":"ENGLAND"}', '06W', @ccgRoleId),
	('CC10227C-318C-46AB-B23E-3F87D240E490', 'NHS Nottingham and Nottinghamshire CCG', '{"line1":"BIRCH HOUSE","line2":"RANSOM WOOD BUSINESS PARK","line3":"SOUTHWELL ROAD WEST","town":"MANSFIELD","postcode":"NG21 0HJ","country":"ENGLAND"}', '52R', @ccgRoleId),
	('042A6173-1313-4085-92B1-DA6EAF41A63B', 'NHS Nottingham City CCG', '{"line1":"1 STANDARD COURT","line2":"PARK ROW","town":"NOTTINGHAM","county":"NOTTINGHAMSHIRE","postcode":"NG1 6GN","country":"ENGLAND"}', '04K', @ccgRoleId),
	('7F440BA0-55A2-493A-8782-CC56BED81C04', 'NHS Nottingham North and East CCG', '{"line1":"GEDLING CIVIC CENTRE","line2":"ARNOT HILL PARK","line3":"ARNOLD","town":"NOTTINGHAM","county":"NOTTINGHAMSHIRE","postcode":"NG5 0TE","country":"ENGLAND"}', '04L', @ccgRoleId),
	('36B2B3EF-66EB-4E0E-8735-F6B5BF98A035', 'NHS Nottingham West CCG', '{"line1":"STAPLEFORD CARE CENTRE","line2":"CHURCH STREET","line3":"STAPLEFORD","town":"NOTTINGHAM","county":"NOTTINGHAMSHIRE","postcode":"NG9 8DB","country":"ENGLAND"}', '04M', @ccgRoleId),
	('998A8180-569F-4AA2-B895-2EA44DEC67F3', 'NHS Oldham CCG', '{"line1":"ELLEN HOUSE","line2":"WADDINGTON STREET","town":"OLDHAM","postcode":"OL9 6EE","country":"ENGLAND"}', '00Y', @ccgRoleId),
	('ADDD2F2B-B025-451A-A6C6-4C8977B0FC2D', 'NHS Oxfordshire CCG', '{"line1":"JUBILEE HOUSE","line2":"5510 JOHN SMITH DRIVE","line3":"OXFORD BUSINESS PARK SOUTH","town":"OXFORD","postcode":"OX4 2LH","country":"ENGLAND"}', '10Q', @ccgRoleId),
	('B427B584-2894-479D-B3FE-8C6C5C4A0A71', 'NHS Portsmouth CCG', '{"line1":"4TH FLOOR","line2":"1 GUILDHALL SQUARE","town":"PORTSMOUTH","county":"HAMPSHIRE","postcode":"PO1 2GJ","country":"ENGLAND"}', '10R', @ccgRoleId),
	('2112DAAF-A9A9-460B-800E-5219B9C2B9E7', 'NHS Redbridge CCG', '{"line1":"6TH FLOOR","line2":"NORTH HOUSE","line3":"ST EDWARDS WAY","town":"ROMFORD","postcode":"RM1 3AE","country":"ENGLAND"}', '08N', @ccgRoleId),
	('9D426A4C-F5F5-48D6-8C7A-B873B1FC8D03', 'NHS Redditch and Bromsgrove CCG', '{"line1":"BARNSLEY COURT","line2":"BARNSLEY HALL ROAD","town":"BROMSGROVE","county":"WORCESTERSHIRE","postcode":"B61 0TX","country":"ENGLAND"}', '05J', @ccgRoleId),
	('9A800A72-7F7C-4B14-85A6-4D74A634B32A', 'NHS Richmond CCG', '{"line1":"CIVIC CENTRE","line2":"44 YORK STREET","town":"TWICKENHAM","county":"MIDDLESEX","postcode":"TW1 3BZ","country":"ENGLAND"}', '08P', @ccgRoleId),
	('097728C8-0059-4E05-BE3F-469B57632C06', 'NHS Rotherham CCG', '{"line1":"OAK HOUSE","line2":"MOORHEAD WAY","line3":"BRAMLEY","town":"ROTHERHAM","postcode":"S66 1YY","country":"ENGLAND"}', '03L', @ccgRoleId),
	('D0B2CDD8-1CC8-4A70-8F89-A0E3F0EA4051', 'NHS Rushcliffe CCG', '{"line1":"EASTHORPE HOUSE","line2":"165 LOUGHBOROUGH ROAD","line3":"RUDDINGTON","town":"NOTTINGHAM","county":"NOTTINGHAMSHIRE","postcode":"NG11 6LQ","country":"ENGLAND"}', '04N', @ccgRoleId),
	('89F06E2A-CC78-41C7-AC3C-8A136B306926', 'NHS Salford CCG', '{"line1":"ST JAMES HOUSE - 7TH FLOOR","line2":"PENDLETON WAY","town":"SALFORD","postcode":"M6 5FW","country":"ENGLAND"}', '01G', @ccgRoleId),
	('5FA5D199-5F53-4DED-A798-1ECDBAE7B9A1', 'NHS Sandwell and West Birmingham CCG', '{"line1":"KINGSTON HOUSE","line2":"438-450 HIGH STREET","town":"WEST BROMWICH","postcode":"B70 9LD","country":"ENGLAND"}', '05L', @ccgRoleId),
	('D7D36877-CC53-4F04-8B34-DED82C4EC52F', 'NHS Scarborough and Ryedale CCG', '{"line1":"SCARBOROUGH TOWN HALL","line2":"ST NICHOLAS STREET","town":"SCARBOROUGH","county":"NORTH YORKSHIRE","postcode":"YO11 2HG","country":"ENGLAND"}', '03M', @ccgRoleId),
	('08524741-3EFD-4BAD-A84F-7579850CBEF8', 'NHS Sheffield CCG', '{"line1":"722 PRINCE OF WALES ROAD","line2":"DARNALL","town":"SHEFFIELD","postcode":"S9 4EU","country":"ENGLAND"}', '03N', @ccgRoleId),
	('4776EE2E-D441-4A68-AA9A-9223AFE0B73D', 'NHS Shropshire CCG', '{"line1":"WILLIAM FARR HOUSE","line2":"MYTTON OAK ROAD","town":"SHREWSBURY","county":"SHROPSHIRE","postcode":"SY3 8XL","country":"ENGLAND"}', '05N', @ccgRoleId),
	('9165F831-2CD0-4BD7-9BBF-CD475103C512', 'NHS Slough CCG', '{"line1":"UPTON HOSPITAL","line2":"ALBERT STREET","town":"SLOUGH","county":"BERKSHIRE","postcode":"SL1 2BJ","country":"ENGLAND"}', '10T', @ccgRoleId),
	('7AB32F09-EEA9-46C0-AB05-D6CAB146C930', 'NHS Solihull CCG', '{"line1":"FRIARS GATE","line2":"1011 STRATFORD ROAD","line3":"SHIRLEY","town":"SOLIHULL","county":"WEST MIDLANDS","postcode":"B90 4BN","country":"ENGLAND"}', '05P', @ccgRoleId),
	('BE3365F2-DCE3-48D2-A5B4-016BB22571BE', 'NHS Somerset CCG', '{"line1":"WYNFORD HOUSE","line2":"LUFTON WAY","line3":"LUFTON","town":"YEOVIL","postcode":"BA22 8HR","country":"ENGLAND"}', '11X', @ccgRoleId),
	('76D17D73-4C80-4C29-B611-DEDFE5675A80', 'NHS Southampton CCG', '{"line1":"OAKLEY ROAD","town":"SOUTHAMPTON","postcode":"SO16 4GX","country":"ENGLAND"}', '10X', @ccgRoleId),
	('C916124E-6E15-4847-982A-C176E36064AB', 'NHS South Cheshire CCG', '{"line1":"BEVAN HOUSE","line2":"BARONY COURT","town":"NANTWICH","county":"CHESHIRE","postcode":"CW5 5QU","country":"ENGLAND"}', '01R', @ccgRoleId),
	('24059963-4BA2-4EDB-BA63-2B13464C3A8B', 'NHS South Devon and Torbay CCG', '{"line1":"POMONA HOUSE","line2":"OAK VIEW CLOSE","town":"TORQUAY","county":"DEVON","postcode":"TQ2 7FF","country":"ENGLAND"}', '99Q', @ccgRoleId),
	('EECB0C7F-4C66-48A0-B7A8-B5139FAE9C74', 'NHS South Eastern Hampshire CCG', '{"line1":"COMMCEN BUILDING","line2":"FORT SOUTHWICK","line3":"JAMES CALLAGHAN DRIVE","town":"FAREHAM","postcode":"PO17 6AR","country":"ENGLAND"}', '10V', @ccgRoleId),
	('61ED577E-89CF-437A-9C3E-68451BE31B36', 'NHS South East London CCG', '{"line1":"160 TOOLEY STREET","town":"LONDON","postcode":"SE1 2TZ","country":"ENGLAND"}', '72Q', @ccgRoleId),
	('7634195F-C33C-4DC8-9DAE-6FB66671987B', 'NHS South East Staffordshire and Seisdon Peninsula CCG', '{"line1":"MERLIN HOUSE","line2":"BITTERSCOTE","line3":"ETCHELL ROAD","town":"TAMWORTH","county":"STAFFORDSHIRE","postcode":"B78 3HF","country":"ENGLAND"}', '05Q', @ccgRoleId),
	('1387BF3E-59B6-412F-9B56-4676B6408AE7', 'NHS Southend CCG', '{"line1":"FLOOR 6","line2":"CIVIC CENTRE","line3":"VICTORIA AVENUE","town":"SOUTHEND-ON-SEA","county":"ESSEX","postcode":"SS2 6EN","country":"ENGLAND"}', '99G', @ccgRoleId),
	('67460358-D1B1-499B-A0C8-0B0F0953EC94', 'NHS Southern Derbyshire CCG', '{"line1":"CARDINAL SQUARE","line2":"1ST FLOOR, NORTH POINT","line3":"10 NOTTINGHAM ROAD","town":"DERBY","county":"DERBYSHIRE","postcode":"DE1 3QT","country":"ENGLAND"}', '04R', @ccgRoleId),
	('4696011C-F79E-4EDC-AC43-496DFFEB3900', 'NHS South Gloucestershire CCG', '{"line1":"CORUM 2, CORUM OFFICE PARK","line2":"CROWN WAY","line3":"WARMLEY","town":"BRISTOL","county":"AVON","postcode":"BS30 8FJ","country":"ENGLAND"}', '12A', @ccgRoleId),
	('D3FED490-A3D5-483C-BDE6-BE6334C8C571', 'NHS South Kent Coast CCG', '{"line1":"DOVER DISTRICT COUNCIL","line2":"HONEYWOOD CLOSE","line3":"WHITFIELD","town":"DOVER","county":"KENT","postcode":"CT16 3PJ","country":"ENGLAND"}', '10A', @ccgRoleId),
	('9BB824F6-52B0-41CE-840F-9BF0705CE523', 'NHS South Lincolnshire CCG', '{"line1":"BRIDGE HOUSE","line2":"THE POINT","line3":"LIONS WAY","town":"SLEAFORD","postcode":"NG34 8GG","country":"ENGLAND"}', '99D', @ccgRoleId),
	('76F261A5-14AF-4FD1-B3B9-6A0F60240014', 'NHS South Manchester CCG', '{"line1":"PARKWAY THREE","line2":"PARKWAY BUSINESS CENTRE","line3":"PRINCESS ROAD","town":"MANCHESTER","county":"GREATER MANCHESTER","postcode":"M14 7LU","country":"ENGLAND"}', '01N', @ccgRoleId),
	('02277D73-4CF1-4EF0-BB83-A6C57CFD0901', 'NHS South Norfolk CCG', '{"line1":"LAKESIDE 400","line2":"OLD CHAPEL WAY","line3":"BROADLAND BUSINESS PARK","town":"NORWICH","postcode":"NR7 0WG","country":"ENGLAND"}', '06Y', @ccgRoleId),
	('8C455180-954B-41C3-B443-EB302D53A546', 'NHS Southport and Formby CCG', '{"line1":"5 CURZON ROAD","town":"SOUTHPORT","county":"MERSEYSIDE","postcode":"PR8 6PL","country":"ENGLAND"}', '01V', @ccgRoleId),
	('F8743DFA-5978-4987-A406-FE8440CC08A1', 'NHS South Reading CCG', '{"line1":"UNIVERSITY MEDICAL CENTRE","line2":"9-11 NORTHCOURT AVENUE","town":"READING","county":"BERKSHIRE","postcode":"RG2 7HE","country":"ENGLAND"}', '10W', @ccgRoleId),
	('CD1CB89C-0483-4B00-A101-7327734A7602', 'NHS South Sefton CCG', '{"line1":"3RD FLOOR","line2":"MERTON HOUSE","line3":"STANLEY ROAD","town":"BOOTLE","postcode":"L20 3DL","country":"ENGLAND"}', '01T', @ccgRoleId),
	('12E735D2-CF98-4AAA-BCD1-CF8ABF8F9F53', 'NHS South Tees CCG', '{"line1":"NORTH ORMESBY HEALTH VILLAGE","line2":"11 TRINITY MEWS","line3":"NORTH ORMESBY","town":"MIDDLESBROUGH","county":"CLEVELAND","postcode":"TS3 6AL","country":"ENGLAND"}', '00M', @ccgRoleId),
	('6C5772CA-0BA7-4DF4-B1F0-7592FD76DFD0', 'NHS South Tyneside CCG', '{"line1":"MONKTON HALL","line2":"MONKTON LANE","line3":"MONKTON VILLAGE","town":"JARROW","postcode":"NE32 5NN","country":"ENGLAND"}', '00N', @ccgRoleId),
	('1AF36DC2-FDD2-4409-B732-F74BB31745DE', 'NHS Southwark CCG', '{"line1":"160 TOOLEY STREET","town":"LONDON","county":"GREATER LONDON","postcode":"SE1 2QH","country":"ENGLAND"}', '08Q', @ccgRoleId),
	('E93B5C6F-BB52-4D43-8BAD-1E5342E2BD18', 'NHS South Warwickshire CCG', '{"line1":"WESTGATE HOUSE","line2":"21 MARKET STREET","town":"WARWICK","county":"WARWICKSHIRE","postcode":"CV34 4DE","country":"ENGLAND"}', '05R', @ccgRoleId),
	('4D7F4B95-1376-4952-8BAD-F9B10D88B01B', 'NHS South West Lincolnshire CCG', '{"line1":"BRIDGE HOUSE","line2":"THE POINT","line3":"LIONS WAY","town":"SLEAFORD","postcode":"NG34 8GG","country":"ENGLAND"}', '04Q', @ccgRoleId),
	('A109827D-FD51-4BF0-87A7-E852073C1081', 'NHS South West London CCG', '{"line1":"120 THE BROADWAY","town":"LONDON","county":"GREATER LONDON","postcode":"SW19 1RH","country":"ENGLAND"}', '36L', @ccgRoleId),
	('209669C4-E49E-462F-B92C-B4D56255A1E8', 'NHS South Worcestershire CCG', '{"line1":"GROUND FLOOR, WEST WING","line2":"WORCESTERSHIRE COUNTRYSIDE CENTRE","line3":"WILDWOOD DRIVE","town":"WORCESTER","county":"WORCESTERSHIRE","postcode":"WR5 2LG","country":"ENGLAND"}', '05T', @ccgRoleId),
	('BCE87B68-477E-4D95-B76A-5696C9F8A459', 'NHS Stafford and Surrounds CCG', '{"line1":"STAFFORDSHIRE PLACE 2","town":"STAFFORD","county":"STAFFORDSHIRE","postcode":"ST16 2LP","country":"ENGLAND"}', '05V', @ccgRoleId),
	('DAEB7DFF-2E7D-4F17-B8FF-D9A2D222A2E4', 'NHS St Helens CCG', '{"line1":"ST HELENS CHAMBER OF COMMERCE","line2":"SALISBURY STREET","line3":"CHALON WAY","town":"ST. HELENS","county":"MERSEYSIDE","postcode":"WA10 1FY","country":"ENGLAND"}', '01X', @ccgRoleId),
	('C63830EB-ED7D-410D-B48A-1C04A6C25ECB', 'NHS Stockport CCG', '{"line1":"4TH FLOOR","line2":"STOPFORD HOUSE","town":"STOCKPORT","county":"CHESHIRE","postcode":"SK1 3XE","country":"ENGLAND"}', '01W', @ccgRoleId),
	('1EEB194D-6A42-47FC-BD1F-190C42351028', 'NHS Stoke On Trent CCG', '{"line1":"HERBERT MINTON BUILDING","line2":"79 LONDON ROAD","town":"STOKE-ON-TRENT","county":"STAFFORDSHIRE","postcode":"ST4 7PZ","country":"ENGLAND"}', '05W', @ccgRoleId),
	('3BBB28C5-02F9-4CCA-A2DE-51FB2AD68156', 'NHS Sunderland CCG', '{"line1":"PEMBERTON HOUSE","line2":"COLIMA AVENUE","line3":"SUNDERLAND ENTERPRISE PARK","town":"SUNDERLAND","postcode":"SR5 3XB","country":"ENGLAND"}', '00P', @ccgRoleId),
	('8392075C-C8DA-4A6C-9B41-0287FCB4DDB5', 'NHS Surrey Downs CCG', '{"line1":"CEDAR COURT","line2":"36 GUILDFORD ROAD","line3":"FETCHAM","town":"LEATHERHEAD","postcode":"KT22 9AE","country":"ENGLAND"}', '99H', @ccgRoleId),
	('6AD948A1-DDC6-4D0E-A341-349FC0923FF3', 'NHS Surrey Heartlands CCG', '{"line1":"CEDAR COURT","line2":"GUILDFORD ROAD","line3":"FETCHAM","town":"LEATHERHEAD","postcode":"KT22 9AE","country":"ENGLAND"}', '92A', @ccgRoleId),
	('44135E79-D5C8-4C8D-B88B-2216F4A5CA65', 'NHS Surrey Heath CCG', '{"line1":"SURREY HEATH HOUSE","line2":"KNOLL ROAD","town":"CAMBERLEY","county":"SURREY","postcode":"GU15 3HD","country":"ENGLAND"}', '10C', @ccgRoleId),
	('C334DA54-7655-466D-8475-63452252D0BF', 'NHS Sutton CCG', '{"line1":"PRIORY CRESCENT","line2":"CHEAM","town":"SUTTON","county":"SURREY","postcode":"SM3 8LR","country":"ENGLAND"}', '08T', @ccgRoleId),
	('7F488418-53E1-40BD-BA52-8200C7E0923B', 'NHS Swale CCG', '{"line1":"BRAMBLEFIELD CLINIC","line2":"GROVEHURST ROAD","line3":"KEMSLEY","town":"SITTINGBOURNE","county":"KENT","postcode":"ME10 2ST","country":"ENGLAND"}', '10D', @ccgRoleId),
	('09736027-A522-40BB-9879-272AA122037F', 'NHS Swindon CCG', '{"line1":"THE PIERRE SIMONET BUILDING","line2":"GATEWAY NORTH","line3":"LATHAM ROAD","town":"SWINDON","county":"WILTSHIRE","postcode":"SN25 4DL","country":"ENGLAND"}', '12D', @ccgRoleId),
	('D8EDF13E-B910-4E12-BBED-C336DAD01121', 'NHS Tameside and Glossop CCG', '{"line1":"TAMESIDE ONE","line2":"MARKET PLACE","town":"ASHTON-UNDER-LYNE","county":"GREATER MANCHESTER","postcode":"OL6 6BH","country":"ENGLAND"}', '01Y', @ccgRoleId),
	('C60E653A-1881-40B1-97F4-652D93097ECA', 'NHS Tees Valley CCG', '{"line1":"NORTH ORMESBY HEALTH VILLAGE","line2":"FIRST FLOOR, 14 TRINITY MEWS","line3":"NORTH ORMESBY","town":"MIDDLESBROUGH","county":"NORTH YORKSHIRE","postcode":"TS3 6AL","country":"ENGLAND"}', '16C', @ccgRoleId),
	('75C969EC-FA84-4F92-A5F0-309E002A8E9B', 'NHS Telford and Wrekin CCG', '{"line1":"HALESFIELD 6","line2":"EPIC PARK","town":"TELFORD","county":"SHROPSHIRE","postcode":"TF7 4BF","country":"ENGLAND"}', '05X', @ccgRoleId),
	('5A1968B3-EBE6-49D7-BFD2-6AE7474E4AC8', 'NHS Thanet CCG', '{"line1":"THANET DISTRICT COUNCIL","line2":"CECIL STREET","town":"MARGATE","county":"KENT","postcode":"CT9 1XZ","country":"ENGLAND"}', '10E', @ccgRoleId),
	('1D898393-037B-477A-8D84-3B1A1E264C04', 'NHS Thurrock CCG', '{"line1":"CIVIC OFFICES","line2":"NEW ROAD","town":"GRAYS","county":"ESSEX","postcode":"RM17 6SL","country":"ENGLAND"}', '07G', @ccgRoleId),
	('9D9F57C6-98E4-4051-AE89-58CC12CEBFA0', 'NHS Tower Hamlets CCG', '{"line1":"2ND FLOOR ALDERNEY BUILDING","line2":"MILE END HOSPITAL","line3":"BANCROFT ROAD","town":"LONDON","postcode":"E1 4DG","country":"ENGLAND"}', '08V', @ccgRoleId),
	('D777A85C-6D2A-4B63-B547-1976A560B92C', 'NHS Trafford CCG', '{"line1":"1ST FLOOR TRAFFORD TOWN HALL","line2":"TALBOT ROAD","line3":"STRETFORD","town":"MANCHESTER","postcode":"M32 0TH","country":"ENGLAND"}', '02A', @ccgRoleId),
	('48EE4717-A630-4C47-9176-92A1CD3370A4', 'NHS Vale of York CCG', '{"line1":"WEST OFFICES","line2":"STATION RISE","town":"YORK","county":"NORTH YORKSHIRE","postcode":"YO1 6GA","country":"ENGLAND"}', '03Q', @ccgRoleId),
	('76017DEE-CA4B-4B34-925C-A8C605A6CE7C', 'NHS Vale Royal CCG', '{"line1":"BEVAN HOUSE","line2":"BARONY COURT","town":"NANTWICH","county":"CHESHIRE","postcode":"CW5 5QU","country":"ENGLAND"}', '02D', @ccgRoleId),
	('81CCDD55-765A-42A9-B9C8-451C64A9EBAB', 'NHS Wakefield CCG', '{"line1":"WHITE ROSE HOUSE","line2":"WEST PARADE","town":"WAKEFIELD","postcode":"WF1 1LT","country":"ENGLAND"}', '03R', @ccgRoleId),
	('24E6792A-04C7-4319-AA2E-17167B0679DE', 'NHS Walsall CCG', '{"line1":"JUBILEE HOUSE","line2":"BLOXWICH LANE","town":"WALSALL","postcode":"WS2 7JL","country":"ENGLAND"}', '05Y', @ccgRoleId),
	('DE739225-91E9-49ED-924D-3D26D02F8152', 'NHS Waltham Forest CCG', '{"line1":"KIRKDALE HOUSE","line2":"7 KIRKDALE ROAD","town":"LONDON","postcode":"E11 1HP","country":"ENGLAND"}', '08W', @ccgRoleId),
	('FAFC9D85-067E-4116-AC6F-2B0F57F8EE43', 'NHS Wandsworth CCG', '{"line1":"1ST FLOOR","line2":"73-75 UPPER RICHMOND ROAD","town":"LONDON","county":"GREATER LONDON","postcode":"SW15 2SR","country":"ENGLAND"}', '08X', @ccgRoleId),
	('8D934FA5-AF86-4D1F-B36D-A01384F81E93', 'NHS Warrington CCG', '{"line1":"110 BIRCHWOOD BOULEVARD","line2":"BIRCHWOOD","town":"WARRINGTON","county":"CHESHIRE","postcode":"WA3 7QH","country":"ENGLAND"}', '02E', @ccgRoleId),
	('34C733AA-FCEC-4EA1-9C86-68486ECAC9FB', 'NHS Warwickshire North CCG', '{"line1":"WESTGATE HOUSE","line2":"21 MARKET STREET","town":"WARWICK","county":"WARWICKSHIRE","postcode":"CV34 4DE","country":"ENGLAND"}', '05H', @ccgRoleId),
	('63D4BA91-4A02-46D4-8390-A41CF6E9829E', 'NHS West Cheshire CCG', '{"line1":"1829 BUILDING","line2":"THE COUNTESS OF CHESTER HEALTH PARK","line3":"LIVERPOOL ROAD","town":"CHESTER","county":"CHESHIRE","postcode":"CH2 1HJ","country":"ENGLAND"}', '02F', @ccgRoleId),
	('B0D7D853-80D6-460D-9AAA-7D61A68986D7', 'NHS West Essex CCG', '{"line1":"BUILDING 4, SPENCER CLOSE","line2":"ST MARGARET''S HOSPITAL","line3":"THE PLAIN","town":"EPPING","postcode":"CM16 6TN","country":"ENGLAND"}', '07H', @ccgRoleId),
	('19913662-CF6B-4A90-B9F6-7B75B303D7EE', 'NHS West Hampshire CCG', '{"line1":"OMEGA HOUSE","line2":"112 SOUTHAMPTON ROAD","town":"EASTLEIGH","postcode":"SO50 5PB","country":"ENGLAND"}', '11A', @ccgRoleId),
	('D20DA8A1-04E9-40B5-83FD-3E52D73754CE', 'NHS West Kent CCG', '{"line1":"WHARF HOUSE","line2":"MEDWAY WHARF ROAD","town":"TONBRIDGE","county":"KENT","postcode":"TN9 1RE","country":"ENGLAND"}', '99J', @ccgRoleId),
	('D9714D33-A62E-4E2F-8412-4BE4AB391CC1', 'NHS West Lancashire CCG', '{"line1":"HILLDALE","line2":"WIGAN ROAD","town":"ORMSKIRK","postcode":"L39 2JW","country":"ENGLAND"}', '02G', @ccgRoleId),
	('ED94E21C-32D8-4E13-BF70-54B24392C1DE', 'NHS West Leicestershire CCG', '{"line1":"55 WOODGATE","town":"LOUGHBOROUGH","postcode":"LE11 2TZ","country":"ENGLAND"}', '04V', @ccgRoleId),
	('48D5C694-C76C-47AD-9BE8-11C0A62F6B98', 'NHS West London CCG', '{"line1":"15 MARYLEBONE ROAD","town":"LONDON","postcode":"NW1 5JD","country":"ENGLAND"}', '08Y', @ccgRoleId),
	('1A748357-249E-4022-83CC-267D6362BD02', 'NHS West Norfolk CCG', '{"line1":"KINGS COURT","line2":"CHAPEL STREET","town":"KING''S LYNN","postcode":"PE30 1EL","country":"ENGLAND"}', '07J', @ccgRoleId),
	('54794D08-9296-4978-9939-B05C9CD7F495', 'NHS West Suffolk CCG', '{"line1":"WEST SUFFOLK HOUSE","line2":"WESTERN WAY","town":"BURY ST. EDMUNDS","county":"SUFFOLK","postcode":"IP33 3YU","country":"ENGLAND"}', '07K', @ccgRoleId),
	('50038033-F4DB-44A9-B6C9-634329F25481', 'NHS West Sussex CCG', '{"line1":"WICKER HOUSE","line2":"HIGH STREET","town":"WORTHING","postcode":"BN11 1DJ","country":"ENGLAND"}', '70F', @ccgRoleId),
	('8AEE2A71-066D-487B-952D-FCF727C4EAC6', 'NHS Wigan Borough CCG', '{"line1":"WIGAN LIFE CENTRE","line2":"COLLEGE AVENUE","town":"WIGAN","postcode":"WN1 1NJ","country":"ENGLAND"}', '02H', @ccgRoleId),
	('AAD9438A-6DAD-4D29-A870-7EFDCD2ABF4F', 'NHS Wiltshire CCG', '{"line1":"SOUTHGATE HOUSE","line2":"PANS LANE","town":"DEVIZES","county":"WILTSHIRE","postcode":"SN10 5EQ","country":"ENGLAND"}', '99N', @ccgRoleId),
	('1A93BCEB-2A16-4B77-817E-0B924D630112', 'NHS Windsor, Ascot and Maidenhead CCG', '{"line1":"KING EDWARD VII HOSPITAL","line2":"ST LEONARD ROAD","town":"WINDSOR","county":"BERKSHIRE","postcode":"SL4 3DP","country":"ENGLAND"}', '11C', @ccgRoleId),
	('947BC625-3742-4ABC-98A4-5ADC5D64A01C', 'NHS Wirral CCG', '{"line1":"OLD MARKET HOUSE","line2":"13 HAMILTON STREET","town":"BIRKENHEAD","postcode":"CH41 5AL","country":"ENGLAND"}', '12F', @ccgRoleId),
	('1291498C-D877-42C8-B03D-0969C53F1807', 'NHS Wokingham CCG', '{"line1":"CHALFONT SURGERY","line2":"CHALFONT CLOSE","line3":"EARLEY","town":"READING","county":"BERKSHIRE","postcode":"RG6 5HZ","country":"ENGLAND"}', '11D', @ccgRoleId),
	('A0EC2116-AFBE-4DC1-B02C-5FDC9E6B623B', 'NHS Wolverhampton CCG', '{"line1":"TECHNOLOGY CENTRE","line2":"GLAISHER DRIVE","line3":"WOLVERHAMPTON SCIENCE PARK","town":"WOLVERHAMPTON","county":"WEST MIDLANDS","postcode":"WV10 9RU","country":"ENGLAND"}', '06A', @ccgRoleId),
	('E21C3221-6CC7-4DC9-94BD-DBC89BF3071B', 'NHS Wyre Forest CCG', '{"line1":"BARNSLEY COURT","line2":"BARNSLEY HALL ROAD","town":"BROMSGROVE","county":"WORCESTERSHIRE","postcode":"B61 0TX","country":"ENGLAND"}', '06D', @ccgRoleId),
	('2E6BBC20-CBCD-4D7B-906B-FC330747D472', 'North East and Yorkshire Commissioning Hub', '{"line1":"C/O NHS ENGLAND","line2":"1W09, 1ST FLOOR, QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","postcode":"LS2 7UA","country":"ENGLAND"}', '85J', @ccgRoleId),
	('F502E057-EA78-4F4E-95E5-F922C0D51A92', 'North East and Yorkshire – H&J Commissioning Hub', '{"line1":"C/O NHS ENGLAND","line2":"1W09, 1ST FLOOR, QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","postcode":"LS2 7UA","country":"ENGLAND"}', '76A', @ccgRoleId),
	('3B2E4572-F9AC-4931-9536-B276A6BDF92A', 'North Midlands Commissioning Hub', '{"line1":"C/O NHS ENGLAND, 1W09, 1ST FLOOR","line2":"QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '14A', @ccgRoleId),
	('25075E59-B8B1-48BA-9628-B405F02DDD2B', 'North Midlands – H&J Commissioning Hub', '{"line1":"C/O NHS ENGLAND","line2":"1W09, FIRST FLOOR, QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '14Q', @ccgRoleId),
	('39E628EB-EA7B-4D95-B8BB-89B90ECAD764', 'North West Commissioning Hub', '{"line1":"C/O NHS ENGLAND","line2":"1W09, 1ST FLOOR, QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","postcode":"LS2 7UA","country":"ENGLAND"}', '27T', @ccgRoleId),
	('86E82555-AEFB-49CE-B113-D19B3CFAB5A0', 'North West – H&J Commissioning Hub', '{"line1":"C/O NHS ENGLAND","line2":"1W09, 1ST FLOOR, QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","postcode":"LS2 7UA","country":"ENGLAND"}', '32T', @ccgRoleId),
	('01631AC8-2FD3-473E-81E1-D99A14956641', 'South Central Commissioning Hub', '{"line1":"C/O NHS ENGLAND, 1W09, 1ST FLOOR","line2":"QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '14H', @ccgRoleId),
	('97962B9A-A02D-40EA-BD4A-A49B1E92A403', 'South East Commissioning Hub', '{"line1":"C/O NHS ENGLAND, 1W09, 1ST FLOOR","line2":"QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '14G', @ccgRoleId),
	('7A9B95B0-1D0B-4BA4-84F6-59D710938141', 'South East – H&J Commissioning Hub', '{"line1":"C/O NHS ENGLAND","line2":"1W09, 1ST FLOOR, QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","postcode":"LS2 7UA","country":"ENGLAND"}', '97T', @ccgRoleId),
	('32A61DA9-2007-4787-B547-762D4EF14FAF', 'South West Commissioning Hub', '{"line1":"C/O NHS ENGLAND, 1W09, 1ST FLOOR","line2":"QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '14F', @ccgRoleId),
	('5F6B5B29-A238-40D3-89BF-50932802E114', 'South West North Commissioning Hub', '{"line1":"NHS ENGLAND","line2":"QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '15H', @ccgRoleId),
	('6672CFE8-1583-4D02-A0AF-83D29BCB0513', 'South West South Commissioning Hub', '{"line1":"NHS ENGLAND","line2":"QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '15G', @ccgRoleId),
	('9D08F4F0-0691-4C81-BA6B-B74EFDE0EC17', 'South West South – H&J Commissioning Hub', '{"line1":"C/O NHS ENGLAND","line2":"1W09, 1ST FLOOR, QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '14T', @ccgRoleId),
	('2A4E6DCA-B9FB-4744-8512-BE65FBF4FB27', 'Wessex Commissioning Hub', '{"line1":"C/O NHS ENGLAND","line2":"1W09, 1ST FLOOR, QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '13N', @ccgRoleId),
	('BAC2E2E0-143F-412A-9E78-8618F09CF779', 'West Midlands Commissioning Hub', '{"line1":"C/O NHS ENGLAND, 1W09, 1ST FLOOR","line2":"QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '14C', @ccgRoleId),
	('2459261A-D8F0-464A-BFF7-45B2D7ADB1EF', 'Yorkshire and Humber Commissioning Hub', '{"line1":"C/O NHS ENGLAND, 1W09, 1ST FLOOR","line2":"QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '13V', @ccgRoleId),
	('348A04D0-1F3E-49D5-B652-2E9DB4C2D423', 'Yorkshire and Humber – H&J Commissioning Hub', '{"line1":"C/O NHS ENGLAND","line2":"1W09, 1ST FLOOR, QUARRY HOUSE","line3":"QUARRY HILL","town":"LEEDS","county":"WEST YORKSHIRE","postcode":"LS2 7UE","country":"ENGLAND"}', '14N', @ccgRoleId);
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./CreateCommissioningSupportUnits.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @csuRoleId AS nchar(5) = 'RO213';

IF NOT EXISTS (SELECT * FROM dbo.Organisations WHERE PrimaryRoleId = @csuRoleId)
	INSERT INTO dbo.Organisations (OrganisationId, [Name], [Address], OdsCode, PrimaryRoleId)
	VALUES
	('C0CEB1BC-8AC6-4574-8D36-2FE828C76ACF', 'NHS Anglia Commissioning Support Unit', '{"line1":"LAKESIDE 400","line2":"OLD CHAPEL WAY","line3":"BROADLAND BUSINESS PARK","town":"NORWICH","county":"NORFOLK","postcode":"NR7 0WG","country":"ENGLAND"}', '0AP', @csuRoleId),
	('1FB7238E-4642-4D7F-836E-EE917162E35C', 'NHS Arden and Greater East Midlands Commissioning Support Unit', '{"line1":"ST JOHNS HOUSE","line2":"30 EAST STREET","town":"LEICESTER","postcode":"LE1 6NB","country":"ENGLAND"}', '0DE', @csuRoleId),
	('27856693-F5F7-4144-8171-CBB2E1D574D8', 'NHS Arden Commissioning Support Unit', '{"line1":"WESTGATE HOUSE","line2":"21 MARKET STREET","town":"WARWICK","county":"WARWICKSHIRE","postcode":"CV34 4DE","country":"ENGLAND"}', '0AA', @csuRoleId),
	('7D19DF0F-2223-4686-B5BD-1F9ECE1C395D', 'NHS Central Eastern Commissioning Support Unit', '{"line1":"CHARTER HOUSE","line2":"PARKWAY","town":"WELWYN GARDEN CITY","county":"HERTFORDSHIRE","postcode":"AL8 6JL","country":"ENGLAND"}', '0CG', @csuRoleId),
	('3231426C-D4E0-44C7-998E-E944AF8661C0', 'NHS Central Midlands Commissioning Support Unit', '{"line1":"KINGSTON HOUSE","line2":"438 HIGH STREET","town":"WEST BROMWICH","county":"WEST MIDLANDS","postcode":"B70 9LD","country":"ENGLAND"}', '0AD', @csuRoleId),
	('77DEBDFF-44E2-4F19-A9FD-CAC9B8EFB838', 'NHS Central Southern Commissioning Support Unit', '{"line1":"OXFORD ROAD","town":"NEWBURY","county":"BERKSHIRE","postcode":"RG14 1PA","country":"ENGLAND"}', '0AE', @csuRoleId),
	('D8BA59ED-3C66-4D5D-8E0A-E15110131FDC', 'NHS Cheshire and Merseyside Commissioning Support Unit', '{"line1":"65 STEPHENSON WAY","line2":"WAVERTREE","line3":"TECHNOLOGY PARK","town":"LIVERPOOL","county":"MERSEYSIDE","postcode":"L13 1HN","country":"ENGLAND"}', '0CE', @csuRoleId),
	('0EF86A18-740C-4A57-8B87-7E3D5874790E', 'NHS Greater East Midlands Commissioning Support Unit', '{"line1":"FRANCIS CRICK HOUSE","line2":"6 SUMMERHOUSE ROAD","line3":"MOULTON PARK INDUSTRIAL ESTATE","town":"NORTHAMPTON","county":"NORTHAMPTONSHIRE","postcode":"NN3 6BF","country":"ENGLAND"}', '0AK', @csuRoleId),
	('D5188F47-DF03-4B25-A837-64F5EB1B2715', 'NHS Greater Manchester Commissioning Support Unit', '{"line1":"ST. JAMES HOUSE","line2":"PENDLETON WAY","town":"SALFORD","county":"GREATER MANCHESTER","postcode":"M6 5FW","country":"ENGLAND"}', '0AJ', @csuRoleId),
	('A3281B99-03DF-448E-BA5E-B0EF943F67F6', 'NHS Kent and Medway Commissioning Support Unit', '{"line1":"KENT HOUSE","line2":"81 STATION ROAD","town":"ASHFORD","county":"KENT","postcode":"TN23 1PP","country":"ENGLAND"}', '0AM', @csuRoleId),
	('EC0D2D4E-5C8F-4BD8-AB0F-12464DB1F272', 'NHS Midlands and Lancashire Commissioning Support Unit', '{"line1":"KINGSTON HOUSE","line2":"438-450 HIGH STREET","town":"WEST BROMWICH","postcode":"B70 9LD","country":"ENGLAND"}', '0CX', @csuRoleId),
	('DB548B45-9E62-418D-8A6D-E71E2EBACE52', 'NHS NEL CSU', '{"line1":"CLIFTON HOUSE","line2":"75-77 WORSHIP STREET","town":"LONDON","postcode":"EC2A 2DU","country":"ENGLAND"}', '0DJ', @csuRoleId),
	('6439317A-BBD5-4C98-AEB6-C890C181A05E', 'NHS North and East London Commissioning Support Unit', '{"line1":"CLIFTON HOUSE","line2":"75-77 WORSHIP STREET","town":"LONDON","county":"GREATER LONDON","postcode":"EC2A 2DU","country":"ENGLAND"}', '0AQ', @csuRoleId),
	('8140E36F-5CC5-4BD6-900E-397537EA2379', 'NHS North of England Commissioning Support Unit', '{"line1":"JOHN SNOW HOUSE","line2":"DURHAM UNIVERSITY SCIENCE PARK","town":"DURHAM","postcode":"DH1 3YG","country":"ENGLAND"}', '0AR', @csuRoleId),
	('38F019A8-E7CD-4E7D-A3E4-0831C66DACC6', 'NHS North West Commissioning Support Unit', '{"line1":"BEVAN HOUSE","line2":"BARONY COURT","town":"NANTWICH","county":"CHESHIRE","postcode":"CW5 5RD","country":"ENGLAND"}', '0CY', @csuRoleId),
	('F242A6A6-1A98-4810-930C-16BA915935E7', 'NHS North West London Commissioning Support Unit', '{"line1":"15 MARYLEBONE ROAD","town":"LONDON","county":"GREATER LONDON","postcode":"NW1 5JD","country":"ENGLAND"}', '0AT', @csuRoleId),
	('79D43636-4E83-44A5-98B9-528627BC5CB5', 'NHS North Yorkshire and Humber Commissioning Support Unit', '{"line1":"TRUINE COURT","line2":"MONKS CROSS DRIVE","line3":"HUNTINGTON","town":"YORK","county":"NORTH YORKSHIRE","postcode":"YO32 9GZ","country":"ENGLAND"}', '0AV', @csuRoleId),
	('138D6F3B-35AD-44B1-8EBC-44B2084A0C8D', 'NHS South, Central and West Commissioning Support Unit', '{"line1":"OMEGA HOUSE","line2":"112 SOUTHAMPTON ROAD","town":"EASTLEIGH","postcode":"SO50 5PB","country":"ENGLAND"}', '0DF', @csuRoleId),
	('3A59582D-AC70-4691-892D-9C66874277E6', 'NHS South Commissioning Support Unit', '{"line1":"OMEGA HOUSE","line2":"112 SOUTHAMPTON ROAD","town":"EASTLEIGH","county":"HAMPSHIRE","postcode":"SO50 5PB","country":"ENGLAND"}', '0AW', @csuRoleId),
	('9FD72B64-DA05-4A63-B0C7-6CBFA2CE163D', 'NHS South East Commissioning Support Unit', '{"line1":"1 LOWER MARSH","line2":"WATERLOO","town":"LONDON","county":"GREATER LONDON","postcode":"SE1 7NT","country":"ENGLAND"}', '0DC', @csuRoleId),
	('51FFCA42-1B61-4BA9-851B-353A8B404EFA', 'NHS South London Commissioning Support Unit', '{"line1":"1 LOWER MARSH","town":"LONDON","county":"GREATER LONDON","postcode":"SE1 7NT","country":"ENGLAND"}', '0AX', @csuRoleId),
	('5544F676-086F-4280-A749-1DCB3C4D4058', 'NHS South West Commissioning Support Unit', '{"line1":"SOUTH PLAZA","line2":"MARLBOROUGH STREET","town":"BRISTOL","county":"AVON","postcode":"BS1 3NX","country":"ENGLAND"}', '0AC', @csuRoleId),
	('B2309A2F-9D57-4C21-891E-CD06DFBDEFFD', 'NHS Staffordshire and Lancashire Commissioning Support Unit', '{"line1":"ANGLESEY HOUSE","line2":"TOWERS BUSINESS PARK","line3":"WHEELHOUSE ROAD","town":"RUGELEY","county":"STAFFORDSHIRE","postcode":"WS15 1UZ","country":"ENGLAND"}', '0CH', @csuRoleId),
	('A6175C17-E90A-4609-85B5-FCEC9D3E3FC8', 'NHS Surrey and Sussex Commissioning Support Unit', '{"line1":"36-38 FRIARS WALK","town":"LEWES","county":"EAST SUSSEX","postcode":"BN7 2PB","country":"ENGLAND"}', '0CC', @csuRoleId),
	('5D27A2D4-040D-4FEE-BE5E-55A957AEB768', 'NHS West and South Yorkshire and Bassetlaw Commissioning Support Unit', '{"line1":"DOUGLAS MILLS","line2":"BOWLING OLD LANE","town":"BRADFORD","county":"WEST YORKSHIRE","postcode":"BD5 7JR","country":"ENGLAND"}', '0CF', @csuRoleId),
	('D91E3103-0F82-4358-9F99-569A16AA9B38', 'NHS Yorkshire and Humber Commissioning Support Unit', '{"line1":"TRUINE COURT","line2":"MONKS CROSS DRIVE","line3":"HUNTINGTON","town":"YORK","county":"NORTH YORKSHIRE","postcode":"YO32 9GZ","country":"ENGLAND"}', '0DA', @csuRoleId),
	('9F6D7251-0237-4BCE-BB57-230B22DC39CB', 'Optum Health Solutions (UK) Limited', '{"line1":"1ST FLOOR STAR HOUSE","line2":"20 GRENFELL ROAD","town":"MAIDENHEAD","county":"BERKSHIRE","postcode":"SL6 1EH","country":"ENGLAND"}', '0DG', @csuRoleId);
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./CreateExecutiveAgency.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @executiveAgencyRoleId AS nchar(5) = 'RO116';

IF NOT EXISTS (SELECT * FROM dbo.Organisations WHERE PrimaryRoleId = @executiveAgencyRoleId)
	INSERT INTO dbo.Organisations (OrganisationId, [Name], [Address], OdsCode, PrimaryRoleId)
	VALUES
	('C7A94E85-025B-403F-B984-20EE5F9EC333', 'NHS Digital', '{"line1":"1 TREVELYAN SQUARE","town":"LEEDS","postcode":"LS1 6AE","country":"ENGLAND"}', 'X26', @executiveAgencyRoleId);
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./CreateExecutiveAgencyUser.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @createUser AS nvarchar(4) = '$(CREATE_EA_USER)';
DECLARE @email AS nvarchar(256) = '$(EA_USER_EMAIL)';

IF UPPER(@createUser) = 'TRUE'
AND NOT EXISTS (
    SELECT *
      FROM dbo.AspNetUsers
      WHERE UserName = @email)
BEGIN
    DECLARE @firstName AS nvarchar(50) = '$(EA_USER_FIRST_NAME)';
    DECLARE @lastName AS nvarchar(50) = '$(EA_USER_LAST_NAME)';
    DECLARE @normalizedUserName AS nvarchar(256) = UPPER(@email);
    DECLARE @passwordHash AS nvarchar(max) = '$(EA_USER_PASSWORD_HASH)';
    DECLARE @phoneNumber AS nvarchar(max) = '$(EA_USER_PHONE)';

    DECLARE @executiveAgencyRoleId AS nchar(5) = 'RO116';
    DECLARE @organisationId AS uniqueidentifier = (SELECT OrganisationId FROM dbo.Organisations WHERE PrimaryRoleId = @executiveAgencyRoleId);

    INSERT INTO dbo.AspNetUsers
    (
        Id, UserName, NormalizedUserName, PasswordHash, 
        FirstName, LastName, Email, NormalizedEmail, EmailConfirmed, PhoneNumber, PhoneNumberConfirmed,
        PrimaryOrganisationId, OrganisationFunction, CatalogueAgreementSigned, [Disabled],
        AccessFailedCount, ConcurrencyStamp, LockoutEnabled, SecurityStamp, TwoFactorEnabled
    )
	VALUES
	(CAST(NEWID() AS nchar(36)), @email, @normalizedUserName, @passwordHash,
        @firstName, @lastName, @email, @normalizedUserName, 1, @phoneNumber, 1,
        @organisationId, 'Authority', 1, 0,
        0, NEWID(), 1, NEWID(), 0);
END;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./CreateTestUsers.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @aliceEmail AS nvarchar(50) = N'AliceSmith@email.com';
DECLARE @bobEmail AS nvarchar(50) = N'BobSmith@email.com';
DECLARE @sueEmail AS nvarchar(50) = N'SueSmith@email.com';

IF '$(INSERT_TEST_DATA)' = 'True'
AND NOT EXISTS (
  SELECT *
  FROM dbo.AspNetUsers
  WHERE UserName IN (@aliceEmail, @bobEmail, @sueEmail))
BEGIN
    DECLARE @ccgRoleId AS nchar(4) = 'RO98';
    DECLARE @executiveAgencyRoleId AS nchar(5) = 'RO116';
    DECLARE @hullCCGOdsCode AS nchar(3) = '03F';

    DECLARE @aliceOrganisationId AS uniqueidentifier = (SELECT TOP (1) OrganisationId FROM dbo.Organisations WHERE PrimaryRoleId = @ccgRoleId ORDER BY OdsCode);
    DECLARE @aliceOrganisationName AS nvarchar(255) =  (SELECT TOP (1) Name FROM dbo.Organisations WHERE PrimaryRoleId = @ccgRoleId ORDER BY OdsCode);

    DECLARE @bobOrganisationId AS uniqueidentifier = (SELECT OrganisationId FROM dbo.Organisations WHERE PrimaryRoleId = @executiveAgencyRoleId);
    DECLARE @bobOrganisationName AS nvarchar(255) =  (SELECT TOP (1) Name FROM dbo.Organisations WHERE PrimaryRoleId = @ccgRoleId ORDER BY OdsCode);

    DECLARE @sueOrganisationId AS uniqueidentifier = (SELECT TOP (1) OrganisationId FROM dbo.Organisations WHERE PrimaryRoleId = @ccgRoleId AND OdsCode = @hullCCGOdsCode);
    DECLARE @sueOrganisationName AS nvarchar(255) =  (SELECT TOP (1) Name FROM dbo.Organisations WHERE PrimaryRoleId = @ccgRoleId AND OdsCode = @hullCCGOdsCode);

	DECLARE @address AS nchar(108) = N'{ "street_address": "One Hacker Way", "locality": "Heidelberg", "postal_code": 69118, "country": "Germany" }';

	DECLARE @aliceId AS nchar(36) = CAST(NEWID() AS nchar(36));
	DECLARE @bobId AS nchar(36) = CAST(NEWID() AS nchar(36));
    DECLARE @sueId AS nchar(36) = CAST(NEWID() AS nchar(36));

	DECLARE @aliceNormalizedEmail AS nvarchar(50) = UPPER(@aliceEmail);
    DECLARE @bobNormalizedEmail AS nvarchar(50) = UPPER(@bobEmail);
    DECLARE @sueNormalizedEmail AS nvarchar(50) = UPPER(@sueEmail);

    DECLARE @phoneNumber AS nvarchar(max) = '01234567890';

	-- 'Pass123$'
	DECLARE @alicePassword AS nvarchar(200) = N'AQAAAAEAACcQAAAAEFSsEthAqGVBLj1P1gF9puxtXm18lKHlmuh9J/Ib0KKBO3GjQvxymJbzpSqy0zuOHg==';

	-- 'Pass123$'
	DECLARE @bobPassword AS nvarchar(200) = N'AQAAAAEAACcQAAAAEOzr1Zwpoo1pKsTa+S65mBZVG4GIy6IYH/IAED6TvBA+FIMg8u/xb0b/cfexV7SHNw==';

	-- 'Pass123$'
	DECLARE @suePassword AS nvarchar(200) =  N'AQAAAAEAACcQAAAAEBRpg4kCDtF5H4UEgv209hSD0TmaRx9JOYorAzNHxzfyZisIDse2AlTA0oF28HlBhQ==';

	INSERT INTO dbo.AspNetUsers
    (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail, AccessFailedCount, ConcurrencyStamp, PhoneNumber,
        EmailConfirmed, LockoutEnabled, PasswordHash, PhoneNumberConfirmed, SecurityStamp, TwoFactorEnabled, 
        FirstName, LastName, PrimaryOrganisationId, OrganisationFunction, Disabled, CatalogueAgreementSigned
    )
	VALUES
	(@aliceId, @aliceEmail, @aliceNormalizedEmail, @aliceEmail, @aliceNormalizedEmail, 0, NEWID(), @phoneNumber, 1, 1, @alicePassword, 0, 'NNJ4SLBPCVUDKXAQXJHCBKQTFEYUAPBC', 0, 'Alice', 'Smith', @aliceOrganisationId, 'Buyer', 0, 1),
	(@bobId, @bobEmail, @bobNormalizedEmail, @bobEmail, @bobNormalizedEmail, 0, NEWID(), @phoneNumber, 1, 1, @bobPassword, 0, 'OBDOPOU5YQ5WQXCR3DITKL6L5IDPYHHJ', 0, 'Bob', 'Smith', @bobOrganisationId, 'Authority', 0, 1),
    (@sueId, @sueEmail, @sueNormalizedEmail, @sueEmail, @sueNormalizedEmail, 0, NEWID(), @phoneNumber, 1, 1, @suePassword, 0, 'NNJ4SLBPCVUDKXAQXJHCBKQTFEYUAPBC', 0, 'Sue', 'Smith', @sueOrganisationId, 'Buyer', 0, 1);

	INSERT INTO dbo.AspNetUserClaims (ClaimType, ClaimValue, UserId)
	VALUES
	(N'email_verified', N'true', @aliceId),
	(N'website', N'http://alice.com/', @aliceId),
	(N'address', @address, @aliceId),
    (N'primaryOrganisationName', @aliceOrganisationName, @aliceId),
	(N'email_verified', N'true', @bobId),
	(N'location', N'somewhere', @bobId),
	(N'website', N'http://bob.com/', @bobId),
	(N'address', @address, @bobId),
    (N'primaryOrganisationName', @bobOrganisationName, @bobId),
	(N'email_verified', N'true', @sueId),
	(N'location', N'somewhere', @sueId),
	(N'website', N'http://sue.com/', @sueId),
	(N'address', @address, @sueId),
    (N'primaryOrganisationName', @sueOrganisationName, @sueId);
END;
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./DropImport.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF UPPER('$(INCLUDE_IMPORT)') <> 'TRUE'
BEGIN
    DROP PROCEDURE IF EXISTS import.ImportAdditionalService;
    DROP TYPE IF EXISTS import.AdditionalServiceCapability;

    DROP PROCEDURE IF EXISTS import.ImportSolution;
    DROP TYPE IF EXISTS import.SolutionCapability;

    DROP ROLE IF EXISTS Importer;

    DROP SCHEMA IF EXISTS import;
END;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./DropPublish.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF UPPER('$(INCLUDE_PUBLISH)') <> 'TRUE'
BEGIN
    DROP PROCEDURE IF EXISTS publish.PublishSolution;
    DROP ROLE IF EXISTS Publisher;

    DROP SCHEMA IF EXISTS publish;
END;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./ProdLikeData/MergeSuppliers.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN
/*********************************************************************************************************************************************/
/* Supplier */
/*********************************************************************************************************************************************/

    CREATE TABLE #Supplier
    (
        Id nvarchar(6) NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        LegalName nvarchar(255) NOT NULL,
        Summary nvarchar(1000) NULL,
        SupplierUrl nvarchar(1000) NULL,
        [Address] nvarchar(500) NULL,
        OdsCode nvarchar(8) NULL,
        CrmRef uniqueidentifier NULL,
        Deleted bit NOT NULL,
        LastUpdated datetime2(7) NOT NULL,
        LastUpdatedBy uniqueidentifier NOT NULL
    );

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10000', N'EMIS Health', N'EMIS Health', N'We’re the UK leader in connected healthcare software & services. Through innovative IT, we help healthcare professionals access the information they need to provide better, faster and more cost effective patient care.

Our clinical software is used in all major healthcare settings from GP surgeries to pharmacies, communities, hospitals, and specialist services. By providing innovative, integrated solutions, we’re working to break the boundaries of system integration & interoperability. 

We also specialise in supplying IT infrastructure, software and engineering services and, through our technical support teams, we have the skills and knowledge to enhance your IT systems.

Patient (www.patient.info) is the UK’s leading health website. Designed to help patients play a key role in their own care, it provides access to clinically authored health information leaflets, videos, health check and assessment tools and patient forums.', N'https://www.emishealth.com/', N'{"line1":"Rawdon House","line2":"Green Lane","town":"Yeadon","county":"West Yorkshire","postcode":"LS19 7BY","country":"England"}', NULL, NULL, 0, CAST(N'2020-04-07T19:46:44.1666667' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10001', N'Involve', N'Involve', NULL, NULL, N'{"line1":"Martin Dawes House","line2":"Europa Boulevard","line3":"Westbrook","town":"Warrington","county":"Cheshire","postcode":"WA5 7WH","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-11T08:54:20.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10002', N'G2 Speech', N'G2 Speech', NULL, NULL, N'{"line1":"4th Floor","line2":"Solar House","line3":"1-9 Romford Road","line4":"Stratford","town":"London","county":"Greater London","postcode":"E15 4LJ","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-11T08:55:09.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10003', N'myOrb', N'myOrb Ltd', NULL, NULL, N'{"line1":"Unit 67","line2":"Surrey Technology Centre","line3":"40 Occam Road","town":"Guildford","county":"Surrey","postcode":"GU2 7YG","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-11T14:15:05.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004', N'Informatica Systems Ltd', N'Informatica Systems Ltd', N'Since 1992, Informatica has delivered innovative solutions that help primary care organisations deliver proactive care, reduce long term diseases and improve population health. Our range of intelligence systems collect healthcare data on patient cohorts, provide real time reporting dashboards for flexible data analysis and empower GPs to deliver care change. 

We have a long history as innovators, introducing the first solutions to market for 

1. touchscreen check-in systems;  

2. interactive prompts for QOF and clinical decision support; and  

3. comprehensive online patient facing services for appointment management.  

We are a fully accredited supplier with NHSD and are assured against the 20000, 22301 & 27001 ISO standards, DSPT, CyberEssentials+ and MHRA registered.  

Backed up by excellent customer service team and reputation we can help you to support primary care and improve patient outcomes.', N'https://www.informatica-systems.co.uk/', N'{"line1":"Aurora House","line2":"Deltic Avenue","line3":"Rooksley","town":"Milton Keyes","county":"Buckinghamshire","postcode":"MK13 8LW","country":"England"}', NULL, NULL, 0, CAST(N'2020-03-31T11:48:07.4800000' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10005', N'PharmData', N'PharmData Ltd', NULL, NULL, N'{"line1":"1 Kirkmanshulme Lane","town":"Manchester","county":"Lancashire","postcode":"M12 4NA","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-14T13:14:40.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10006', N'Medical Director', N'Medical Director', NULL, NULL, N'{"line1":"C-/Linklaters","line2":"One Silk Street","town":"London","county":"Greater London","postcode":"EC2Y 8HQ","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-18T14:23:07.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10007', N'DXS International PLC', N'DXS International PLC', N'DXS International Plc is a health information technology company that provides clinical decision support to more than 2,000 GP practices.

DXS’ core Clinical Decision Support solution, Best Pathway, provides UK primary care staff with treatment guidance and referral services helping to improve health care outcomes and reduce treatment costs.

It integrates with all Primary Care Clinical Systems and presents information to the clinician, relevant to the patient’s condition, via the workflow alerts.

The result is saving GP’s time, empowering Nurses and Pharmacists to take on increased responsibility of care and improving outcomes for the NHS and patients.', N'https://www.dxs-systems.co.uk/dxs-point-of-care.php#intro', N'{"line1":"119 St. Mary''s Road","town":"Market Harborough","county":"Leicestershire","postcode":"LE16 7DT","country":"England"}', NULL, NULL, 0, CAST(N'2020-03-25T12:12:24.0033333' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10008', N'LumiraDX Care Solutions UK Ltd', N'LumiraDX Care Solutions UK Ltd', NULL, NULL, N'{"line1":"Francis Clark LLP","line2":"Lowin House","line3":"Tregolls Road","town":"Truro","county":"Cornwall","postcode":"TR1 2NA","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-20T07:57:23.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10009', N'Care IS LTD', N'Care IS LTD', NULL, NULL, N'{"line1":"Biocity, Pennyfoot Street","town":"Nottingham","county":"Nottingham","postcode":"NG1 1GF","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-21T14:27:44.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10010', N'Excelicare', N'Excelicare', NULL, NULL, N'{"line1":"Axsys House","line2":"Marchburn Drive","line3":"Glasgow Airport Business Park","town":"Paisley","county":"","postcode":"PA3 2SJ","country":"Scotland"}', NULL, NULL, 0, CAST(N'2019-06-26T10:12:29.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10011', N'DoctorLink', N'DoctorLink', NULL, NULL, N'{"line1":"Oakhill House","line2":"130 Tonbridge Road","town":"Hildenborough","county":"Kent","postcode":"TN11 9DZ","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-26T16:46:52.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10012', N'MedXnote', N'MedXnote', NULL, NULL, N'{"line1":"Digital Exchange Building","line2":"Crane Street","town":"Dublin","county":"","postcode":"D08 HKR9","country":"Ireland"}', NULL, NULL, 0, CAST(N'2019-06-26T19:48:39.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10013', N'Medidata Exchange Limited', N'Medidata Exchange Limited', NULL, NULL, N'{"line1":"Ty Derw","line2":"Lime Tree Court","line3":"Cardiff Gate Business Park","town":"Cardiff","county":"South Glamorgan","postcode":"CF23 8AB","country":"Wales"}', NULL, NULL, 0, CAST(N'2019-06-27T07:01:49.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10014', N'Total Billing Solutions', N'Total Billing Solutions', NULL, NULL, N'{"line1":"Beachside Business Centre","line2":"Rue Du Hocq","town":"St Clement","county":"","postcode":"JE2 6LF","country":"Jersey"}', NULL, NULL, 0, CAST(N'2019-06-27T07:30:45.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10015', N'Optum Health Solutions UK Limited', N'Optum Health Solutions UK Limited', NULL, NULL, N'{"line1":"10th Floor","line2":"5 Merchant Square","town":"London","county":"Greater London","postcode":"W2 1AS","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-27T09:43:11.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10016', N'BigHand Limited', N'BigHand Limited', NULL, NULL, N'{"line1":"27-29 Union Street","town":"London","county":"","postcode":"SE1 1SD","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-27T11:49:02.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10017', N'Numed Healthcare', N'Numed Holdings Ltd', NULL, NULL, N'{"line1":"Alliance House","line2":"Roman Ridge Road","town":"Sheffield","county":"South Yorkshire","postcode":"S9 1GB","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-27T11:51:36.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10018', N'Sesui', N'Sesui Ltd', NULL, NULL, N'{"line1":"Magdalen Centre","line2":"1 Robert Robinson Avenue","town":"Oxford","county":"Oxfordshire","postcode":"OX4 4GA","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-27T13:32:13.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10019', N'Codegate Ltd', N'Codegate Ltd', NULL, NULL, N'{"line1":"39 Chapel Road","town":"Southampton","county":"","postcode":"SO30 3FG","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-27T14:54:44.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10020', N'MyMed Ltd', N'MyMed Ltd', N'MyMed Ltd, trading as Q doctor, are a market leading supplier of video consultation software and services. Founded by NHS clinicians', N'https://www.qdoctor.io/', N'{"line1":"Willowbrook","line2":"Burbridge Close","line3":"Lytchett Matravers","town":"Poole","county":"Dorset","postcode":"BH16 6EG","country":"England"}', NULL, NULL, 0, CAST(N'2020-04-07T19:21:45.3600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10021', N'Black Pear Software Ltd', N'Black Pear Software Ltd', NULL, NULL, N'{"line1":"Bartlam House","line2":"Shrawley","town":"Worcester","county":"Worcestershire","postcode":"WR6 6TP","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-27T18:38:31.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10022', N'Gnosco AB', N'Gnosco AB', NULL, NULL, N'{"line1":"Kungsgatan 4","town":"Göteborg","county":"","postcode":"411 19","country":"Sweden"}', NULL, NULL, 0, CAST(N'2019-06-27T20:01:46.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10023', N'Docobo Ltd', N'Docobo Ltd', NULL, NULL, N'{"line1":"21A High Street","town":"Bookham","county":"Surrey","postcode":"KT23 4AA","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-01T06:13:23.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10024', N'Servelec HSC', N'Servelec', NULL, NULL, N'{"line1":"The Straddle","line2":"Wharf Street","line3":"Victoria Quays","town":"Sheffield","county":"South Yorkshire","postcode":"S2 5SY","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-01T10:56:20.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10025', N'Clarity Informatics', N'Clarity Informatics', NULL, NULL, N'{"line1":"Deltic House","line2":"Kingfisher Way","line3":"Silverlink Business Park","line4":"Wallsend","town":"Newcastle on Tyne","county":"","postcode":"NE28 9NX","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-01T10:58:07.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10026', N'eConsult Health', N'eConsult Health', NULL, NULL, N'{"line1":"Nightingale House","line2":"46-48 East Street","town":"Epsom","county":"Surrey","postcode":"KT17 1HQ","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-01T15:43:57.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10027', N'Lantum', N'Lantum', NULL, NULL, N'{"line1":"4th Floor","line2":"15 Bonhill Street","town":"London","county":"Greater London","postcode":"EC2A 4DN","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-01T16:54:37.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10028', N'Clinical Architecture', N'Clinical Architecture', NULL, NULL, N'{"line1":"Suite 2","line2":"Okehampton Business Centre","line3":"Higher Stockley Mead","town":"Okehampton","county":"Devon","postcode":"EX20 1FJ","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-02T10:55:03.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10029', N'Targett Business Technology Limited', N'Targett Business Technology Limited', N'Targett Business Technology (TBT) is an innovative health and social care company that has developed RIVIAM as a secure platform for co-ordinating care. TBT has over 17 years of experience helping NHS and independent providers deliver services to improve healthcare outcomes.

Initially, we provided consulting expertise to establish the Healthcare Commission, CQC and worked for national bodies including NHS Employers and NICE. Through this work we gained an excellent understanding of health and social care regulatory, management and data systems. Since 2010, we have focused on providing services to national and local care provider organisations.

Over the past 5 years, we have invested in and launched our RIVIAM platform for enabling new models of care for health and social care services. In 2019 we launched the Secure Video Service.

Today, RIVIAM is contracted to provide services to NHS Trusts, GP Federations and independent organisations delivering NHS services.', N'http://www.riviam.com/', N'{"line1":"141 Englishcombe Lane","town":"Bath","county":"Bath & North East Somerset","postcode":"BA2 2EL","country":"England"}', NULL, NULL, 0, CAST(N'2020-04-08T10:03:33.9066667' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10030', N'AccuRx Limited', N'AccuRx Limited', N'AccuRx is on a mission to make patients healthier and the healthcare workforce happier. We’re doing that by building the platform to bring together all communication around a patient. Our vision is that anyone involved in a patient''s care can easily communicate with everyone else involved in that patient''s care, including the patient. AccuRx was founded in 2016 and has since been adopted by 6,500 GP practices (>95%) and over 150 NHS Trusts.', N'https://www.accurx.com/', N'{"line1":"47 Woodland Rise","town":"London","county":"","postcode":"N10 3UN","country":"England"}', NULL, NULL, 0, CAST(N'2020-06-11T13:33:17.8233333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10031', N'The Martin Bell Partnership', N'The Martin Bell Partnership', N'We are an Independent consultancy, The Martin Bell Partnership, providing expert strategic & practical support for healthcare, healthcare IT & business support. 

Experienced at Board level in the NHS as a CIO, deputy Managing Director previously of one of the UK’s leading clinical systems providers; a wealth of experience from different sectors over many years.

Working with Trusted Associates, themselves steeped in the NHS, many at a senior level within clinical systems suppliers.

Strategic advice & guidance to practical support, clients include large corporates, investment houses, start-ups & healthcare providers.

We have been working with GP&Me since early in 2019 getting this solution onto GP IT Futures. GP&Me, created by two practising GPs, offers easy video consultation facilities. The Martin Bell Partnership, provides the support, sales, marketing & deployment expertise, from years of experience across our Associates in primary care, & many other health care settings.', N'', N'{"line1":"The Old Court House","line2":"Back Street","town":"Aldborough","county":"North Yorkshire","postcode":"YO51 9EX","country":"England"}', NULL, NULL, 0, CAST(N'2020-04-01T12:48:09.8933333' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10032', N'Elemental Software', N'In Your Elemental Ltd', NULL, NULL, N'{"line1":"1st Floor","line2":"Progressive House","line3":"25-31 The Diamond","town":"Derry","county":"Northern Ireland","postcode":"BT48 6HP","country":"Northern Ireland"}', NULL, NULL, 0, CAST(N'2019-07-03T08:39:46.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10033', N'AllDayDr Group Ltd', N'AllDayDr Group Ltd', NULL, NULL, N'{"line1":"7 Wendover Drive","line2":"Ladybridge","town":"Bolton","county":"Lancashire","postcode":"BL3 4RX","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-03T12:07:00.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10034', N'Vision Healthcare Limited', N'Vision Healthcare Limited', NULL, NULL, N'{"line1":"The Bread Factory","line2":"1a Broughton Street","town":"London","county":"","postcode":"SW8 3QJ","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-03T13:04:47.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035', N'Evergreen Life', N'Evergreen Life', N'Wellness app and provider of GP services, Evergreen Life lets people own their health information, driving informed healthcare and giving people the best chance of staying well. 

Collaborating with NHS England, NHS Digital and 3 major GP systems, Evergreen Life has facilitated mutually efficient interaction between practices and patients through online prescription ordering and appointment booking.

The app platform allows all patients in England to keep a copy of their GP record whenever and wherever they need it. Users can add their own information not held in the GP record, including allergies, conditions and medications, building a complete, accurate record that they can share with anyone they wish.

Providing access to up-to-date health information, our person-led solution empowers people to feel more in control to make self-care decisions and manage their care independently, whilst delivering a platform to access primary care services if they need it.', N'https://www.evergreen-life.co.uk/', N'{"line1":"The Edge Business Centre","line2":"The Clowes Street","town":"Manchester","county":"Greater Manchester","postcode":"M3 5NA","country":"England"}', NULL, NULL, 0, CAST(N'2020-04-01T12:08:32.4233333' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10036', N'Elsevier Limited', N'Elsevier BV', NULL, NULL, N'{"line1":"Radarweg 29","town":"Amsterdam","county":"Noord-Holland","postcode":"1043NX","country":"Netherlands"}', NULL, NULL, 0, CAST(N'2019-07-03T17:04:49.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10037', N'DictateIT', N'DictateIT', NULL, NULL, N'{"line1":"Aurora House, Deltic Avenue","line2":"Rooksley","town":"Milton Keynes","county":"Buckinghamshire","postcode":"MK13 8LW","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T11:58:29.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10038', N'Edenbridge', N'Edenbridge Healthcare Limited', NULL, NULL, N'{"line1":"1 Mariner Court","line2":"Calder Business Park","town":"Wakefield","county":"West Yorkshire","postcode":"WF4 3FL","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T12:17:17.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10039', N'Doc Abode Ltd', N'Doc Abode Ltd', NULL, NULL, N'{"line1":"Suite 12 Jason House Kerry Hill","town":"Leeds","county":"West Yorkshire","postcode":"LS18 4JR","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T12:17:20.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10040', N'Swiftqueue', N'Swiftqueue', NULL, NULL, N'{"line1":"Eolas Building","line2":"Maynooth University North Campus","town":"Maynooth","county":"county Kildare","postcode":"","country":"Ireland"}', NULL, NULL, 0, CAST(N'2019-07-04T13:48:16.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10041', N'Quality Medical Solutions (QMS)', N'Quality Medical Solutions', NULL, NULL, N'{"line1":"23 Hinton Rd","town":"Bournemouth","county":"Dorset","postcode":"BH1 2EF","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T13:51:36.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10042', N'Engage Health Systems Limited', N'Engage Health Systems Limited', NULL, NULL, N'{"line1":"1a, St Nicholas Court","town":"North Walsham","county":"Norfolk","postcode":"NR28 9BY","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T14:00:05.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10043', N'IBS Software', N'IBS, Inc.', NULL, NULL, N'{"line1":"Unit 18685","line2":"13 Freeland Park","line3":"Wareham Road","town":"Poole","county":"","postcode":"BH16 6FA","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T14:38:44.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10044', N'Answer Digital Ltd', N'Answer Digital Ltd', NULL, NULL, N'{"line1":"Union Mills","line2":"9 Dewsbury Road","town":"Leeds","county":"West Yorkshire","postcode":"LS11 5DD","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T14:43:46.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10045', N'Meddbase', N'Medical Management Systems', NULL, NULL, N'{"line1":"140 Buckingham Palace Road","town":"London","county":"Greater London","postcode":"SW1 9SA","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T14:44:48.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046', N'Advanced Health and Care Limited', N'Advanced Health and Care Limited', N'Advanced is one of the UK’s largest providers of business software and services with 19,000+ customers and 2,350+ employees. We provide enterprise and market-focused solutions that allow our customers to reimagine what is possible, innovate in their sectors and improve the lives of millions of people in the UK.

By continually investing in our people, partnerships and technologies, we provide right-first-time solutions that evolve with the changing needs of our customers and the markets they operate in. Our strategy is to enable our customers to drive efficiencies, make informed decisions, act with pace and meet challenges head on.

True partnership is what differentiates us from our competition. We deliver focused solutions for health & care organisations that simplify complex challenges and deliver immediate value.

Advanced solutions help care for 65 million patients in the UK, send millions of clinical documents and support over 80% of the NHS 111 service.', N'https://www.oneadvanced.com/', N'{"line1":"Ditton Park","line2":"Riding Court Road","town":"Datchet","county":"Berkshire","postcode":"SL3 9LL","country":"England"}', NULL, NULL, 0, CAST(N'2020-03-31T14:06:17.6333333' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10047', N'GP Access', N'GP Access Ltd', N'', N'https://askmygp.uk/about/', N'{"line1":"The Manor House","line2":"70 Main Street","town":"Cossington","county":"Leicestershire","postcode":"LE7 4UW","country":"England"}', NULL, NULL, 0, CAST(N'2020-04-01T13:05:48.0266667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10048', N'Priority Digital Health Limited', N'Priority Digital Health Limited', NULL, NULL, N'{"line1":"Denny Lodge Business Park","line2":"Ely Road","line3":"Chittering","town":"Cambridge","county":"Cambridgeshire","postcode":"CB25 9PH","country":""}', NULL, NULL, 0, CAST(N'2019-07-04T16:19:39.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10049', N'Medical Data Solutions and Services', N'Medical Data Solutions and Services', NULL, NULL, N'{"line1":"74 Dickenson Road","line2":"Rusholme","town":"Manchester","county":"Greater Manchester","postcode":"M14 5HF","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T16:43:17.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10050', N'Philips', N'Philips Electronics UK Limited', NULL, NULL, N'{"line1":"Philips Centre","line2":"Unit 3","line3":"Guildford Business Park","town":"Guildford","county":"Surrey","postcode":"GU2 8XG","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T18:14:27.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10051', N'Integrated Care 24 Ltd (IC24)', N'Integrated Care 24 Ltd (IC24)', NULL, NULL, N'{"line1":"Kingston House","line2":"The Long Barrow","line3":"Orbital Park","town":"Ashford","county":"Kent","postcode":"TN24 0GP","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-05T07:00:03.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052', N'TPP – The Phoenix Partnership (Leeds) Ltd', N'TPP-UK - The Phoenix Partnership Ltd', N'TPP is a digital health company, committed to delivering world-class healthcare software around the world. 

Its EHR product, SystmOne, is used by over 7,000 NHS organisations in over 25 different care settings. This includes significant deployments in Acute Hospitals, Emergency Departments, Mental Health services, Social Care services and General Practice. 

In recent years, TPP has increased its international presence, with live deployments in China and across the Middle East.', N'https://www.tpp-uk.com/', N'{"line1":"TPP House","line2":"Horsforth","town":"Leeds","county":"West Yorkshire","postcode":"LS18 5PX","country":"England"}', NULL, NULL, 0, CAST(N'2020-05-27T06:26:48.5033333' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10053', N'Arissian', N'Arissian Ltd', NULL, NULL, N'{"line1":"Basepoint Centre","line2":"Bromsgrove Technology Park","line3":"Isidore Road","town":"Bromsgrove","county":"Worcestershire","postcode":"B60 3ET","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-05T14:34:54.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10054', N'Health Intelligence Ltd', N'Health Intelligence Ltd', NULL, NULL, N'{"line1":"Beechwood Hall","line2":"Kingsmead Road","town":"High Wycombe","county":"","postcode":"HP11 1JL","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-10T13:37:46.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10055', N'Accenda Limited', N'Accenda Limited', NULL, NULL, N'{"line1":"Suite 322","line2":"3rd Floor Broadstone Mill","line3":"Broadstone Road","town":"Stockport","county":"Cheshire","postcode":"SK5 7DL","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-11T10:07:20.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10056', N'Metabolic Healthcare Ltd', N'Metabolic Healthcare Ltd', NULL, NULL, N'{"line1":"1 Westpoint Trading Estate","line2":"Alliance Road","line3":"Ealing","town":"London","county":"","postcode":"W3 0RA","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-15T11:55:36.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10057', N'Redwood Technologies Group Limited', N'Redwood Technologies Group Limited', NULL, NULL, N'{"line1":"Radius Court","line2":"Eastern Road","town":"Bracknell","county":"Berkshire","postcode":"RG12 2UP","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-15T15:44:31.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10058', N'Niche Health', N'Niche Health', NULL, NULL, N'{"line1":"Beasleys Farm","line2":"Upper Gambolds Lane","town":"Bromsgrove","county":"Worcestershire","postcode":"B60 3EZ","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-16T07:13:10.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10059', N'Prescribing Services', N'Prescribing Services Limited', NULL, NULL, N'{"line1":"2 Regis Place","line2":"North Lynn Industrial Estate","town":"King''s Lynn","county":"Norfolk","postcode":"PE30 2JN","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-17T09:24:52.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10060', N'Locum''s Nest', N'Locum''s Nest', NULL, NULL, N'{"line1":"12 Hammersmith Grove","line2":"Hammersmith Fulham","town":"London","county":"","postcode":"W6 7AP","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-17T10:50:45.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10061', N'Microtest Ltd', N'Microtest Ltd', NULL, NULL, N'{"line1":"16-18 Normandy Way","town":"Bodmin","county":"","postcode":"PL31 1EX","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-18T14:47:34.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10062', N'Silicon Practice', N'Silicon Practice', N'We have rolled out over 540 FootFall sites in the UK across a variety of demographics and different practice structures. Working closely with Practices and CCG’s, we have evolved strategies, processes and video tutorials to ensure FootFall is successfully rolled-out in each area.

We have a strong track record of developing the product with CCGs. We have recently made extensive changes to the product with Norfolk and Waveney STP, which have enriched and extended our focus on the digital triage features of FootFall.

We develop a joint project management approach with the CCG, for example arranging Project Initiation Workshops, weekly project management WebEx meetings, pre-mobilisation and post mobilisation practice communication together with reporting requirements.', N'', N'{"line1":"Westbury Court","line2":"Church Road","town":"Westbury On Trym","county":"Gloucester","postcode":"BS9 3EF","country":"England"}', NULL, NULL, 0, CAST(N'2020-04-07T09:44:04.4600000' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10063', N'Aire Logic', N'Aire Logic', NULL, NULL, N'{"line1":"Newlaithes Manor","line2":"Newlaithes Road","town":"Leeds","county":"West Yorkshire","postcode":"LS18 4LG","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-23T16:27:52.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10064', N'Medloop', N'Medloop', NULL, NULL, N'{"line1":"St James House","line2":"13 Kensington Square","town":"London","county":"Greater London","postcode":"W8 5HD","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-24T08:42:08.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10065', N'Sensely', N'Sensely', NULL, NULL, N'{"line1":"Pound House","line2":"62a Highgate High Street","town":"London","county":"Greater London","postcode":"N6 5HX","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-25T14:54:59.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10066', N'Primary Care IT Ltd', N'Primary Care IT Ltd', NULL, NULL, N'{"line1":"GF6 Trumpeter House","line2":"Trumpeter Rise","town":"Long Stratton","county":"Norfolk","postcode":"NR15 2DY","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-26T16:40:07.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10067', N'MEDITECH GROUP LIMITED', N'MEDITECH GROUP LIMITED', NULL, NULL, N'{"line1":"1 Northumberland Avenue","town":"London","county":"Greater London","postcode":"WC2N 5BW","country":"USA"}', NULL, NULL, 0, CAST(N'2019-07-26T20:54:06.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10068', N'Substrakt Health', N'Substrakt Health', NULL, NULL, N'{"line1":"2a Victoria Works","line2":"Vittoria St","town":"Birmingham","county":"West Midlands","postcode":"B1 3PE","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-29T08:26:17.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10069', N'Apollo Medical Software Solutions Limited', N'Apollo Medical Software Solutions Limited', NULL, NULL, N'{"line1":"12 Mansfield Centre Office Suite 0:1","line2":"Hamilton Way","line3":"Oakham Business Park","town":"Mansfield","county":"","postcode":"NG18 5FB","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-30T16:39:19.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10070', N'eSynergy Solutions Limited', N'eSynergy Solutions Limited', NULL, NULL, N'{"line1":"50 Fenchurch Street","town":"London","county":"","postcode":"EC3M 3JY","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-31T14:15:00.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10071', N'Connected Tech Group', N'Connected Tech Group', NULL, NULL, N'{"line1":"22, Mount Ephraim","town":"Tunbridge Wells","county":"Kent","postcode":"TN4 8AS","country":"England"}', NULL, NULL, 0, CAST(N'2019-08-01T14:36:00.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10072', N'Push Dr Limited', N'Push Dr Limited', NULL, NULL, N'{"line1":"Arkwright House","line2":"Parsonage","town":"Manchester","county":"Cheshire","postcode":"M3 2LF","country":"England"}', NULL, NULL, 0, CAST(N'2019-08-02T05:22:07.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10073', N'iPLATO', N'iPlato Healthcare Ltd', N'iPLATO has been operating mobile patient services in primary care since 2005. In this time we have launched the iPLATO service with over 1,800 GP practices in 75 CCG/PCTs reaching over 25 million patients.

iPLATO is the digital bridge between any of the principal GP IT systems used by the GP network and their patients using one of the most ubiquitous of modern tools – their own mobile phone.

•	Over 90% of adults in Britain own (at least one) mobile phone.
•	Personal, private and immediate – ideal for health communication.
•	Preferred by many patients, particularly the ‘hard-to-reach’.

More than 25 million patients in the UK already receive messages from their GP or Healthcare provider through iPLATO Patient Care Messaging for Primary Care system.', N'', N'{"line1":"1 King Street","town":"London","county":"Greater London","postcode":"W6 9HR","country":"England"}', NULL, NULL, 0, CAST(N'2020-04-01T13:48:54.1466667' AS datetime2), N'00000000-0000-0000-0000-000000000000');

MERGE INTO dbo.Supplier AS TARGET
USING #Supplier AS SOURCE
ON TARGET.Id = SOURCE.Id 
WHEN MATCHED THEN  
       UPDATE SET TARGET.[Name] = SOURCE.[Name],
                  TARGET.LegalName = SOURCE.LegalName,
                  TARGET.Summary = SOURCE.Summary,
                  TARGET.SupplierUrl = SOURCE.SupplierUrl,
                  TARGET.[Address] = SOURCE.[Address],
                  TARGET.OdsCode = SOURCE.OdsCode,
                  TARGET.CrmRef = SOURCE.CrmRef,
                  TARGET.Deleted = SOURCE.Deleted,
                  TARGET.LastUpdated = SOURCE.LastUpdated,
                  TARGET.LastUpdatedBy = SOURCE.LastUpdatedBy
WHEN NOT MATCHED BY TARGET THEN  
        INSERT (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
        VALUES (SOURCE.Id, SOURCE.[Name], SOURCE.LegalName, SOURCE.Summary, SOURCE.SupplierUrl, SOURCE.[Address], SOURCE.OdsCode, SOURCE.CrmRef, SOURCE.Deleted, SOURCE.LastUpdated, SOURCE.LastUpdatedBy);
END;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./ProdLikeData/MergeCatalogueItems.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN
    /*********************************************************************************************************************************************/
    /* CatalogueItem */
    /*********************************************************************************************************************************************/

    CREATE TABLE #CatalogueItem
    (
        CatalogueItemId nvarchar(14) NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        CatalogueItemTypeId int NOT NULL,
        SupplierId nvarchar(6) NOT NULL,
        PublishedStatusId int NOT NULL,
        Created datetime2(7) NOT NULL
    );

    -- Catalogue Solutions
    INSERT INTO #CatalogueItem (CatalogueItemId, [Name], CatalogueItemTypeId, SupplierId, PublishedStatusId, Created) 
         VALUES (N'10000-001', N'Emis Web GP',                          1, N'10000', 3, CAST(N'2020-03-25T07:30:18.1133333' AS datetime2)),
                (N'10000-002', N'Anywhere Consult',                     1, N'10000', 3, CAST(N'2020-04-06T10:50:03.2166667' AS datetime2)),
                (N'10000-054', N'Online and Video Consult',             1, N'10000', 3, CAST(N'2020-04-03T12:25:59.0533333' AS datetime2)),
                (N'10000-062', N'Video Consult',                        1, N'10000', 3, CAST(N'2020-04-06T10:53:50.6266667' AS datetime2)),
                (N'10004-001', N'Audit+',                               1, N'10004', 3, CAST(N'2020-03-26T12:13:20.0833333' AS datetime2)),
                (N'10004-002', N'FrontDesk',                            1, N'10004', 3, CAST(N'2020-03-30T13:14:43.1666667' AS datetime2)),
                (N'10007-002', N'Best Pathway',                         1, N'10007', 3, CAST(N'2020-03-25T11:40:44.2900000' AS datetime2)),
                (N'10011-003', N'Rapid VC',                             1, N'10011', 1, CAST(N'2020-06-18T14:20:53.8233333' AS datetime2)),
                (N'10020-001', N'Q doctor',                             1, N'10020', 3, CAST(N'2020-04-06T12:50:27.8800000' AS datetime2)),
                (N'10029-001', N'RIVIAM GP Portal',                     1, N'10029', 1, CAST(N'2020-04-08T07:42:58.2633333' AS datetime2)),
                (N'10029-003', N'RIVIAM Secure Video Services',         1, N'10029', 3, CAST(N'2020-04-08T08:59:03.8100000' AS datetime2)),
                (N'10030-001', N'AccuRx',                               1, N'10030', 3, CAST(N'2020-04-01T10:39:24.7066667' AS datetime2)),
                (N'10031-001', N'GP&Me',                                1, N'10031', 1, CAST(N'2020-04-01T10:37:59.3066667' AS datetime2)),
                (N'10033-001', N'AlldayDr',                             1, N'10033', 3, CAST(N'2020-04-01T10:40:33.7566667' AS datetime2)),
                (N'10035-001', N'Evergreen Life',                       1, N'10035', 3, CAST(N'2020-04-01T10:42:08.5066667' AS datetime2)),
                (N'10046-001', N'Docman 10',                            1, N'10046', 3, CAST(N'2020-03-30T13:02:24.5200000' AS datetime2)),
                (N'10046-003', N'Docman 7',                             1, N'10046', 3, CAST(N'2020-03-30T13:04:21.6500000' AS datetime2)),
                (N'10046-006', N'PATCHS Online Consultation',           1, N'10046', 1, CAST(N'2020-06-25T14:31:07.2366667' AS datetime2)),
                (N'10047-001', N'askmyGP',                              1, N'10047', 3, CAST(N'2020-04-01T10:43:15.8533333' AS datetime2)),
                (N'10052-002', N'SystmOne GP',                          1, N'10052', 3, CAST(N'2020-03-30T13:19:48.8766667' AS datetime2)),
                (N'10059-001', N'Advice & Guidance (Eclipse Live)',     1, N'10059', 3, CAST(N'2020-03-30T13:16:49.4100000' AS datetime2)),
                (N'10062-001', N'FootFall',                             1, N'10062', 3, CAST(N'2020-04-03T12:28:52.3800000' AS datetime2)),
                (N'10063-002', N'Forms4Health',                         1, N'10063', 1, CAST(N'2020-06-25T14:30:56.3300000' AS datetime2)),
                (N'10064-003', N'Medloop Patient Management Optimiser', 1, N'10064', 1, CAST(N'2020-06-25T14:30:49.8600000' AS datetime2)),
                (N'10072-003', N'Push Consult',                         1, N'10072', 1, CAST(N'2020-06-25T14:30:33.5166667' AS datetime2)),
                (N'10072-004', N'Digital Locum',                        1, N'10072', 1, CAST(N'2020-06-25T14:31:34.0466667' AS datetime2)),
                (N'10072-006', N'Push Access',                          1, N'10072', 1, CAST(N'2020-06-25T14:31:15.0166667' AS datetime2)),
                (N'10073-009', N'Remote Consultation',                  1, N'10073', 3, CAST(N'2020-04-01T12:49:33.9433333' AS datetime2)),
    -- Additional Services
                (N'10030-001A001', N'AccuRx Video Consultation',          2, N'10030', 3, GETUTCDATE()),
                (N'10007-002A001', N'Localised Referral Forms',           2, N'10007', 3, GETUTCDATE()),
                (N'10007-002A002', N'Localised Supporting Content',       2, N'10007', 3, GETUTCDATE()),
                (N'10000-001A008', N'Enterprise Search and Reports',      2, N'10000', 3, GETUTCDATE()),
                (N'10000-001A007', N'Risk Stratification',                2, N'10000', 3, GETUTCDATE()),
                (N'10000-001A006', N'Document Management',                2, N'10000', 3, GETUTCDATE()),
                (N'10000-001A005', N'EMIS Web Dispensing',                2, N'10000', 3, GETUTCDATE()),
                (N'10000-001A003', N'Automated Arrivals',                 2, N'10000', 3, GETUTCDATE()),
                (N'10000-001A004', N'Extract Services',                   2, N'10000', 3, GETUTCDATE()),
                (N'10000-001A002', N'EMIS Mobile',                        2, N'10000', 3, GETUTCDATE()),
                (N'10000-001A001', N'Long Term Conditions Manager',       2, N'10000', 3, GETUTCDATE()),
                (N'10035-001A001', N'Digital First Consultations',        2, N'10035', 3, GETUTCDATE()),
                (N'10052-002A001', N'SystmOne Enhanced',                  2, N'10052', 3, GETUTCDATE()),
                (N'10052-002A002', N'SystmOne Mobile Working',            2, N'10052', 3, GETUTCDATE()),
                (N'10052-002A004', N'SystemOne Shared Admin',             2, N'10052', 3, GETUTCDATE()),
                (N'10052-002A003', N'SystmOne Auto Planner',              2, N'10052', 3, GETUTCDATE()),
                (N'10052-002A005', N'TPP Video Conferencing with Airmid', 2, N'10052', 3, GETUTCDATE()),
    -- Associated Services
                (N'10046-S-001',  N'Consultancy',                                                                        3, N'10046', 3, GETUTCDATE()),
                (N'10046-S-007',  N'Data Import Activity',                                                               3, N'10046', 3, GETUTCDATE()),
                (N'10046-S-010',  N'Database creation/software initialisation',                                          3, N'10046', 3, GETUTCDATE()),
                (N'10046-S-009',  N'Docman 10 upgrade for current Docman 7 customer',                                    3, N'10046', 3, GETUTCDATE()),
                (N'10046-S-008',  N'eLearning',                                                                          3, N'10046', 3, GETUTCDATE()),
                (N'10046-S-006',  N'Post-Deployment/ Upgrade Day',                                                       3, N'10046', 3, GETUTCDATE()),
                (N'10046-S-002',  N'Practice Merger or Split',                                                           3, N'10046', 3, GETUTCDATE()),
                (N'10046-S-005',  N'Pre-Deployment/ Upgrade Day',                                                        3, N'10046', 3, GETUTCDATE()),
                (N'10046-S-003',  N'Remote Training',                                                                    3, N'10046', 3, GETUTCDATE()),
                (N'10046-S-004',  N'Training Services',                                                                  3, N'10046', 3, GETUTCDATE()),
                (N'10007-S-001',  N'Bespoke Documentation',                                                              3, N'10007', 3, GETUTCDATE()),
                (N'10007-S-002',  N'Clinical System Migration/Data Consolidation',                                       3, N'10007', 3, GETUTCDATE()),
                (N'10007-S-003',  N'Deployment',                                                                         3, N'10007', 3, GETUTCDATE()),
                (N'10007-S-004',  N'Process Optimisation',                                                               3, N'10007', 3, GETUTCDATE()),
                (N'10007-S-005',  N'Training',                                                                           3, N'10007', 3, GETUTCDATE()),
                (N'10000-S-001',  N'Video Consult, Online and Video Consult – Additional Minute Package (3000 minutes)', 3, N'10000', 3, GETUTCDATE()),
                (N'10000-S-036',  N'Engineering',                                                                        3, N'10000', 3, GETUTCDATE()),
                (N'10000-S-004',  N'Automated Arrivals – Engineering Half Day',                                          3, N'10000', 3, GETUTCDATE()),
                (N'10000-S-141',  N'Online and Video Consult – Implementation',                                          3, N'10000', 3, GETUTCDATE()),
                (N'10000-S-002',  N'Installation',                                                                       3, N'10000', 3, GETUTCDATE()),
                (N'10000-S-005',  N'Enterprise Search and Reports – Installation',                                       3, N'10000', 3, GETUTCDATE()),
                (N'10000-S-006',  N'Automated Arrivals – Installation of wall mounted kiosk, excludes cabling',          3, N'10000', 3, GETUTCDATE()),
                (N'10000-S-009',  N'Anywhere Consult – Integrated Device',                                               3, N'10000', 3, GETUTCDATE()),
                (N'10000-S-037',  N'Lloyd George digitisation',                                                          3, N'10000', 3, GETUTCDATE()),
                (N'10000-S-038',  N'Lloyd George Digitisation (upload only)',                                            3, N'10000', 3, GETUTCDATE()),
                (N'10000-S-039',  N'Migration',                                                                          3, N'10000', 3, GETUTCDATE()),
                (N'10000-S-069',  N'Doc Management – Migration',                                                         3, N'10000', 3, GETUTCDATE()),
                (N'10000-S-040',  N'Practice Reorganisation',                                                            3, N'10000', 3, GETUTCDATE()),
                (N'10000-S-041',  N'Project Management',                                                                 3, N'10000', 3, GETUTCDATE()),
                (N'10000-S-007',  N'Automated Arrivals – Specialist Cabling',                                            3, N'10000', 3, GETUTCDATE()),
                (N'10000-S-003',  N'Training Day at Practice',                                                           3, N'10000', 3, GETUTCDATE()),
                (N'10000-S-008',  N'Automated Arrivals – Wall Mount Bracket',                                            3, N'10000', 3, GETUTCDATE()),
                (N'10047-S-001',  N'Pathfinder',                                                                         3, N'10047', 3, GETUTCDATE()),
                (N'10047-S-002',  N'Transform',                                                                          3, N'10047', 3, GETUTCDATE()),
                (N'10004-S-002',  N'Customisation',                                                                      3, N'10004', 3, GETUTCDATE()),
                (N'10004-S-004',  N'Data Migration from existing appointments solution',                                 3, N'10004', 3, GETUTCDATE()),
                (N'10004-S-003',  N'End User Training Seminar',                                                          3, N'10004', 3, GETUTCDATE()),
                (N'10004-S-001',  N'Training Day at Practice',                                                           3, N'10004', 3, GETUTCDATE()),
                (N'10073-S-021',  N'Setup/Provisioning/Launch',                                                          3, N'10073', 3, GETUTCDATE()),                
                (N'10073-S-023',  N'my GP Integration',                                                                  3, N'10073', 3, GETUTCDATE()),
                (N'10029-S-007',  N'Business Analysis',                                                                  3, N'10029', 3, GETUTCDATE()),
                (N'10029-S-009',  N'Information Architecture',                                                           3, N'10029', 3, GETUTCDATE()),
                (N'10029-S-006',  N'Project Management',                                                                 3, N'10029', 3, GETUTCDATE()),
                (N'10029-S-008',  N'Solution Model Development',                                                         3, N'10029', 3, GETUTCDATE()),
                (N'10029-S-010',  N'Staff Training',                                                                     3, N'10029', 3, GETUTCDATE()),
                (N'10052-S-012',  N'Data Extraction Service',                                                            3, N'10052', 3, GETUTCDATE()),
                (N'10052-S-001',  N'Full Training Day',                                                                  3, N'10052', 3, GETUTCDATE()),
                (N'10052-S-002',  N'Half Training Day',                                                                  3, N'10052', 3, GETUTCDATE()),
                (N'10052-S-009',  N'Practice Merge',                                                                     3, N'10052', 3, GETUTCDATE()),
                (N'10052-S-010',  N'Practice Split',                                                                     3, N'10052', 3, GETUTCDATE()),
                (N'10052-S-011',  N'Provision of Legacy Data',                                                           3, N'10052', 3, GETUTCDATE()),
                (N'10052-S-003',  N'Super User Course',                                                                  3, N'10052', 3, GETUTCDATE()),
                (N'10052-S-013',  N'SystmOne GP Deployment (Data Migration, Full Training)',                             3, N'10052', 3, GETUTCDATE()),
                (N'10052-S-008',  N'SystmOne GP Deployment (Data Migration, Go-Live Support)',                           3, N'10052', 3, GETUTCDATE()),
                (N'10052-S-004',  N'Train the Trainer Course',                                                           3, N'10052', 3, GETUTCDATE()),
                (N'10052-S-005',  N'Training Environment',                                                               3, N'10052', 3, GETUTCDATE()),
                (N'10063-S-004',  N'Implementation Services',                                                            3, N'10063', 3, GETUTCDATE()),
                (N'10063-S-005',  N'Integration Services',                                                               3, N'10063', 3, GETUTCDATE()),
                (N'10063-S-006',  N'Support Services',                                                                   3, N'10063', 3, GETUTCDATE()),
                (N'10063-S-007',  N'Training',                                                                           3, N'10063', 3, GETUTCDATE()),
                (N'10063-S-008',  N'Training Day at Practice- Half Day',                                                 3, N'10063', 3, GETUTCDATE()),
                (N'10063-S-009',  N'End User Training',                                                                  3, N'10063', 3, GETUTCDATE()),
                (N'10063-S-010',  N'Form Creation',                                                                      3, N'10063', 3, GETUTCDATE()),
                (N'10072-S-005',  N'Push Access - Implementation',                                                       3, N'10072', 3, GETUTCDATE()),
                (N'10072-S-010',  N'Implementation (delivered remotely)',                                                3, N'10072', 3, GETUTCDATE()),
                (N'10072-S-011',  N'Implementation (On-site)',                                                           3, N'10072', 3, GETUTCDATE()),
                (N'10072-S-012',  N'Refresher Training',                                                                 3, N'10072', 3, GETUTCDATE()),
                (N'10072-S-013',  N'Marketing material to advertise the service',                                        3, N'10072', 3, GETUTCDATE()),
                (N'10030-S-001',  N'SMS Messaging',                                                                      3, N'10030', 3, GETUTCDATE()),
                (N'10073-S-022',  N'Training',                                                                      3, N'10030', 3, GETUTCDATE());

    MERGE INTO dbo.CatalogueItem AS TARGET
    USING #CatalogueItem AS SOURCE
    ON TARGET.CatalogueItemId = SOURCE.CatalogueItemId  
    WHEN MATCHED THEN  
           UPDATE SET TARGET.[Name] = SOURCE.[Name],
                      TARGET.CatalogueItemTypeId = SOURCE.CatalogueItemTypeId,
                      TARGET.SupplierId = SOURCE.SupplierId,
                      TARGET.PublishedStatusId = SOURCE.PublishedStatusId,
                      TARGET.Created = SOURCE.Created
    WHEN NOT MATCHED BY TARGET THEN  
        INSERT (CatalogueItemId, [Name], CatalogueItemTypeId, SupplierId, PublishedStatusId, Created) 
        VALUES (SOURCE.CatalogueItemId, SOURCE.[Name], SOURCE.CatalogueItemTypeId, SOURCE.SupplierId, SOURCE.PublishedStatusId, SOURCE.Created);
END;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./ProdLikeData/MergeSolutions.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN
/*********************************************************************************************************************************************/
/* Solution */
/*********************************************************************************************************************************************/

    CREATE TABLE #Solution
    (
         Id nvarchar(14) NOT NULL,
         [Version] nvarchar(10) NULL,
         Summary nvarchar(300) NULL,
         FullDescription nvarchar(3000) NULL,
         Features nvarchar(max) NULL,
         ClientApplication nvarchar(max) NULL,
         Hosting nvarchar(max) NULL,
         ImplementationDetail nvarchar(1000) NULL,
         RoadMap nvarchar(1000) NULL,
         IntegrationsUrl nvarchar(1000) NULL,
         AboutUrl nvarchar(1000) NULL,
         ServiceLevelAgreement nvarchar(1000) NULL,
         WorkOfPlan nvarchar(max) NULL,
         LastUpdated datetime2(7) NOT NULL,
         LastUpdatedBy uniqueidentifier NOT NULL,     
    );

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
         VALUES (N'10000-001', NULL, N'EMIS Web is the most widely used GP clinical system in the UK. Created by clinicians for clinicians, it helps run efficient practices, whilst delivering the best possible patient care. With patient safety at its core, EMIS Web enables you to deliver safe & informed on-demand care across locations.', N'We’re the UK leader in connected healthcare software and services. Through innovative IT we help healthcare professionals access the information they need to provide better, faster and more cost effective patient care.

This all came from one idea shared by two GPs: that technology can be used to give clinicians access to complete and shared medical records, no matter where patients present for care.

Our clinical software is now used in all major healthcare settings from GP surgeries to pharmacies, hospitals, and specialist community services. By providing innovative, integrated solutions, we’re working to break the boundaries of system integration and interoperability. 

For over 30 years, we’ve been working to ensure that healthcare professionals across the NHS have all the information they need by providing them with instant access to electronic patient records. We support this by supplying IT infrastructure and engineering services to enhance these systems.', N'["Access to real-time patient data that can be shared between locations & healthcare organisations","One-click access to patient summary information","Quick data entry using protocols, templates and concepts tailored to your practice requirements","Integrated clinical safety alerts graded to highlight severity","Automatic notification of linked pre-existing conditions when recording a new acute problem","Integrated patient recall system to target specific lists of patients for specific clinics","Intelligent alerts and auto-templates to capture outstanding QOF information","Integrated QOF-finder to identify patients where you’re losing QOF points","Seamless data exchange with over 100 partners including Graphnet, Cerner and Optum","Integration with Patient Access enables patients to book appointments and order prescriptions"]', N'{"ClientApplicationTypes":["browser-based","native-mobile","native-desktop"],"BrowsersSupported":["Google Chrome","Chromium","Internet Explorer 11","Internet Explorer 10"],"MobileResponsive":true,"Plugins":{"Required":false,"AdditionalInformation":""},"MinimumConnectionSpeed":"2Mbps","MinimumDesktopResolution":"16:9 – 1366 x 768","HardwareRequirements":"The browser activities are only supported in relation to the native desktop client and therefore mirror the native desktop client hardware requirements detailed below.","NativeMobileHardwareRequirements":"Any device capable of supporting the listed supported operating systems is compliant.","NativeDesktopHardwareRequirements":"The spoke server is an important part of the solution. It provides a patch distribution system for client updates and acts as a local cache. \r\n\r\nEMIS Health recommends that your spoke is a dedicated device. However, if you use your spoke to perform other functions, such as act as a domain controller, store business documents or host other applications, then a Windows server class operating system will be required, along with an appropriate specification of server hardware.","AdditionalInformation":"","MobileFirstDesign":false,"NativeMobileFirstDesign":false,"MobileOperatingSystems":{"OperatingSystems":["Apple IOS","Android","Other"],"OperatingSystemsDescription":"•\tiOS v 10.3.3.3 and above\r\n\r\n•\tAndroid v 6 and above\r\n\r\n•\tWindows 10 (Build 14393)"},"MobileConnectionDetails":{"MinimumConnectionSpeed":"","ConnectionType":["GPRS","3G","LTE","4G","5G","Wifi"],"Description":"The mobile application only requires internet connectivity to synchronise, therefore there is no minimum connection speed required."},"MobileMemoryAndStorage":{"MinimumMemoryRequirement":"2GB","Description":"All compliant devices must have a minimum 16GB storage."},"MobileThirdParty":{"ThirdPartyComponents":"","DeviceCapabilities":"The device should have access to the relevant App Store to enable the installation of the respective application although deployment via mobile device management solutions is supported."},"NativeMobileAdditionalInformation":"Apple have recently announced that a new operating system, designed specifically for iPad devices.\r\n\r\nWe have tested this and can confirm that EMIS Mobile is fully compatible.","NativeDesktopOperatingSystemsDescription":"Microsoft Windows 7 (x86 x64)\r\n\r\nMicrosoft Windows 8.1 (x86 x64)\r\n\r\nMicrosoft Windows 10 (x86 x64)","NativeDesktopMinimumConnectionSpeed":"2Mbps","NativeDesktopThirdParty":{"ThirdPartyComponents":".NET framework 4.","DeviceCapabilities":"The application requires connectivity to the EMIS Data Centre."},"NativeDesktopMemoryAndStorage":{"MinimumMemoryRequirement":"4GB","StorageRequirementsDescription":"10GB free disk space.","MinimumCpu":"Intel Core i3 equivalent or higher.","RecommendedResolution":"16:9 – 1366 x 768"},"NativeDesktopAdditionalInformation":"The minimum connection speed is dependent on the number of clients that need to be supported.\r\n\r\nEMIS Health do not support the use of on-screen keyboards for 2 in 1 devices."}', N'{"PublicCloud":{"Summary":"This service is not available on public cloud.","Link":""},"PrivateCloud":{"Summary":"EMIS Web is hosted in EMIS’s own data centres and the Solution is provided as Software as a Service.","Link":"","HostingModel":"Model complies with GP IT Futures requirements for hosting.","RequiresHSCN":"End user devices must be connected to HSCN/N3"},"HybridHostingType":{"Summary":"","Link":"","HostingModel":""},"OnPremise":{"Summary":"","Link":"","HostingModel":""}}', N'The typical timescales for the implementation of EMIS Web are around 12 weeks depending on the patient list size. 

An implementation plan is provided and agreed at the beginning of the process outlining all key dates and activities. Where required these dates are negotiable to fit the needs of the customer.

Key activities:

•	Customer supplied with high level implementation plan and welcome pack

•	Engineer visit to perform install of client software and check connectivity

•	Customer supplied test data loaded into a test system and made available

•	Learning needs analysis performed & agreed training plan for go-live

•	On site visit to train the customer how to check and cleanse their data

•	Any defects and corrections completed on migrated data

•	Practice sign-off of test data

•	Agreed training provide pre and post go-live in line with results from LNA

•	Go-live day, trainers and engineer onsite to support a smooth transition', N'Our roadmap details all GP IT Futures managed capacity items. EMIS Health is committed to delivering against the effective date. The roadmap provides visibility on which items have been completed, are scheduled and are in the pipeline to be scheduled.', N'https://www.emishealth.com/products/partner-products/', N'', NULL, NULL, CAST(N'2020-03-25T07:30:18.1133333' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
         VALUES (N'10000-002', NULL, N'EMIS’ mobile working solution designed to provide full access to EMIS Web & other essential NHS digital services via a portable 2-in-1 device. Connectivity via WAN, Wi-Fi or 4G helps maintain real-time access to vital patient information at the point of care, supporting informed decision making.', N'Flexible mobile working is an essential part of ensuring that you can deliver great care when and where it’s needed. By giving you complete access to EMIS Web when working remotely, Anywhere Consult provides the support you need to make the most effective decisions at the point of care. 

With full and secure access to a patient’s medical record, including their medications and appointments, it gives you access to important information no matter where you are. Attachments and information can be easily stored too, and you can view, edit and update patient records in real time. 

Anywhere Consult provides you with a back-up in an emergency, since it allows you to continue to use your clinical system even if your site is inaccessible.', N'["acts as one core device that brings all your on-site and remote needs together.","access to network through a secure 3G, Wi-Fi or LAN connection, with integrated smart card reader","this means you can access essential NHS systems and services, including email and NWW websites.","allows you to keep connected with your colleagues when working remotely.","can be integrated with EMIS Mobile to combine offline working with online.","gain the same capabilities when on your organisation’s premises with an optional Microsoft licence.","","","",""]', N'{"ClientApplicationTypes":["browser-based","native-desktop"],"BrowsersSupported":["Google Chrome","Microsoft Edge","Mozilla Firefox","Internet Explorer 10"],"MobileResponsive":true,"Plugins":{"Required":false,"AdditionalInformation":""},"MinimumConnectionSpeed":"2Mbps","MinimumDesktopResolution":"16:9 – 1366 x 768","HardwareRequirements":"The browser activities are only supported in relation to the native desktop client therefore mirror the native desktop client hardware requirements detailed below.","NativeDesktopHardwareRequirements":"EMIS Health will provide you with all relevant hardware.","AdditionalInformation":"Browser versions supported: \r\n\r\n- Google Chrome v48 and later \r\n- Internet Explorer 10 \r\n- Firefox v38 \r\n- Microsoft Edge v38","MobileFirstDesign":false,"NativeDesktopOperatingSystemsDescription":"Microsoft Windows 8.1 (x86 x64) \r\nMicrosoft Windows 10 (x86 x64)","NativeDesktopMinimumConnectionSpeed":"2Mbps","NativeDesktopThirdParty":{"ThirdPartyComponents":".NET framework 4","DeviceCapabilities":"The application requires connectivity to the EMIS Data Centre."},"NativeDesktopMemoryAndStorage":{"MinimumMemoryRequirement":"4GB","StorageRequirementsDescription":"10GB free disk space","MinimumCpu":"Intel Core i3 equivalent or higher","RecommendedResolution":"16:9 – 1366 x 768"},"NativeDesktopAdditionalInformation":"EMIS Health do not support the use of on-screen keyboards for 2 in 1 devices."}', N'{"PrivateCloud":{"Summary":"","Link":"","HostingModel":"The data centre hosting model is the same as EMIS Web GP. \r\n\r\nA HSCN/N3 connection is supplied with the solution.","RequiresHSCN":"End user devices must be connected to HSCN/N3"}}', N'The typical timescales for the implementation of Anywhere Consult is 6 weeks depending on the patient list size.', NULL, N'https://www.emishealth.com/products/partner-products/', N'', NULL, NULL, CAST(N'2020-04-06T10:50:03.2166667' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
         VALUES (N'10000-054', NULL, N'Our online triaging system allows patients to easily access self-help advice, and fill out forms containing their requests and queries that go into their practice''s workflow. Helping to improve access to services, Online Consult is configurable, and integrates with all leading GP Clinical Systems.', N'Work efficiently and collaboratively with our online triaging system. Simply accessed by clicking a button on your practice website, Online Consult allows patients to easily seek support or access self-help advice, either for themselves or for those in their care. Video Consult allows you to see patients without them having to attend your practice in person. 

Whether you’re a single practice looking to streamline triage or a CCG wanting to transform care across an area, our integrated system can help organisations of all sizes to improve access to their services.', N'["Save time for clinicians and patients: Collect relevant information before consultations","Quickly interpret info: Forms designed by practicing GPs meaning details can be understood by all","Reduce strain on reception/phones: Assign appointments based on clinical need, not patient pressure","Manage expectations: choose the response times and completion messages that patients see","Streamline triage: EMIS Web, Patient Access & Docman integration will feed forms into your workflow","Message Exchange for Social Care & Health integration: integration with all clinical systems","SMS messaging: Through EMIS Web, messages to patients can be sent through accuRx partner product","SMS messaging: If using TPP SystmOne messages to patients can be sent through the SMS feature","SMS messaging: If using Vision messages to patients can be sent through the SMS feature",""]', N'{"ClientApplicationTypes":["browser-based","native-mobile","native-desktop"],"BrowsersSupported":["Google Chrome","Microsoft Edge","Mozilla Firefox","Safari","Internet Explorer 10"],"MobileResponsive":true,"Plugins":{"Required":false,"AdditionalInformation":""},"MinimumConnectionSpeed":"2Mbps","MinimumDesktopResolution":"16:9 – 1366 x 768","HardwareRequirements":"Integrated or standalone Webcam and microphone\r\n required for video consultations.","NativeMobileHardwareRequirements":"Integrated or standalone Webcam and microphone\r\nrequired for video consultations","NativeDesktopHardwareRequirements":"Camera and microphone","AdditionalInformation":"Browser versions supported: \r\n\r\n• Google Chrome v.48 and later\r\n• Internet Explorer 10\r\n• Firefox v63 and later\r\n• Safari 10 and later\r\n• Edge v42 and later","MobileFirstDesign":true,"NativeMobileFirstDesign":true,"MobileOperatingSystems":{"OperatingSystems":["Apple IOS","Android","Other"],"OperatingSystemsDescription":"•\tiOS v 10.3.3.3 and above\r\n•\tAndroid v 6 and above\r\n•\tWindows 10 (Build 14393)"},"MobileConnectionDetails":{"MinimumConnectionSpeed":"2Mbps","ConnectionType":["GPRS","3G","LTE","4G","5G","Wifi"],"Description":""},"MobileMemoryAndStorage":{"MinimumMemoryRequirement":"256MB","Description":"N/A"},"MobileThirdParty":{"ThirdPartyComponents":"","DeviceCapabilities":"The device should have access to the relevant App Store to enable the installation of the respective application although deployment via mobile device management solutions is supported."},"NativeDesktopOperatingSystemsDescription":"Microsoft Windows 7 (x86 x64)\r\nMicrosoft Windows 8.1 (x86 x64)\r\nMicrosoft Windows 10 (x86 x64)","NativeDesktopMinimumConnectionSpeed":"2Mbps","NativeDesktopMemoryAndStorage":{"MinimumMemoryRequirement":"4GB","StorageRequirementsDescription":"N/A","MinimumCpu":"N/A","RecommendedResolution":"16:9 – 1366 x 768"}}', N'{"PublicCloud":{"Summary":"Patient facing Online and Video Consult","Link":""},"PrivateCloud":{"Summary":"Private version of Online Consult","Link":"","HostingModel":"Same as EMIS Web."}}', N'EMIS’s Deployment & Implementation Team has in depth experience in supporting the delivery of our Online Consult & Video Consult solutions.  We have implemented & support across 24 CCGs with Online Consult & 23 CCGs with Video Consult, in addition to numerous single practices & have developed structured, tried & tested workflows to support deployment for both solutions  Our team comprises Project Managers and support staff who work in partnership with CCG & practice reps to fulfil the requirements of each contract. 
 
Our project management approach is based on Prince2 methodology, providing a robust yet flexible framework to help ensure the best possible outcomes for all stakeholders. 
 
A typical practice deployment takes up to 4 weeks, but some practices are ‘live’ much sooner, especially those who understand the benefits & are fully engaged.  We can & deliver against, the need to shorten this lead-time to support practices in response to COVID-19, working as a team with them.', N'The following roadmap details all IT Futures managed capacity items. EMIS Health is committed to delivering against the effective date. The roadmap provides visibility on which items have been completed, are scheduled and are in the pipeline to be scheduled.', NULL, N'', NULL, NULL, CAST(N'2020-04-03T12:25:59.0533333' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
    VALUES (N'10000-062', NULL, N'Offer patients an effective appointment alternative with Video Consult. Available on multiple platforms, including desktop, and native iOS and Android apps, Video Consult can be used alongside any GP clinical system and offers additional integration with EMIS Web and Patient Access.', N'Whether a patient is housebound, has a long-term condition or simply can’t fit an appointment at your premises into their day, Video Consult can help. By allowing patients to get the help they need without travelling to your surgery, it improves both their experience and access to care.

Our secure video consultation system has the option to integrate with Patient Access to give patients a convenient and accessible alternative to traditional appointments. 

Providing a convenient alternative to traditional appointments, our video consultation software for GPs allows patients to consult with their clinician over video. It means that, no matter if they’re in the office, at home or somewhere else, patients can still get the help they require – even at times that they normally wouldn’t be able to make.', N'["Gives patients an accessible way to see their doctor, via video calls, with any clinical software","Provide connected & comprehensive care to invite colleagues & specialists into video consultations","Ensure that everyone is better off by using this version across your practice, CCG or federation","Video Consult with EMIS Web& Patient Access can be integrated in order to make everything seamless","Patient information accessed easily by starting & managing consultations from within EMIS Web","Seamless video consultation experience through easy access to the ‘Patient Access’ system","","","",""]', N'{"ClientApplicationTypes":["browser-based","native-mobile","native-desktop"],"BrowsersSupported":["Google Chrome","Microsoft Edge","Mozilla Firefox","Safari","Internet Explorer 10"],"MobileResponsive":true,"Plugins":{"Required":false,"AdditionalInformation":""},"MinimumConnectionSpeed":"2Mbps","MinimumDesktopResolution":"16:9 – 1366 x 768","HardwareRequirements":"Camera and microphone required.","NativeMobileHardwareRequirements":"Integrated or standalone Webcam and microphone","NativeDesktopHardwareRequirements":"Integrated or standalone Webcam and microphone","AdditionalInformation":"Supported Browser versions: \r\n\r\n• Google Chrome v.48 and later\r\n• Internet Explorer 10\r\n• Firefox v63 and later\r\n• Safari 10 and later\r\n• Edge v42 and later","MobileFirstDesign":true,"NativeMobileFirstDesign":true,"MobileOperatingSystems":{"OperatingSystems":["Apple IOS","Android","Other"],"OperatingSystemsDescription":"• iOS v 10.3.3.3 and above\r\n• Android v 6 and above\r\n• Windows 10 (Build 14393)"},"MobileConnectionDetails":{"MinimumConnectionSpeed":"2Mbps","ConnectionType":["GPRS","3G","LTE","4G","5G","Wifi"],"Description":""},"MobileMemoryAndStorage":{"MinimumMemoryRequirement":"256MB","Description":"N/A"},"MobileThirdParty":{"ThirdPartyComponents":"","DeviceCapabilities":"The device should have access to the relevant App Store to enable the installation of the respective application although deployment via mobile device management solutions is supported."},"NativeDesktopOperatingSystemsDescription":"Microsoft Windows 7 (x86 x64)\r\nMicrosoft Windows 8.1 (x86 x64)\r\nMicrosoft Windows 10 (x86 x64)","NativeDesktopMinimumConnectionSpeed":"2Mbps","NativeDesktopMemoryAndStorage":{"MinimumMemoryRequirement":"4GB","StorageRequirementsDescription":"N/A","MinimumCpu":"N/A","RecommendedResolution":"16:9 – 1366 x 768"}}', N'{"PublicCloud":{"Summary":"Patient facing Online and Video Consult","Link":""},"PrivateCloud":{"Summary":"Private version of Video Consult","Link":"","HostingModel":"Same as EMIS Web."}}', N'EMIS’s Deployment & Implementation Team has in depth experience in supporting the delivery of our Online Consult & Video Consult solutions.  We have implemented & support across 24 CCGs with Online Consult & 23 CCGs with Video Consult, in addition to numerous single practices & have developed structured, tried & tested workflows to support deployment for both solutions  Our team comprises Project Managers and support staff who work in partnership with CCG & practice reps to fulfil the requirements of each contract. 
 
Our project management approach is based on Prince2 methodology, providing a robust yet flexible framework to help ensure the best possible outcomes for all stakeholders. 
 
A typical practice deployment takes up to 4 weeks, but some practices are ‘live’ much sooner, especially those who understand the benefits & are fully engaged.  We can & deliver against, the need to shorten this lead-time to support practices in response to COVID-19, working as a team with them.', N'The following roadmap details all IT Futures managed capacity items. EMIS Health is committed to delivering against the effective date. The roadmap provides visibility on which items have been completed, are scheduled and are in the pipeline to be scheduled.', NULL, N'', NULL, NULL, CAST(N'2020-04-06T10:53:50.6266667' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
    VALUES (N'10004-001', NULL, N'Audit+ is a tool designed to raise standards of clinical care. Comparing the patients clinical record with a database of evidence based guidance, specific to medical history & personal circumstances, delivering prompts to the GP & tools to support patient management within & outside a consultation.', N'Audit+ is a unique software suite that helps healthcare organisations deliver highly effective Advanced Population Health to reduce the prevalence of long-term conditions more effectively. 

The software suite provides the most customisable, flexible data audits on the market for more advanced patient data identification & collection.? By automatically identifying eligible patients according to a list of programmed rules, Audit+ helps primary care teams manage high impact & effective preventative programs with little administrative burden & easy automatic processes.? 

Simple, discreet signposts & alerts give GPs the information they need to deliver the most appropriate care programmes to amenable patients & central reporting functionality allows accurate analysis of the needs of a population, individual patients & smaller cohorts. 

Audit+ is proven to deliver effective programs for: 

Preventative care 

Diabetes and cardiovascular disease prevention 

NHS Health Checks', N'["Highly configurable algorithms to ensure accurate patient identification","Customisable and flexible audits in line with local and national healthcare challenges","Consistent and comparable data measures across communities","Easy reporting across all practices with a single standard approach","Relevant alerts, reminders and feedback for GPs to promote behaviour change without alert fatigue","All your preventative care initiatives managed on one dedicated platform","Integrates with leading primary care systems, including EMIS and Microtest","ISO27001 and Cyber Essentials Plus","Accredited by NHS Digital",""]', N'{"ClientApplicationTypes":["native-desktop"],"BrowsersSupported":[],"NativeDesktopOperatingSystemsDescription":"Windows 7 and above","NativeDesktopMinimumConnectionSpeed":"1Mbps","NativeDesktopMemoryAndStorage":{"MinimumMemoryRequirement":"512MB","StorageRequirementsDescription":"150Mb local disk space for client file installation","MinimumCpu":"500Mhz processor or better","RecommendedResolution":"4:3 – 1024 x 768"},"NativeDesktopAdditionalInformation":"Integration with Vision is via the commercial API."}', N'{"OnPremise":{"Summary":"The Informatica Server software is installed on local hardware within the Surgery.","Link":"","HostingModel":"","RequiresHSCN":"End user devices must be connected to HSCN/N3"}}', N'Audit+ is a standalone product and is available for use as soon as the IM1 feed is enabled by your GP System provider.', N'New features and improvements are detailed on our Roadmap:

https://www.informatica-systems.co.uk/roadmaps/', N'', N'https://www.informatica-systems.co.uk/', NULL, NULL, CAST(N'2020-03-26T12:13:20.0833333' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
         VALUES (N'10004-002', NULL, N'FrontDesk is a fully featured appointment system which provides practices with an easy to use and flexible way to manage appointments.', N'FrontDesk is the dedicated practice management system from primary care software experts, Informatica.

FrontDesk supports thousands of clinicians, practice managers and administrative staff with the day to day running of a busy practice.

Using the latest technology, FrontDesk helps to streamline processes, create administration efficiencies and provide a seamless experience for patients.', N'["Manages complex surgeries for larger practices","Full book management system","Built in flexible rota management","Highly configurable for local systems","Multiple views of the appointment book","Manages complex resource and device allocation","Supports open surgeries and joint appointments","Huge range of built in reports to optimise working practices","",""]', N'{"ClientApplicationTypes":["native-desktop"],"BrowsersSupported":[],"NativeDesktopOperatingSystemsDescription":"Windows 7 and above","NativeDesktopMinimumConnectionSpeed":"1Mbps","NativeDesktopMemoryAndStorage":{"MinimumMemoryRequirement":"512MB","StorageRequirementsDescription":"150Mb local disk space for client file installation","MinimumCpu":"500Mhz processor or better","RecommendedResolution":"4:3 – 1024 x 768"}}', N'{"OnPremise":{"Summary":"Locally hosted","Link":"","HostingModel":"","RequiresHSCN":"End user devices must be connected to HSCN/N3"}}', N'FrontDesk is a standalone product, it is available for use as soon as the IM1 feed is enabled by your GP System provider. It is recommended that Practices commission at least 1 days staff training prior to go live.', N'New features and improvements are detailed on our Roadmap.

https://frontdesk.info/roadmaps/', NULL, N'https://frontdesk.info/', NULL, NULL, CAST(N'2020-03-30T13:14:43.1666667' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
        VALUES (N'10007-002', NULL, N'Best Pathway enables national and regional authorities to manage and distribute decision support information directly to primary care users. Users have access to templates, pathways, referral forms, patient information leaflets, travel information and medicine reference during consultations.', N'Best Pathway is a single, essential clinical knowledge base and information platform that overcomes information overload. Activated in the clinician consultation workflow providing immediate access to relevant and standardised knowledge and information that is tightly aligned to national and local protocols, guidelines, intelligently coded and presented at the appropriate time. It is designed to improve clinical knowledge, provide instant access best evidence treatment pathways, medicines advice and features that save time and improve outcomes. 

Our comprehensive, searchable database with more than 25,000 national, regional and local clinically relevant documents ranging from professional guidance, patient and medicines information is sourced from the UK’s leading medical publishers and organisations. It enables national, regional authorities and/or local practices to manage and distribute relevant and policy aligned decision support information to primary care users.', N'["Local & National Clinical pathways in the clinician consultation workflow","NICE, SIGN and Red Whale guidance documents","Diagnostic Scoring Tools","Up to date travel advice","Patient UK and DXS Insight leaflets plus illustrations","Medicines information – Summary of Product Characteristics & Patient Information","National Directory of Services","Your content can be localised to your requirements","Integrated with all clinical systems","Content maintained & updated daily by clinical team"]', N'{"ClientApplicationTypes":["native-desktop"],"BrowsersSupported":[],"NativeDesktopHardwareRequirements":"Best Pathway requires a central server to host the Best Pathway database and database back end.","NativeDesktopOperatingSystemsDescription":"•\tWindows Server 2012*\r\n•\tWindows Server 2012*,\r\n•\tWindows Server 2016*,\r\n•\tWindows Server 2019*, \r\n•\tWindows 7*, Windows 8.1*\r\n•\tWindows 10* \r\n\r\n*UAC now supported with DXS (Files are now code signed with a Thawte certificate)","NativeDesktopMinimumConnectionSpeed":"5Mbps","NativeDesktopThirdParty":{"ThirdPartyComponents":"The catalogue solution is dependent on an integration with a principle clinical system.","DeviceCapabilities":""},"NativeDesktopMemoryAndStorage":{"MinimumMemoryRequirement":"2GB","StorageRequirementsDescription":"4GB (Server/Installation Machine), 200MB (Workstation)","MinimumCpu":"Intel i3 or better","RecommendedResolution":"5:4 – 1280 x 1024"}}', N'{"HybridHostingType":{"Summary":"Best Pathway requires a central server for hosting the database as well as the database back end to which all workstations will connect. Best Pathway will be installed on all workstations.","Link":"","HostingModel":"The central server will be hosted at a HSCN approved aggregator.","RequiresHSCN":"End user devices must be connected to HSCN/N3"},"OnPremise":{"Summary":"Best Pathway requires a central server for hosting the database as well as the database back end to which all workstations will connect. Best Pathway will be installed on all workstations.","Link":"","HostingModel":"The data centre hosting model is dependent on the CSU or practice hosting the central server."}}', N'Minimum: 20 business days
Maximum: 40 business days
Average Timescale: 30 business days

The minimum time is indicative of a Service Recipient (being a CCG) only taking the basic offering with no customised content, the average is indicative of the national deployment time as well as the time that is required to localise an average amount of content for a Service Recipient (being a CCG) and the maximum is indicative of the national deployment time as well as the time that is required to localise a large amount of content for a Service Recipient (being a CCG) and or transitioning from another solution.

Buyer expectations per practice:

A machine that runs 24/7 and meets our recommended specification and the corresponding logon credentials of this machine i.e username & password.

An account with administrator rights and the corresponding logon credentials of this account i.e username & password or SMART card.

To ensure that your practice DXS user list has been sent to DXS.', NULL, NULL, N'https://www.dxs-systems.co.uk/dxs-point-of-care.php#intro', NULL, NULL, CAST(N'2020-03-25T11:40:44.2900000' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
         VALUES (N'10011-003', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2020-06-18T14:20:53.8233333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10020-001', NULL, N'Q doctor is a secure video consultation platform used around the NHS nationwide, allowing remote consultations by clinicians and patients.', N'Q doctor has been implemented as the secure video consulting system of choice across hundreds of NHS organisations; it became the first solution approved for 111/CAS environments and supported the first Trust delivering specialist outpatients appointments over video.', N'["Clinician dashboard with real-time appointment list","Patient facing app (iOS and Android) and web access","Secure video consultation interface","Secure instant messaging","Secure image upload capability","Clinician safety tools in consultation","Live Chat functionality on apps and web with 8am-8pm support for all users","Organisation level roles and permissions, built in partnership with an STP","Organisation level metrics real-time dashboards for clinical and business insights","Q health integrates with eConsult, allowing ‘total triage’ for practices as a single pathway"]', N'{"ClientApplicationTypes":["browser-based","native-mobile"],"BrowsersSupported":["Google Chrome","Microsoft Edge","Safari","Chromium"],"MobileResponsive":true,"Plugins":{"Required":false,"AdditionalInformation":""},"MinimumConnectionSpeed":"5Mbps","MinimumDesktopResolution":"16:9 – 1280 x 720","HardwareRequirements":"Webcam","NativeMobileHardwareRequirements":"Supports biometric device security if available.","AdditionalInformation":"Q health is compatible with Google Chrome and Edge chromium browsers for the clinicians and administrators.\r\n\r\nDetails of Browsers Supported: \r\n\r\nMinimum Edge; full support for Microsoft Edge 79+ (Chromium-based versions of Edge). \r\n\r\nGoogle Chrome; Version 80.0.3987.149 (Official Build) (64-bit) or later \r\n\r\nBrowsers supported on the  patient side: All of the above, plus Safari; Safari 11 or later on MacOS and Safari on iOS 11 or later. For the best experience Safari 13 or later is recommended.","MobileFirstDesign":true,"NativeMobileFirstDesign":true,"MobileOperatingSystems":{"OperatingSystems":["Apple IOS","Android"],"OperatingSystemsDescription":"iOS version 11.4 or later.  Android version 7 or later."},"MobileConnectionDetails":{"MinimumConnectionSpeed":"5Mbps","ConnectionType":["3G","LTE","4G","5G","Wifi"],"Description":""},"MobileMemoryAndStorage":{"MinimumMemoryRequirement":"256MB","Description":"Patient side, mobile app (if used) is small (20-30mb)"},"MobileThirdParty":{"ThirdPartyComponents":"","DeviceCapabilities":"Device must have a camera for use in video consultation (supports rear facing camera additionally if available)"},"NativeMobileAdditionalInformation":"Tablets or smartphones using iOS 11.4 or later or Android 7 or later are compatible via the apps or the web. iOS and Android apps support standard Accessibility features."}', N'{"PrivateCloud":{"Summary":"For access to their Clinical System, Customers need to have sufficient HSCN connectivity already in place, OR establish via Q doctor a Virtual Private Cloud setup, allowing for software-only remote access (please contact info@qdoctor.io for more information).","Link":"","HostingModel":""}}', N'Our accelerated mobilisation process involves a deployment window of one week per practice for video consultation. The system, co-designed with end users and the digital health team at Lincolnshire STP, is intuitive, adopting a ‘plug-and-play’ approach.

• For the Covid-19 response, we have moved to a video-led training model, with daily ‘drop in’ webinar sessions for Q&A with the Named Contact.

• Named Contact initially gets in touch, sets up Key Users, and gives access to our training area (Training Videos and handbooks), and confirms technical setup using our automated test page. Drop in sessions are provided daily for that week for Q&A.

• Implementation is supported with our Live Chat functionality (also has dynamic content access) for all users of the system (clinicians, receptionists, patients) 8am-8pm 7 days per week. This has excellent feedback for responsiveness and interactive troubleshooting.', N'Q doctor are continually progressing towards their roadmap; 2020 incorporates integrations with NHS Login, deeper HSCN connectivity to enable our Remote Digital Working Platform at scale for clinical partner NHS teams, and enhanced integration with sector software partners.  The Q doctor team are also working with the NHS App team to provide integration that allows a simplified patient journey.

Simultaneously, Q doctor is investing extensively in enhancements driven by Deep Learning to deliver AI-powered clinical tools to better inform clinicians in real-time during consultations.  Real-time dashboards are also being expanded to allow for FHIR-standard telehealth integration and organisation level configurability.', NULL, N'https://www.qhealth.io/', NULL, NULL, CAST(N'2020-04-06T12:50:27.8800000' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
         VALUES (N'10029-001', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2020-04-08T07:42:58.2633333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10029-003', NULL, N'RIVIAM Secure Video Service with TPP SystmOne™ integration provides real-time video & audio-conferencing services between RIVIAM users & external users (patients, family, carers & professionals). 

This is a fully managed service. Recordings are stored securely in RIVIAM & the clinical system.', N'The service provides a real-time secure video or audio-conferencing call for organisations that want to deliver the clinical services themselves. RIVIAM doesn’t store or process any information outside the UK. With RIVIAM''s direct interface with TPP SystmOne™, a GP or clinician can book video appointments using their regular clinical rotas. After the patient’s consultation, relevant information is automatically written back into the clinical system.

Customers can fully customise the patient experience including the digital waiting room that allows patients to see where they are in the queue and answer any pre-consultation questions. The waiting room is integrated into TPP SystmOne™''s diary to provide the patient with a realistic waiting time.

Booking a video consultation triggers an email invitation to the patient containing a URL link and pin code. The patient is able to access and launch the video and adjust settings with RIVIAM’s mobile app using any connected device.', N'["Ideal for GP Federations or PCNs to offer online services across geographies","Provides GPs with ability to set up ad hoc or scheduled patient video or audio consultations","Integrates with TPP SystmOne™ enabling clinicians to book appointments using their clinical rotas","After consultation, relevant information is automatically written back into TPP SystmOne™","All information is securely hosted in the UK and stored using encrypted AES256","Service uses modern WebRTC internet technology","Enables users on the secure HSCN network to have secure video calls with internet users","Provides option to record and hold recordings in RIVIAM","Users can easily moderate video and audio calls","Works across all modern mobile phones, desktops and tablets"]', N'{"ClientApplicationTypes":["browser-based"],"BrowsersSupported":["Google Chrome","Mozilla Firefox","Opera","Safari"],"MobileResponsive":true,"Plugins":{"Required":false,"AdditionalInformation":""},"MinimumConnectionSpeed":"1.5Mbps","MinimumDesktopResolution":"16:9 – 1280 x 720","HardwareRequirements":"Clinicians will require a device with a microphone, speakers and video capability.","AdditionalInformation":"Further information about supported browsers:\r\n\r\n• Any HTML 5 compliant browser with WebRTC support\r\n\r\n• Google Chrome 28+\r\n\r\n• Safari 11+ on iOS/OSX\r\n\r\n• Firefox 22+\r\n\r\n• Opera 18+","MobileFirstDesign":true}', N'{"PrivateCloud":{"Summary":"The Secure Video Service is hosted in UK Cloud''s Corsham (Wiltshire, England) and Farnborough (Hampshire, England) high security data centres. UK Cloud is ISO27001 certified. All data is stored at rest encrypted with the AES256 algorithm, and persistent storage is supported.","Link":"","HostingModel":"With UK Cloud, RIVIAM''s data centre is connected to HSCN across 2 locations.\r\n\r\nRIVIAM operates a multi-layered architecture to ensure a very high level of data security and hosting. \r\n\r\nEach of the nodes in the RIVIAM network has an RSA 2048 bit public/private key pair. This key pair is used to facilitate communication between nodes. The RSA keys provide a key pair that is used to encrypt data communication between nodes and also within the node. \r\n\r\nThe nodes also have a separate key pair used for web HTTPS RESTFUL services. Each node manages a service of queues persisted in a SQL database. The message data is held in AES256 encrypted form. Only the node can decrypt the data.  \r\n\r\nAll data transferred between nodes is encrypted with AES256 before communication with the HTTPS restful services. This approach means that even if the HTTPS services are compromised data is encrypted with AES256 at rest.","RequiresHSCN":"End user devices must be connected to HSCN/N3"}}', N'Implementation typically takes 10 – 12 days. RIVIAM’s process is as follows: Discovery (1 day), delivery of a Solution Model (1 day), configuration (3 days), development of the technical infrastructure and supporting completion of a DPIA (4 days), testing and user acceptance testing (2 days), training and go live (1 day). 

The extent of your configuration requirements will have an impact on the timescales for configuration.

Your main responsibility will be the roll-out of the RIVIAM GP Connector (RIVIAM’s secure software which connects RIVIAM’s Secure Video Service to your clinical system) to clinician’s devices. This should take up to 1 day, depending on the number of users. RIVIAM will provide documentation. You will also need to carry out any necessary firewall changes. 

You will need to carry out testing and attend RIVIAM training. RIVIAM will provide a user guide and will follow a ‘train the trainer’ approach.', N'Within RIVIAM’s roadmap for the Secure Video Service for 2020, is development of the capability to add multiple users to a video or conference call. 

This capability will mean that multiple users can join a single video consultation from multiple locations. It will enable service to be used by GPs and other clinicians for online video Multi-disciplinary Meetings.', N'https://www.riviam.com/nhs-integration', N'https://www.riviam.com/know-how/riviams-integration-with-tpp-systmone-now-delivers-with-real-time-updates', NULL, NULL, CAST(N'2020-04-08T08:59:03.8100000' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
         VALUES (N'10030-001', NULL, N'AccuRx makes communication for healthcare professionals and patients easier. It offers patient messaging (including questionnaires, documents and photos), video calls and online consultations. AccuRx is continually building new features to join up all communication around the patient.', N'AccuRx is on a mission to make patients healthier and the healthcare workforce happier. We’re doing that by building the platform to bring together all communication around a patient. Our vision is that anyone involved in a patient''s care can easily communicate with everyone else involved in that patient''s care, including the patient. AccuRx was founded in 2016 and has since been adopted by 6,500 GP practices (>95%) and over 150 NHS Trusts.', N'["Patient Triage is an online consultation solution to make communication with patients easier","Video consultations in two clicks without needing to download an app or create an account","SMS a patient with personalised messages saved instantly to the medical record","Patient questionnaires (such as for QOF) with >60% response rate (based on current usage)","Patient responses (including attaching photos/documents)","Send documents via SMS to patients","100+ pre-written message templates or create custom templates for individuals or your organisation","Send a series of scheduled messages","SMS delivery receipts","Track practice usage"]', N'{"ClientApplicationTypes":["browser-based","native-mobile","native-desktop"],"BrowsersSupported":["Google Chrome","Microsoft Edge","Mozilla Firefox","Opera","Safari","Chromium","Internet Explorer 11","Internet Explorer 10"],"MobileResponsive":true,"Plugins":{"Required":false,"AdditionalInformation":""},"MinimumConnectionSpeed":"2Mbps","MinimumDesktopResolution":"4:3 – 1024 x 768","HardwareRequirements":"Personal Computer with Windows 8.1 Professional & above (32/64 bit)","NativeMobileHardwareRequirements":"In-built camera for video consultation.","NativeDesktopHardwareRequirements":"","MobileFirstDesign":true,"NativeMobileFirstDesign":true,"MobileOperatingSystems":{"OperatingSystems":["Apple IOS","Android","Other"],"OperatingSystemsDescription":""},"MobileConnectionDetails":{"MinimumConnectionSpeed":"2Mbps","ConnectionType":["GPRS","3G","LTE","4G","5G","Bluetooth","Wifi"],"Description":""},"MobileMemoryAndStorage":{"MinimumMemoryRequirement":"256MB","Description":"N/A"},"MobileThirdParty":{"ThirdPartyComponents":"","DeviceCapabilities":""},"NativeMobileAdditionalInformation":"","NativeDesktopOperatingSystemsDescription":"Windows 8.1 Professional & above (32/64 bit)","NativeDesktopMinimumConnectionSpeed":"2Mbps","NativeDesktopThirdParty":{"ThirdPartyComponents":"","DeviceCapabilities":""},"NativeDesktopMemoryAndStorage":{"MinimumMemoryRequirement":"256MB","StorageRequirementsDescription":"N/A","MinimumCpu":"N/A","RecommendedResolution":"4:3 – 1024 x 768"},"NativeDesktopAdditionalInformation":""}', N'{"PublicCloud":{"Summary":"Our England-based Microsoft Azure servers now use the Microsoft Azure NHS Platform as a Service Blueprint, based on the National Cyber Security Centre’s cloud security principles.","Link":"https://docs.microsoft.com/en-gb/previous-versions/azure/security/blueprints/uknhs-paaswa-overview"}}', N'AccuRx has been built around the needs of frontline healthcare staff. That means healthcare users can start using AccuRx via our website within minutes without any training.', N'AccuRx is on a mission to make patients healthier and the healthcare workforce happier. We’re doing that by building the platform to bring together all communication around a patient. Our vision is that anyone involved in a patient''s care can easily communicate with everyone else involved in that patient''s care, including the patient. AccuRx’s roadmap is filled with features that each take us a step closer to realising that vision.', N'https://support.accurx.com/en/articles/2312031-what-systems-does-accurx-work-with', N'https://www.accurx.com/', NULL, NULL, CAST(N'2020-04-01T10:39:24.7066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10031-001', NULL, N'GP&Me is a simple, straight forward, secure video consultation app and application. Called GP&Me, it can in fact be used in any health or care setting, by any primary care clinician or professional.', N'GP&Me is a non-emergency communications platform and App providing live video consultation facilities on smartphones and tablets for NHS Patients and Clinicians in the UK.

Arrange and attend secure, remote video consultations with your Clinician (such as your GP, Nurse Practitioner or Pharmacist, or other clinicians and professionals in other health and care settings) via an App on your smartphone or tablet.

1. Set up your account It is free and easy to download the App from the Google or iOS App Store. 
2. Book with your surgery Arrange your consultation with your surgery receptionist in person or over the phone.
3. Video consultation When it’s time for your consultation, simply sign into the App, select your appointment and ‘check in’ to notify your GP that you are ready.', N'["Fast, secure set up of Video Consultations between patients and clinicians.","","","","","","","","",""]', N'{"ClientApplicationTypes":["browser-based","native-mobile"],"BrowsersSupported":["Google Chrome","Microsoft Edge","Mozilla Firefox","Opera","Safari","Chromium","Internet Explorer 11","Internet Explorer 10"],"MobileResponsive":true,"Plugins":{"Required":false,"AdditionalInformation":""},"MinimumConnectionSpeed":"0.5Mbps","MinimumDesktopResolution":"5:4 – 1280 x 1024","HardwareRequirements":"The solution requires the availability of Broadband/Wifi or 3G, 4G or 5G. The better the connection for either patient or clinician, the better the quality of the video call. As with ALL video solutions.","NativeMobileHardwareRequirements":"The patient/person will require a smart phone or device. The clinician or professional will also require either a smartphone or device, or a desktop/laptop to use the version via the website.","MobileFirstDesign":true,"NativeMobileFirstDesign":true,"MobileOperatingSystems":{"OperatingSystems":["Apple IOS"],"OperatingSystemsDescription":"•\tiOS (Version 8 and above) and Android (version 5 and above) are both supported.\r\n\r\n•\tIf you have really, really old versions of either you might need to upgrade, but this would be true for pretty much any app you might use on your smart phone or device."},"MobileConnectionDetails":{"MinimumConnectionSpeed":"0.5Mbps","ConnectionType":["3G","4G","5G","Wifi"],"Description":"The better the connection, the better the quality of the call, for obvious reasons."},"MobileMemoryAndStorage":{"MinimumMemoryRequirement":"512MB","Description":"."}}', N'{"PublicCloud":{"Summary":"The solution is hosted in a secure data centre.","Link":""},"PrivateCloud":{"Summary":"The solution is hosted in a secure data centre.","Link":"","HostingModel":"The solution is hosted in a fully serviced data centre, professionally managed by the data centre provider."}}', N'Organisations can be got up and running within an hour. A practice or other health or care organisation is created on the system, and normally a key person in that organisation is set up and shown how to create clinicians, patients and appointments. Very easy to use – and that’s it, you are away – and can start to have video consultations.

We have some online training and video materials, should further support be required, and are always at the end of the phone or email, should support be required.', N'Unlike larger players in the market, we are a partnership of two small companies, with decades of clinical and non-clinical experience between us. The Martin Bell Partnership, in association with GP&Me and the solution, will look at developments for the future, as both the need arises, and as business develops and user requirements become clear.', NULL, N'https://www.gpandme.co.uk/', NULL, NULL, CAST(N'2020-04-01T10:37:59.3066667' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
         VALUES (N'10033-001', NULL, N'AlldayDr allows patients to consult with a GP by a secure video consultation. Connection can be either via a web browser and mobile app. Outcome of the consultation is fed back. 

The app also has the ability for the patient to enter a query or other information and can be securely transmitted.', N'AlldayDr was founded by Dr Suhel I Ahmed (MBChB, MRCGP). He is a UK qualified doctor, qualified in 2002 from the University of Liverpool with the highest possible accolade of Distinction. During his working career he has worked extensively in acute medicine and within unscheduled care departments as a GP. 

Our founder and clinical lead has a deep understanding and know-how in skills of effective distant telecommunication based history taking and subsequent management of medical conditions; both acute and chronic disease management. Dr Ahmed and clinicians working via alldayDr deliver Primary Care Services within the NHS and Private Sector. 

Since the introduction of the Online Consultation service, our doctors have held roles within services and are very familiar with remote and online consultations. Our clinicians are given detailed inductions on online consultations, including the safety mechanisms and safeguarding measures that are implemented on the platform.', N'["Hard to reach groups – patients who are on holiday, out of the country most of the year","Medication Usage Review – reduce wastage, unnecessary prescribing","Minimising Travel – adults struggling to attend the practice","GP Practices – to help reduce DNA rates, spread of infections","Online Video Consultations – patients no longer needing to look at commitments. work pressures","Repeat Medication Management  the ability to use free alert & reminders for medication needs","","","",""]', N'{"ClientApplicationTypes":["browser-based","native-mobile"],"BrowsersSupported":["Google Chrome","Microsoft Edge","Mozilla Firefox","Safari","Chromium"],"MobileResponsive":true,"Plugins":{"Required":false,"AdditionalInformation":""},"MinimumConnectionSpeed":"Higher than 30Mbps","MinimumDesktopResolution":"16:9 – 1280 x 720","AdditionalInformation":"Browsers supported\r\n\r\n•\tGoogle Chrome (up to Version 80.0.3987.149)\r\n\r\n•\tMicrosoft Edge (up to Version 80.0.361.62)\r\n\r\n•\tMozilla Firefox (up to Version 74.0)\r\n\r\n•\tSafari (up to Version 13.0.5)\r\n\r\n•\tChromium (up to Version 83.0.4095.0)\r\n\r\n•\tAvast (up to Version  80.0.3569.123)","MobileFirstDesign":true,"NativeMobileFirstDesign":true,"MobileOperatingSystems":{"OperatingSystems":["Apple IOS","Android"],"OperatingSystemsDescription":"Requires IOS 10.0 or later. Compatible with iPhone, iPad, and iPod touch.\r\n\r\nRequires Android 5.0 or later."},"MobileConnectionDetails":{"MinimumConnectionSpeed":"Higher than 30Mbps","ConnectionType":["GPRS","3G","LTE","4G","5G","Wifi"],"Description":""},"MobileMemoryAndStorage":{"MinimumMemoryRequirement":"256MB","Description":"No additional storage is required."}}', N'{"PrivateCloud":{"Summary":"Amazon Virtual Private Cloud (Amazon VPC) lets you provision a logically isolated section of the AWS Cloud where you can launch AWS resources in a virtual network that you define. \r\n\r\nYou have complete control over your virtual networking environment, including selection of your own IP address range, creation of sub-nets, and configuration of route tables and network gateways. \r\n\r\nYou can use both IPv4 and IPv6 in your VPC for secure and easy access to resources and applications.","Link":"https://aws.amazon.com/vpc/","HostingModel":"AWS pioneered cloud computing in 2006, creating cloud infrastructure that allows you to securely build and innovate faster. \r\n\r\nThey’re continuously innovating the design and systems of their data centres to protect them from man-made and natural risks. They then implement controls, build automated systems, and undergo third-party audits to confirm security and compliance. \r\n\r\nAs a result, the most highly-regulated organisations in the world trust AWS every day. Take a virtual tour of one of our data centres to learn about our security approach to protect the data of millions of active monthly customers."}}', N'We can deploy our Solutions to CCGs in the following timescales: 

•	1 CCG area in week 1
•	3 CCG areas in week 2
•	5 CCG areas in week 3

Each practice will be provided with all the training, necessary to help deliver the service. Support will be available to all practices between the core working hours of 8am-6.30pm, recognising the pressing & urgent needs presented by Covid-19.

We’ll provide off-site remote training for practices, coupled with online training videos to access any time. We’ll provide 1st Line tech support and 2nd Line patient support.

Staff at alldayDr have the ability to answer any questions  practices have. We’ll also try to provide the training to help all staff make use of the solution as easily as possible. The technology also has an inbuilt chat system and in-app messaging function.

We have already deployed remote working across our on-shore and off-shore teams to ensure services will be maintained and patient access will remain disruption free.', N'At alldayDr, we’re looking into integrating with IM1, GP Connect and NHS login. This is to help all GP’s using the Solution provide better and accurate care to the patients.  

We’ll be looking at trying to integrate the above within the next few months.', N'', N'https://www.alldaydr.com/home', NULL, NULL, CAST(N'2020-04-01T10:40:33.7566667' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
         VALUES (N'10035-001', NULL, N'Evergreen Life is facilitating remote digital consultations including video appointments for  GPs in the UK, with patient record access to clinicians with or without access to the practice system in England.

The app lets patients access GP online services & curate a personal health record.', N'In the Evergreen Life app, digital consultations allow doctors to discuss patients’ symptoms in real time through video calls. Before and during consultations, clinicians can view patients’ GP records. As a provider of GP services and linked to all three major clinical systems in England, Evergreen Life is uniquely positioned to provide remote record access to clinicians with or without access to the practice system. 

The solution allows routine services in primary care to continue. Clinicians or emergency stand-ins can communicate with patients whilst avoiding face-to-face contact through confidential, encrypted software. Any notes and prescriptions can be written back to the GP system.

The app also allows users to access PFS, including ordering repeat prescriptions for delivery. In the patient-powered app, users can add personal health data and understand their well-being through personalised insights, building a complete, accurate record that they can share with anyone.', N'["Patients can book digital consultations with clinicians, including video appointments","Patients can order repeat prescriptions and get them delivered to any UK address","Patients can download their GP-held medical record","Patients can add personal health data to the app, such as allergies, measurements & documents","Patients can grant access to their GP record & personal health data to clinicians or caregivers","Patients can get personalised information to empower themselves to stay well independently","Clinicians can provide medical advice through confidential digital consultations","Clinicians can access patient medical records instantly and remotely","Clinicians can write consultation notes and prescriptions back to GP system",""]', N'{"ClientApplicationTypes":["browser-based","native-mobile"],"BrowsersSupported":["Google Chrome","Microsoft Edge","Mozilla Firefox","Safari"],"MobileResponsive":true,"MinimumConnectionSpeed":"0.5Mbps","MinimumDesktopResolution":"4:3 – 1024 x 768","HardwareRequirements":"Hardware must be capable of running a web-browser but no specific limitations apply.","NativeMobileHardwareRequirements":"The application requires a front-facing camera to support video conferencing, standard on most smartphones and modern tablets.","AdditionalInformation":"There is no browser-based version of the consultation feature. The GP Patient Facing Services (Record view, appointments management, repeat medication management) all work on the following browsers:\r\n\r\n•\tIE9+\r\n\r\n•\tEdge\r\n\r\n•\tFirefox 3.5+\r\n\r\n•\tChrome 4+\r\n\r\n•\tSafari 3.1+","MobileFirstDesign":true,"NativeMobileFirstDesign":true,"MobileOperatingSystems":{"OperatingSystems":["Apple IOS","Android"],"OperatingSystemsDescription":""},"MobileConnectionDetails":{"MinimumConnectionSpeed":"0.5Mbps","ConnectionType":["3G","LTE","4G","5G","Wifi"],"Description":"The connection must support video transfer. The recommended minimum bandwidth is 350Kbps to maintain a stable video connection."},"MobileMemoryAndStorage":{"MinimumMemoryRequirement":"256MB","Description":"Less than 1MB storage required for session state etc, no record or PID storage on the device."},"MobileThirdParty":{"ThirdPartyComponents":"","DeviceCapabilities":"The application requires a smartphone or tablet device running a recent (nominally 4 years or newer) version of IOS or Android."}}', NULL, N'Implementation of the Solution is typically less than 2 working days and requires download and registration for the service.', N'On our roadmap is the next phase of our Digital Consultations solution which supports Total Triage. This means that every patient contacting the practice is first triaged digitally before making an appointment.

Every patient requesting an appointment undergoes a Digital Triage process by answering a series of questions either through the app or web-based interface. Our Digital Triage process exists to provide reference information to safely manage risk within the digital queue of waiting patients.

Practices will be able to access the digital queue & initiate a digital consultation by downloading the Evergreen App. We have worked with the GMC to provide a secure identification verification process that ensures any GP accessing the system is registered. 
On login, the GP is shown the digital queue for a given practice. This gives practices the opportunity to share workload within their PCN. The GP can initiate the same functionality as in our initial Digital Consultations release.', N'', N'', NULL, NULL, CAST(N'2020-04-01T10:42:08.5066667' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
         VALUES (N'10046-001', NULL, N'Docman 10 supports the effective management of inbound correspondence from multiple sources. This includes clinical and non-clinical documentation, which is stored in a central, cloud-based repository. Users can review, annotate and workflow across the practice, with full auditability.', N'Docman 10 is a cloud-based software platform that helps manage the transfer of care within a healthcare organisation. It is accessed entirely through a web browser, providing complete secure access to clinical/non-clinical documentation and structured data.

Documents are workflowed around a GP practice ensuring that documents are sent to the right locations in the right order with full traceability and audit history. Receive and send information in an instant as automation and simplicity are at the heart of the Docman electronic document management and workflow solution. Have information, history, documents and data at your fingertips. Do the things you do every day more easily and from anywhere, on any device, anytime with a secure N3 connection.', N'["Browser based platform with no reliance on locally installed software and hardware","Access from anywhere at any time with an N3 connection","Electronic document management and workflow","Automated and centralised document collection from a variety of sources including NHS Mail","Intellisense uses Optical Character Recognition for fast and automated filing & clinical coding","Visual timeline of significant events and a full document history","Enhanced reporting suite and dashboards","A fully audited discussion platform for patient, practice and business matters","Cross organisational workflow, messaging and tasking","Easy software updates with no overheads"]', N'{"ClientApplicationTypes":["browser-based","native-desktop"],"BrowsersSupported":["Google Chrome","Microsoft Edge","Mozilla Firefox","Safari","Internet Explorer 11"],"MobileResponsive":true,"Plugins":{"Required":false,"AdditionalInformation":""},"MinimumConnectionSpeed":"2Mbps","MinimumDesktopResolution":"4:3 – 1024 x 768","NativeDesktopHardwareRequirements":"Physical scanner for document scanning.","AdditionalInformation":"We integrate with eRS and some Secondary Care PAS systems.","MobileFirstDesign":false,"NativeDesktopOperatingSystemsDescription":"Windows 10, Server 2012, 2016 and 2019","NativeDesktopMinimumConnectionSpeed":"2Mbps","NativeDesktopMemoryAndStorage":{"MinimumMemoryRequirement":"2GB","StorageRequirementsDescription":"User settings stored locally.","MinimumCpu":"At least 2GHz","RecommendedResolution":"4:3 – 1024 x 768"},"NativeDesktopAdditionalInformation":"We integrate with eRS and some Secondary Care PAS systems."}', N'{"PrivateCloud":{"Summary":"The solution is hosted entirely within AWS. The AWS eu-west-2 London Region where Docman10 is hosted operates across 3 availability zones within the UK. Each AZ is highly resilient and secure. \r\n\r\nThe infrastructure is designed to survive failure. The design is aligned to the AWS well architected design pillar for resiliency.?Automatic scaling is employed to ensure automatic recovery and deliver maximum resilience within the UK sovereign area.","Link":"https://aws.amazon.com/","HostingModel":"","RequiresHSCN":"End user devices must be connected to HSCN/N3"}}', N'Standard implementation time for a typical new site installation is 5 working days.  Migrations from a previous version of Docman on average take 8 working days. These timescales include pre migration activities, application deployment and document upload (only required for migrations), database migration or creation, Go Live Training, and a Post Migration training Day. 

The time required for application deployment and document upload have the greatest impact on these timescales. Your main responsibilities include, ensuring that the practice meets the minimum requirements for the software, applications are deployed to all machines required, current data cleanse activities are completed if a Docman Migration is required and staff are available for training.', N'Advanced takes a structured approach to roadmap planning to ensure the right level & type of investment is prioritised within each product. The roadmaps are approached in layers which are Contractual / Legislative; Technical efficiency; Maintenance / Support; New functionality, driven by both market demands & customer feedback. Each product has its own Ideas Portal where customers can post their own, & vote on, ideas posted by others for product improvements they would like included. These ideas are used, in conjunction with market analysis, to help determine the roadmap priorities.

A variety of workshops are used to obtain customer feedback, including problem definition, visioning, futurespectives, risks/benefits & prototype design/review. This helps gain an in-depth understanding of the business need behind each development idea with the aim of developing solutions to meet the needs of all customers, particularly for customers operating in the same or similar environments.', NULL, N'https://www.docman.com/docman10/', NULL, NULL, CAST(N'2020-03-30T13:02:24.5200000' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
         VALUES (N'10046-003', NULL, N'Docman 7 supports the effective management of inbound correspondence from multiple sources. This includes clinical and non-clinical documentation, which is stored on a local server. 

Users can review, annotate and workflow across the practice, with full auditability.', N'Docman 7 is an on-premise application that manages the transfer of care within a healthcare organisation. It’s installed locally, providing complete secure access to clinical/non-clinical documentation and structured data.

Documents are workflowed around a GP practice ensuring that documents are sent to the right locations in the right order with full traceability and audit history. 

Receive and send information in an instant. Automation and simplicity is at the heart of the Docman electronic document management and workflow process. Have information, history, documents and data at your fingertips. Streamline your working processes and improve data quality, helping you to free professionals to concentrate on patient care.

Docman 7: Delivering Paper Free Care.', N'["Fast and accurate document capture ensuring data quality","Optical Character Recognition technology enabling intelligent document matching and filing","Advanced workflow simplifying practice processes","Incorporates ''action-based workflow'' for highly efficient document handling","Increases QOF points through intelligent summarising","Seamless clinical system integration","Instant access to records and documents","Manages non-patient documents as easily as patient documents","",""]', N'{"ClientApplicationTypes":["native-desktop"],"BrowsersSupported":[],"NativeDesktopHardwareRequirements":"Physical scanner for scanning.","NativeDesktopOperatingSystemsDescription":"•\tWindows 10\r\n\r\n•\tServer 2012, 2016 and 2019","NativeDesktopMinimumConnectionSpeed":"2Mbps","NativeDesktopThirdParty":{"ThirdPartyComponents":"Omnipage OCR for Intellisense (installed by our solution).","DeviceCapabilities":""},"NativeDesktopMemoryAndStorage":{"MinimumMemoryRequirement":"2GB","StorageRequirementsDescription":"Documents and database are stored locally so a recommendation of a 100GB hard drive.","MinimumCpu":"At least 2GHz.","RecommendedResolution":"4:3 – 1024 x 768"},"NativeDesktopAdditionalInformation":"We integrate with eRS and some Secondary Care PAS systems."}', N'{"PublicCloud":{"Summary":"","Link":""}}', N'', N'Advanced takes a structured approach to roadmap planning to ensure the right level & type of investment is prioritised within each product. The roadmaps are approached in layers which are Contractual/Legislative; Technical efficiency; Maintenance/Support; New functionality, driven by both market demands and customer feedback. 

Each product has its own Ideas Portal where customers can post their own, & vote on, ideas posted by others for product improvements they would like included. These ideas are used, in conjunction with market analysis, to help determine the roadmap priorities. 

A variety of workshops are used to obtain customer feedback, including problem definition, visioning, futurespectives, risks/benefits and prototype design/review. This helps gain an in-depth understanding of the business need behind each development idea with the aim of developing solutions to meet the needs of all customers, particularly for customers operating in the same or similar environments.', N'', N'', NULL, NULL, CAST(N'2020-03-30T13:04:21.6500000' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
        VALUES (N'10046-006', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2020-06-25T14:31:07.2366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
               (N'10047-001', NULL, N'askmyGP provides online consultation and workflow tools to maintain doctor-patient relationships and keeps patients in touch with their registered practice. Clinicians use askmyGP to prioritise and deliver care by secure message, telephone, video or face-to-face appointment.', N'Practice teams are supported via change interventions to ensure that all patients or their carers can use askmyGP to contact their practice. Patients explain their symptoms in their own words, allowing structured triage and consultation information to be passed directly to the practice.

•	62% of patient requests are resolved by secure message or telephone, dramatically improving clinician productivity

•	Median completion time for all requests drops to 120 minutes, with over 90% on the same day

•	20% of patients search on self-care: no patients are turned away

•	Practices experience these outcomes immediately from the day of launch

askmyGP helps practices prioritise their caseload, regardless of source, including clinical priority flags. Triage and consultation models and templates are configurable to support the practice’s preferred workflow structure, subject to appropriate clinical governance.', N'["Rapid, consistent triage and prioritisation of all patient care","74% of patient requests online (all requests managed regardless of communication channel)","Clear presentation of patient submissions and preferences","Ability to verify and record patient identification status (saved for later interactions)","Direct two-way confidential messaging service between clinicians and their patients","Ability for patient and practice to send attachments (symptom photos/patient information)","Single click initiation of phone and video consultation features","All consultation activity can be initiated in response to a patient request or by the practice","Full case history and reporting tools provide comprehensive performance analysis and audit","Full support to practice during implementation and for duration of contract"]', N'{"ClientApplicationTypes":["browser-based"],"BrowsersSupported":["Google Chrome","Microsoft Edge","Mozilla Firefox","Opera","Safari","Chromium","Internet Explorer 11"],"MobileResponsive":true,"Plugins":{"Required":false,"AdditionalInformation":""},"MinimumConnectionSpeed":"0.5Mbps","MinimumDesktopResolution":"4:3 – 1024 x 768","HardwareRequirements":"askmyGP is browser-based and has no specific hardware requirements for either practice or patients. All versions of modern browser are supported.\r\n\r\nWe recommend the use of dual screens in practices for ease of interaction, and a webcam and headset for video consultations.","AdditionalInformation":"The askmyGP patient interface has similarly minimal hardware requirements.  It operates on all modern browsers and is independent of device. \r\n\r\nA Progressive Web Application may optionally be used by patients, which replicates the functionality and adds the ability to, for example, manage android notifications. \r\n\r\nVideo calls utilise device camera and microphone, attachments may be sent to or from the practice.","MobileFirstDesign":false}', N'{"PrivateCloud":{"Summary":"Secure hosting within HSCN.","Link":"","HostingModel":"All patient data is hosted within the UK and subject to the connection requirements of the N3/HSCN network. Data hosting arrangements meet ISO 27001:2013 and physical security to BS5979.\r\n\r\nAt the patient interface, data is encrypted and secured by passwords of validated complexity. Personal and sensitive data is only available to the practice at which an individual patient is registered.\r\n\r\nRobust clinical governance exceeds the requirements of DCB 0129 and DCB 0160 accreditation.","RequiresHSCN":"End user devices must be connected to HSCN/N3"}}', N'The implementation programme is readily scalable and supports practices to quickly release benefits.  We work with practice staff on-site to deliver:

•	Planning: detailed plan is set out according to the needs and constraints of individual practices

•	Preparation: training and patient communication commence

•	Launch day: system is activated; benefits are felt, first by patients and reception

•	Adapt and refine: intensive work with the practice using ongoing performance information, and psychology of change

•	Waypoint: consolidates future direction

•	Continuous improvement continues throughout the year, raising efficiency with multiple small changes.

Our model of change operates in practice, cluster or wider groupings, evidenced by delivery to CCG-wide groups of practices across the UK, and commissions by individual practices. Pace of implementation is chosen by the practice – most implementations take 3-4 weeks, but a same day service is available if required.', N'We recognise the needs of practices and wider requirements evolve rapidly:

• Requests for change are gathered from stakeholders on an ongoing basis, either directly from customers & patient surveys, via our active ‘user group’ or in response to national or local policy developments

• Requirements are collated, evaluated, and prioritised to reflect the significance of the change, effect on active users & evaluation of risk and benefits

• Features are shared & tested via live demonstrations, for example, to user groups or other stakeholders

• Further feedback is gathered & used to inform future iterations

Example roadmap items:

• Network-wide patient referral mechanisms & demand management tools to support PCNs & groups

• Access to foundation system APIs for automated links & coding between patient enquiry and clinical record

• Automated selection & booking of appointments (once required by clinician)

• Optional patient validation via interaction with NHS App', NULL, N'https://askmygp.uk/live/', NULL, NULL, CAST(N'2020-04-01T10:43:15.8533333' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
         VALUES (N'10052-002', NULL, N'SystmOne GP has been evolving for over 20 years, with continuous clinical input. It is one of the most advanced clinical systems in the world and is used by more than 2,700 GP practices nationwide. SystmOne GP is the ideal solution to meet the ever-changing needs of modern General Practice.', N'SystmOne GP has been in use across UK General Practice for over 20 years. It is the system of choice for over 2,700 practices and is used by over 75,000 staff members. SystmOne GP is an advanced solution that goes far beyond the main functionality required for running a GP practice. It contains complete workflow support, a full analytics module, QOF tracking & a comprehensive clinical development kit.  

Improving the quality of care across settings is core to TPP’s vision. The GP product is ideal for cross-organisational working & fully supports the requirements of Primary Care Networks. It enables true integrated care between GP, hospital, mental health & social care settings. 

TPP GP is Spine-accredited, providing access to the latest versions of GP2GP, EPS, & eRS. The system is fully compliant with SNOMED CT. SystmOne GP is leading on national interoperability programmes, compliant with national open FHIR standards for access to GP data & for transfer of care documentation.', N'["Full Spine Compliance – EPS, PDS, SCR, eRS, GP2GP","Standards – SNOMED CT, HL7 V2, V3, FHIR, GP Connect","Appointments – Configurable Clinics, Dedicated Appointments, Visits Screens, SMS Integration","Prescribing – Acute, Repeat, Formularies, Action Groups, Decision Support","Complete Electronic Health Record (EHR)","Comprehensive consultations – Recalls, Referrals, Structured Data","Clinical Development Kit – Data Entry Templates, Views, Questionnaires, Integrated Word Letters","Full Workflow Support including Automatic Consultations","Analytics – Customisable Reports, Batch Reports, Bulk Actions, QOF Tools, Automatic Submissions","Patient Online Services – Appointment Booking, Medication Requests, Record Access, Proxy Access"]', N'{"ClientApplicationTypes":["native-desktop"],"BrowsersSupported":[],"NativeDesktopHardwareRequirements":"The OS system drive must have a drive letter of C.","NativeDesktopOperatingSystemsDescription":"TPP supports all versions of Windows for desktops that are currently supported by Microsoft. Following verification of the configuration by TPP, installation of Windows to a virtual environment is supported to the products and versions including Virtual VMware View 5+, Citrix Xen Desktop 6+ and Microsoft Server 2012+. \r\n\r\nInstallation of the SystmOne client to any Server Operating System is not licensed by TPP. It should also be noted that both 32-bit and 64-bit versions of Microsoft Windows are supported unless otherwise stated. Windows RT is not supported.","NativeDesktopMinimumConnectionSpeed":"0.5Mbps","NativeDesktopThirdParty":{"ThirdPartyComponents":"Windows 7 requires 1GB and Office Word 2010 requires 256 MB. Other third party applications, shared graphics or peripherals (such as attached printers) should also be taken into account. These will all increase the amount of memory required for the computer to run smoothly.","DeviceCapabilities":"A minimum screen resolution of 1024 x 768 pixels with 16-bit colours is required. TPP recommends a minimum of a 17” TFT flat screen monitor with a resolution of 1280 x 1024 and 32-bit colours."},"NativeDesktopMemoryAndStorage":{"MinimumMemoryRequirement":"512MB","StorageRequirementsDescription":"4GB of free space on the C drive. Where a SystmOne Gateway client is used, 100GB of free space on the C drive is recommended.","MinimumCpu":"A minimum of a 2.0 GHz Pentium 4 series CPI is required.","RecommendedResolution":"5:4 – 1280 x 1024"},"NativeDesktopAdditionalInformation":"Applications that can open/view rich text file (.rtf) and comma separated (.csv) documents are required. To perform letter writing, Microsoft Word is also required. TPP only supports versions of Office that are supported by Microsoft which currently includes Office 2010, 2013, 2016 and 2019."}', N'{"PrivateCloud":{"Summary":"The SystmOne Solution requires the following key items to be in place for smooth operation:\r\n-UDP Ports 2120-2130 and TCP Ports 2130-2140 should be opened to 20.146.120.128/25 and 20.146.248.128/25. TCP port 443 is also required for SystmOnline and Mobile Working to systmonline.tpp-uk.com. TPP also recommend allowing ICMP traffic for diagnostic purposes. \r\n\r\nA full list of requirements can be found in the SystmOne WES.","Link":"","HostingModel":"TPP provide a centralised solution with all server hardware hosted in TPP''s private cloud infrastructure. All server patching, security updates and feature releases are managed by TPP. The solution is hosted within 2 geographically separated private cloud instances with data replicated between the sites in real time in order to provide a high level of resiliency.\r\n\r\nTPP use a number of tools to monitor capacity, analyse usage trends and log the utilisation of the system. This ensures the solution scales to demand and new functionality / business requirements.","RequiresHSCN":"End user devices must be connected to HSCN/N3"}}', N'If a greenfield unit is required, the turn-around time to receive the Live unit can be as quick as two weeks, once a signed contract is in place and all staff have received the required training.

TPP will assess the request and set up the unit as specified in the order details. Once the Live system is ready to use, TPP will be in touch with the contact who requested the unit.

When transitioning from a previous system that has a mature adapter in place (EMIS Web, Vision, Microtest), implementation is a quick rollout of 8 weeks, including data migration of any existing patient records.

The main phases for this implementation are:
• Initial data production
• Data checking
• Training
• Data reload & sign-off
• Final data production
• Go Live

If transitioning from any other system, an additional 8-week adapter build period would be required.

TPP maintain close contact with staff at the unit throughout these phases to ensure an efficient and accurate implementation', NULL, N'https://www.tpp-uk.com/partners', N'https://www.tpp-uk.com/products/systmone', NULL, NULL, CAST(N'2020-03-30T13:19:48.8766667' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
         VALUES (N'10059-001', NULL, N'The service drives pro-active patient healthcare by delivering patient healthcare alerts and Calls to Action. These are generated by executing Risk Stratification algorithms against patient data. The algorithms analyse the patient data to construct a clinical risk profile for each patient.', N'Risk Stratification : Identifying those patients needing urgent intervention. Automated patient alerts are highly specific in order to identify patients with genuine reversible risk.

Active Monitoring of your patients : Over 10% of patients need enhanced monitoring, these are often the same patients that do not present at the GP Practice. Through the validated Advice and Guidance (Eclipse Live) service optimal monitoring is delivered.

Reduced GP Workload: Central patient tracking and standardised Calls to Action enables GPs to delegate workload to their wider team whilst ensuring clinical excellence.', N'["A reduction in overall workload through better utilisation of practice resources.","Identify patients with reversible risk, reducing complications, exacerbations & admissions.","Effortlessly enhance the monitoring of patients on high risk medications.","Enable comparison with other GP Practices utilising the service.","Improve delegation of workload to your healthcare teams.","","","","",""]', N'{"ClientApplicationTypes":["browser-based"],"BrowsersSupported":["Google Chrome","Microsoft Edge","Mozilla Firefox","Opera","Safari","Chromium","Internet Explorer 11","Internet Explorer 10"],"MobileResponsive":false,"Plugins":{"Required":false,"AdditionalInformation":""},"MinimumConnectionSpeed":"5Mbps","MinimumDesktopResolution":"16:9 – 1280 x 720","HardwareRequirements":"Any computer capable of operating a web browser.","MobileFirstDesign":false}', N'{"PrivateCloud":{"Summary":"Advice and Guidance (Eclipse Live) is a web-based service which is NHSD approved and centrally hosted by Prescribing Services.","Link":"","HostingModel":""}}', NULL, NULL, NULL, N'', NULL, NULL, CAST(N'2020-03-30T13:16:49.4100000' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
         VALUES (N'10062-001', NULL, N'FootFall is a complete digital practice that enables patients to engage with all areas of the practice online. With the option to operate in Digital Triage mode, FootFall can transform the way your patients access care and how you manage demands on the practice.', N'FootFall saves administration and clinical time by reducing the number of on demand phone calls and unnecessary appointments. All requests are triaged, prioritised, tracked and when appropriate are assigned to clinicians who decide on an online response, phone call, video consultation or face to face appointment.

• Free up clinician’s time for those patients most in need by utilising their time more effectively.

• Help practices to manage their workload.

• Keep track of patient requests through the practice workflow.

• Increased patient access. Available 24/7 and avoids long waits on the phone.

• Incorporates video consultations.

• Supports remote access to the FootFall system for example, from backup premises or from staff member’s homes.

Can operate in PCN mode, allowing member practices to share services and resources across the PCN.', N'["Digital triage, online consultation, and video consultation","WCAG 2.1 AAA compliant","PCN mode to allow resources and services to be shared between practices","Integration with SystmOne TPP, EMIS, and Vision via MESH using Docman","Admin requests (including new patient, changing details, sick notes and more)","Wellbeing centre – local self-help organisations by category customised to each practice","A-Z of health symptom check live synced with NHS with a searchable local pharmacy directory","Patient questionnaires – PHQ9, GAD7, IPSS, Oxford Knee/Hip Scores, COPD/Asthma Assessment & more","Long term health review forms including Asthma, Blood Pressure, Epilepsy and more","Easy to use practice dashboard to manage & assign all requests. This also supports remote access"]', N'{"ClientApplicationTypes":["browser-based"],"BrowsersSupported":["Google Chrome","Microsoft Edge","Mozilla Firefox","Opera","Safari","Internet Explorer 11"],"MobileResponsive":true,"Plugins":{"Required":false,"AdditionalInformation":"Internet Explorer 11 will require a plug-in to support video consultations.\r\n\r\nWe integrate with the clinical systems (SystemOne TPP, EMIS, Vision) via MESH using Docman. As it is a MESH message, the document will arrive as a task in TPP, into workflow for EMIS systems, and as a Docman document for Vision."},"MinimumConnectionSpeed":"2Mbps","MinimumDesktopResolution":"16:9 – 1920 x 1080","HardwareRequirements":"Video consultation requires an inbuilt or external camera, speakers and a microphone","AdditionalInformation":"Browsers supported on desktop:\r\n\r\n• Google Chrome (59+) \r\n• Firefox (46+) \r\n• Edge (79+) \r\n• Internet Explorer 11 (with plugin) \r\n• Opera (latest release version) \r\n• Electron (latest release version) \r\n• Safari (11+) \r\n\r\nBrowsers supported on Mobile devices:\r\n\r\n• Google Chrome (59+) for Android  \r\n• Firefox (46+) for Android \r\n• Safari (11+) for iOS","MobileFirstDesign":true,"NativeMobileAdditionalInformation":"Supplier Asserted Integrations\r\n\r\nWe integrate with the clinical systems (SystemOne TPP, EMIS, Vision) via MESH using Docman. As it is a MESH message, the document will arrive as a task in TPP, into workflow for EMIS systems, and as a Docman document for Vision."}', N'{"PublicCloud":{"Summary":"","Link":""}}', N'FootFall can be deployed as a rapid roll-out implementation to help practices quickly manage patient demand in a pandemic. In this example, the extent of the customisation of the FootFall site is restricted to ensure that a fast roll-out is achieved. The CCG is responsible for agreeing the template FootFall site. In this scenario we are able to roll-out 65 sites in one day with 50% of these sites fully deployed, with training having taken place, the following week and the remaining 50% the week after. The deployment date for the latter 50% was driven by practice requirements. Full customisation. branding and text from their existing website transitioned to FootFall site is completed on a date to be agreed with the CCG.

Alternatively, if the practice requests full customisation at the outset this can take 2 – 6 weeks from initial deployment to go live as the timing is driven by practice requirements.', NULL, N'', N'', NULL, NULL, CAST(N'2020-04-03T12:28:52.3800000' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Solution (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy)
         VALUES (N'10063-002', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2020-06-25T14:30:56.3300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10064-003', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2020-06-25T14:30:49.8600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10072-003', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2020-06-25T14:30:33.5166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10072-004', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2020-06-25T14:31:34.0466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10072-006', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2020-06-25T14:31:15.0166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10073-009', NULL, N'Remote Consultation is a solution integrated into the iPLATO platform and myGP® app that helps GP practices avoid unnecessary appointments and supports remote consultation via chat, video and voice.

It allows practices to communicate with patients via video, audio and asynchronous messaging.', N'', N'["Proven to successfully reduce appointments demand by 20%.","Automated, clinically safe sign-posting to alternative pathways.","Offer local and national services to suitable patients.","Automated gathering of unstructured input from patient.","Read coding of patient information directly to IT system workflow.","CCG/STP service promotion e.g. smoking cessation, mental health.","myGP® app free for patients.","Fully GDPR compliant.","Manual appointment triage with advanced filtering and full-text search.","Consent requested during set up when enabling camera and audio."]', N'{"ClientApplicationTypes":["native-mobile"],"BrowsersSupported":[],"NativeMobileHardwareRequirements":"The user’s smartphone must be capable of making/receiving video calls.","NativeMobileFirstDesign":true,"MobileOperatingSystems":{"OperatingSystems":["Apple IOS","Android"],"OperatingSystemsDescription":"iPlato recommend users are always on the latest version of the OS"},"MobileConnectionDetails":{"MinimumConnectionSpeed":"1Mbps","ConnectionType":["4G","5G","Wifi"],"Description":"The minimum connection speed required to support a video call."},"MobileMemoryAndStorage":{"MinimumMemoryRequirement":"256MB","Description":"Application size is 33 Mb for iOS and 45 Mb for Android."},"MobileThirdParty":{"ThirdPartyComponents":"The application requires no additional 3rd party components or libraries.","DeviceCapabilities":"The user’s smartphone requires no special capabilities."}}', N'{"PrivateCloud":{"Summary":"All data used in the delivery of Remote Consultation is hosted at a UK based data centre by iPlato Healthcare. This data centre is secured and iPLATO is DSTP accredited.","Link":"","HostingModel":"","RequiresHSCN":"End user devices must be connected to HSCN/N3"}}', N'Remote consultation as a feature is available to all patients that have downloaded myGP v5.0 upwards so there is no implementation timescale as clinicians can opt to initiate a video call with the patient directly from the iPLATO system.', NULL, NULL, N'http://www.iplato.com/', NULL, NULL, CAST(N'2020-04-01T12:49:33.9433333' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    MERGE INTO dbo.Solution AS TARGET
    USING #Solution AS SOURCE
    ON TARGET.Id = SOURCE.Id 
    WHEN MATCHED THEN  
           UPDATE SET TARGET.[Version] = SOURCE.[Version],
                      TARGET.Summary = SOURCE.Summary,
                      TARGET.FullDescription = SOURCE.FullDescription,
                      TARGET.Features = SOURCE.Features,
                      TARGET.ClientApplication = SOURCE.ClientApplication,
                      TARGET.Hosting = SOURCE.Hosting,
                      TARGET.ImplementationDetail = SOURCE.ImplementationDetail,
                      TARGET.RoadMap = SOURCE.RoadMap,
                      TARGET.IntegrationsUrl = SOURCE.IntegrationsUrl,
                      TARGET.AboutUrl = SOURCE.AboutUrl,
                      TARGET.ServiceLevelAgreement = SOURCE.ServiceLevelAgreement,
                      TARGET.WorkOfPlan = SOURCE.WorkOfPlan,
                      TARGET.LastUpdated = SOURCE.LastUpdated,
                      TARGET.LastUpdatedBy= SOURCE.LastUpdatedBy
    WHEN NOT MATCHED BY TARGET THEN  
            INSERT (Id, [Version], Summary, FullDescription, Features, ClientApplication, Hosting, ImplementationDetail, RoadMap, IntegrationsUrl, AboutUrl, ServiceLevelAgreement, WorkOfPlan, LastUpdated, LastUpdatedBy) 
            VALUES (SOURCE.Id, SOURCE.[Version], SOURCE.Summary, SOURCE.FullDescription, SOURCE.Features, SOURCE.ClientApplication, SOURCE.Hosting, SOURCE.ImplementationDetail, SOURCE.RoadMap, SOURCE.IntegrationsUrl, SOURCE.AboutUrl, SOURCE.ServiceLevelAgreement, SOURCE.WorkOfPlan, SOURCE.LastUpdated, SOURCE.LastUpdatedBy);
END;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./ProdLikeData/MergeAdditionalServices.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN
    /*********************************************************************************************************************************************/
    /* AdditionalService */
    /*********************************************************************************************************************************************/

    CREATE TABLE #AdditionalService
    (
        CatalogueItemId nvarchar(14) NOT NULL,
        Summary nvarchar(300) NULL,
        FullDescription nvarchar(3000) NULL,
        LastUpdated datetime2(7) NULL,
        LastUpdatedBy uniqueidentifier NULL,
        SolutionId nvarchar(14) NULL,
    );

    INSERT INTO #AdditionalService (CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId) 
         VALUES (N'10030-001A001', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10030-001'),
                (N'10007-002A001', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10007-002'),
                (N'10007-002A002', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10007-002'),
                (N'10000-001A008', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10000-001'),
                (N'10000-001A007', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10000-001'),
                (N'10000-001A006', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10000-001'),
                (N'10000-001A005', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10000-001'),
                (N'10000-001A003', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10000-001'),
                (N'10000-001A004', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10000-001'),
                (N'10000-001A002', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10000-001'),
                (N'10000-001A001', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10000-001'),
                (N'10035-001A001', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10035-001'),
                (N'10052-002A001', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10052-002'),
                (N'10052-002A002', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10052-002'),
                (N'10052-002A004', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10052-002'),
                (N'10052-002A003', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10052-002'),
                (N'10052-002A005', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10052-002');

    MERGE INTO dbo.AdditionalService AS TARGET
    USING #AdditionalService AS SOURCE
    ON TARGET.CatalogueItemId = SOURCE.CatalogueItemId 
    WHEN MATCHED THEN  
           UPDATE SET TARGET.Summary = SOURCE.Summary,
                      TARGET.FullDescription = SOURCE.FullDescription,
                      TARGET.SolutionId = SOURCE.SolutionId,
                      TARGET.LastUpdated = SOURCE.LastUpdated,
                      TARGET.LastUpdatedBy = SOURCE.LastUpdatedBy
    WHEN NOT MATCHED BY TARGET THEN  
        INSERT (CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId) 
        VALUES (SOURCE.CatalogueItemId, SOURCE.Summary, SOURCE.FullDescription, SOURCE.LastUpdated, SOURCE.LastUpdatedBy, SOURCE.SolutionId);
END;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./ProdLikeData/MergeAssociatedServices.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN

    /*********************************************************************************************************************************************/
    /* AssociatedService */
    /*********************************************************************************************************************************************/

    CREATE TABLE #AssociatedService
    (
        CatalogueItemId nvarchar(14) NOT NULL,
        [Description] nvarchar(1000) NULL,
        OrderGuidance nvarchar(1000) NULL,
        LastUpdated datetime2(7) NULL,
        LastUpdatedBy uniqueidentifier NULL,
    );

    INSERT INTO #AssociatedService (CatalogueItemId, [Description], OrderGuidance, LastUpdated, LastUpdatedBy) 
         VALUES (N'10000-S-001', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-S-002', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-S-003', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-S-004', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-S-005', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-S-006', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-S-009', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-S-036', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-S-037', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-S-038', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-S-039', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-S-069', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-S-040', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-S-041', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-S-007', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),                
                (N'10000-S-008', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10047-S-001', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10047-S-002', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-S-002', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-S-004', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-S-003', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-S-001', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),                
                (N'10073-S-021', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10073-S-023', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10029-S-007', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10029-S-009', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10007-S-001', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10007-S-002', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10007-S-003', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10007-S-004', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10007-S-005', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10029-S-006', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10029-S-008', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10029-S-010', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-S-012', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-S-001', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-S-002', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-S-009', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-S-010', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-S-011', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-S-003', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-S-013', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-S-008', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-S-004', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-S-001', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-S-002', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-S-003', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-S-004', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-S-005', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-S-006', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-S-007', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-S-009', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-S-010', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10063-S-004', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10063-S-005', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10063-S-006', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10063-S-007', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10063-S-008', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10063-S-009', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10063-S-010', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-S-005', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10072-S-005', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10072-S-010', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10072-S-011', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10072-S-012', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10072-S-013', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-S-008', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10030-S-001', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10073-S-022', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-S-141', N'DESCRIPTION', N'', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000');

    MERGE INTO dbo.AssociatedService AS TARGET
    USING #AssociatedService AS SOURCE
    ON TARGET.AssociatedServiceId = SOURCE.CatalogueItemId 
    WHEN MATCHED THEN
           UPDATE SET TARGET.[Description] = SOURCE.[Description],
                      TARGET.OrderGuidance = SOURCE.OrderGuidance,
                      TARGET.LastUpdated = SOURCE.LastUpdated,
                      TARGET.LastUpdatedBy = SOURCE.LastUpdatedBy
    WHEN NOT MATCHED BY TARGET THEN
    INSERT (AssociatedServiceId, [Description], OrderGuidance, LastUpdated, LastUpdatedBy)
    VALUES (SOURCE.CatalogueItemId, SOURCE.[Description], SOURCE.OrderGuidance, SOURCE.LastUpdated, SOURCE.LastUpdatedBy);
END;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./ProdLikeData/MergeMarketingContacts.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN
    /*********************************************************************************************************************************************/
    /* MarketingContact */
    /*********************************************************************************************************************************************/

    CREATE TABLE #MarketingContact
    (
        Id int NOT NULL,
        SolutionId nvarchar(14) NOT NULL,
        FirstName nvarchar(35) NULL,
        LastName nvarchar(35) NULL,
        Email nvarchar(255) NULL,
        PhoneNumber nvarchar(35) NULL,
        Department nvarchar(50) NULL,
        LastUpdated datetime2(7) NOT NULL,
        LastUpdatedBy uniqueidentifier NOT NULL,
    );

    INSERT INTO #MarketingContact (Id, SolutionId, FirstName, LastName, Email, PhoneNumber, Department, LastUpdated, LastUpdatedBy) 
         VALUES (1003, N'10000-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-31T10:50:17.6233333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1049, N'10000-002', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-08T10:55:58.5100000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1046, N'10000-054', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-07T19:57:45.7166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1045, N'10000-062', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-07T19:47:00.3600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1002, N'10004-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-26T13:24:14.8666667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1009, N'10004-002', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-31T11:46:17.2700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1001, N'10007-002', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-25T12:12:59.8166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1043, N'10020-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-07T19:21:57.3566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1044, N'10020-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-07T19:21:57.3566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1047, N'10029-003', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-08T10:05:14.8833333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1048, N'10029-003', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-08T10:05:14.8866667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1036, N'10030-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-02T16:28:19.8700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1037, N'10030-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-02T16:28:19.8700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1017, N'10031-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T12:50:18.1366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1018, N'10031-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T12:50:18.1366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1029, N'10033-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T14:09:25.0333333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1013, N'10035-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T12:22:15.4700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1014, N'10035-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T12:22:15.4700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1012, N'10046-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-31T14:40:22.0600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1010, N'10046-003', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-31T14:07:29.2166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1011, N'10046-003', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-31T14:07:29.2200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1030, N'10047-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T15:11:13.1100000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1031, N'10047-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T15:11:13.1133333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1034, N'10052-002', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-02T12:24:23.3200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1035, N'10052-002', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-02T12:24:23.3200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1006, N'10059-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-31T11:03:30.5666667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1007, N'10059-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-31T11:03:30.5666667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1038, N'10062-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-07T09:45:13.5733333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1039, N'10062-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-07T09:45:13.5733333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1027, N'10073-009', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T13:56:52.4200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1028, N'10073-009', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T13:56:52.4200000' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    SET IDENTITY_INSERT dbo.MarketingContact ON; 

    MERGE INTO dbo.MarketingContact AS TARGET
    USING #MarketingContact AS SOURCE
    ON TARGET.Id = SOURCE.Id 
    WHEN MATCHED THEN  
           UPDATE SET TARGET.SolutionId = SOURCE.SolutionId,
                      TARGET.FirstName = SOURCE.FirstName,
                      TARGET.LastName = SOURCE.LastName,
                      TARGET.Email = SOURCE.Email,
                      TARGET.PhoneNumber = SOURCE.PhoneNumber,
                      TARGET.Department = SOURCE.Department,
                      TARGET.LastUpdated = SOURCE.LastUpdated,
                      TARGET.LastUpdatedBy = SOURCE.LastUpdatedBy
    WHEN NOT MATCHED BY TARGET THEN  
        INSERT (Id, SolutionId, FirstName, LastName, Email, PhoneNumber, Department, LastUpdated, LastUpdatedBy) 
        VALUES (SOURCE.Id, SOURCE.SolutionId, SOURCE.FirstName, SOURCE.LastName, SOURCE.Email, SOURCE.PhoneNumber, SOURCE.Department, SOURCE.LastUpdated, SOURCE.LastUpdatedBy);

    SET IDENTITY_INSERT dbo.MarketingContact OFF; 
END;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./ProdLikeData/MergeSolutionEpics.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN
    /*********************************************************************************************************************************************/
    /* SolutionEpic */
    /*********************************************************************************************************************************************/

    CREATE TABLE #SolutionEpic
    (
        SolutionId nvarchar(14) NOT NULL,
        CapabilityId uniqueidentifier NOT NULL,
        EpicId nvarchar(10) NOT NULL,
        StatusId int NOT NULL,
        LastUpdated datetime2(7) NOT NULL,
        LastUpdatedBy uniqueidentifier NOT NULL
    );

    INSERT INTO #SolutionEpic (SolutionId, CapabilityId, EpicId, StatusId, LastUpdated, LastUpdatedBy) 
         VALUES (N'10000-001', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E1',   1, CAST(N'2020-03-31T10:44:23.3733333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E2',   1, CAST(N'2020-03-31T10:44:23.3700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E3',   1, CAST(N'2020-03-31T10:44:23.3700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E4',   1, CAST(N'2020-03-31T10:44:23.3666667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E5',   1, CAST(N'2020-03-31T10:44:23.3633333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E6',   1, CAST(N'2020-03-31T10:44:23.3633333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E7',   1, CAST(N'2020-03-31T10:44:23.3600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', N'C12E1',  1, CAST(N'2020-03-31T10:44:23.2566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', N'C12E2',  1, CAST(N'2020-03-31T10:44:23.2566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', N'C12E3',  1, CAST(N'2020-03-31T10:44:23.2533333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', N'C12E4',  1, CAST(N'2020-03-31T10:44:23.2533333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', N'C12E5',  1, CAST(N'2020-03-31T10:44:23.2500000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', N'C12E6',  1, CAST(N'2020-03-31T10:44:23.2500000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', N'C12E7',  2, CAST(N'2020-03-31T10:44:23.2466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8bee1ff3-84d4-430b-a678-336f57c57387', N'C30E1',  1, CAST(N'2020-03-31T10:44:23.1933333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8bee1ff3-84d4-430b-a678-336f57c57387', N'C30E10', 2, CAST(N'2020-03-31T10:44:23.1766667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8bee1ff3-84d4-430b-a678-336f57c57387', N'C30E11', 2, CAST(N'2020-03-31T10:44:23.1766667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8bee1ff3-84d4-430b-a678-336f57c57387', N'C30E12', 2, CAST(N'2020-03-31T10:44:23.1733333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8bee1ff3-84d4-430b-a678-336f57c57387', N'C30E13', 2, CAST(N'2020-03-31T10:44:23.1700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8bee1ff3-84d4-430b-a678-336f57c57387', N'C30E14', 2, CAST(N'2020-03-31T10:44:23.1700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8bee1ff3-84d4-430b-a678-336f57c57387', N'C30E2',  1, CAST(N'2020-03-31T10:44:23.1900000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8bee1ff3-84d4-430b-a678-336f57c57387', N'C30E3',  1, CAST(N'2020-03-31T10:44:23.1900000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8bee1ff3-84d4-430b-a678-336f57c57387', N'C30E4',  2, CAST(N'2020-03-31T10:44:23.1866667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8bee1ff3-84d4-430b-a678-336f57c57387', N'C30E5',  2, CAST(N'2020-03-31T10:44:23.1866667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8bee1ff3-84d4-430b-a678-336f57c57387', N'C30E6',  2, CAST(N'2020-03-31T10:44:23.1833333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8bee1ff3-84d4-430b-a678-336f57c57387', N'C30E7',  2, CAST(N'2020-03-31T10:44:23.1800000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8bee1ff3-84d4-430b-a678-336f57c57387', N'C30E8',  2, CAST(N'2020-03-31T10:44:23.1800000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8bee1ff3-84d4-430b-a678-336f57c57387', N'C30E9',  2, CAST(N'2020-03-31T10:44:23.1800000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'20b09859-6fc2-404c-b7a4-3830790e63ab', N'C11E1',  1, CAST(N'2020-03-31T10:44:23.2633333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'20b09859-6fc2-404c-b7a4-3830790e63ab', N'C11E2',  1, CAST(N'2020-03-31T10:44:23.2600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9d805aad-d43a-480e-9bc0-41a755bafe2f', N'C10E1',  1, CAST(N'2020-03-31T10:44:23.3600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9d805aad-d43a-480e-9bc0-41a755bafe2f', N'C10E2',  2, CAST(N'2020-03-31T10:44:23.3566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'e5521a71-a28e-4bc9-bddf-599f0a90719d', N'C17E1',  1, CAST(N'2020-03-31T10:44:23.2433333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'e5521a71-a28e-4bc9-bddf-599f0a90719d', N'C17E2',  2, CAST(N'2020-03-31T10:44:23.2300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'dd649cc4-a710-4472-98b3-663d9d12a8b7', N'C16E1',  1, CAST(N'2020-03-31T10:44:23.2600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E1',  1, CAST(N'2020-03-31T10:44:23.3566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E10', 1, CAST(N'2020-03-31T10:44:23.3400000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E11', 1, CAST(N'2020-03-31T10:44:23.3400000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E12', 1, CAST(N'2020-03-31T10:44:23.3366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E13', 2, CAST(N'2020-03-31T10:44:23.3366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E14', 2, CAST(N'2020-03-31T10:44:23.3333333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E15', 2, CAST(N'2020-03-31T10:44:23.3333333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E16', 2, CAST(N'2020-03-31T10:44:23.3300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E17', 2, CAST(N'2020-03-31T10:44:23.3300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E18', 2, CAST(N'2020-03-31T10:44:23.3266667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E19', 2, CAST(N'2020-03-31T10:44:23.3266667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E2',  1, CAST(N'2020-03-31T10:44:23.3533333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E20', 2, CAST(N'2020-03-31T10:44:23.3233333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E21', 2, CAST(N'2020-03-31T10:44:23.3200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E3',  1, CAST(N'2020-03-31T10:44:23.3533333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E4',  1, CAST(N'2020-03-31T10:44:23.3500000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E5',  1, CAST(N'2020-03-31T10:44:23.3500000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E6',  1, CAST(N'2020-03-31T10:44:23.3466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E7',  1, CAST(N'2020-03-31T10:44:23.3466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E8',  1, CAST(N'2020-03-31T10:44:23.3433333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E9',  1, CAST(N'2020-03-31T10:44:23.3433333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9442dcc4-22df-494b-8672-b7b4dd077496', N'C15E1',  1, CAST(N'2020-03-31T10:44:23.2766667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9442dcc4-22df-494b-8672-b7b4dd077496', N'C15E2',  1, CAST(N'2020-03-31T10:44:23.2766667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9442dcc4-22df-494b-8672-b7b4dd077496', N'C15E3',  1, CAST(N'2020-03-31T10:44:23.2733333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9442dcc4-22df-494b-8672-b7b4dd077496', N'C15E4',  1, CAST(N'2020-03-31T10:44:23.2733333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9442dcc4-22df-494b-8672-b7b4dd077496', N'C15E5',  1, CAST(N'2020-03-31T10:44:23.2700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9442dcc4-22df-494b-8672-b7b4dd077496', N'C15E6',  1, CAST(N'2020-03-31T10:44:23.2700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9442dcc4-22df-494b-8672-b7b4dd077496', N'C15E7',  1, CAST(N'2020-03-31T10:44:23.2666667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9442dcc4-22df-494b-8672-b7b4dd077496', N'C15E8',  1, CAST(N'2020-03-31T10:44:23.2633333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E1',  1, CAST(N'2020-03-31T10:44:23.3100000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E10', 1, CAST(N'2020-03-31T10:44:23.2900000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E11', 1, CAST(N'2020-03-31T10:44:23.2900000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E12', 1, CAST(N'2020-03-31T10:44:23.2866667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E13', 1, CAST(N'2020-03-31T10:44:23.2866667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E14', 2, CAST(N'2020-03-31T10:44:23.2833333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E15', 2, CAST(N'2020-03-31T10:44:23.2800000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E16', 2, CAST(N'2020-03-31T10:44:23.2800000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E2',  1, CAST(N'2020-03-31T10:44:23.3033333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E3',  1, CAST(N'2020-03-31T10:44:23.3033333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E4',  1, CAST(N'2020-03-31T10:44:23.3000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E5',  1, CAST(N'2020-03-31T10:44:23.3000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E6',  1, CAST(N'2020-03-31T10:44:23.3000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E7',  1, CAST(N'2020-03-31T10:44:23.2966667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E8',  1, CAST(N'2020-03-31T10:44:23.2933333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E9',  1, CAST(N'2020-03-31T10:44:23.2933333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', N'C6E1',   1, CAST(N'2020-03-31T10:44:23.2133333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', N'C6E2',   3, CAST(N'2020-03-31T10:44:23.2000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', N'C6E3',   2, CAST(N'2020-03-31T10:44:23.2000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', N'C6E4',   2, CAST(N'2020-03-31T10:44:23.1966667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E1',  2, CAST(N'2020-03-31T10:44:23.1700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E10', 2, CAST(N'2020-03-31T10:44:23.1366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E11', 1, CAST(N'2020-03-31T10:44:23.1333333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E12', 2, CAST(N'2020-03-31T10:44:23.1333333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E13', 2, CAST(N'2020-03-31T10:44:23.1300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E2',  2, CAST(N'2020-03-31T10:44:23.1666667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E3',  1, CAST(N'2020-03-31T10:44:23.1500000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E4',  2, CAST(N'2020-03-31T10:44:23.1466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E5',  1, CAST(N'2020-03-31T10:44:23.1466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E6',  2, CAST(N'2020-03-31T10:44:23.1433333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E7',  1, CAST(N'2020-03-31T10:44:23.1400000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E8',  2, CAST(N'2020-03-31T10:44:23.1400000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E9',  1, CAST(N'2020-03-31T10:44:23.1366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-001', N'12c6a61c-013c-475f-bb0c-2da5d414c03b', N'C35E1',  1, CAST(N'2020-03-31T11:41:49.3900000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-001', N'2271b113-5d5d-4899-b259-3046caea76ed', N'C34E1',  1, CAST(N'2020-03-31T11:41:49.3866667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-001', N'2271b113-5d5d-4899-b259-3046caea76ed', N'C34E2',  1, CAST(N'2020-03-31T11:41:49.3866667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-001', N'2271b113-5d5d-4899-b259-3046caea76ed', N'C34E3',  1, CAST(N'2020-03-31T11:41:49.3833333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-001', N'2271b113-5d5d-4899-b259-3046caea76ed', N'C34E4',  1, CAST(N'2020-03-31T11:41:49.3833333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-001', N'2271b113-5d5d-4899-b259-3046caea76ed', N'C34E5',  1, CAST(N'2020-03-31T11:41:49.3800000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-001', N'2271b113-5d5d-4899-b259-3046caea76ed', N'C34E6',  2, CAST(N'2020-03-31T11:41:49.3800000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', N'C6E1',   1, CAST(N'2020-03-31T11:41:49.3766667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', N'C6E2',   1, CAST(N'2020-03-31T11:41:49.3633333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', N'C6E3',   1, CAST(N'2020-03-31T11:41:49.3633333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', N'C6E4',   2, CAST(N'2020-03-31T11:41:49.3600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E1',   1, CAST(N'2020-03-30T16:13:21.2300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E2',   1, CAST(N'2020-03-30T16:13:21.2300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E3',   1, CAST(N'2020-03-30T16:13:21.2266667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E4',   1, CAST(N'2020-03-30T16:13:21.2266667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E5',   1, CAST(N'2020-03-30T16:13:21.2233333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E6',   1, CAST(N'2020-03-30T16:13:21.2200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E7',   1, CAST(N'2020-03-30T16:13:21.2200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10007-002', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', N'C6E1',   1, CAST(N'2020-03-25T12:49:14.9000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10007-002', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', N'C6E2',   2, CAST(N'2020-03-25T12:49:14.9000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10007-002', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', N'C6E3',   2, CAST(N'2020-03-25T12:49:14.8966667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10007-002', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', N'C6E4',   2, CAST(N'2020-03-25T12:49:14.8966667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E1',   1, CAST(N'2020-04-01T13:13:06.5500000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E10',  2, CAST(N'2020-04-01T13:13:06.5300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E2',   1, CAST(N'2020-04-01T13:13:06.5466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E3',   2, CAST(N'2020-04-01T13:13:06.5433333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E4',   2, CAST(N'2020-04-01T13:13:06.5433333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E5',   2, CAST(N'2020-04-01T13:13:06.5400000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E6',   2, CAST(N'2020-04-01T13:13:06.5400000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E7',   2, CAST(N'2020-04-01T13:13:06.5366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E8',   2, CAST(N'2020-04-01T13:13:06.5366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E9',   2, CAST(N'2020-04-01T13:13:06.5333333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'e5e3be58-e5ec-4423-85dd-61d88640c22a', N'C33E1',  1, CAST(N'2020-04-01T13:13:06.5266667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'e5e3be58-e5ec-4423-85dd-61d88640c22a', N'C33E2',  2, CAST(N'2020-04-01T13:13:06.5233333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'e5e3be58-e5ec-4423-85dd-61d88640c22a', N'C33E3',  2, CAST(N'2020-04-01T13:13:06.5200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'e5e3be58-e5ec-4423-85dd-61d88640c22a', N'C33E4',  2, CAST(N'2020-04-01T13:13:06.5200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'21ae013d-42a4-4748-b435-73d5887944c2', N'C1E1',   1, CAST(N'2020-04-01T13:13:06.5566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'21ae013d-42a4-4748-b435-73d5887944c2', N'C1E2',   2, CAST(N'2020-04-01T13:13:06.5533333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'4f09e77b-e3a3-4a25-8ec1-815921f83628', N'C2E1',   1, CAST(N'2020-04-01T13:13:06.5533333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'4f09e77b-e3a3-4a25-8ec1-815921f83628', N'C2E2',   2, CAST(N'2020-04-01T13:13:06.5500000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'64e5986d-1ebf-4df0-8219-c150c082ca7b', N'C4E1',   1, CAST(N'2020-04-01T13:13:06.5300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'64e5986d-1ebf-4df0-8219-c150c082ca7b', N'C4E2',   2, CAST(N'2020-04-01T13:13:06.5300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E1',   1, CAST(N'2020-03-30T16:22:34.7266667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E10',  1, CAST(N'2020-03-30T16:22:34.7100000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E11',  1, CAST(N'2020-03-30T16:22:34.7100000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E12',  1, CAST(N'2020-03-30T16:22:34.7066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E13',  1, CAST(N'2020-03-30T16:22:34.7066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E14',  1, CAST(N'2020-03-30T16:22:34.7033333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E15',  1, CAST(N'2020-03-30T16:22:34.7033333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E16',  1, CAST(N'2020-03-30T16:22:34.7000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E17',  1, CAST(N'2020-03-30T16:22:34.6933333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E2',   1, CAST(N'2020-03-30T16:22:34.7233333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E3',   1, CAST(N'2020-03-30T16:22:34.7233333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E4',   1, CAST(N'2020-03-30T16:22:34.7200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E5',   1, CAST(N'2020-03-30T16:22:34.7200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E6',   1, CAST(N'2020-03-30T16:22:34.7166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E7',   1, CAST(N'2020-03-30T16:22:34.7166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E8',   1, CAST(N'2020-03-30T16:22:34.7133333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E9',   1, CAST(N'2020-03-30T16:22:34.7133333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'e5521a71-a28e-4bc9-bddf-599f0a90719d', N'C17E1',  1, CAST(N'2020-03-30T16:22:34.6900000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'e5521a71-a28e-4bc9-bddf-599f0a90719d', N'C17E2',  2, CAST(N'2020-03-30T16:22:34.6900000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'1e82cc7c-87c7-4379-b86f-cf36c59d1a46', N'C19E1',  1, CAST(N'2020-03-30T16:22:34.6866667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'1e82cc7c-87c7-4379-b86f-cf36c59d1a46', N'C19E2',  1, CAST(N'2020-03-30T16:22:34.6866667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'1e82cc7c-87c7-4379-b86f-cf36c59d1a46', N'C19E3',  1, CAST(N'2020-03-30T16:22:34.6833333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E1',  1, CAST(N'2020-03-30T16:22:34.6833333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E10', 1, CAST(N'2020-03-30T16:22:34.6600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E11', 1, CAST(N'2020-03-30T16:22:34.6566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E12', 1, CAST(N'2020-03-30T16:22:34.6566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E13', 2, CAST(N'2020-03-30T16:22:34.6533333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E2',  2, CAST(N'2020-03-30T16:22:34.6733333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E3',  1, CAST(N'2020-03-30T16:22:34.6700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E4',  1, CAST(N'2020-03-30T16:22:34.6700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E5',  3, CAST(N'2020-03-30T16:22:34.6666667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E6',  3, CAST(N'2020-03-30T16:22:34.6666667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E7',  1, CAST(N'2020-03-30T16:22:34.6633333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E8',  2, CAST(N'2020-03-30T16:22:34.6633333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', N'C20E9',  1, CAST(N'2020-03-30T16:22:34.6600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E1',   1, CAST(N'2020-03-30T16:18:23.4633333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E10',  1, CAST(N'2020-03-30T16:18:23.4466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E11',  1, CAST(N'2020-03-30T16:18:23.4466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E12',  1, CAST(N'2020-03-30T16:18:23.4433333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E13',  1, CAST(N'2020-03-30T16:18:23.4400000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E14',  1, CAST(N'2020-03-30T16:18:23.4400000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E15',  1, CAST(N'2020-03-30T16:18:23.4366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E16',  1, CAST(N'2020-03-30T16:18:23.4366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E17',  3, CAST(N'2020-03-30T16:18:23.4333333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E2',   1, CAST(N'2020-03-30T16:18:23.4600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E3',   1, CAST(N'2020-03-30T16:18:23.4600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E4',   3, CAST(N'2020-03-30T16:18:23.4566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E5',   1, CAST(N'2020-03-30T16:18:23.4566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E6',   1, CAST(N'2020-03-30T16:18:23.4533333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E7',   3, CAST(N'2020-03-30T16:18:23.4500000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E8',   1, CAST(N'2020-03-30T16:18:23.4500000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', N'C9E9',   1, CAST(N'2020-03-30T16:18:23.4500000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'e5521a71-a28e-4bc9-bddf-599f0a90719d', N'C17E1',  1, CAST(N'2020-03-30T16:18:23.4333333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'e5521a71-a28e-4bc9-bddf-599f0a90719d', N'C17E2',  2, CAST(N'2020-03-30T16:18:23.4300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E1',   1, CAST(N'2020-03-31T14:22:18.0033333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E10',  2, CAST(N'2020-03-31T14:22:17.9900000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E2',   1, CAST(N'2020-03-31T14:22:18.0033333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E3',   2, CAST(N'2020-03-31T14:22:18.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E4',   2, CAST(N'2020-03-31T14:22:18.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E5',   2, CAST(N'2020-03-31T14:22:17.9966667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E6',   2, CAST(N'2020-03-31T14:22:17.9933333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E7',   2, CAST(N'2020-03-31T14:22:17.9933333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E8',   2, CAST(N'2020-03-31T14:22:17.9900000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'60c2f5b0-b950-44c8-a246-099335a1c816', N'C3E9',   2, CAST(N'2020-03-31T14:22:17.9900000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'7547e181-c897-4a01-86d9-09b76ab1c906', N'C23E6',  2, CAST(N'2020-03-31T14:22:17.9200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'7547e181-c897-4a01-86d9-09b76ab1c906', N'C23E7',  2, CAST(N'2020-03-31T14:22:17.9200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'7547e181-c897-4a01-86d9-09b76ab1c906', N'C23E8',  2, CAST(N'2020-03-31T14:22:17.9166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'7547e181-c897-4a01-86d9-09b76ab1c906', N'C23E9',  2, CAST(N'2020-03-31T14:22:17.9133333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E1',   1, CAST(N'2020-03-31T14:22:17.9800000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E2',   1, CAST(N'2020-03-31T14:22:17.9800000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E3',   1, CAST(N'2020-03-31T14:22:17.9766667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E4',   1, CAST(N'2020-03-31T14:22:17.9766667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E5',   1, CAST(N'2020-03-31T14:22:17.9733333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E6',   1, CAST(N'2020-03-31T14:22:17.9733333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', N'C5E7',   1, CAST(N'2020-03-31T14:22:17.9700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', N'C12E1',  1, CAST(N'2020-03-31T14:22:17.9500000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', N'C12E2',  1, CAST(N'2020-03-31T14:22:17.9466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', N'C12E3',  1, CAST(N'2020-03-31T14:22:17.9466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', N'C12E4',  1, CAST(N'2020-03-31T14:22:17.9433333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', N'C12E5',  1, CAST(N'2020-03-31T14:22:17.9433333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', N'C12E6',  1, CAST(N'2020-03-31T14:22:17.9400000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', N'C12E7',  2, CAST(N'2020-03-31T14:22:17.9400000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'59696227-602a-421d-a883-29e88997ac17', N'C39E1',  1, CAST(N'2020-03-31T14:22:18.1166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'59696227-602a-421d-a883-29e88997ac17', N'C39E2',  2, CAST(N'2020-03-31T14:22:18.1133333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'59696227-602a-421d-a883-29e88997ac17', N'C39E3',  2, CAST(N'2020-03-31T14:22:18.1133333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'20b09859-6fc2-404c-b7a4-3830790e63ab', N'C11E1',  1, CAST(N'2020-03-31T14:22:18.0833333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'20b09859-6fc2-404c-b7a4-3830790e63ab', N'C11E2',  1, CAST(N'2020-03-31T14:22:18.0833333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'0a372f63-add4-4529-a6cd-4437c6ef115b', N'C7E1',   1, CAST(N'2020-03-31T14:22:17.9700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'0a372f63-add4-4529-a6cd-4437c6ef115b', N'C7E10',  3, CAST(N'2020-03-31T14:22:17.9533333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'0a372f63-add4-4529-a6cd-4437c6ef115b', N'C7E2',   3, CAST(N'2020-03-31T14:22:17.9666667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'0a372f63-add4-4529-a6cd-4437c6ef115b', N'C7E3',   1, CAST(N'2020-03-31T14:22:17.9666667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'0a372f63-add4-4529-a6cd-4437c6ef115b', N'C7E4',   3, CAST(N'2020-03-31T14:22:17.9633333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'0a372f63-add4-4529-a6cd-4437c6ef115b', N'C7E5',   1, CAST(N'2020-03-31T14:22:17.9633333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'0a372f63-add4-4529-a6cd-4437c6ef115b', N'C7E6',   1, CAST(N'2020-03-31T14:22:17.9600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'0a372f63-add4-4529-a6cd-4437c6ef115b', N'C7E7',   1, CAST(N'2020-03-31T14:22:17.9600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'0a372f63-add4-4529-a6cd-4437c6ef115b', N'C7E8',   3, CAST(N'2020-03-31T14:22:17.9566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'0a372f63-add4-4529-a6cd-4437c6ef115b', N'C7E9',   3, CAST(N'2020-03-31T14:22:17.9566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'dd649cc4-a710-4472-98b3-663d9d12a8b7', N'C16E1',  1, CAST(N'2020-03-31T14:22:17.9866667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E1',  1, CAST(N'2020-03-31T14:22:18.0400000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E10', 1, CAST(N'2020-03-31T14:22:18.0266667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E11', 1, CAST(N'2020-03-31T14:22:18.0233333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E12', 1, CAST(N'2020-03-31T14:22:18.0233333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E13', 1, CAST(N'2020-03-31T14:22:18.0200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E14', 1, CAST(N'2020-03-31T14:22:18.0200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E15', 2, CAST(N'2020-03-31T14:22:18.0166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E16', 2, CAST(N'2020-03-31T14:22:18.0133333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E17', 2, CAST(N'2020-03-31T14:22:18.0133333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E18', 2, CAST(N'2020-03-31T14:22:18.0100000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E19', 2, CAST(N'2020-03-31T14:22:18.0100000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E2',  1, CAST(N'2020-03-31T14:22:18.0400000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E20', 2, CAST(N'2020-03-31T14:22:18.0066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E21', 2, CAST(N'2020-03-31T14:22:18.0066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E3',  1, CAST(N'2020-03-31T14:22:18.0366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E4',  1, CAST(N'2020-03-31T14:22:18.0366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E5',  1, CAST(N'2020-03-31T14:22:18.0333333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E6',  1, CAST(N'2020-03-31T14:22:18.0333333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E7',  1, CAST(N'2020-03-31T14:22:18.0300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E8',  1, CAST(N'2020-03-31T14:22:18.0300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', N'C13E9',  1, CAST(N'2020-03-31T14:22:18.0266667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'21ae013d-42a4-4748-b435-73d5887944c2', N'C1E1',   1, CAST(N'2020-03-31T14:22:18.1333333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'21ae013d-42a4-4748-b435-73d5887944c2', N'C1E2',   1, CAST(N'2020-03-31T14:22:18.1300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'4f09e77b-e3a3-4a25-8ec1-815921f83628', N'C2E1',   1, CAST(N'2020-03-31T14:22:17.9533333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'4f09e77b-e3a3-4a25-8ec1-815921f83628', N'C2E2',   1, CAST(N'2020-03-31T14:22:17.9500000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'5db79ff4-fa9c-4da2-bbfc-8ca40fec0b43', N'C26E1',  1, CAST(N'2020-03-31T14:22:17.9366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'5db79ff4-fa9c-4da2-bbfc-8ca40fec0b43', N'C26E10', 1, CAST(N'2020-03-31T14:22:17.9200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'5db79ff4-fa9c-4da2-bbfc-8ca40fec0b43', N'C26E2',  1, CAST(N'2020-03-31T14:22:17.9366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'5db79ff4-fa9c-4da2-bbfc-8ca40fec0b43', N'C26E3',  1, CAST(N'2020-03-31T14:22:17.9333333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'5db79ff4-fa9c-4da2-bbfc-8ca40fec0b43', N'C26E4',  1, CAST(N'2020-03-31T14:22:17.9333333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'5db79ff4-fa9c-4da2-bbfc-8ca40fec0b43', N'C26E5',  3, CAST(N'2020-03-31T14:22:17.9300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'5db79ff4-fa9c-4da2-bbfc-8ca40fec0b43', N'C26E6',  1, CAST(N'2020-03-31T14:22:17.9300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'5db79ff4-fa9c-4da2-bbfc-8ca40fec0b43', N'C26E7',  2, CAST(N'2020-03-31T14:22:17.9266667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'5db79ff4-fa9c-4da2-bbfc-8ca40fec0b43', N'C26E8',  1, CAST(N'2020-03-31T14:22:17.9266667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'5db79ff4-fa9c-4da2-bbfc-8ca40fec0b43', N'C26E9',  3, CAST(N'2020-03-31T14:22:17.9233333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'd1532ca0-ef0c-457c-9cfc-affa0fbdf134', N'C36E1',  1, CAST(N'2020-03-31T14:22:18.1100000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'd1532ca0-ef0c-457c-9cfc-affa0fbdf134', N'C36E10', 2, CAST(N'2020-03-31T14:22:18.0900000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'd1532ca0-ef0c-457c-9cfc-affa0fbdf134', N'C36E11', 2, CAST(N'2020-03-31T14:22:18.0900000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'd1532ca0-ef0c-457c-9cfc-affa0fbdf134', N'C36E12', 2, CAST(N'2020-03-31T14:22:18.0866667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'd1532ca0-ef0c-457c-9cfc-affa0fbdf134', N'C36E2',  1, CAST(N'2020-03-31T14:22:18.1100000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'd1532ca0-ef0c-457c-9cfc-affa0fbdf134', N'C36E3',  1, CAST(N'2020-03-31T14:22:18.1066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'd1532ca0-ef0c-457c-9cfc-affa0fbdf134', N'C36E4',  1, CAST(N'2020-03-31T14:22:18.1033333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'd1532ca0-ef0c-457c-9cfc-affa0fbdf134', N'C36E5',  2, CAST(N'2020-03-31T14:22:18.1033333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'd1532ca0-ef0c-457c-9cfc-affa0fbdf134', N'C36E6',  2, CAST(N'2020-03-31T14:22:18.0966667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'd1532ca0-ef0c-457c-9cfc-affa0fbdf134', N'C36E7',  2, CAST(N'2020-03-31T14:22:18.0966667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'd1532ca0-ef0c-457c-9cfc-affa0fbdf134', N'C36E8',  2, CAST(N'2020-03-31T14:22:18.0933333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'd1532ca0-ef0c-457c-9cfc-affa0fbdf134', N'C36E9',  2, CAST(N'2020-03-31T14:22:18.0900000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'9442dcc4-22df-494b-8672-b7b4dd077496', N'C15E1',  1, CAST(N'2020-03-31T14:22:18.1300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'9442dcc4-22df-494b-8672-b7b4dd077496', N'C15E2',  1, CAST(N'2020-03-31T14:22:18.1266667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'9442dcc4-22df-494b-8672-b7b4dd077496', N'C15E3',  1, CAST(N'2020-03-31T14:22:18.1266667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'9442dcc4-22df-494b-8672-b7b4dd077496', N'C15E4',  1, CAST(N'2020-03-31T14:22:18.1233333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'9442dcc4-22df-494b-8672-b7b4dd077496', N'C15E5',  1, CAST(N'2020-03-31T14:22:18.1233333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'9442dcc4-22df-494b-8672-b7b4dd077496', N'C15E6',  1, CAST(N'2020-03-31T14:22:18.1200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'9442dcc4-22df-494b-8672-b7b4dd077496', N'C15E7',  1, CAST(N'2020-03-31T14:22:18.1200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'9442dcc4-22df-494b-8672-b7b4dd077496', N'C15E8',  1, CAST(N'2020-03-31T14:22:18.1166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E1',  1, CAST(N'2020-03-31T14:22:18.0800000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E10', 1, CAST(N'2020-03-31T14:22:18.0600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E11', 1, CAST(N'2020-03-31T14:22:18.0566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E12', 1, CAST(N'2020-03-31T14:22:18.0533333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E13', 1, CAST(N'2020-03-31T14:22:18.0466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E14', 2, CAST(N'2020-03-31T14:22:18.0466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E15', 2, CAST(N'2020-03-31T14:22:18.0433333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E16', 2, CAST(N'2020-03-31T14:22:18.0433333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E2',  1, CAST(N'2020-03-31T14:22:18.0800000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E3',  1, CAST(N'2020-03-31T14:22:18.0766667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E4',  1, CAST(N'2020-03-31T14:22:18.0733333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E5',  1, CAST(N'2020-03-31T14:22:18.0733333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E6',  1, CAST(N'2020-03-31T14:22:18.0700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E7',  1, CAST(N'2020-03-31T14:22:18.0666667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E8',  1, CAST(N'2020-03-31T14:22:18.0633333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', N'C14E9',  1, CAST(N'2020-03-31T14:22:18.0600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'64e5986d-1ebf-4df0-8219-c150c082ca7b', N'C4E1',   1, CAST(N'2020-03-31T14:22:17.9866667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'64e5986d-1ebf-4df0-8219-c150c082ca7b', N'C4E2',   2, CAST(N'2020-03-31T14:22:17.9833333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10059-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', N'C6E1',   1, CAST(N'2020-03-30T16:08:55.3200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10059-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', N'C6E2',   1, CAST(N'2020-03-30T16:08:55.3200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10059-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', N'C6E3',   1, CAST(N'2020-03-30T16:08:55.3166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10059-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', N'C6E4',   2, CAST(N'2020-03-30T16:08:55.3166667' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    MERGE INTO dbo.SolutionEpic AS TARGET
    USING #SolutionEpic AS SOURCE
    ON TARGET.SolutionId = SOURCE.SolutionId AND TARGET.CapabilityId = SOURCE.CapabilityId AND TARGET.EpicId = SOURCE.EpicId 
    WHEN MATCHED THEN
           UPDATE SET TARGET.StatusId = SOURCE.StatusId,
                      TARGET.LastUpdated = SOURCE.LastUpdated,
                      TARGET.LastUpdatedBy = SOURCE.LastUpdatedBy
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (SolutionId, CapabilityId, EpicId, StatusId, LastUpdated, LastUpdatedBy)
        VALUES (SOURCE.SolutionId, SOURCE.CapabilityId, SOURCE.EpicId, SOURCE.StatusId, SOURCE.LastUpdated, SOURCE.LastUpdatedBy);
END;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./ProdLikeData/MergeSolutionCapabilities.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN
    /*********************************************************************************************************************************************/
    /* SolutionCapability */
    /*********************************************************************************************************************************************/

    CREATE TABLE #SolutionCapability
    (
        SolutionId nvarchar(14) NOT NULL,
        CapabilityId uniqueidentifier NOT NULL,
        StatusId int NOT NULL,
        LastUpdated datetime2(7) NOT NULL,
        LastUpdatedBy uniqueidentifier NOT NULL
    );

    INSERT INTO #SolutionCapability (SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy) 
         VALUES (N'10000-001', N'efd93d25-447b-4ca3-9d78-108d42afeae0', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'8bee1ff3-84d4-430b-a678-336f57c57387', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'20b09859-6fc2-404c-b7a4-3830790e63ab', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'9d805aad-d43a-480e-9bc0-41a755bafe2f', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'e5521a71-a28e-4bc9-bddf-599f0a90719d', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'dd649cc4-a710-4472-98b3-663d9d12a8b7', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'9442dcc4-22df-494b-8672-b7b4dd077496', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-002', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-04-06T10:50:03.2200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-062', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-04-06T10:53:50.6300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-001', N'12c6a61c-013c-475f-bb0c-2da5d414c03b', 1, CAST(N'2020-03-26T12:13:20.1066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-001', N'2271b113-5d5d-4899-b259-3046caea76ed', 1, CAST(N'2020-03-26T12:13:20.1066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', 1, CAST(N'2020-03-26T12:13:20.1066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', 1, CAST(N'2020-03-30T13:14:43.1666667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10007-002', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', 1, CAST(N'2020-03-25T11:42:40.8100000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10011-003', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-06-18T14:20:53.8266667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10029-003', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-04-08T09:19:04.7600000' AS datetime2), N'6842fa00-b899-43e3-845c-d4569f0b53df'),
                (N'10030-001', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-04-02T09:50:51.4966667' AS datetime2), N'71b953b6-8433-4afe-a3a6-21a93f3f3a4d'),
                (N'10031-001', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-04-02T09:39:09.7500000' AS datetime2), N'ba95e474-ed3d-4f0d-ab29-d3a960cf0f5d'),
                (N'10035-001', N'60c2f5b0-b950-44c8-a246-099335a1c816', 1, CAST(N'2020-04-01T10:42:08.5066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'e5e3be58-e5ec-4423-85dd-61d88640c22a', 1, CAST(N'2020-04-01T10:42:08.5066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'21ae013d-42a4-4748-b435-73d5887944c2', 1, CAST(N'2020-04-01T10:42:08.5066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'4f09e77b-e3a3-4a25-8ec1-815921f83628', 1, CAST(N'2020-04-01T10:42:08.5066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'64e5986d-1ebf-4df0-8219-c150c082ca7b', 1, CAST(N'2020-04-01T10:42:08.5066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', 1, CAST(N'2020-03-30T13:02:24.5200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'e5521a71-a28e-4bc9-bddf-599f0a90719d', 1, CAST(N'2020-03-30T13:02:24.5200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'1e82cc7c-87c7-4379-b86f-cf36c59d1a46', 1, CAST(N'2020-03-30T13:02:24.5200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', 1, CAST(N'2020-03-30T13:02:24.5200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', 1, CAST(N'2020-03-30T13:04:21.6500000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'e5521a71-a28e-4bc9-bddf-599f0a90719d', 1, CAST(N'2020-03-30T13:04:21.6500000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-006', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-06-25T14:31:07.2366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'60c2f5b0-b950-44c8-a246-099335a1c816', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'59696227-602a-421d-a883-29e88997ac17', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'20b09859-6fc2-404c-b7a4-3830790e63ab', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'dd649cc4-a710-4472-98b3-663d9d12a8b7', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'21ae013d-42a4-4748-b435-73d5887944c2', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'4f09e77b-e3a3-4a25-8ec1-815921f83628', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'5db79ff4-fa9c-4da2-bbfc-8ca40fec0b43', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'd1532ca0-ef0c-457c-9cfc-affa0fbdf134', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'9442dcc4-22df-494b-8672-b7b4dd077496', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'64e5986d-1ebf-4df0-8219-c150c082ca7b', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10059-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', 1, CAST(N'2020-03-30T13:16:49.4133333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10063-002', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-06-25T14:30:56.3300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10063-002', N'7be309d9-696f-4b90-a65e-eb16dd5ac4ed', 1, CAST(N'2020-06-25T14:30:56.3300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10072-003', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-06-25T14:30:33.5166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10072-004', N'21ae013d-42a4-4748-b435-73d5887944c2', 1, CAST(N'2020-06-25T14:31:34.0466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10072-004', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-06-25T14:31:34.0466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10073-009', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-04-01T12:49:33.9433333' AS datetime2), N'00000000-0000-0000-0000-000000000000');
    
    MERGE INTO dbo.SolutionCapability AS TARGET
    USING #SolutionCapability AS SOURCE
    ON TARGET.SolutionId = SOURCE.SolutionId AND TARGET.CapabilityId = SOURCE.CapabilityId 
    WHEN MATCHED THEN
           UPDATE SET TARGET.StatusId = SOURCE.StatusId,
                      TARGET.LastUpdated = SOURCE.LastUpdated,
                      TARGET.LastUpdatedBy = SOURCE.LastUpdatedBy
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
        VALUES (SOURCE.SolutionId, SOURCE.CapabilityId, SOURCE.StatusId, SOURCE.LastUpdated, SOURCE.LastUpdatedBy);
END;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./ProdLikeData/MergeFrameworkSolutions.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN
    /*********************************************************************************************************************************************/
    /* FrameworkSolutions */
    /*********************************************************************************************************************************************/

    CREATE TABLE #FrameworkSolutions
    (
        FrameworkId nvarchar(10) NOT NULL,
        SolutionId nvarchar(14) NOT NULL,
        IsFoundation bit CONSTRAINT DF_FrameworkSolutions_IsFoundation DEFAULT 0 NOT NULL,
        LastUpdated datetime2(7) NOT NULL,
        LastUpdatedBy uniqueidentifier NOT NULL,
    );

    INSERT INTO #FrameworkSolutions (FrameworkId, SolutionId, IsFoundation, LastUpdated, LastUpdatedBy) 
         VALUES (N'NHSDGP001', N'10000-001', 1, CAST(N'2020-03-25T07:30:18.1133333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10000-002', 0, CAST(N'2020-04-06T10:50:03.2166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10000-054', 0, CAST(N'2020-04-03T12:25:59.0566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10000-062', 0, CAST(N'2020-04-06T10:53:50.6266667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10004-001', 0, CAST(N'2020-03-26T12:13:20.0866667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10004-002', 0, CAST(N'2020-03-30T13:14:43.1666667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10007-002', 0, CAST(N'2020-03-25T11:40:44.2900000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10011-003', 0, CAST(N'2020-06-18T14:20:53.8233333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10020-001', 0, CAST(N'2020-04-06T12:50:27.8800000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10029-001', 0, CAST(N'2020-04-08T07:42:58.2633333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10029-003', 0, CAST(N'2020-04-08T08:59:03.8100000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10030-001', 0, CAST(N'2020-04-01T10:39:24.7100000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10031-001', 0, CAST(N'2020-04-01T10:37:59.3066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10033-001', 0, CAST(N'2020-04-01T10:40:33.7566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10035-001', 0, CAST(N'2020-04-01T10:42:08.5066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10046-001', 0, CAST(N'2020-03-30T13:02:24.5200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10046-003', 0, CAST(N'2020-03-30T13:04:21.6500000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10046-006', 0, CAST(N'2020-06-25T14:31:07.2366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10047-001', 0, CAST(N'2020-04-01T10:43:15.8533333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10052-002', 1, CAST(N'2020-03-30T13:19:48.8766667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10059-001', 0, CAST(N'2020-03-30T13:16:49.4100000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10062-001', 0, CAST(N'2020-04-03T12:28:52.3800000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10063-002', 0, CAST(N'2020-06-25T14:30:56.3300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10064-003', 0, CAST(N'2020-06-25T14:30:49.8600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10072-003', 0, CAST(N'2020-06-25T14:30:33.5166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10072-004', 0, CAST(N'2020-06-25T14:31:34.0466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10072-006', 0, CAST(N'2020-06-25T14:31:15.0200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'NHSDGP001', N'10073-009', 0, CAST(N'2020-04-01T12:49:33.9433333' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    MERGE INTO dbo.FrameworkSolutions AS TARGET
    USING #FrameworkSolutions AS SOURCE
    ON TARGET.FrameworkId = SOURCE.FrameworkId AND TARGET.SolutionId = SOURCE.SolutionId 
    WHEN MATCHED THEN
           UPDATE SET TARGET.IsFoundation = SOURCE.IsFoundation,
                      TARGET.LastUpdated = SOURCE.LastUpdated,
                      TARGET.LastUpdatedBy = SOURCE.LastUpdatedBy
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (FrameworkId, SolutionId, IsFoundation, LastUpdated, LastUpdatedBy)
        VALUES (SOURCE.FrameworkId, SOURCE.SolutionId, SOURCE.IsFoundation, SOURCE.LastUpdated, SOURCE.LastUpdatedBy);
END;
GO
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--      ./ProdLikeData/MergeCataloguePrices.sql
----------------------------------------------------------------------------------------------------------------------------------------------------------------
IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN
    /*********************************************************************************************************************************************/
    /* CataloguePrice */
    /*********************************************************************************************************************************************/

    CREATE TABLE #CataloguePrice
    (
        CataloguePriceId int NOT NULL,
        CatalogueItemId nvarchar(14) NOT NULL,
        ProvisioningTypeId int NOT NULL,
        CataloguePriceTypeId int NOT NULL,
        PricingUnitId uniqueidentifier NOT NULL,
        TimeUnitId int NULL,
        CurrencyCode nvarchar(3) NOT NULL,
        LastUpdated datetime2(7) NOT NULL,
        Price decimal(18,3) NULL,
    );

    -- Solutions per patient
    INSERT INTO #CataloguePrice (CataloguePriceId, CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
         VALUES (1001, N'10000-001', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 1.26),
                (1002, N'10000-054', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 0.15),
                (1003, N'10000-062', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 0.02),
                (1004, N'10004-001', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 0.22),
                (1005, N'10004-002', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 0.19),
                (1006, N'10007-002', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 0.14),
                (1007, N'10020-001', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 0.5),
                (1008, N'10029-003', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 0.84),
                (1009, N'10046-001', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 0.28),
                (1010, N'10046-003', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 0.28),
                (1011, N'10047-001', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 0.84),
                (1012, N'10052-002', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 1.26),
                (1013, N'10059-001', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 0.14),
                (1014, N'10030-001', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 0),
                (1015, N'10033-001', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 1.26),
                (1016, N'10062-001', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 0.3),
                (1094, N'10063-002', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 0.3),
                (1095, N'10072-003', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 0.7),                
                (1099, N'10072-006', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', GETUTCDATE(), 0.20),                
    -- Solutions Variable On Demand 
                (1017, N'10035-001', 3, 1,'8a5e119f-9b33-4017-8cc9-552e86e20898', NULL, 'GBP', GETUTCDATE(), 0),
                (1096, N'10072-004', 3, 1,'60d07eb0-01ef-44e4-bed3-d34ad1352e19', NULL, 'GBP', GETUTCDATE(), 18),
                (1097, N'10072-004', 3, 1,'93931091-8945-43a0-b181-96f2b41ed3c3', NULL, 'GBP', GETUTCDATE(), 20),
                (1098, N'10072-004', 3, 1,'fec28905-5670-4c45-99f3-1f93c8aa156c', NULL, 'GBP', GETUTCDATE(), 30),
    -- Solutions Declarative
                (1018, N'10000-002', 2, 1,'8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', 1, 'GBP', GETUTCDATE(), 37.92),
                (1019, N'10073-009', 2, 1,'AAD2820E-472D-4BAC-864E-853F92E9B3BC', 1, 'GBP', GETUTCDATE(),207.92),
    -- Additional Service Per Patient
                (1020, N'10000-001A001', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP',GETUTCDATE(), 0.25),
                (1021, N'10000-001A002', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP',GETUTCDATE(), 0.07),
                (1022, N'10000-001A004', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP',GETUTCDATE(), 0.06),
                (1023, N'10000-001A005', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP',GETUTCDATE(), 0.25),
                (1024, N'10000-001A006', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP',GETUTCDATE(), 0.12),
                (1025, N'10000-001A007', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP',GETUTCDATE(), 0.06),
                (1026, N'10000-001A008', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP',GETUTCDATE(), 0.06),
                (1027, N'10007-002A001', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP',GETUTCDATE(), 0.04),
                (1028, N'10007-002A002', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP',GETUTCDATE(), 0.05),
                (1029, N'10030-001A001', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP',GETUTCDATE(), 0.20),
                (1100, N'10052-002A001', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP',GETUTCDATE(), 0.20),
                (1101, N'10052-002A002', 1, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP',GETUTCDATE(), 0.25),
    -- Additional Service Variable
                (1031, N'10035-001A001', 3, 1,'774E5A1D-D15C-4A37-9990-81861BEAE42B', NULL, 'GBP', GETUTCDATE(), 10),
    -- Additional Service Declarative
                (1033, N'10000-001A003', 2, 1,'8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', 1, 'GBP', GETUTCDATE(),35.51),
                (1034, N'10052-002A002', 2, 1,'9f8888de-69fb-4395-83ce-066d4a4dc120', 1, 'GBP', GETUTCDATE(),68.5),
                (1035, N'10052-002A003', 2, 1,'9f8888de-69fb-4395-83ce-066d4a4dc120', 1, 'GBP', GETUTCDATE(),68.5),
                (1036, N'10052-002A004', 2, 1,'9f8888de-69fb-4395-83ce-066d4a4dc120', 1, 'GBP', GETUTCDATE(),291.67),
                (1032, N'10052-002A005', 2, 1,'9f8888de-69fb-4395-83ce-066d4a4dc120', 1, 'GBP', GETUTCDATE(),0),
    -- Associated Service Variable
                (1037, N'10000-S-037', 3, 1,'60523726-bbaf-4ec3-b29c-dee2f3d3eca8', NULL, 'GBP', GETUTCDATE(), 4.35),
                (1038, N'10000-S-038', 3, 1,'60523726-bbaf-4ec3-b29c-dee2f3d3eca8', NULL, 'GBP', GETUTCDATE(), 1.25),
                (1039, N'10047-S-002', 3, 1,'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', NULL, 'GBP', GETUTCDATE(), 1.06),
                (1102, N'10030-S-001', 3, 1,'e72500e5-4cb4-4ddf-a8b8-d898187691ca', NULL, 'GBP', GETUTCDATE(), 0.017),
    -- Associated Service Declarative
      --per 1hr session 
                (1040, N'10073-S-022', 2, 1,'8eea4a69-977d-4fb1-b4d1-2f0971beb04b', NULL, 'GBP', GETUTCDATE(), 75),
      --per Course
                (1041, N'10052-S-003', 2, 1,'e17fbd0b-208f-453f-938a-9880bab1ec5e', NULL, 'GBP', GETUTCDATE(), 3188.72),
                (1042, N'10052-S-004', 2, 1,'e17fbd0b-208f-453f-938a-9880bab1ec5e', NULL, 'GBP', GETUTCDATE(), 6377.44),
      --per data extraction
                (1043, N'10052-S-011', 2, 1,'6f65c40f-e7cc-4140-85c5-592dcd216132', NULL, 'GBP', GETUTCDATE(), 1415.13),
      --per day
                (1044, N'10000-S-036', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 595),
                (1045, N'10000-S-041', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 510),
                (1046, N'10000-S-003', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 650),
                (1047, N'10004-S-002', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 500),
                (1048, N'10004-S-001', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 500),
                (1049, N'10007-S-002', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 300),
                (1050, N'10007-S-004', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 405),
                (1051, N'10007-S-005', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 495),
                (1052, N'10029-S-006', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 750),
                (1053, N'10029-S-007', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 750),
                (1054, N'10029-S-008', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 750),
                (1055, N'10029-S-009', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 750),
                (1056, N'10029-S-010', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 750),
                (1057, N'10046-S-005', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 750),
                (1058, N'10046-S-006', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 750),
                (1059, N'10046-S-007', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 750),
                (1060, N'10046-S-009', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 750),
                (1061, N'10046-S-010', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 750),
                (1062, N'10046-S-001', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 925),
                (1063, N'10046-S-002', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 750),
                (1064, N'10046-S-003', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 700),
                (1065, N'10046-S-004', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 750),
                (1066, N'10052-S-001', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 637.74),
                (1067, N'10073-S-021', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 600),
                (1068, N'10073-S-023', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 600),
                (1103, N'10063-S-004', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 850),
                (1104, N'10063-S-005', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 850),
                (1105, N'10063-S-006', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 850),
                (1106, N'10063-S-007', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 850),
                (1107, N'10063-S-009', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 850),
                (1108, N'10063-S-010', 2, 1,'599a1105-a16a-4861-b54b-d65da84366c9', NULL, 'GBP', GETUTCDATE(), 850),
      --per document
                (1069, N'10007-S-001', 2, 1,'66443acc-7e40-4f95-955b-47234016cff1', NULL, 'GBP', GETUTCDATE(), 20),
      --per extract
                (1070, N'10052-S-012', 2, 1,'6f65c40f-e7cc-4140-85c5-592dcd216132', NULL, 'GBP', GETUTCDATE(), 1979.21),
      --per half day
                (1071, N'10000-S-004', 2, 1,'121bd710-b73b-48f9-a429-f269a7bb0bf2', NULL, 'GBP', GETUTCDATE(), 252.5),
                (1072, N'10052-S-002', 2, 1,'121bd710-b73b-48f9-a429-f269a7bb0bf2', NULL, 'GBP', GETUTCDATE(), 637.74),
                (1109, N'10063-S-008', 2, 1,'121bd710-b73b-48f9-a429-f269a7bb0bf2', NULL, 'GBP', GETUTCDATE(), 630),
      --per implementation
                (1073, N'10000-S-039', 2, 1,'701afb98-699e-4eda-9a66-e79a91769614', NULL, 'GBP', GETUTCDATE(), 3000),
      --per installation
                (1074, N'10000-S-141', 2, 1,'7e4dd1fd-c953-41a8-9e62-64dc306a6307', NULL, 'GBP', GETUTCDATE(), 800),
                (1075, N'10000-S-002', 2, 1,'7e4dd1fd-c953-41a8-9e62-64dc306a6307', NULL, 'GBP', GETUTCDATE(), 1000),
                (1076, N'10000-S-005', 2, 1,'7e4dd1fd-c953-41a8-9e62-64dc306a6307', NULL, 'GBP', GETUTCDATE(), 2500),
      --per item
                (1077, N'10000-S-009', 2, 1,'60523726-bbaf-4ec3-b29c-dee2f3d3eca8', NULL, 'GBP', GETUTCDATE(), 1680),
                (1078, N'10000-S-001', 2, 1,'60523726-bbaf-4ec3-b29c-dee2f3d3eca8', NULL, 'GBP', GETUTCDATE(), 90),
                (1079, N'10000-S-006', 2, 1,'60523726-bbaf-4ec3-b29c-dee2f3d3eca8', NULL, 'GBP', GETUTCDATE(), 2048.88),
                (1080, N'10000-S-007', 2, 1,'60523726-bbaf-4ec3-b29c-dee2f3d3eca8', NULL, 'GBP', GETUTCDATE(), 435),
                (1081, N'10000-S-008', 2, 1,'60523726-bbaf-4ec3-b29c-dee2f3d3eca8', NULL, 'GBP', GETUTCDATE(), 75),
      --per migration
                (1082, N'10000-S-069', 2, 1,'59fa7cad-87b8-4e78-92b7-5689f6b123dc', NULL, 'GBP', GETUTCDATE(), 1500),
                (1083, N'10052-S-008', 2, 1,'59fa7cad-87b8-4e78-92b7-5689f6b123dc', NULL, 'GBP', GETUTCDATE(), 2927.93),
                (1084, N'10052-S-013', 2, 1,'59fa7cad-87b8-4e78-92b7-5689f6b123dc', NULL, 'GBP', GETUTCDATE(), 4826.88),
      --per practice
                (1085, N'10004-S-004', 2, 1,'aad2820e-472d-4bac-864e-853f92e9b3bc', NULL, 'GBP', GETUTCDATE(), 1300),
                (1086, N'10007-S-003', 2, 1,'aad2820e-472d-4bac-864e-853f92e9b3bc', NULL, 'GBP', GETUTCDATE(), 400),
                (1087, N'10047-S-001', 2, 1,'aad2820e-472d-4bac-864e-853f92e9b3bc', NULL, 'GBP', GETUTCDATE(), 2300),
                (1110, N'10072-S-005', 2, 1,'aad2820e-472d-4bac-864e-853f92e9b3bc', NULL, 'GBP', GETUTCDATE(), 150),
                (1111, N'10072-S-010', 2, 1,'aad2820e-472d-4bac-864e-853f92e9b3bc', NULL, 'GBP', GETUTCDATE(), 400),
                (1112, N'10072-S-011', 2, 1,'aad2820e-472d-4bac-864e-853f92e9b3bc', NULL, 'GBP', GETUTCDATE(), 600),
                (1113, N'10072-S-012', 2, 1,'aad2820e-472d-4bac-864e-853f92e9b3bc', NULL, 'GBP', GETUTCDATE(), 200),
                (1114, N'10072-S-013', 2, 1,'aad2820e-472d-4bac-864e-853f92e9b3bc', NULL, 'GBP', GETUTCDATE(), 500),
      --per practice merge/split
                (1088, N'10000-S-040', 2, 1,'f2bb1b9d-b546-40b3-bfd9-d55221d96880', NULL, 'GBP', GETUTCDATE(), 1500),
      --per seminar
                (1089, N'10004-S-003', 2, 1,'626b43e6-c9a0-4ede-99ed-da3a1ad2d8d3', NULL, 'GBP', GETUTCDATE(), 850),
      --per training environment
                (1090, N'10052-S-005', 2, 1,'1d40c0d1-6bd5-40b3-ba2f-cf433f339787', NULL, 'GBP', GETUTCDATE(), 953.03),
      --per unit merge
                (1091, N'10052-S-009', 2, 1,'a4012e6c-caf3-430c-b8d3-9c45ab9fd0de', NULL, 'GBP', GETUTCDATE(), 2089.16),
      --per unit split
                (1092, N'10052-S-010', 2, 1,'bede8599-7a4e-4753-a928-f419681b7c93', NULL, 'GBP', GETUTCDATE(), 2089.16),
      --per user
                (1093, N'10046-S-008', 2, 1,'4b9a4640-a97a-4e30-8ed5-cccae9829616', NULL, 'GBP', GETUTCDATE(), 40);

    SET IDENTITY_INSERT dbo.CataloguePrice ON; 

    MERGE INTO dbo.CataloguePrice AS TARGET
    USING #CataloguePrice AS SOURCE
    ON TARGET.CataloguePriceId = SOURCE.CataloguePriceId
    WHEN MATCHED THEN
           UPDATE SET TARGET.CatalogueItemId = SOURCE.CatalogueItemId,
                      TARGET.ProvisioningTypeId = SOURCE.ProvisioningTypeId,
                      TARGET.CataloguePriceTypeId = SOURCE.CataloguePriceTypeId,
                      TARGET.PricingUnitId = SOURCE.PricingUnitId,
                      TARGET.CurrencyCode = SOURCE.CurrencyCode,
                      TARGET.LastUpdated = SOURCE.LastUpdated
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (CataloguePriceId, CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price)
        VALUES (SOURCE.CataloguePriceId, SOURCE.CatalogueItemId, SOURCE.ProvisioningTypeId, SOURCE.CataloguePriceTypeId, SOURCE.PricingUnitId, SOURCE.TimeUnitId, SOURCE.CurrencyCode, SOURCE.LastUpdated, SOURCE.Price);

    SET IDENTITY_INSERT dbo.CataloguePrice OFF;
END;
GO
GO

