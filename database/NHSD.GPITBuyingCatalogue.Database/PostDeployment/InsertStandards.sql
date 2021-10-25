DECLARE @Standards AS TABLE
(
	[Id] NVARCHAR(5) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [Url] NVARCHAR(1000) NOT NULL,
    [RequiredForAllSolutions] BIT NOT NULL
);
-- Overarching Standards
INSERT INTO @Standards(Id,[Name],[Description],[Url],RequiredForAllSolutions)
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
INSERT INTO @Standards(Id,[Name],[Description],[Url],RequiredForAllSolutions)
VALUES
('S33', '111', 'Supports the receiving and consuming of data sent by 111 provider systems for transfer of care to a GP practice.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133945/111', 0),
('S34', 'Digital Diagnostics & Pathology Messaging', 'Supports the transmission and validation of results data to GP systems where a test request has been placed with a laboratory.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133752/Digital+Diagnostics+Pathology+Messaging', 0),
('S77', 'Digital Medicines and Pharmacy FHIR Payload', 'Supports the receiving and consuming of data sent by a pharmacy to a GP practice relating to administration of immunisations or supply of emergency medication to patients.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/10752983078/Digital+Medicines+and+Pharmacy+FHIR+Payload', 0),
('S35', 'e-Referrals Service (e-RS)', 'Supports electronic booking of hospital or clinic appointments, with a choice of place, date and time.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391135075/eMED3+Fit+Notes', 0),
('S36', 'Electronic Prescription Service (EPS)', 'Supports the sending of prescriptions electronically to a dispenser, for example a pharmacy, of the patient''s choice.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133858/Electronic+Prescription+Service+EPS+-+Prescribing', 0),
('S37', 'Electronic Yellow Card Reporting', 'Supports electronic reporting of suspected adverse drug reactions.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134080/Electronic+Yellow+Card+Reporting', 0),
('S39', 'eMED3 (Fit Notes)', 'Supports the creation of eMED3 data, its integration into a Patient Record, printing MED3 fit note certificates and the extraction of data to NHS Digital.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391135075/eMED3+Fit+Notes', 0),
('S43', 'GP2GP', ' Supports sharing of data held within GP IT solutions across health and social care organisations.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134336/GP2GP', 0),
('S44', 'GP Connect', 'Supports sharing of data held within GP IT solutions across health and social care organisations.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133650/GP+Connect', 0),
('S78', 'GP Data for Planning and Research', 'Supports the extraction and submission of GP data, for health and social care purposes (including planning, policy development, public health, commissioning and research) to NHS Digital.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/11235426957/GP+Data+for+Planning+and+Research', 0),
('S46', 'GPES Data Extraction', 'Supports secure collection of information from GP practices and its delivery to approved organisations.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134238/GPES+Data+Extraction', 0),
('S47', 'IM1 - Interface Mechanism', 'Supports the implementation of interfaces between solutions.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133895/IM1+-+Interface+Mechanism', 0),
('S53', 'NHAIS HA/GP Links', 'Supports the management of patient registration and demographic information with National Health Application and Infrastructure Services.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133619/NHAIS+HA+GP+Links', 0),
('S56', 'Personal Demographics Service (PDS)', 'Supports solution integration with the Personal Demographics Service which stores patient details.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133999/Personal+Demographics+Service+PDS', 0),
('S58', 'Screening Messaging', 'Supports the validation and transfer of screening result data to solutions.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133789/Screening+messaging', 0),
('S60', 'Summary Care Record (SCR)', 'Supports the management of Summary Care Record data and staff access to the data.', 'https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133699/Summary+Care+Record+SCR', 0);

-- The code below should not need to be changed

MERGE INTO catalogue.Standards AS TARGET
    USING @Standards AS SOURCE
        ON Target.Id = SOURCE.Id
    WHEN MATCHED THEN
UPDATE SET TARGET.[Name] = SOURCE.[Name],
           TARGET.[Description] = SOURCE.[Description],
           TARGET.[Url] = SOURCE.[Url],
           TARGET.RequiredForAllSolutions = SOURCE.RequiredForAllSolutions
    WHEN NOT MATCHED THEN
    INSERT (Id, [Name], [Description], [Url], RequiredForAllSolutions)
    VALUES(SOURCE.Id, SOURCE.[Name], SOURCE.[Description], SOURCE.[Url], SOURCE.RequiredForAllSolutions);
