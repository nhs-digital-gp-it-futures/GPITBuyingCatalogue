IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
BEGIN

    DECLARE @capabilities AS TABLE
    (
        Id int NOT NULL PRIMARY KEY,
        [Name] nvarchar(255) NOT NULL,
        [Description] nvarchar(500) NOT NULL,
        SourceUrl nvarchar(1000) NOT NULL,
        [Version] nvarchar(10) DEFAULT '1.0.1' NULL,
        [StatusId] int NOT NULL,
        EffectiveDate date DEFAULT '2019-12-31' NOT NULL,
        CategoryId int DEFAULT 0 NOT NULL
    )

    DECLARE @frameworkCapabilities AS TABLE
    (
         FrameworkId nvarchar(36) NOT NULL,
         CapabilityId int NOT NULL
    )

    INSERT INTO @capabilities(Id, [Name], [Description], [SourceUrl], [CategoryId], [Version], [StatusId])
    VALUES
    (1, 'Appointments Management - Citizen', 'Enables Citizens to manage their Appointments online. Supports the use of Appointment Slots that have been configured in Appointments Management - GP.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134205/Appointments+Management+-+Citizen', 3, '1.0.2', 1),
    (2, 'Communicate With Practice - Citizen', 'Supports secure and trusted electronic communications between Patients and the Healthcare Organisation. Integrates with Patient Information Maintenance - GP Solution.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134188/Communicate+With+Practice+-+Citizen', 3, '1.0.2', 1),
    (3, 'Prescription Ordering - Citizen', 'Enables Patients to request for a Repeat medication recorded in Prescribing to be issued online.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134214/Prescription+Ordering+-+Citizen', 3, '1.0.2', 1),
    (4, 'View Record - Citizen', 'Enables Citizens to view content from their Electronic Patient Record (EPR) online. Integrates with Patient Information Maintenance - GP Solution.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134197/View+Record+-+Citizen', 3, '1.0.2', 1),
    (5, 'Appointments Management - GP', 'Supports the administration, scheduling, resourcing and viewing of Appointments and Appointment availability. Also supports submission of data to NHS Digital for monitoring, planning and research purposes.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134029/Appointments+Management+-+GP', 1, '1.1.1', 1),
    (6, 'Clinical Decision Support', 'Supports clinical decision-making to improve Patient safety at the point of care.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134150/Clinical+Decision+Support', 5, '1.0.1', 1),
    (7, 'Communication Management', 'Supports the delivery of communications to recipients.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134087/Communication+Management', 2, '1.0.2', 1),
    (8, 'Digital Diagnostics', 'Supports the recording of Requests for Investigations information for Patients. Investigation results can also be received, reviewed and stored against the Electronic Patient Record (EPR).', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133770/Digital+Diagnostics', 2, '1.0.2', 1),
    (9, 'Document Management', 'Supports the secure management of all forms of unstructured electronic documents including those created by scanning paper documents. Also enables processing of documents and matching documents with Patients/Service Users.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134166/Document+Management', 6, '1.0.2', 1),
    (10, 'GP Extracts Verification', 'Supports Health or Care Professionals to ensure the accuracy of the data extracted from Electronic Patient Records (EPR) via the General Practice Extraction Service (GPES) and sent to the Calculating Quality Reporting Service (CQRS), both operated by NHS Digital. ', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133797/GP+Extracts+Verification', 10, '1.0.2', 1),
    (11, 'Referral Management - GP', 'Supports the creation of Referral information for Patients and sending referral requests to the e-Referrals Service (e-RS).', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133614/Referral+Management+-+GP', 2, '1.0.2', 1),
    (12, 'Resource Management', 'Supports the management of Health or Care Organisation site information including management of Staff Members at Health or Care Organisations.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133939/Resource+Management', 7, '1.0.2', 1),
    (13, 'Patient Information Maintenance - GP', 'Supports the registration of Patients and the maintenance of all Patient personal information including demographics. Supports the organisation and presentation of a comprehensive Electronic Patient Record (EPR) and enables other organisations to access this information. Also supports configuring access to Citizen Services and the submission of data to NHS Digital for monitoring, research and payment purposes.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134180/Patient+Information+Maintenance+-+GP', 9, '5.0.2', 1),
    (14, 'Prescribing', 'Supports the effective and safe prescribing of medicinal products and appliances to Patients. Information relating to prescribable items will be available at the point of prescribing.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134158/Prescribing', 8, '1.0.3', 1),
    (15, 'Recording Consultations - GP', 'Supports the standardised recording of Consultations and other General Practice activities. Also supports eMED3 (fit notes), the extraction of Female Genital Mutilation (FGM), and submission of Adverse Drug Reactions (ADRs) for the Yellow Card scheme.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134389/Recording+Consultations+-+GP', 5, '1.0.2', 1),
    (16, 'Reporting', 'Enables reporting and analysis using a range of data items to support clinical care and Health or Care Organisation management.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133718/Reporting', 10, '1.0.2', 1),
    (17, 'Scanning', 'Supports the conversion of paper documentation into digital format.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134270/Scanning', 6, '1.0.2', 1),
    (18, 'Telehealth', 'Enables�Citizens and Patients�that use health monitoring solutions to share monitoring data with health and care professionals to support remote delivery of care and increase self-care outside of clinical settings.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134248/Telehealth', 3, '1.0.1', 1),
    (19, 'Unstructured Data Extraction', 'Enables automated and manual interpretation and extraction of structured data from paper documents and unstructured electronic documents to support their classification and matching with Patient Records.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133668/Unstructured+Data+Extractio', 6, '1.0.1', 0),
    (20, 'Custom Workflows', 'Supports the creation of custom Workflows for the management of activities within the Health or Care Organisation.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134020/Workflow', 7, '1.0.2', 1),
    (21, 'Care Homes', 'Enables a record of the Resident�s health and care needs to be maintained and shared with parties who are involved in providing care, to support decision making and the effective planning and delivery of care.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133439/Care+Homes', 4, '1.0.1', 1),
    (22, 'Caseload Management', 'Supports the allocation of appropriate Health and Care Professionals to Patients/Service Users in need of support, ensuring balanced workloads and the efficient use of staff and other resources.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133457/Caseload+Management', 7, '1.0.1', 1),
    (23, 'Cross-organisation Appointment Booking', 'Enables Appointments for Patients/Service Users to be booked by Health or Care Professionals across organisational boundaries.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391135407/Cross-organisation+Appointment+Booking', 1, '1.0.2', 1),
    (24, 'Cross-organisation Workflow Tools', 'Supports and automates clinical and business processes across Organisational boundaries to make processes and communication more efficient.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133492/Cross-organisation+Workflow+Tools', 7, '1.0.1', 1),
    (25, 'Cross-organisation Workforce Management', 'Supports the efficient planning and scheduling of the health and care workforce to ensure that services can be delivered effectively by the right staff.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391135659/Cross-organisation+Workforce+Management', 7, '1.0.1', 1),
    (26, 'Data Analytics for Integrated and Federated Care', 'Supports the analysis of multiple and complex datasets and presentation of the output to enable decision-making, service design and performance management.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391135590/Data+Analytics+for+Integrated+and+Federated+Care', 10, '1.0.1', 1),
    (27, 'Domiciliary Care', 'Enables Service Providers to effectively plan and manage Domiciliary Care services to ensure care needs are met and that Care Workers can manage their schedule.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133451/Domiciliary+Care', 4, '1.0.1', 1),
    (29, 'e-Consultations (Professional to Professional)', 'Enables the communication and sharing of specialist knowledge and advice between Health and Care Professionals to support better care decisions and professional development.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391135495/e-Consultations+Professional+to+Professional', 5, '1.0.1', 1),
    (30, 'Medicines Optimisation', 'Supports clinicians and pharmacists in reviewing a Patient''s medication and requesting changes to medication to ensure the Patient is taking the best combination of medicines.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133405/Medicines+Optimisation', 8, '1.0.1', 1),
    (32, 'Personal Health Budget', 'Enables a Patient/Service User to set up and manage a Personal Health Budget giving them more choice and control over the management of their identified healthcare and well-being needs.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133426/Personal+Health+Budget', 3, '1.0.1', 1),
    (33, 'Personal Health Record', 'Enables a Patient/Service User to manage and maintain their own Electronic Health Record and to share that information with relevant Health and Care Professionals.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391135480/Personal+Health+Record', 3, '1.0.1', 1),
    (34, 'Population Health Management', 'Enables Organisations to accumulate, analyse and report on Patient healthcare data to identify improvement in care and identify and track Patient outcomes.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391135469/Population+Health+Management', 10, '1.0.1', 1),
    (35, 'Risk Stratification', 'Supports Health and Care Professionals by providing trusted models to predict future Patient events, informing interventions to achieve better Patient outcomes.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133445/Risk+Stratification', 10, '1.0.1', 1),
    (36, 'Shared Care Plans', 'Enables the maintenance of a single, shared care plan across multiple Organisations to ensure more co-ordinated working and more efficient management of activities relating to the Patient/Service User''s health and care.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134486/Shared+Care+Plans', 9, '1.0.1', 1),
    (37, 'Social Prescribing', 'Supports the referral of Patients/Service Users to non-clinical services to help address their health and well-being needs.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391135572/Social+Prescribing', 4, '1.0.1', 1),
    (38, 'Telecare', 'Supports the monitoring of Patients/Service Users or their environment to ensure quick identification and response to any adverse event.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391135549/Telecare', 4, '1.0.1', 1),
    (39, 'Unified Care Record', 'Provides a consolidated view to Health and Care Professionals of a Patient/Service User''s complete and up-to-date records, sourced from various health and care settings.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134504/Unified+Care+Record', 9, '1.0.1', 1),
    (40, 'Medicines Verification', 'Supports compliance with the Falsified Medicines Directive and minimise the risk that falsified medicinal products are supplied to the public.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391135093/Medicines+Verificatio', 8, '1.0.1', 0),
    (41, 'Productivity', 'Supports Patients/Service Users and Health and Care Professionals by delivering improved efficiency or experience related outcomes.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391135618/Productivity', 11, '1.0.1', 1),
    (42, 'Dispensing', 'Supports the timely and effective dispensing of medical products and appliances to Patients.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133465/Dispensing', 8, '1.0.1', 1),
    (43, 'Online Consultation', 'The Online Consultation Capability allows Patients/Service Users/Proxies to request and receive support relating to healthcare concerns, at a time and place convenient for them.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/5132484695/Online+Consultation', 5, '1.0.1', 1),
    (44, 'Video Consultation', 'The Video Consultation Capability allows Health or Care Professionals to conduct secure live remote video consultations with individual or groups of Patients/Service Users/Proxies ensuring they can receive support relating to healthcare concerns when a Video Consultation is most appropriate.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/5132484947/Video+Consultation', 5, '1.0.0', 1),
    (45, 'Cohort Identification', 'The Cohort Identification Capability enables the identification of Patient cohorts by identifying Patients that require a COVID-19 vaccination based on nationally defined criteria.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/7918551305/Cohort+Identification', 2, '1.0.2', 1),
    (46, 'PCN Appointments Management - Vaccinations', 'The Appointments Management - COVID-19 Vaccinations Capability enables the administration and scheduling of COVID-19 vaccination appointments for Patients.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/7918551324/PCN+Appointments+Management+-+Vaccinations', 1, '1.0.1', 1),
    (47, 'Vaccination and Adverse Reaction Recording', 'The Vaccination and Adverse Reaction Recording Capability enables the recording of COVID-19 vaccination and adverse reaction data at the point of care. The Capability also supports the delivery of this data to the Patient�s registered GP Practice Foundation Solution and to NHS Digital.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/7918551342/Vaccination+and+Adverse+Reaction+Recording', 5, '4.0.1', 1),
    (50, 'Task Management', 'Supports the management of Tasks relating to activities within a Health or Care Organisation.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/12417368065/Task+Management', 7, '1.0.0', 1)

    INSERT INTO @frameworkCapabilities(FrameworkId, CapabilityId)
    VALUES
    ('DFOCVC001', 43), -- Online Consultation
    ('DFOCVC001', 44), -- Video Consultation
    ('NHSDGP001', 1), -- Appointments Management - Citizen
    ('NHSDGP001', 2), -- Communicate With Practice - Citizen
    ('NHSDGP001', 3), -- Prescription Ordering - Citizen
    ('NHSDGP001', 4), -- View Record - Citizen
    ('NHSDGP001', 5), -- Appointments Management - GP
    ('NHSDGP001', 6), -- Clinical Decision Support
    ('NHSDGP001', 7), -- Communication Management
    ('NHSDGP001', 8), -- Digital Diagnostics
    ('NHSDGP001', 9), -- Document Management
    ('NHSDGP001', 10), -- GP Extracts Verification
    ('NHSDGP001', 11), -- Referral Management - GP
    ('NHSDGP001', 12), -- Resource Management
    ('NHSDGP001', 13), -- Patient Information Maintenance - GP
    ('NHSDGP001', 14), -- Prescribing
    ('NHSDGP001', 15), -- Recording Consultations - GP
    ('NHSDGP001', 16), -- Reporting
    ('NHSDGP001', 17), -- Scanning
    ('NHSDGP001', 18), -- Telehealth
    ('NHSDGP001', 19), -- Unstructured Data Extraction
    ('NHSDGP001', 20), -- Custom Workflows
    ('NHSDGP001', 21), -- Care Homes
    ('NHSDGP001', 22), -- Caseload Management
    ('NHSDGP001', 23), -- Cross-organisation Appointment Booking
    ('NHSDGP001', 24), -- Cross-organisation Workflow Tools
    ('NHSDGP001', 25), -- Cross-organisation Workforce Management
    ('NHSDGP001', 26), -- Data Analytics for Integrated and Federated Care
    ('NHSDGP001', 27), -- Domiciliary Care
    ('NHSDGP001', 29), -- e-Consultations (Professional to Professional)
    ('NHSDGP001', 30), -- Medicines Optimisation
    ('NHSDGP001', 32), -- Personal Health Budget
    ('NHSDGP001', 33), -- Personal Health Record
    ('NHSDGP001', 34), -- Population Health Management
    ('NHSDGP001', 35), -- Risk Stratification
    ('NHSDGP001', 36), -- Shared Care Plans
    ('NHSDGP001', 37), -- Social Prescribing
    ('NHSDGP001', 38), -- Telecare
    ('NHSDGP001', 39), -- Unified Care Record
    ('NHSDGP001', 40), -- Medicines Verification
    ('NHSDGP001', 41), -- Productivity
    ('NHSDGP001', 42), -- Dispensing
    ('NHSDGP001', 50), -- Task Management
    ('TIF001', 1), -- Appointments Management - Citizen
    ('TIF001', 2), -- Communicate With Practice - Citizen
    ('TIF001', 3), -- Prescription Ordering - Citizen
    ('TIF001', 4), -- View Record - Citizen
    ('TIF001', 5), -- Appointments Management - GP
    ('TIF001', 6), -- Clinical Decision Support
    ('TIF001', 7), -- Communication Management
    ('TIF001', 8), -- Digital Diagnostics
    ('TIF001', 9), -- Document Management
    ('TIF001', 10), -- GP Extracts Verification
    ('TIF001', 11), -- Referral Management - GP
    ('TIF001', 12), -- Resource Management
    ('TIF001', 13), -- Patient Information Maintenance - GP
    ('TIF001', 14), -- Prescribing
    ('TIF001', 15), -- Recording Consultations - GP
    ('TIF001', 16), -- Reporting
    ('TIF001', 17), -- Scanning
    ('TIF001', 18), -- Telehealth
    ('TIF001', 20), -- Custom Workflows
    ('TIF001', 21), -- Care Homes
    ('TIF001', 22), -- Caseload Management
    ('TIF001', 23), -- Cross-organisation Appointment Booking
    ('TIF001', 24), -- Cross-organisation Workflow Tools
    ('TIF001', 25), -- Cross-organisation Workforce Management
    ('TIF001', 26), -- Data Analytics for Integrated and Federated Care
    ('TIF001', 27), -- Domiciliary Care
    ('TIF001', 29), -- e-Consultations (Professional to Professional)
    ('TIF001', 30), -- Medicines Optimisation
    ('TIF001', 32), -- Personal Health Budget
    ('TIF001', 33), -- Personal Health Record
    ('TIF001', 34), -- Population Health Management
    ('TIF001', 35), -- Risk Stratification
    ('TIF001', 36), -- Shared Care Plans
    ('TIF001', 37), -- Social Prescribing
    ('TIF001', 38), -- Telecare
    ('TIF001', 39), -- Unified Care Record
    ('TIF001', 42), -- Dispensing
    ('TIF001', 50) -- Task Management

    MERGE INTO catalogue.Capabilities AS TARGET
    USING @capabilities AS SOURCE
            ON TARGET.Id = SOURCE.Id
    WHEN MATCHED THEN
        UPDATE SET TARGET.[Version] = SOURCE.[Version],
               TARGET.[Name] = SOURCE.[Name],
               TARGET.[Description] = SOURCE.[Description],
               TARGET.[SourceUrl] = SOURCE.[SourceUrl],
               TARGET.[EffectiveDate] = SOURCE.[EffectiveDate],
               TARGET.[CategoryId] = SOURCE.[CategoryId],
               TARGET.[StatusId] = SOURCE.[StatusId]
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Id, [Version], [StatusId], [Name], [Description], [SourceUrl], [EffectiveDate], [CategoryId])
        VALUES (SOURCE.Id, SOURCE.[Version], SOURCE.[StatusId], SOURCE.[Name], SOURCE.[Description], SOURCE.[SourceUrl], SOURCE.[EffectiveDate], SOURCE.[CategoryId]);

    MERGE INTO catalogue.FrameworkCapabilities AS TARGET
    USING @frameworkcapabilities AS SOURCE
            ON TARGET.CapabilityId = SOURCE.CapabilityId AND TARGET.FrameworkId = SOURCE.FrameworkId 
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (FrameworkId, CapabilityId)
        VALUES (SOURCE.FrameworkId, SOURCE.CapabilityId)
    WHEN NOT MATCHED BY SOURCE THEN
        DELETE;

END
