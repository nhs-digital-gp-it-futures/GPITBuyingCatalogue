IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
BEGIN

    DECLARE @Standards AS TABLE
    (
        [Id] NVARCHAR(5) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(255) NOT NULL,
        [Description] NVARCHAR(500) NOT NULL,
        [Url] NVARCHAR(1000) NOT NULL,
        [StandardTypeId] INT NOT NULL,
        [Version] NVARCHAR(10) NULL,
        [IsDeleted] BIT NOT NULL
    )

    -- Overarching Standards
    INSERT INTO @Standards(Id,[Name],[Description],[Url],[StandardTypeId],[Version],[IsDeleted])
    VALUES
    ('S24', 'Business Continuity & Disaster Recovery', 'Ensures that suppliers solutions are supported by robust business continuity plans and disaster recovery measures.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134058/Business+Continuity+and+Disaster+Recovery', 1, '1.1.1', 0),
    ('S25', 'Clinical Safety', 'Supports the management of clinical risk and Patient safety.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134225/Clinical+Safety', 1, '1.0.1', 0),
    ('S26', 'Data Migration', 'Supports the secure migration of Practice data between solutions provided by different suppliers.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134456/Data+Migration', 1, '2.0.2', 0),
    ('S27', 'Data Standards', 'Defines detailed technical standards for the storage, management and organisation of data and specifies standardised reference data, terminology and codes.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134407/Data+Standards', 1, '2.0.2', 0),
    ('S28', 'Training', 'Defines the training activities and collateral expected from suppliers to support the buyers and users of their solutions.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133846/Training', 1, '1.5.1', 0),
    ('S29', 'Hosting & Infrastructure', 'Supports best practices for infrastructure and hosting of systems. For example, ensuring that systems are cost effective, secure and energy efficient.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134324/Hosting+Infrastructure', 1, '1.1.2', 0),
    ('S30', 'Information Governance', 'Supports the�controls needed to ensure that sensitive personal data is kept confidential, is accurate and is available to authorised users when required.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134230/Information+Governance', 1, '3.0.1', 0),
    ('S31', 'Commercial Standard', 'This Standard underpins all commercial activity relating to the Catalogue. It does this by defining a number of rules governing the commercial relationship of relevant parties and by setting out standards of behaviour and principles of access to data and services charges.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134121/Commercial', 1, '2.1.1', 0),
    ('S63', 'Non-Functional Questions', 'Enables NHS Digital to assess the risk associated with the Compliance Assessment of the Solution against appropriate Overarching Standards.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1569980472/Non-Functional+Questions', 1, '2.1.2', 0),
    ('S65', 'Service Management', 'Supports suppliers in the delivery and management of services that support and provide their Solutions.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133802/Service+Management', 1, '2.0.0', 0),
    ('S69', 'Testing', 'Ensures that�Suppliers'' software delivery test processes are of sufficient quality and rigour.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133583/Testing', 1, '1.0.2', 0),
    ('S81', 'Primary Care Technology Innovation Standard', 'As part of our move to modernise core clinical systems for Primary Care, NHS Digital have been engaging Suppliers around our Modern Technology Standards. These standards and the NHS Architecture Principles lay out the future ways of working and technology we want to see developing to serve the needs of Primary Care and simplify the complexity of healthcare systems provision.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/11969953793/Primary+Care+Technology+Innovation+Standard', 1, '1.0.1', 0)
    
    --Interoperability Standards
    INSERT INTO @Standards(Id,[Name],[Description],[Url],[StandardTypeId],[Version],[IsDeleted])
    VALUES
    ('S32', 'Interoperability Standard', 'Defines a comprehensive set of standards, interfaces and protocols that Solutions and systems will use when interoperating.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133681/Interoperability+Standard', 2, '14.0.2', 0),
    ('S33', '111', 'Supports receipt and consumption of data sent by 111 provider systems for transfer of care to the GP Practice.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133945/111', 2, '1.0.1', 0),
    ('S34', 'Digital Diagnostics & Pathology Messaging', 'Supports the transmission and validation of results data to GP systems where a test request has been placed with a laboratory.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133752/Digital+Diagnostics+Pathology+Messaging', 2, '1.0.1', 0),
    ('S35', 'e-Referrals Service (eRS)', 'Supports electronic booking with a choice of place, date and time for hospital or clinic appointments.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133966/e-Referrals+Service+e-RS', 2, '2.0.1', 0),
    ('S36', 'Electronic Prescription Service (EPS) - Prescribing', 'Supports sending prescriptions electronically to a dispenser (such as a pharmacy) of the Patient''s choice.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133858/Electronic+Prescription+Service+%28EPS%29+-+Prescribing', 2, '3.0.1', 0),
    ('S37', 'Electronic Yellow Card Reporting', 'Supports electronic reporting of suspected adverse drug reactions (ADRs).', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134080/Electronic+Yellow+Card+Reporting', 2, '1.0.0', 0),
    ('S38', 'Email', 'Ensures that email services within solutions are securely configured.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133922/Email', 2, '1.0.0', 0),
    ('S40', 'External Interface Specification (EIS)', 'This standard provides detail on how to connect to NHS Digital Spine Services.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133762/External+Interface+Specification+EIS', 2, '2.0.0', 0),
    ('S43', 'GP2GP', 'Supports the electronic transfer of a Patient Record when a Patient moves from one Practice to another.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134336/GP2GP', 2, '4.0.0', 0),
    ('S44', 'GP Connect', 'Supports sharing of data held within GP IT solutions across health and social care organisations.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133650/GP+Connect', 2, '5.0.1', 0),
    ('S46', 'GPES Data Extraction', 'Supports secure collection of information from General Practices and its delivery to approved organisations.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134238/GPES+Data+Extraction', 2, '1.0.0', 0),
    ('S47', 'IM1 - Interface Mechanism', 'Supports the implementation of interfaces between solutions.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133895/IM1+-+Interface+Mechanism', 2, '2.0.0', 0),
    ('S48', 'Interoperability Toolkit (ITK)', 'Supports interoperability within and between solutions with common specifications, frameworks and implementation guides.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133906/Interoperability+Toolkit+ITK', 2, '1.0.0', 0),
    ('S50', 'Messaging Exchange for Social Care and Health (MESH)', 'Supports the secure transfer of clinical and non clinical data across health and social care.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133887/Messaging+Exchange+for+Social+Care+and+Health+MESH', 2, '1.0.0', 0),
    ('S53', 'NHAIS HA/GP Links', 'Supports the management of Patient registration and demographic information with�National Health Application and Infrastructure Services.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133619/NHAIS+HA+GP+Links', 2, '2.0.1', 0),
    ('S54', 'Authentication and Access', 'Supports user authentication�and access controls to NHS systems and national services.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133913/Authentication+and+Access', 2, '2.0.0', 0),
    ('S55', 'NHS Messaging Implementation Manual (MIM)', 'Supports the messaging standard intended for use on the Spine.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133738/NHS+Messaging+Implementation+Manual+MIM', 2, '1.0.0', 0),
    ('S56', 'Personal Demographics Service (PDS)', 'Supports Solution integration with the Personal Demographics Service which stores Patient details.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133999/Personal+Demographics+Service+PDS', 2, '2.0.0', 0),
    ('S58', 'Screening Messaging', 'Supports validation and transfer of screening result data to solutions.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133789/Screening+messaging', 2, '1.0.1', 0),
    ('S59', 'Secure Electronic File Transfer (SEFT)', 'Supports secure data transfer to and from external organisations.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134107/Secure+Electronic+File+Transfer+SEFT', 2, '1.0.0', 0),
    ('S60', 'Summary Care Record (SCR)', 'Supports the management of Summary Care Record data and staff access to those data.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133699/Summary+Care+Record+SCR', 2, '1.0.0', 0),
    ('S64', 'Clinical Document Architecture (CDA)', 'Defines the standard for exchange of clinical information between systems.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133745/Clinical+Document+Architecture+CDA', 2, '1.0.0', 0),
    ('S66', 'Spine Mini Services', 'Supports read-only access to national services made available through the Spine.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133525/Spine+Mini+Services', 2, '1.0.0', 0),
    ('S68', 'NHS Login Service', 'Supports the verification of Citizens and services requesting access to digital health records.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133874/NHS+login+service', 2, '2.0.0', 0),
    ('S73', 'National Data Opt-Out', 'National Data Opt-Out Specification', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/11931680923/National+Data+Opt-Out', 2, '4.0.1', 0),
    ('S74', 'Interoperability Standard (DFOCVC)', 'Defines a comprehensive set of standards, interfaces and protocols that Solutions and systems will use when interoperating.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/5132484737/Interoperability+Standard+DFOCVC', 2, '1.0.0', 0),
    ('S76', 'Generic FHIR Receiver', 'Supports the receipt of incoming ITK3 FHIR messages', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/10886316215/Generic+FHIR+Receiver', 2, '1.0.0', 0),
    ('S77', 'Digital Medicines and Pharmacy FHIR Payload', 'Supports transmission of medicines information between care settings.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/10752983078/Digital+Medicines+and+Pharmacy+FHIR+Payload', 2, '2.0.1', 0),
    ('S78', 'GP Data for Planning and Research', 'Support the data needs of the health and social care system', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/11235426957/GP+Data+for+Planning+and+Research', 2, '1.0.0', 0),
    ('S79', 'Transfer of Care FHIR Payload', 'Introduction of MVP FHIR messaging for Transfer of Care. Specification v0.7', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/11888099337/Transfer+of+Care+FHIR+Payload', 2, '1.0.0', 0)
    
    --Capability Specific Standards
    INSERT INTO @Standards(Id,[Name],[Description],[Url],[StandardTypeId],[Version],[IsDeleted])
    VALUES
    ('S1', 'Appointments Management - Citizen', 'Enables citizens to manage their appointments online and supports the use of appointment slots that have been configured in Appointments Management - GP.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134369/Appointments+Management+-+Citizen+-+Standard', 3, NULL, 1),
    ('S10', 'GP Extracts Verification', 'Supports GP practice staff in ensuring accuracy of the data that is used with the Calculating Quality Reporting Service (CQRS).', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133808/GP+Extracts+Verification+-+Standard', 3, NULL, 1),
    ('S11', 'Referral Management', 'Supports recording, reviewing, sending, and reporting of patient referrals. Enables referral information to be included in a Patient Record. ', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133630/Referral+Management+-+Standard', 3, NULL, 1),
    ('S12', 'Resource Management', 'Supports the management and reporting of practice information, resources, staff members and related organisations. Also enables management of staff member availability and inactivity.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133828/Resource+Management+-+Standard', 3, NULL, 1),
    ('S13', 'Patient Information Maintenance', 'Supports the registration of patients and the maintenance of all their personal information. Enables the organisation and presentation of a comprehensive Patient Record.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134401/Patient+Information+Maintenance+-+Standard', 3, NULL, 1),
    ('S14', 'Prescribing', 'Supports the effective and safe prescribing of medical products and appliances to patients and makes available information to support prescribing.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134363/Prescribing+-+Standard', 3, NULL, 1),
    ('S15', 'Recording Consultations', 'Supports the standardised recording of consultations and other GP practice activities. Also supports the extraction of Female Genital Mutilation (FGM) data for the FGM data set.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134136/Recording+Consultations+-+Standard', 3, NULL, 1),
    ('S16', 'Reporting', 'Enables reporting and analysis of data from other Capabilities in the GP practice solution to support clinical care and practice management.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134256/Reporting+-+Standard', 3, NULL, 1),
    ('S19', 'Unstructured Data Extraction', 'Enables automated and manual interpretation and extraction of structured data from paper documents and unstructured electronic documents to support their classification and matching with a Patient Record.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134330/Unstructured+Data+Extraction+-+Standard', 3, NULL, 1),
    ('S2', 'Communicate with Practice - Citizen', 'Supports secure and trusted electronic communications between citizens and a GP practice.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134173/Communicate+with+Practice+-+Citizen+-+Standard', 3, NULL, 1),
    ('S20', 'Workflow', 'Supports manual and automated management of work in a GP practice, and enables effective planning, tracking, monitoring and reporting.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134376/Workflow+-+Standard', 3, NULL, 1),
    ('S21', 'Citizen Access', 'Enables citizens to access their services safely and securely and also supports them in viewing and updating patient information online.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133929/Citizen+Access', 3, NULL, 1),
    ('S22', 'Common Reporting', 'Supports the reporting needs common to GP practices and includes searchable report templates.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133675/Common+Reporting', 3, NULL, 1),
    ('S23', 'Management Information (MI) Reporting', 'Supports the submission of aggregated counts of information regarding services, appointments, prescriptions and documents to NHS Digital.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134317/Management+Information+MI+Reporting', 3, NULL, 1),
    ('S3', 'Prescription Ordering - Citizen', 'Enables citizens to request medication online and manage nominated and preferred pharmacies for patients.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134356/Prescription+Ordering+-+Citizen+-+Standard', 3, NULL, 1),
    ('S39', 'eMED3 (Fit Notes)', 'Supports the creation of eMED3 data, its integration into the Patient Record, printing MED3 fit note certificates and extraction of data to NHS Digital.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391135075/eMED3+Fit+Notes', 3, '1.3.1', 0),
    ('S4', 'View Record - Citizen', 'Enables citizens to view their Patient Record online.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134394/View+Record+-+Citizen+-+Standard', 3, NULL, 1),
    ('S5', 'Appointments Management - GP', 'Supports the administration, scheduling, resourcing and reporting of appointments.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134382/Appointments+Management+-+GP+-+Standard', 3, NULL, 1),
    ('S6', 'Clinical Decision Support - Standard', 'Supports clinical decision-making to improve Patient safety at the point of care.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134143/Clinical+Decision+Support+-+Standard', 3, '1.0.1', 0),
    ('S62', 'General Practice Appointments Data Reporting', 'Supports the collection and submission to NHS digital of appointment data to support capacity planning and management.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391133692/General+Practice+Appointments+Data+Reporting', 3, '4.0.0', 0),
    ('S7', 'Communication Management', 'Supports the delivery and management of communications to citizens and GP practice staff.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134296/Communication+Management+-+Standard', 3, NULL, 1),
    ('S70', 'Primary Care Clinical Terminology Usage Report', 'Supports the collection and submission to NHS digital of information about usage of clinical coded terms.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134283/Primary+Care+Clinical+Terminology+Usage+Report', 3, '1.0.0', 0),
    ('S8', 'Digital Diagnostics', 'Supports electronic requesting with other healthcare organisations. Test results can be received, reviewed and stored against a Patient Record.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133783/Digital+Diagnostics+-+Standard', 3, NULL, 1),
    ('S9', 'Document Management', 'Supports the secure management and classification of all forms of unstructured electronic documents including those created by scanning paper documents. Also enables processing of documents and matching documents with patients.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134289/Document+Management+-+Standard', 3, NULL, 1)
    
    --Context Specific
    INSERT INTO @Standards(Id,[Name],[Description],[Url],[StandardTypeId],[Version],[IsDeleted])
    VALUES
    ('S49', 'Management Information (MI) Reporting', 'Supports the submission of aggregated counts of information regarding Citizen Services, appointments, prescriptions and documents to NHS Digital.', 'https://gpitbjss.atlassian.net/wiki/spaces/DCSDCS/pages/1391134317/Management+Information+%28MI%29+Reporting', 4, '1.0.0', 0)

    MERGE INTO catalogue.Standards AS TARGET
        USING @Standards AS SOURCE
            ON Target.Id = SOURCE.Id
        WHEN MATCHED THEN
    UPDATE SET TARGET.[Name] = SOURCE.[Name],
               TARGET.[Description] = SOURCE.[Description],
               TARGET.[Url] = SOURCE.[Url],
               TARGET.[StandardTypeId] = SOURCE.[StandardTypeId],
               TARGET.[Version] = SOURCE.[Version],
               TARGET.[IsDeleted] = SOURCE.[IsDeleted]
        WHEN NOT MATCHED THEN
        INSERT (Id, [Name], [Description], [Url], [StandardTypeId], [Version], [IsDeleted])
        VALUES(SOURCE.Id, SOURCE.[Name], SOURCE.[Description], SOURCE.[Url], SOURCE.[StandardTypeId], SOURCE.[Version], SOURCE.[IsDeleted]);

END
