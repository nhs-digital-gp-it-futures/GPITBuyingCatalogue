DECLARE @capabilities AS TABLE
(
    Id int NOT NULL PRIMARY KEY,
    [Name] nvarchar(255) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    SourceUrl nvarchar(1000) NOT NULL,
    IsFoundation bit DEFAULT 0 NOT NULL,
    [Version] nvarchar(10) DEFAULT '1.0.1' NULL,
    EffectiveDate date DEFAULT '2019-12-31' NOT NULL,
    CategoryId int DEFAULT 0 NOT NULL,
    FrameworkId nvarchar(20) DEFAULT 'NHSDGP001' NOT NULL
);

DECLARE @gpitFuturesBaseUrl AS char(55) = 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/';

INSERT INTO @capabilities(Id, [Name], [Description], SourceUrl, IsFoundation, CategoryId)
VALUES
(1, 'Appointments Management – Citizen', 'Enables citizens to manage their appointments online. Supports the use of appointment slots that have been configured in Appointments Management – GP.', @gpitFuturesBaseUrl + '1391134205/Appointments+Management+-+Citize', 0, 1),
(2, 'Communicate With Practice – Citizen', 'Supports secure and trusted electronic communications between citizens and the practice. Integrates with Patient Information Maintenance.', @gpitFuturesBaseUrl + '1391134188/Communicate+With+Practice+-+Citize', 0, 3),
(3, 'Prescription Ordering – Citizen', 'Enables citizens to request medication online and manage nominated and preferred pharmacies for patients.', @gpitFuturesBaseUrl + '1391134214/Prescription+Ordering+-+Citizen', 0, 3),
(4, 'View Record – Citizen', 'Enables citizens to view their Patient Record online.', @gpitFuturesBaseUrl + '1391134197/View+Record+-+Citize', 0, 3),
(5, 'Appointments Management – GP', 'Supports the administration, scheduling, resourcing and reporting of appointments.', @gpitFuturesBaseUrl + '1391134029/Appointments+Management+-+GP', 1, 1),
(6, 'Clinical Decision Support', 'Supports clinical decision-making to improve patient safety at the point of care.', @gpitFuturesBaseUrl + '1391134150/Clinical+Decision+Support', 0, 5),
(7, 'Communication Management', 'Supports the delivery and management of communications to citizens and practice staff.', @gpitFuturesBaseUrl + '1391134087/Communication+Management', 0, 2),
(8, 'Digital Diagnostics', 'Supports electronic requesting with other healthcare organisations. Test results can be received, reviewed and stored against the Patient Record.', @gpitFuturesBaseUrl + '1391133770/Digital+Diagnostics', 0, 2),
(9, 'Document Management', 'Supports the secure management and classification of all forms unstructured electronic documents including those created by scanning paper documents. Also enables processing of documents and matching documents with patients.', @gpitFuturesBaseUrl + '1391134166/Document+Management', 0, 6),
(10, 'GP Extracts Verification', 'Supports practice staff in ensuring accuracy of the data that is used with the Calculating Quality Reporting Service (CQRS).', @gpitFuturesBaseUrl + '1391133797/GP+Extracts+Verificatio', 0, 10),
(11, 'Referral Management', 'Supports recording, reviewing, sending, and reporting of patient referrals. Enables referral information to be included in the Patient Record.', @gpitFuturesBaseUrl + '1391133614/Referral+Management', 1, 2),
(12, 'Resource Management', 'Supports the management and reporting of practice information, resources, staff members and related organisations. Also enables management of staff member availability and inactivity.', @gpitFuturesBaseUrl + '1391133939/Resource+Management', 1, 7),
(13, 'Patient Information Maintenance', 'Supports the registration of patients and the maintenance of all patient personal information. Supports the organisation and presentation of a comprehensive Patient Record. Also supports the management of related persons and configuring access to Citizen Services.', @gpitFuturesBaseUrl + '1391134180/patient+Information+Maintenance', 1, 9),
(14, 'Prescribing', 'Supports the effective and safe prescribing of medical products and appliances to patients. Information to support prescribing will be available.', @gpitFuturesBaseUrl + '1391134158/Prescribing', 1, 8),
(15, 'Recording Consultations', 'Supports the standardised recording of consultations and other general practice activities. Also supports the extraction of Female Genital Mutilation (FGM) data for the FGM data set.', @gpitFuturesBaseUrl + '1391134389/Recording+Consultations', 1, 5),
(16, 'Reporting', 'Enables reporting and analysis of data from other Capabilities in the practice solution to support clinical care and practice management.', @gpitFuturesBaseUrl + '1391133718/Reporting', 0, 10),
(17, 'Scanning', 'Support the con[Version] of paper documentation into digital format preserving the document quality and structure.', @gpitFuturesBaseUrl + '1391134270/Scanning', 0, 6),
(18, 'Telehealth', 'Enables citizens and patients that use health monitoring solutions to share monitoring data with health and care professionals to support remote delivery of care and increase self-care outside of clinical settings.', @gpitFuturesBaseUrl + '1391134248/Telehealth', 0, 3),
(19, 'Unstructured Data Extraction', 'Enables automated and manual interpretation and extraction of structured data from paper documents and unstructured electronic documents to support their classification and matching with Patient Records.', @gpitFuturesBaseUrl + '1391133668/Unstructured+Data+Extractio', 0, 6),
(20, 'Workflow', 'Supports manual and automated management of work in the practice. Also supports effective planning, tracking, monitoring and reporting.', @gpitFuturesBaseUrl + '1391134020/Workflow', 0, 7),
(21, 'Care Homes', 'Enables a record of the resident''s health and care needs to be maintained and shared with parties who are involved in providing care, to support decision making and the effective planning and delivery of care.', @gpitFuturesBaseUrl + '1391133439/Care+Homes', 0, 4),
(22, 'Caseload Management', 'Supports the allocation of appropriate health and care professionals to patients or service users in need of support, ensuring balanced workloads and the efficient use of staff and other resources.', @gpitFuturesBaseUrl + '1391133457/Caseload+Management', 0, 7),
(23, 'Cross-organisation Appointment Booking', 'Enables appointments to be made available and booked across organisational boundaries, creating flexibility for health and care professionals and patients or service users.', @gpitFuturesBaseUrl + '1391135407/Cross-organisation+Appointment+Booking', 0, 1),
(24, 'Cross-organisation Workflow Tools', 'Supports and automates clinical and business processes across organisational boundaries to make processes and communication more efficient.', @gpitFuturesBaseUrl + '1391133492/Cross-organisation+Workflow+Tools', 0, 7),
(25, 'Cross-organisation Workforce Management', 'Supports the efficient planning and scheduling of the health and care workforce to ensure that services can be delivered effectively by the right staff.', @gpitFuturesBaseUrl + '1391135659/Cross-organisation+Workforce+Management', 0, 7),
(26, 'Data Analytics for Integrated and Federated Care', 'Supports the analysis of multiple and complex datasets and presentation of the output to enable decision-making, service design and performance management.', @gpitFuturesBaseUrl + '1391135590/Data+Analytics+for+Integrated+and+Federated+Care', 0, 10),
(27, 'Domiciliary Care', 'Enables service providers to effectively plan and manage Domiciliary Care services to ensure care needs are met and that care workers can manage their schedule.', @gpitFuturesBaseUrl + '1391133451/Domiciliary+Care', 0, 4),
(29, 'e-Consultations (Professional to Professional)', 'Enables the communication and sharing of specialist knowledge and advice between health and care professionals to support better care decisions and professional development.', @gpitFuturesBaseUrl + '1391135495/e-Consultations+Professional+to+Professional', 0, 5),
(30, 'Medicines Optimisation', 'Supports clinicians and pharmacists in reviewing a patient''s medication and requesting changes to medication to ensure the patient is taking the best combination of medicines.', @gpitFuturesBaseUrl + '1391133405/Medicines+Optimisatio', 0, 8),
(32, 'Personal Health Budget', 'Enables a patient or service user to set up and manage a Personal Health Budget giving them more choice and control over the management of their identified healthcare and well-being needs.', @gpitFuturesBaseUrl + '1391133426/Personal+Health+Budget', 0, 3),
(33, 'Personal Health Record', 'Enables a patient or service user to manage and maintain their own Electronic Health Record and to share that information with relevant health and care professionals.', @gpitFuturesBaseUrl + '1391135480/Personal+Health+Record', 0, 3),
(34, 'Population Health Management', 'Enables organisations to accumulate, analyse and report on patient healthcare data to identify improvement in care and identify and track patient outcomes.', @gpitFuturesBaseUrl + '1391135469/Population+Health+Management', 0, 10),
(35, 'Risk Stratification', 'Supports health and care professionals by providing trusted models to predict future patient events, informing interventions to achieve better patient outcomes.', @gpitFuturesBaseUrl + '1391133445/Risk+Stratificatio', 0, 10),
(36, 'Shared Care Plans', 'Enables the maintenance of a single, shared care plan across multiple organisations to ensure more co-ordinated working and more efficient management of activities relating to the patient or service user''s health and care.', @gpitFuturesBaseUrl + '1391134486/Shared+Care+Plans', 0, 9),
(37, 'Social Prescribing', 'Supports the referral of patients or service users to non-clinical services to help address their health and well-being needs.', @gpitFuturesBaseUrl + '1391135572/Social+Prescribing', 0, 4),
(38, 'Telecare', 'Supports the monitoring of patients or service users or their environment to ensure quick identification and response to any adverse event.', @gpitFuturesBaseUrl + '1391135549/Telecare', 0, 4),
(39, 'Unified Care Record', 'Provides a consolidated view to health and care professionals of a patient or service user''s complete and up-to-date records, sourced from various health and care settings.', @gpitFuturesBaseUrl + '1391134504/Unified+Care+Record', 0, 9),
(40, 'Medicines Verification', 'Supports compliance with the Falsified Medicines Directive and minimise the risk that falsified medicinal products are supplied to the public.', @gpitFuturesBaseUrl + '1391135093/Medicines+Verificatio', 0, 8),
(41, 'Productivity', 'Supports patients or service users and health and care professionals by delivering improved efficiency or experience related outcomes.', @gpitFuturesBaseUrl + '1391135618/Productivity', 0, 11),
(42, 'Dispensing', 'Supports the timely and effective dispensing of medical products and appliances to patients.', @gpitFuturesBaseUrl + '1391133465/Dispensing', 0, 8);

