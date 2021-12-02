DECLARE @Standards AS TABLE
(
    [Id] NVARCHAR(5) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [Url] NVARCHAR(1000) NOT NULL,
    [StandardTypeId] INT NOT NULL
);
-- Overarching Standards
INSERT INTO @Standards(Id,[Name],[Description],[Url],[StandardTypeId])
VALUES
('S24', 'Business Continuity and Disaster Recovery', 'Ensures that solutions are supported by robust business continuity plans and disaster recovery measures.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134058/Business+Continuity+and+Disaster+Recovery', 1),
('S25', 'Clinical Safety', 'Supports the management of clinical risk and patient safety.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134225/Clinical+Safety', 1),
('S31', 'Commercial', 'Underpins all commercial activity relating to the Buying Catalogue by defining rules governing commercial relationships and setting out standards of behaviour.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134121/Commercial', 1),
('S26', 'Data Migration', 'Supports the safe and effective migration of data if a buyer changes from one solution to another.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134456/Data+Migration', 1),
('S27', 'Data Standards', 'Defines detailed technical standards for the storage, management and organisation of data and specifies standardised reference data, terminology and codes.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134407/Data+Standards', 1),
('S29', 'Hosting & Infrastructure', 'Supports best practices for infrastructure and hosting of systems. For example, ensuring systems are cost effective, secure and energy efficient.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134324/Hosting+Infrastructure', 1),
('S30', 'Information Governance', 'Supports the controls needed to ensure that sensitive personal data is kept confidential, is accurate and is available to authorised users when required.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134230/Information+Governance', 1),
('S63', 'Non-Functional Questions', 'Enables NHS Digital to assess the risk associated with the assessment of a solution against other overarching Standards.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1569980472/Non-Functional+Questions', 1),
('S65', 'Service Management', 'Supports suppliers in the delivery and management of services that enable their solutions to continue working.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133802/Service+Management', 1),
('S69', 'Testing', 'Ensures that a suppliers’ software delivery test processes are of sufficient quality and rigour.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133583/Testing', 1),
('S28', 'Training', 'Enables NHS Digital to assess the risk associated with the assessment of a solution against other overarching Standards.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133846/Training', 1);

--Interoperability Standards
INSERT INTO @Standards(Id,[Name],[Description],[Url],[StandardTypeId])
VALUES
('S32','Interoperability','Defines a comprehensive set of standards, interfaces and protocols that solutions will use when working together.','https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133681/Interoperability+Standard', 2),
('S33', '111', 'Supports the receiving and consuming of data sent by 111 provider systems for transfer of care to a GP practice.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133945/111', 2),
('S34', 'Digital Diagnostics & Pathology Messaging', 'Supports the transmission and validation of results data to GP systems where a test request has been placed with a laboratory.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133752/Digital+Diagnostics+Pathology+Messaging', 2),
('S77', 'Digital Medicines and Pharmacy FHIR Payload', 'Supports the receiving and consuming of data sent by a pharmacy to a GP practice relating to administration of immunisations or supply of emergency medication to patients.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/10752983078/Digital+Medicines+and+Pharmacy+FHIR+Payload', 2),
('S35', 'e-Referrals Service (e-RS)', 'Supports electronic booking of hospital or clinic appointments, with a choice of place, date and time.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391135075/eMED3+Fit+Notes', 2),
('S36', 'Electronic Prescription Service (EPS)', 'Supports the sending of prescriptions electronically to a dispenser, for example a pharmacy, of the patient''s choice.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133858/Electronic+Prescription+Service+EPS+-+Prescribing', 2),
('S37', 'Electronic Yellow Card Reporting', 'Supports electronic reporting of suspected adverse drug reactions.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134080/Electronic+Yellow+Card+Reporting', 2),
('S39', 'eMED3 (Fit Notes)', 'Supports the creation of eMED3 data, its integration into a Patient Record, printing MED3 fit note certificates and the extraction of data to NHS Digital.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391135075/eMED3+Fit+Notes', 2),
('S43', 'GP2GP', ' Supports sharing of data held within GP IT solutions across health and social care organisations.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134336/GP2GP', 2),
('S44', 'GP Connect', 'Supports sharing of data held within GP IT solutions across health and social care organisations.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133650/GP+Connect', 2),
('S78', 'GP Data for Planning and Research', 'Supports the extraction and submission of GP data, for health and social care purposes (including planning, policy development, public health, commissioning and research) to NHS Digital.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/11235426957/GP+Data+for+Planning+and+Research', 2),
('S46', 'GPES Data Extraction', 'Supports secure collection of information from GP practices and its delivery to approved organisations.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134238/GPES+Data+Extraction', 2),
('S47', 'IM1 - Interface Mechanism', 'Supports the implementation of interfaces between solutions.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133895/IM1+-+Interface+Mechanism', 2),
('S53', 'NHAIS HA/GP Links', 'Supports the management of patient registration and demographic information with National Health Application and Infrastructure Services.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133619/NHAIS+HA+GP+Links', 2),
('S56', 'Personal Demographics Service (PDS)', 'Supports solution integration with the Personal Demographics Service which stores patient details.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133999/Personal+Demographics+Service+PDS', 2),
('S58', 'Screening Messaging', 'Supports the validation and transfer of screening result data to solutions.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133789/Screening+messaging', 2),
('S60', 'Summary Care Record (SCR)', 'Supports the management of Summary Care Record data and staff access to the data.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133699/Summary+Care+Record+SCR', 2);

