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
    CategoryId int DEFAULT 1 NOT NULL
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
('C332947A-D29E-4169-A7B1-FF277CF513C2', 'C28', 'e-Consultations (Patient/Service User to Professional)', 'Enables Patients/Service Users to access support from Health and Care Professionals, across a range of settings, without the need for a face to face encounter.', @gpitFuturesBaseUrl + '1391133433/e-Consultations+Patient+Service+User+to+Professional', 0),
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
('151CA7DF-5227-4EBF-9200-93258EFA3735', 'C45', 'Cohort Identification', 'The Cohort Identification Capability enables the identification of Patient cohorts by identifying Patients that require a COVID-19 vaccination based on nationally defined criteria.', @covidVaccinationBaseUrl + '7918551305/Cohort+Identification', '1.0.0', '2020-12-09', 2),
('53255CB2-C5F6-427B-A18A-D3055E310FD6', 'C46', 'Appointments Management – COVID-19 Vaccinations', 'The Appointments Management – COVID-19 Vaccinations Capability enables the administration and scheduling of COVID-19 vaccination appointments for Patients.', @covidVaccinationBaseUrl + '7918551324/Appointments+Management+-+COVID-19+Vaccinations', '1.0.0', '2020-12-09', 2),
('EC26D316-4F3D-45B1-BDF5-717D24DAB360', 'C47', 'Vaccination and Adverse Reaction Recording', 'The Vaccination and Adverse Reaction Recording Capability enables the recording of COVID-19 vaccination and adverse reaction data at the point of care. The Capability also supports the delivery of this data to the Patient’s registered GP Practice Foundation Solution and to NHS Digital.', @covidVaccinationBaseUrl + '7918551342/Vaccination+and+Adverse+Reaction+Recording', '1.0.0', '2020-12-09', 2);

-- It's unlikely that the code below will need to change unless/until there is more than one framework

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
        ON TARGET.FrameworkId = 'NHSDGP001'
       AND TARGET.CapabilityId = SOURCE.Id
      WHEN MATCHED THEN
UPDATE SET TARGET.IsFoundation = SOURCE.IsFoundation
      WHEN NOT MATCHED BY TARGET THEN
    INSERT (FrameworkId, CapabilityId, IsFoundation)
    VALUES ('NHSDGP001', SOURCE.Id, SOURCE.IsFoundation);
GO
