DECLARE @capabilities AS TABLE
(
    Id int NOT NULL PRIMARY KEY,
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

INSERT INTO @capabilities(Id, [Name], [Description], SourceUrl, IsFoundation)
VALUES
(1, 'Appointments Management – Citizen', 'Enables Citizens to manage their Appointments online. Supports the use of Appointment slots that have been configured in Appointments Management – GP.', @gpitFuturesBaseUrl + '1391134205/Appointments+Management+-+Citize', 0),
(2, 'Communicate With Practice – Citizen', 'Supports secure and trusted electronic communications between Citizens and the Practice. Integrates with Patient Information Maintenance.', @gpitFuturesBaseUrl + '1391134188/Communicate+With+Practice+-+Citize', 0),
(3, 'Prescription Ordering – Citizen', 'Enables Citizens to request medication online and manage nominated and preferred Pharmacies for Patients.', @gpitFuturesBaseUrl + '1391134214/Prescription+Ordering+-+Citizen', 0),
(4, 'View Record – Citizen', 'Enables Citizens to view their Patient Record online.', @gpitFuturesBaseUrl + '1391134197/View+Record+-+Citize', 0),
(5, 'Appointments Management – GP', 'Supports the administration, scheduling, resourcing and reporting of appointments.', @gpitFuturesBaseUrl + '1391134029/Appointments+Management+-+GP', 1),
(6, 'Clinical Decision Support', 'Supports clinical decision-making to improve Patient safety at the point of care.', @gpitFuturesBaseUrl + '1391134150/Clinical+Decision+Support', 0),
(7, 'Communication Management', 'Supports the delivery and management of communications to Citizens and Practice staff.', @gpitFuturesBaseUrl + '1391134087/Communication+Management', 0),
(8, 'Digital Diagnostics', 'Supports electronic requesting with other healthcare organisations. Test results can be received, reviewed and stored against the Patient record.', @gpitFuturesBaseUrl + '1391133770/Digital+Diagnostics', 0),
(9, 'Document Management', 'Supports the secure management and classification of all forms unstructured electronic documents including those created by scanning paper documents. Also enables processing of documents and matching documents with Patients.', @gpitFuturesBaseUrl + '1391134166/Document+Management', 0),
(10, 'GP Extracts Verification', 'Supports Practice staff in ensuring accuracy of the data that is used with the Calculating Quality Reporting Service (CQRS).', @gpitFuturesBaseUrl + '1391133797/GP+Extracts+Verificatio', 0),
(11, 'Referral Management', 'Supports recording, reviewing, sending, and reporting of Patient Referrals. Enables Referral information to be included in the Patient Record.', @gpitFuturesBaseUrl + '1391133614/Referral+Management', 1),
(12, 'Resource Management', 'Supports the management and reporting of Practice information, resources, Staff Members and related organisations. Also enables management of Staff Member availability and inactivity.', @gpitFuturesBaseUrl + '1391133939/Resource+Management', 1),
(13, 'Patient Information Maintenance', 'Supports the registration of Patients and the maintenance of all Patient personal information. Supports the organisation and presentation of a comprehensive Patient Record. Also supports the management of related persons and configuring access to Citizen Services.', @gpitFuturesBaseUrl + '1391134180/Patient+Information+Maintenance', 1),
(14, 'Prescribing', 'Supports the effective and safe prescribing of medical products and appliances to Patients. Information to support prescribing will be available.', @gpitFuturesBaseUrl + '1391134158/Prescribing', 1),
(15, 'Recording Consultations', 'Supports the standardised recording of Consultations and other General Practice activities. Also supports the extraction of Female Genital Mutilation (FGM) data for the FGM data set.', @gpitFuturesBaseUrl + '1391134389/Recording+Consultations', 1),
(16, 'Reporting', 'Enables reporting and analysis of data from other Capabilities in the Practice Solution to support clinical care and Practice management.', @gpitFuturesBaseUrl + '1391133718/Reporting', 0),
(17, 'Scanning', 'Support the con[Version] of paper documentation into digital format preserving the document quality and structure.', @gpitFuturesBaseUrl + '1391134270/Scanning', 0),
(18, 'Telehealth', 'Enables Citizens and Patients that use health monitoring solutions to share monitoring data with health and care professionals to support remote delivery of care and increase self-care outside of clinical settings.', @gpitFuturesBaseUrl + '1391134248/Telehealth', 0),
(19, 'Unstructured Data Extraction', 'Enables automated and manual interpretation and extraction of structured data from paper documents and unstructured electronic documents to support their classification and matching with Patient Records.', @gpitFuturesBaseUrl + '1391133668/Unstructured+Data+Extractio', 0),
(20, 'Workflow', 'Supports manual and automated management of work in the Practice. Also supports effective planning, tracking, monitoring and reporting.', @gpitFuturesBaseUrl + '1391134020/Workflow', 0),
(21, 'Care Homes', 'Enables a record of the Resident''s health and care needs to be maintained and shared with parties who are involved in providing care, to support decision making and the effective planning and delivery of care.', @gpitFuturesBaseUrl + '1391133439/Care+Homes', 0),
(22, 'Caseload Management', 'Supports the allocation of appropriate Health and Care Professionals to Patients/Service Users in need of support, ensuring balanced workloads and the efficient use of staff and other resources.', @gpitFuturesBaseUrl + '1391133457/Caseload+Management', 0),
(23, 'Cross-organisation Appointment Booking', 'Enables appointments to be made available and booked across Organisational boundaries, creating flexibility for Health and Care Professionals and Patients/Service Users.', @gpitFuturesBaseUrl + '1391135407/Cross-organisation+Appointment+Booking', 0),
(24, 'Cross-organisation Workflow Tools', 'Supports and automates clinical and business processes across Organisational boundaries to make processes and communication more efficient.', @gpitFuturesBaseUrl + '1391133492/Cross-organisation+Workflow+Tools', 0),
(25, 'Cross-organisation Workforce Management', 'Supports the efficient planning and scheduling of the health and care workforce to ensure that services can be delivered effectively by the right staff.', @gpitFuturesBaseUrl + '1391135659/Cross-organisation+Workforce+Management', 0),
(26, 'Data Analytics for Integrated and Federated Care', 'Supports the analysis of multiple and complex datasets and presentation of the output to enable decision-making, service design and performance management.', @gpitFuturesBaseUrl + '1391135590/Data+Analytics+for+Integrated+and+Federated+Care', 0),
(27, 'Domiciliary Care', 'Enables Service Providers to effectively plan and manage Domiciliary Care services to ensure care needs are met and that Care Workers can manage their schedule.', @gpitFuturesBaseUrl + '1391133451/Domiciliary+Care', 0),
(29, 'e-Consultations (Professional to Professional)', 'Enables the communication and sharing of specialist knowledge and advice between Health and Care Professionals to support better care decisions and professional development.', @gpitFuturesBaseUrl + '1391135495/e-Consultations+Professional+to+Professional', 0),
(30, 'Medicines Optimisation', 'Supports clinicians and pharmacists in reviewing a Patient''s medication and requesting changes to medication to ensure the Patient is taking the best combination of medicines.', @gpitFuturesBaseUrl + '1391133405/Medicines+Optimisatio', 0),
(32, 'Personal Health Budget', 'Enables a Patient/Service User to set up and manage a Personal Health Budget giving them more choice and control over the management of their identified healthcare and well-being needs.', @gpitFuturesBaseUrl + '1391133426/Personal+Health+Budget', 0),
(33, 'Personal Health Record', 'Enables a Patient/Service User to manage and maintain their own Electronic Health Record and to share that information with relevant Health and Care Professionals.', @gpitFuturesBaseUrl + '1391135480/Personal+Health+Record', 0),
(34, 'Population Health Management', 'Enables Organisations to accumulate, analyse and report on Patient healthcare data to identify improvement in care and identify and track Patient outcomes.', @gpitFuturesBaseUrl + '1391135469/Population+Health+Management', 0),
(35, 'Risk Stratification', 'Supports Health and Care Professionals by providing trusted models to predict future Patient events, informing interventions to achieve better Patient outcomes.', @gpitFuturesBaseUrl + '1391133445/Risk+Stratificatio', 0),
(36, 'Shared Care Plans', 'Enables the maintenance of a single, shared care plan across multiple Organisations to ensure more co-ordinated working and more efficient management of activities relating to the Patient/Service User''s health and care.', @gpitFuturesBaseUrl + '1391134486/Shared+Care+Plans', 0),
(37, 'Social Prescribing', 'Supports the referral of Patients/Service Users to non-clinical services to help address their health and well-being needs.', @gpitFuturesBaseUrl + '1391135572/Social+Prescribing', 0),
(38, 'Telecare', 'Supports the monitoring of Patients/Service Users or their environment to ensure quick identification and response to any adverse event.', @gpitFuturesBaseUrl + '1391135549/Telecare', 0),
(39, 'Unified Care Record', 'Provides a consolidated view to Health and Care Professionals of a Patient/Service User''s complete and up-to-date records, sourced from various health and care settings.', @gpitFuturesBaseUrl + '1391134504/Unified+Care+Record', 0),
(40, 'Medicines Verification', 'Supports compliance with the Falsified Medicines Directive and minimise the risk that falsified medicinal products are supplied to the public.', @gpitFuturesBaseUrl + '1391135093/Medicines+Verificatio', 0),
(41, 'Productivity', 'Supports Patients/Service Users and Health and Care Professionals by delivering improved efficiency or experience related outcomes.', @gpitFuturesBaseUrl + '1391135618/Productivity', 0),
(42, 'Dispensing', 'Supports the timely and effective dispensing of medical products and appliances to Patients.', @gpitFuturesBaseUrl + '1391133465/Dispensing', 0);

DECLARE @covidVaccinationBaseUrl AS char(55) = 'https://gpitbjss.atlassian.net/wiki/spaces/CVPDR/pages/';

INSERT INTO @capabilities(Id, [Name], [Description], SourceUrl, [Version], EffectiveDate, CategoryId)
VALUES
(45, 'Cohort Identification', 'The Cohort Identification Capability enables the identification of Patient cohorts by identifying Patients that require a COVID-19 vaccination based on nationally defined criteria.', @covidVaccinationBaseUrl + '7918551305/Cohort+Identification', '1.0.2', '2021-01-25', 2),
(46, 'Appointments Management – COVID-19 Vaccinations', 'The Appointments Management – COVID-19 Vaccinations Capability enables the administration and scheduling of COVID-19 vaccination appointments for Patients.', @covidVaccinationBaseUrl + '7918551324/Appointments+Management+-+COVID-19+Vaccinations', '1.0.0', '2020-12-09', 2),
(47, 'Vaccination and Adverse Reaction Recording', 'The Vaccination and Adverse Reaction Recording Capability enables the recording of COVID-19 vaccination and adverse reaction data at the point of care. The Capability also supports the delivery of this data to the Patient’s registered GP Practice Foundation Solution and to NHS Digital.', @covidVaccinationBaseUrl + '7918551342/Vaccination+and+Adverse+Reaction+Recording', '4.0.0', '2021-03-29', 2);

INSERT INTO @capabilities(Id, [Name], [Description], SourceUrl, [Version], EffectiveDate, CategoryId, FrameworkId)
VALUES
(43, 'Online Consultation', 'The Online Consultation Capability allows Patients/Service Users/Proxies to request and receive support relating to healthcare concerns, at a time and place convenient for them.', 'https://gpitbjss.atlassian.net/wiki/spaces/DFOCVCL/pages/5132484695/Online+Consultation', '1.0.1', '2021-03-11', 3, 'DFOCVC001'),
(44, 'Video Consultation', 'The Video Consultation Capability allows Health or Care Professionals to conduct secure live remote video consultations with individual or groups of Patients/Service Users/Proxies ensuring they can receive support relating to healthcare concerns when a Video Consultation is most appropriate.', 'https://gpitbjss.atlassian.net/wiki/spaces/DFOCVCL/pages/5132484947/Video+Consultation', '1.0.0', '2021-03-11', 3, 'DFOCVC001');

-- The code below should not need to be changed

MERGE INTO catalogue.Capabilities AS TARGET
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
    INSERT (Id, [Version], StatusId, [Name], [Description], SourceUrl, EffectiveDate, CategoryId)
    VALUES (SOURCE.Id, SOURCE.[Version], 1, SOURCE.[Name], SOURCE.[Description], SOURCE.SourceUrl, SOURCE.EffectiveDate, SOURCE.CategoryId);

MERGE INTO catalogue.FrameworkCapabilities AS TARGET
     USING @capabilities AS SOURCE
        ON TARGET.CapabilityId = SOURCE.Id
      WHEN MATCHED THEN
UPDATE SET TARGET.IsFoundation = SOURCE.IsFoundation
      WHEN NOT MATCHED BY TARGET THEN
    INSERT (FrameworkId, CapabilityId, IsFoundation)
    VALUES (SOURCE.FrameworkId, SOURCE.Id, SOURCE.IsFoundation);
GO