DECLARE @covidVaccinationBaseUrl AS char(55) = 'https://gpitbjss.atlassian.net/wiki/spaces/CVPDR/pages/';

INSERT INTO @capabilities(Id, [Name], [Description], SourceUrl, [Version], EffectiveDate, CategoryId)
VALUES
(45, 'Cohort Identification', 'The Cohort Identification Capability enables the identification of patient cohorts by identifying patients that require a COVID-19 vaccination based on nationally defined criteria.', @covidVaccinationBaseUrl + '7918551305/Cohort+Identification', '1.0.2', '2021-01-25', 2),
(46, 'Appointments Management – COVID-19 Vaccinations', 'The Appointments Management – COVID-19 Vaccinations Capability enables the administration and scheduling of COVID-19 vaccination appointments for patients.', @covidVaccinationBaseUrl + '7918551324/Appointments+Management+-+COVID-19+Vaccinations', '1.0.0', '2020-12-09', 1),
(47, 'Vaccination and Adverse Reaction Recording', 'The Vaccination and Adverse Reaction Recording Capability enables the recording of COVID-19 vaccination and adverse reaction data at the point of care. The Capability also supports the delivery of this data to the patient’s registered GP practice Foundation Solution and to NHS Digital.', @covidVaccinationBaseUrl + '7918551342/Vaccination+and+Adverse+Reaction+Recording', '4.0.0', '2021-03-29', 5);

INSERT INTO @capabilities(Id, [Name], [Description], SourceUrl, [Version], EffectiveDate, CategoryId, FrameworkId)
VALUES
(43, 'Online Consultation', 'The Online Consultation Capability allows patients, service users or proxies to request and receive support relating to healthcare concerns, at a time and place convenient for them.', 'https://gpitbjss.atlassian.net/wiki/spaces/DFOCVCL/pages/5132484695/Online+Consultation', '1.0.1', '2021-03-11', 5, 'DFOCVC001'),
(44, 'Video Consultation', 'The Video Consultation Capability allows health and care professionals to conduct secure live remote video consultations with individual or groups of patients or service users or proxies ensuring they can receive support relating to healthcare concerns when a video consultation is most appropriate.', 'https://gpitbjss.atlassian.net/wiki/spaces/DFOCVCL/pages/5132484947/Video+Consultation', '1.0.0', '2021-03-11', 5, 'DFOCVC001');

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