--Capability Specific Standards
INSERT INTO @Standards(Id,[Name],[Description],[Url],[StandardTypeId])
VALUES
('S1', 'Appointments Management - Citizen', 'Enables citizens to manage their appointments online and supports the use of appointment slots that have been configured in Appointments Management - GP.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134369/Appointments+Management+-+Citizen+-+Standard', 3),
('S2', 'Communicate with Practice - Citizen', 'Supports secure and trusted electronic communications between citizens and a GP practice.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134173/Communicate+with+Practice+-+Citizen+-+Standard', 3),
('S3', 'Prescription Ordering - Citizen', 'Enables citizens to request medication online and manage nominated and preferred pharmacies for patients.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134356/Prescription+Ordering+-+Citizen+-+Standard', 3),
('S4', 'View Record - Citizen', 'Enables citizens to view their Patient Record online.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134394/View+Record+-+Citizen+-+Standard', 3),
('S5', 'Appointments Management - GP', 'Supports the administration, scheduling, resourcing and reporting of appointments.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134382/Appointments+Management+-+GP+-+Standard', 3),
('S62','General Practice Appointments Data Reporting', 'Enables the collection and submission to NHS Digital of appointment data to support capacity planning and management.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133692/General+Practice+Appointments+Data+Reporting', 3),
('S6', 'Clinical Decision Support', 'Supports clinical decision-making to improve patient safety at the point of care.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134143/Clinical+Decision+Support+-+Standard', 3),
('S7', 'Communication Management', 'Supports the delivery and management of communications to citizens and GP practice staff.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134296/Communication+Management+-+Standard', 3),
('S8', 'Digital Diagnostics', 'Supports electronic requesting with other healthcare organisations. Test results can be received, reviewed and stored against a Patient Record.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133783/Digital+Diagnostics+-+Standard', 3),
('S9', 'Document Management', 'Supports the secure management and classification of all forms of unstructured electronic documents including those created by scanning paper documents. Also enables processing of documents and matching documents with patients.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134289/Document+Management+-+Standard', 3),
('S10', 'GP Extracts Verification', 'Supports GP practice staff in ensuring accuracy of the data that is used with the Calculating Quality Reporting Service (CQRS).', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133808/GP+Extracts+Verification+-+Standard', 3),
('S13', 'Patient Information Maintenance', 'Supports the registration of patients and the maintenance of all their personal information. Enables the organisation and presentation of a comprehensive Patient Record.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134401/Patient+Information+Maintenance+-+Standard', 3),
('S70', 'Primary Care Clinical Terminology Usage Report', 'Supports the collection and submission to NHS Digital of information about usage of clinical coded terms.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134283/Primary+Care+Clinical+Terminology+Usage+Report', 3),
('S14', 'Prescribing', 'Supports the effective and safe prescribing of medical products and appliances to patients and makes available information to support prescribing.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134363/Prescribing+-+Standard', 3),
('S15', 'Recording Consultations', 'Supports the standardised recording of consultations and other GP practice activities. Also supports the extraction of Female Genital Mutilation (FGM) data for the FGM data set.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134136/Recording+Consultations+-+Standard', 3),
('S11', 'Referral Management', 'Supports recording, reviewing, sending, and reporting of patient referrals. Enables referral information to be included in a Patient Record. ', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133630/Referral+Management+-+Standard', 3),
('S16', 'Reporting', 'Enables reporting and analysis of data from other Capabilities in the GP practice solution to support clinical care and practice management.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134256/Reporting+-+Standard', 3),
('S12', 'Resource Management', 'Supports the management and reporting of practice information, resources, staff members and related organisations. Also enables management of staff member availability and inactivity.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133828/Resource+Management+-+Standard', 3),
('S19', 'Unstructured Data Extraction', 'Enables automated and manual interpretation and extraction of structured data from paper documents and unstructured electronic documents to support their classification and matching with a Patient Record.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134330/Unstructured+Data+Extraction+-+Standard', 3),
('S20', 'Workflow', 'Supports manual and automated management of work in a GP practice, and enables effective planning, tracking, monitoring and reporting.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134376/Workflow+-+Standard', 3),
('S21', 'Citizen Access', 'Enables citizens to access their services safely and securely and also supports them in viewing and updating patient information online.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133929/Citizen+Access', 3),
('S22', 'Common Reporting', 'Supports the reporting needs common to GP practices and includes searchable report templates.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133675/Common+Reporting', 3),
('S23', 'Management Information (MI) Reporting', 'Supports the submission of aggregated counts of information regarding services, appointments, prescriptions and documents to NHS Digital.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134317/Management+Information+MI+Reporting', 3);

-- The code below should not need to be changed

MERGE INTO catalogue.Standards AS TARGET
    USING @Standards AS SOURCE
        ON Target.Id = SOURCE.Id
    WHEN MATCHED THEN
UPDATE SET TARGET.[Name] = SOURCE.[Name],
           TARGET.[Description] = SOURCE.[Description],
           TARGET.[Url] = SOURCE.[Url],
           TARGET.StandardTypeId = SOURCE.[StandardTypeId]
    WHEN NOT MATCHED THEN
    INSERT (Id, [Name], [Description], [Url], [StandardTypeId])
    VALUES(SOURCE.Id, SOURCE.[Name], SOURCE.[Description], SOURCE.[Url], SOURCE.[StandardTypeId]);
