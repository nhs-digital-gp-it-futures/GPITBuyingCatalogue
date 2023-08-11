IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
BEGIN

    DECLARE @epics AS TABLE
    (
        Id nvarchar(10) NOT NULL PRIMARY KEY,
        [Name] nvarchar(500) NOT NULL,
        IsActive bit NOT NULL,
        SupplierDefined bit DEFAULT 0 NOT NULL
    )

    DECLARE @capabilityEpics AS TABLE
    (
        [CapabilityId] INT NOT NULL,
        [EpicId] NVARCHAR(10) NOT NULL,
        CompliancyLevelId int NULL
    )

    INSERT INTO @epics(Id, [Name], IsActive, SupplierDefined)
    VALUES
    ('C10E1', 'View GPES payment extract reports', 0, 0),
    ('C10E2', 'View national GPES non-payment extract reports', 0, 0),
    ('C11E1', 'Manage Referrals', 0, 0),
    ('C11E2', 'View Referral reports', 0, 0),
    ('C12E1', 'Manage General Practice and Branch site information', 0, 0),
    ('C12E2', 'Manage General Practice Staff Members', 0, 0),
    ('C12E3', 'Manage Staff Member inactivity periods', 0, 0),
    ('C12E4', 'Manage Staff Member Groups', 0, 0),
    ('C12E5', 'Manage Related Organisations information', 0, 0),
    ('C12E6', 'Manage Related Organisation Staff Members', 0, 0),
    ('C12E7', 'Manage Non-human Resources', 0, 0),
    ('C13E1', 'Manage Patients ', 0, 0),
    ('C13E10', 'Configure Citizen service access for the Practice', 0, 0),
    ('C13E11', 'Identify Patients outside of Catchment Area', 0, 0),
    ('C13E12', 'Manage Patient Cohorts from Search Results', 0, 0),
    ('C13E13', 'View Subject Access Request reports', 0, 0),
    ('C13E14', 'Manage Acute Prescription Request Service', 0, 0),
    ('C13E15', 'Notify the Patient of changes', 0, 0),
    ('C13E16', 'Manage Subject Access Request (SAR) requests', 0, 0),
    ('C13E17', 'Notify the Proxy of changes', 0, 0),
    ('C13E18', 'Manage Practice notifications – Proxy', 0, 0),
    ('C13E19', 'Configure Proxy notifications', 0, 0),
    ('C13E2', 'Access Patient Record', 0, 0),
    ('C13E20', 'Manage Proxy Communications', 0, 0),
    ('C13E21', 'Manage Proxys for Citizen Services', 0, 0),
    ('C13E3', 'Manage Patient Related Persons', 0, 0),
    ('C13E4', 'Manage Patients for Citizen Services', 0, 0),
    ('C13E5', 'Manage Patient Communications', 0, 0),
    ('C13E6', 'Configure Patient notifications', 0, 0),
    ('C13E7', 'Manage Practice notifications – Patient', 0, 0),
    ('C13E8', 'Search for Patient Records', 0, 0),
    ('C13E9', 'View Patient Reports', 0, 0),
    ('C14E1', 'Access prescribable items', 0, 0),
    ('C14E10', 'Manage Repeat Medication requests', 0, 0),
    ('C14E11', 'Manage Acute Medication requests', 0, 0),
    ('C14E12', 'Manage Authorising Prescribers', 0, 0),
    ('C14E13', 'Access Patient Record', 0, 0),
    ('C14E14', 'View EPS Nominated Pharmacy changes', 0, 0),
    ('C14E15', 'Configure warnings for prescribable items', 0, 0),
    ('C14E16', 'Medications are linked to diagnoses', 0, 0),
    ('C14E2', 'Manage Formularies', 0, 0),
    ('C14E3', 'Manage shared Formularies', 0, 0),
    ('C14E4', 'Set default Formulary for Practice Users', 0, 0),
    ('C14E5', 'Manage prescribed medication', 0, 0),
    ('C14E6', 'Manage prescriptions', 0, 0),
    ('C14E7', 'Manage Patient''s Preferred Pharmacy', 0, 0),
    ('C14E8', 'Manage Patient medication reviews', 0, 0),
    ('C14E9', 'View prescribed medication reports', 0, 0),
    ('C15E1', 'Record Consultation information', 0, 0),
    ('C15E2', 'View report on calls and recalls', 0, 0),
    ('C15E3', 'View report of Consultations', 0, 0),
    ('C15E4', 'Access Patient Record', 0, 0),
    ('C15E5', 'Manage Consultation form templates', 0, 0),
    ('C15E6', 'Share Consultation form templates', 0, 0),
    ('C15E7', 'Use supplier implemented national Consultation form templates', 0, 0),
    ('C15E8', 'Extract Female Genital Mutilation data', 0, 0),
    ('C16E1', 'Report data from other Capabilities', 0, 0),
    ('C17E1', 'Scan documents', 0, 0),
    ('C17E2', 'Image enhancement', 0, 0),
    ('C18E1', 'share monitoring data with my General Practice', 1, 0),
    ('C18E2', 'configure Telehealth for the Practice', 1, 0),
    ('C18E3', 'configure Telehealth for the Patient', 1, 0),
    ('C18E4', 'manage incoming Telehealth data', 1, 0),
    ('C19E1', 'Document classification', 0, 0),
    ('C19E2', 'Manage Document Classification rules', 0, 0),
    ('C19E3', 'Document and Patient matching', 0, 0),
    ('C1E1', 'Manage Appointments', 0, 0),
    ('C1E2', 'Manage Appointments by Proxy', 0, 0),
    ('C20E1', 'Manage Task templates', 0, 0),
    ('C20E10', 'View Workflow reports', 0, 0),
    ('C20E11', 'Access Patient Record', 0, 0),
    ('C20E12', 'Share Task List configuration', 0, 0),
    ('C20E13', 'Share Workflow List configuration', 0, 0),
    ('C20E2', 'Manage Workflow templates', 0, 0),
    ('C20E3', 'Configure Task rules', 0, 0),
    ('C20E4', 'Configure Workflow rules', 0, 0),
    ('C20E5', 'Manage Tasks', 0, 0),
    ('C20E6', 'Manage Workflows', 0, 0),
    ('C20E7', 'Manage Task List configurations', 0, 0),
    ('C20E8', 'Manage Workflows List configurations', 0, 0),
    ('C20E9', 'View Task reports', 0, 0),
    ('C21E1', 'maintain Resident''s Care Home Record', 1, 0),
    ('C21E2', 'maintain Resident Proxy preferences', 1, 0),
    ('C21E3', 'view and maintain End of Life Care Plans', 1, 0),
    ('C21E4', 'record incident and adverse events', 1, 0),
    ('C21E5', 'maintain Staff Records', 1, 0),
    ('C21E6', 'maintain Staff Task schedules', 1, 0),
    ('C21E7', 'manage Tasks', 1, 0),
    ('C21E8', 'reporting', 1, 0),
    ('C22E1', 'manage Cases', 1, 0),
    ('C22E2', 'maintain Caseloads', 1, 0),
    ('C22E3', 'generate and manage contact schedules', 1, 0),
    ('C22E4', 'update Case details', 1, 0),
    ('C22E5', 'review and comment on Caseload', 1, 0),
    ('C22E6', 'review and comment on contact schedule', 1, 0),
    ('C22E7', 'view and update Patient/Service User''s Health or Care Record', 1, 0),
    ('C22E8', 'reporting', 1, 0),
    ('C22E9', 'care Pathway templates', 1, 0),
    ('C23E1', 'Make Appointments available to external organisations', 0, 0),
    ('C23E2', 'Search externally bookable Appointment slots', 0, 0),
    ('C23E3', 'Book externally bookable Appointment slots', 0, 0),
    ('C23E4', 'Maintain Appointments', 0, 0),
    ('C23E5', 'Notifications', 0, 0),
    ('C23E6', 'Manage Appointment Requests', 0, 0),
    ('C23E7', 'Booking approval', 0, 0),
    ('C23E8', 'Report on usage of Cross-Organisation Appointments', 0, 0),
    ('C23E9', 'Manage Cross-Organisation Appointment Booking templates', 0, 0),
    ('C24E1', 'use Workflow to run a Cross-organisational Process', 1, 0),
    ('C24E2', 'maintain cross-organisational workflows', 1, 0),
    ('C24E3', 'maintain cross-organisational workflow templates', 1, 0),
    ('C24E4', 'share workflow templates', 1, 0),
    ('C24E5', 'manage automated notifications and reminders', 1, 0),
    ('C24E6', 'manage ad-hoc notifications', 1, 0),
    ('C24E7', 'report on Cross-organisational Workflows', 1, 0),
    ('C25E1', 'maintain service schedule', 1, 0),
    ('C25E2', 'share service schedule', 1, 0),
    ('C25E3', 'workforce management reporting', 1, 0),
    ('C26E1', 'analyse data across multiple organisations within the Integrated/Federated Care Setting (Federation)', 1, 0),
    ('C26E10', 'enable reporting at different levels', 1, 0),
    ('C26E2', 'analyse data across different datasets', 1, 0),
    ('C26E3', 'create new or update existing reports ', 1, 0),
    ('C26E4', 'run existing reports', 1, 0),
    ('C26E5', 'present output', 1, 0),
    ('C26E6', 'define selection rules on reports', 1, 0),
    ('C26E7', 'create and run performance-based reports', 1, 0),
    ('C26E8', 'drill down to detailed information', 1, 0),
    ('C26E9', 'forecasting', 1, 0),
    ('C27E1', 'maintain Domiciliary Care schedules', 1, 0),
    ('C27E2', 'share Domiciliary Care schedules', 1, 0),
    ('C27E3', 'manage Appointments', 1, 0),
    ('C27E4', 'Service User manages their schedule for Domiciliary Care', 1, 0),
    ('C27E5', 'manage Care Plans for Service Users', 1, 0),
    ('C27E6', 'remote access to Domiciliary Care schedule', 1, 0),
    ('C27E7', 'receive notifications relating to Service User', 1, 0),
    ('C27E8', 'reports', 1, 0),
    ('C27E9', 'nominated individuals to view Domiciliary Care schedule ', 1, 0),
    ('C29E1', 'Health or Care Professional requests support', 1, 0),
    ('C29E2', 'respond to request for support from another Health or Care Professional', 1, 0),
    ('C29E3', 'link additional information to a request for support', 1, 0),
    ('C29E4', 'live Consultation: Health and Care Professionals', 1, 0),
    ('C29E5', 'link Consultation to Patient/Service User''s Record', 1, 0),
    ('C29E6', 'reports', 1, 0),
    ('C2E1', 'Manage communications – Patient', 0, 0),
    ('C2E2', 'Manage communications – Proxy', 0, 0),
    ('C30E1', 'single unified medication view', 1, 0),
    ('C30E10', 'access national or local Medicines Optimisation guidance', 1, 0),
    ('C30E11', 'prescribing decision support', 1, 0),
    ('C30E12', 'Medicines Optimisation reporting', 1, 0),
    ('C30E13', 'configure notifications for required Medicines Reviews', 1, 0),
    ('C30E14', 'receive notification for required medicines reviews', 1, 0),
    ('C30E2', 'request medication changes', 1, 0),
    ('C30E3', 'identify Patients requiring medicines review', 1, 0),
    ('C30E4', 'maintain medicines review', 1, 0),
    ('C30E5', 'notify Patient and Proxies of medication changes', 1, 0),
    ('C30E6', 'notify other interested parties of medication changes', 1, 0),
    ('C30E7', 'configure medication substitutions', 1, 0),
    ('C30E8', 'use pre-configured medication substitutions', 1, 0),
    ('C30E9', 'maintain prescribed medication', 1, 0),
    ('C32E1', 'manage Personal Health Budget', 1, 0),
    ('C32E10', 'manage multiple budgets', 1, 0),
    ('C32E11', 'link to Patient Record', 1, 0),
    ('C32E12', 'link to Workflow', 1, 0),
    ('C32E13', 'provider view', 1, 0),
    ('C32E14', 'Management Information', 1, 0),
    ('C32E15', 'identify candidates for Personal Health Budgets', 1, 0),
    ('C32E2', 'record Personal Health Budget purchases', 1, 0),
    ('C32E3', 'assess Personal Health Budgets', 1, 0),
    ('C32E4', 'link Personal Health Budget with care plan', 1, 0),
    ('C32E5', 'support different models for management of Personal Health Budgets', 1, 0),
    ('C32E6', 'apply criteria for the use of Personal Health Budgets', 1, 0),
    ('C32E7', 'payments under Personal Health Budgets', 1, 0),
    ('C32E8', 'maintain directory of equipment, treatments and services', 1, 0),
    ('C32E9', 'search a directory of equipment, treatments and services', 1, 0),
    ('C33E1', 'maintain Personal Health Record content', 1, 0),
    ('C33E2', 'organise Personal Health Record ', 1, 0),
    ('C33E3', 'manage access to Personal Health Record', 1, 0),
    ('C33E4', 'manage data coming into Personal Health Record', 1, 0),
    ('C34E1', 'access healthcare data', 1, 0),
    ('C34E2', 'maintain cohorts', 1, 0),
    ('C34E3', 'stratify population by risk', 1, 0),
    ('C34E4', 'data analysis and reporting', 1, 0),
    ('C34E5', 'outcomes', 1, 0),
    ('C34E6', 'dashboard', 1, 0),
    ('C35E1', 'run Risk Stratification algorithms', 1, 0),
    ('C36E1', 'create Shared Care Plan', 1, 0),
    ('C36E10', 'reports', 1, 0),
    ('C36E11', 'manage Shared Care Plan templates', 1, 0),
    ('C36E12', 'manage care schedules', 1, 0),
    ('C36E2', 'view Shared Care Plan', 1, 0),
    ('C36E3', 'amend Shared Care Plan', 1, 0),
    ('C36E4', 'close Shared Care Plan ', 1, 0),
    ('C36E5', 'assign Shared Care Plan actions', 1, 0),
    ('C36E6', 'access Shared Care Plans remotely', 1, 0),
    ('C36E7', 'aearch and view Shared Care Plans', 1, 0),
    ('C36E8', 'real-time access to Shared Care Plans', 1, 0),
    ('C36E9', 'notifications', 1, 0),
    ('C37E1', 'assess wellness or well-being of the Patient or Service User', 1, 0),
    ('C37E10', 'Patient self-referral', 1, 0),
    ('C37E11', 'integrate Social Prescribing Referral Record with Clinical Record', 1, 0),
    ('C37E12', 'receive notification of an Appointment', 1, 0),
    ('C37E13', 'remind Patients/Service Users of Appointments', 1, 0),
    ('C37E14', 'provide service feedback', 1, 0),
    ('C37E15', 'view service feedback', 1, 0),
    ('C37E16', 'Obtain Management Information (MI) on Social Prescribing', 1, 0),
    ('C37E2', 'search the directory', 1, 0),
    ('C37E3', 'refer Patient/Service User to service(s)', 1, 0),
    ('C37E4', 'maintain referral record', 1, 0),
    ('C37E5', 'link to national or local directory of services', 1, 0),
    ('C37E6', 'maintain directory of services', 1, 0),
    ('C37E7', 'maintain service criteria', 1, 0),
    ('C37E8', 'refer Patient/Service User to Link Worker', 1, 0),
    ('C37E9', 'capture Patient/Service User consent', 1, 0),
    ('C38E1', 'define response to event', 1, 0),
    ('C38E10', 'manual testing of Telecare device', 1, 0),
    ('C38E11', 'sustainability of Telecare device', 1, 0),
    ('C38E2', 'monitor and alert', 1, 0),
    ('C38E3', 'receive alerts', 1, 0),
    ('C38E4', 'meet availability targets', 1, 0),
    ('C38E5', 'ease of use', 1, 0),
    ('C38E6', 'Patient/Service Users with sensory impairment(s)', 1, 0),
    ('C38E7', 'obtain Management Information (MI) on Telecare', 1, 0),
    ('C38E8', 'enable 2-way communication with Patient/Service User', 1, 0),
    ('C38E9', 'remote testing of Telecare device', 1, 0),
    ('C39E1', 'view Unified Care Record', 1, 0),
    ('C39E2', 'Patient/Service User views the Unified Care Record', 1, 0),
    ('C39E3', 'default Views', 1, 0),
    ('C3E1', 'Manage Repeat Medications – Patient', 0, 0),
    ('C3E10', 'View medication information as a Proxy', 0, 0),
    ('C3E2', 'Manage my nominated EPS pharmacy', 0, 0),
    ('C3E3', 'Manage my Preferred PharmacyAs a Patient', 0, 0),
    ('C3E4', 'Manage Acute Medications', 0, 0),
    ('C3E5', 'View medication information', 0, 0),
    ('C3E6', 'Manage Repeat Medications as a Proxy', 0, 0),
    ('C3E7', 'Manage nominated EPS pharmacy as a Proxy', 0, 0),
    ('C3E8', 'Manage Preferred Pharmacy as a Proxy', 0, 0),
    ('C3E9', 'Manage Acute Medications as a Proxy', 0, 0),
    ('C40E1', 'Verify Medicinal Product Unique Identifiers', 0, 0),
    ('C40E2', 'Decommission Medicinal Products', 0, 0),
    ('C40E3', 'Record the integrity of Anti-tampering Devices', 0, 0),
    ('C42E1', 'manage Stock in a Dispensary', 1, 0),
    ('C42E2', 'manage Stock Orders', 1, 0),
    ('C42E3', 'manage Dispensing tasks for a Dispensary', 1, 0),
    ('C42E4', 'dispense Medication', 1, 0),
    ('C42E5', 'manage Dispensaries', 1, 0),
    ('C42E6', 'manage Endorsements', 1, 0),
    ('C42E7', 'manage Supplier Accounts', 1, 0),
    ('C42E8', 'view Stock reports', 1, 0),
    ('C45E1', 'identify COVID-19 vaccination cohorts', 1, 0),
    ('C45E2', 'verify Patient information using Personal Demographics Service (PDS)', 1, 0),
    ('C45E3', 'import or consume COVID-19 vaccination data for Patients', 1, 0),
    ('C45E4', 'extract COVID-19 vaccination cohorts data in .CSV file format', 1, 0),
    ('C45E5', 'bulk send SMS messages for COVID-19 vaccination invite communications', 1, 0),
    ('C45E6', 'bulk create letters for COVID-19 vaccination invite communications', 1, 0),
    ('C45E7', 'bulk send email for COVID-19 vaccination invite communications', 1, 0),
    ('C45E8', 'automatically record which Patients have had COVID-19 vaccination invites created', 1, 0),
    ('C45E9', 'view whether Patients have had a COVID-19 vaccination invite communication created', 1, 0),
    ('C46E1', 'define appointment availability for a vaccination site', 1, 0),
    ('C46E10', 'automatically record which Patients have had COVID-19 vaccination invites created', 1, 0),
    ('C46E11', 'view whether Patients have had a COVID-19 vaccination invite communication created', 1, 0),
    ('C46E12', 'automatically bulk send booking reminders to Patients via SMS messages for COVID-19 vaccination invites', 1, 0),
    ('C46E13', 'automatically bulk create booking reminders to Patients as letters for COVID-19 vaccination invites', 1, 0),
    ('C46E14', 'automatically bulk send booking reminders to Patients via email for COVID-19 vaccination invites', 1, 0),
    ('C46E15', 'book Appointments across Solutions using GP Connect Appointments Management', 1, 0),
    ('C46E16', 'Patients can book their own COVID-19 vaccination appointments', 1, 0),
    ('C46E17', 'Patients can re-schedule their own future COVID-19 vaccination appointment', 1, 0),
    ('C46E18', 'Patients can cancel their own future COVID-19 vaccination appointment', 1, 0),
    ('C46E19', 'automatically send booking notifications to Patients via SMS messages for COVID-19 vaccination appointments', 1, 0),
    ('C46E2', 'book vaccination appointments for eligible Patients registered across different GP Practices', 1, 0),
    ('C46E20', 'automatically create booking notifications to Patients as letters for vaccination appointments', 1, 0),
    ('C46E21', 'automatically send booking notifications to Patients via email for vaccination appointments', 1, 0),
    ('C46E22', 'create ad-hoc booking notifications to Patients for vaccination appointments', 1, 0),
    ('C46E23', 'automatically bulk send appointment reminders to Patients via SMS messages for vaccination appointments', 1, 0),
    ('C46E24', 'automatically bulk create booking reminders to Patients as letters for vaccination appointments', 1, 0),
    ('C46E25', 'automatically bulk send appointment reminders to Patients via email for vaccination appointments', 1, 0),
    ('C46E26', 'send ad-hoc appointment reminders to Patients for vaccination appointments', 1, 0),
    ('C46E27', 'view all booked vaccination appointments for a specified time period', 1, 0),
    ('C46E28', 'export all booked vaccination appointments for a specified time period', 1, 0),
    ('C46E29', 'cancel booked vaccination appointments for Patients', 1, 0),
    ('C46E3', 'record that a vaccination appointment for a Patient has been completed', 1, 0),
    ('C46E30', 're-schedule booked vaccination appointments for Patients', 1, 0),
    ('C46E31', 'automatically send appointment cancellation notifications to Patients via SMS messages for appointments', 1, 0),
    ('C46E32', 'automatically create appointment cancellation notifications to Patients as letters for appointments', 1, 0),
    ('C46E33', 'automatically send appointment cancellation notifications to Patients via email for appointments', 1, 0),
    ('C46E4', 'extract vaccination appointments data for NHS Digital', 1, 0),
    ('C46E5', 'import vaccination Patient cohorts data via .CSV file', 1, 0),
    ('C46E6', 'verify Patient information using Personal Demographics Service (PDS)', 1, 0),
    ('C46E7', 'bulk send SMS messages for vaccination invite communications', 1, 0),
    ('C46E8', 'bulk create letters for vaccination invite communications', 1, 0),
    ('C46E9', 'bulk send email for vaccination invite communications', 1, 0),
    ('C47E1', 'record structured vaccination data at the point of care for Patients registered at different GP Practices', 1, 0),
    ('C47E10', 'view Summary Care Record (SCR) for a Patient', 1, 0),
    ('C47E11', 'scanning of a GS1 barcode when recording vaccination data', 1, 0),
    ('C47E12', 'record structured vaccination data at the point of care directly into GP Patient Record', 1, 0),
    ('C47E13', 'record structured adverse reaction data at the point of care directly into GP Patient Record', 1, 0),
    ('C47E14', 'verify Patient information using Personal Demographics Service (PDS)', 1, 0),
    ('C47E15', 'latest Clinical Screening Questions at the point of care for Patients registered at different GP Practices', 1, 0),
    ('C47E16', 'record structured vaccination data at the point of care for Patients using pre-configured vaccine batches', 1, 0),
    ('C47E17', 'view vaccination information for a Patient held by the National Immunisation Management Service (NIMS) at point of care', 1, 0),
    ('C47E18', 'update previously recorded structured vaccination and adverse reaction data for Patients', 1, 0),
    ('C47E19', 'extract COVID-19 Extended Attributes data for NHS Digital Extended Attributes Extract', 1, 0),
    ('C47E2', 'record structured adverse reaction data at the point of care for Patients registered at different GP Practices', 1, 0),
    ('C47E20', 'view reports on vaccination data', 1, 0),
    ('C47E3', 'extract vaccination data for NHS Digital Daily Clinical Vaccination Extract', 1, 0),
    ('C47E4', 'extract adverse reaction data for NHS Digital Daily Clinical Adverse Reaction Extract', 1, 0),
    ('C47E5', 'automatically send vaccination data to Patient''s registered GP Practice Foundation Solution using Digital Medicines FHIR messages', 1, 0),
    ('C47E6', 'automatically send adverse reaction data to Patient''s registered GP Practice Foundation Solution using Digital Medicines FHIR messages', 1, 0),
    ('C47E7', 'automatically send vaccination data to the NHS Business Services Authority (NHSBSA)', 1, 0),
    ('C47E8', 'view information from the GP Patient Record using GP Connect Access Record HTML', 1, 0),
    ('C47E9', 'view information from the GP Patient Record held by the same Solution', 1, 0),
    ('C4E1', 'View Patient Record – Patient', 0, 0),
    ('C4E2', 'View Patient Record – Proxy', 0, 0),
    ('C5E1', 'Manage Session templates', 0, 0),
    ('C5E2', 'Manage Sessions', 0, 0),
    ('C5E3', 'Configure Appointments', 0, 0),
    ('C5E4', 'Practice configuration', 0, 0),
    ('C5E5', 'Manage Appointments', 0, 0),
    ('C5E6', 'View Appointment reports', 0, 0),
    ('C5E7', 'Access Patient Record', 0, 0),
    ('C6E1', 'access to Clinical Decision Support', 1, 0),
    ('C6E2', 'local configuration for Clinical Decision Support triggering', 1, 0),
    ('C6E3', 'view Clinical Decision Support reports', 1, 0),
    ('C6E4', 'configuration for custom Clinical Decision Support', 1, 0),
    ('C7E1', 'Manage communication consents for a Patient', 0, 0),
    ('C7E10', 'Manage incoming communications', 0, 0),
    ('C7E2', 'Manage communication preferences for a Patient', 0, 0),
    ('C7E3', 'Manage communication templates', 0, 0),
    ('C7E4', 'Create communications', 0, 0),
    ('C7E5', 'Manage automated communications', 0, 0),
    ('C7E6', 'View communication reports', 0, 0),
    ('C7E7', 'Access Patient Record', 0, 0),
    ('C7E8', 'Manage communication consents for a Proxy', 0, 0),
    ('C7E9', 'Manage communication preferences for a Proxy', 0, 0),
    ('C8E1', 'Manage Requests for Investigations', 0, 0),
    ('C8E2', 'View Requests for Investigations reports', 0, 0),
    ('C8E3', 'Create a Request for Investigation for multiple Patients', 0, 0),
    ('C8E4', 'Receive external Request for Investigation information', 0, 0),
    ('C9E1', 'Manage document classifications', 0, 0),
    ('C9E10', 'Visually compare multiple documents', 0, 0),
    ('C9E11', 'View any version of a document', 0, 0),
    ('C9E12', 'Print documents', 0, 0),
    ('C9E13', 'Export documents to new formats', 0, 0),
    ('C9E14', 'Document reports', 0, 0),
    ('C9E15', 'Receipt of electronic documents', 0, 0),
    ('C9E16', 'Access Patient Record', 0, 0),
    ('C9E17', 'Search for documents using document content', 0, 0),
    ('C9E2', 'Manage document properties', 0, 0),
    ('C9E3', 'Manage document attributes', 0, 0),
    ('C9E4', 'Manage document coded entries', 0, 0),
    ('C9E5', 'Document workflows', 0, 0),
    ('C9E6', 'Manage document annotation', 0, 0),
    ('C9E7', 'Search for documents', 0, 0),
    ('C9E8', 'Search document content', 0, 0),
    ('C9E9', 'Document and Patient matching', 0, 0),
    ('E00001', 'Online Consultation', 1, 0),
    ('E00002', 'Online Consultation with a Proxy', 1, 0),
    ('E00003', 'Patient/Service User requests for Online Consultation support and provides information', 1, 0),
    ('E00004', 'Proxy requests for Online Consultation support and provides information', 1, 0),
    ('E00005', 'respond to Online Consultation requests for support from Patients/Service Users', 1, 0),
    ('E00006', 'respond to Online Consultation requests for support from Proxies', 1, 0),
    ('E00007', 'include attachments in Online Consultation requests', 1, 0),
    ('E00008', 'include attachments in Online Consultation requests from a Proxy', 1, 0),
    ('E00009', 'automated response to Online Consultation requests for support from Patients/Service Users', 1, 0),
    ('E00010', 'automated response to Online Consultation requests for support from Proxies', 1, 0),
    ('E00011', 'Patient/Service User makes an administrative request', 1, 0),
    ('E00012', 'Proxy makes an administrative request', 1, 0),
    ('E00013', 'respond to administrative requests for support from Patients/Service Users', 1, 0),
    ('E00014', 'respond to administrative requests for support from Proxies', 1, 0),
    ('E00015', 'automated responses to administrative requests from Patients/Service Users', 1, 0),
    ('E00016', 'automated responses to administrative requests from Proxies', 1, 0),
    ('E00017', 'link Online Consultation requests for support and responses', 1, 0),
    ('E00018', 'link Online Consultation requests for support from a Proxy and responses', 1, 0),
    ('E00019', 'self-help and signposting', 1, 0),
    ('E00020', 'Proxy supporting self-help and signposting', 1, 0),
    ('E00021', 'symptom checking', 1, 0),
    ('E00022', 'symptom checking by a Proxy', 1, 0),
    ('E00023', 'Direct Messaging', 1, 0),
    ('E00024', 'Direct Messaging by a Proxy', 1, 0),
    ('E00025', 'view the Patient Record during Online Consultation', 1, 0),
    ('E00026', 'electronically share files during Direct Messaging', 1, 0),
    ('E00027', 'electronically share files during Direct Messaging with a Proxy', 1, 0),
    ('E00028', 'customisation of report', 1, 0),
    ('E00029', 'report on utilisation of Online Consultation requests for support', 1, 0),
    ('E00030', 'report on outcomes or dispositions provided to the Patient/Service User', 1, 0),
    ('E000304', 'Patient-based report on lipid management', 1, 0),
    ('E00031', 'report on the status of Online Consultations', 1, 0),
    ('E00032', 'report on Patient demographics using Online Consultation', 1, 0),
    ('E00033', 'manually prioritise Online Consultation requests for support', 1, 0),
    ('E00034', 'assign Online Consultation requests to a Health or Care Professional manually', 1, 0),
    ('E00035', 'categorise outcome of Online Consultation requests', 1, 0),
    ('E00037', 'automatically prioritise Online Consultation requests', 1, 0),
    ('E00038', 'assign Online Consultation requests to Health or Care Professional automatically', 1, 0),
    ('E00039', 'conduct Video Consultation', 1, 0),
    ('E000396', 'firearms warnings', 1, 0),
    ('E00040', 'conduct Video Consultation with a Proxy', 1, 0),
    ('E00041', 'conduct a Video Consultation with the Patient/Service User without registration', 1, 0),
    ('E00042', 'conduct Video Consultation with a Proxy without registration', 1, 0),
    ('E00043', 'end Video Consultation with a Patient/Service User', 1, 0),
    ('E00045', 'Direct Messaging during a Video Consultation', 1, 0),
    ('E00047', 'view the Patient Record during Video Consultation', 1, 0),
    ('E00048', 'conduct group Video Consultations', 1, 0),
    ('E00051', 'electronically share files during a Video Consultation', 1, 0),
    ('E00053', 'Health or Care Professional can share their screen during a Video Consultation', 1, 0),
    ('E00055', 'record Video Consultation outcome to the Patient record', 1, 0),
    ('E00056', 'disable and enable Direct Messaging for a Healthcare Organisation', 1, 0),
    ('E00057', 'disable and enable Direct Messaging for a Patient/Service User', 1, 0),
    ('E00058', 'disable and enable electronic file sharing during Direct Messaging for a Healthcare Organisation', 1, 0),
    ('E00059', 'Health or Care Professional can record a Video Consultation', 1, 0),
    ('E00060', 'Patient/Service User can record a Video Consultation', 1, 0),
    ('E00061', 'accessibility options for Video Consultation', 1, 0),
    ('E00062', 'waiting room', 1, 0),
    ('E00063', 'disable and enable Direct Messaging during a Video Consultation for the Patient/Service User', 1, 0),
    ('E00064', 'record Direct Messages to the Patient Record', 1, 0),
    ('E00065', 'Patient/Service User name is not automatically visible in a group Video Consultation', 1, 0),
    ('E00066', 'invite new participants to an existing Video Consultation with a Patient/Service User', 1, 0),
    ('E00067', 'disable and enable electronic file sharing during a Video Consultation', 1, 0),
    ('E00068', 'disable and enable screen sharing during a Video Consultation', 1, 0),
    ('E00069', 'Patient/Service User feedback on Video Consultations', 1, 0),
    ('E00070', 'test the Video Consultation settings', 1, 0),
    ('E00071', 'consecutive consultations with multiple Patients/Service Users via a single Video Consultation', 1, 0),
    ('E00072', 'reminder of upcoming or scheduled Video Consultation', 1, 0),
    ('E00073', 'disable and enable audio during a Video Consultation', 1, 0),
    ('E00074', 'disable and enable video during a Video Consultation', 1, 0),
    ('E00075', 'Patient/Service User feedback for Online Consultation', 1, 0),
    ('E00076', 'record Online Consultation outcome to the Patient Record', 1, 0),
    ('E00077', 'retain attachments (file and images) in the Patient Record', 1, 0),
    ('E00078', 'Verify Patient/Service User details against Personal Demographics Service (PDS)', 1, 0),
    ('E00079', 'SNOMED code Online Consultation', 1, 0),
    ('E00080', 'customisation of the question sets for Patients/Service Users requesting Online Consultation support', 1, 0),
    ('E00081', 'accessibility options for Online Consultation', 1, 0),
    ('E00082', 'notification to Patients/Service Users', 1, 0),
    ('E00083', 'customisation of instructions to Patients/Service Users using Online Consultation Solution', 1, 0),
    ('E00084', 'categorise administration requests', 1, 0),
    ('E00085', 'disable and enable Direct Messaging for an Online Consultation request for support', 1, 0),
    ('E00086', 'configuration of the triage process', 1, 0),
    ('E00087', 'retain attachments (file and images) received during Video Consultation in the Patient Record', 1, 0),
    ('E00088', 'SNOMED code Video Consultation', 1, 0),
    ('E00089', 'save the complete record of an Online Consultation to the Patient Record', 1, 0),
    ('E00090', 'Health or Care Professional initiates an Online Consultations request', 1, 0),
    ('E00091', 'Proxy Verification', 1, 0),
    ('E00092', 'access pre-configured prescribable items and related information at the point of prescribing', 1, 0),
    ('E00093', 'manage Prescriber Type for Health or Care Professionals', 1, 0),
    ('E00094', 'manage prescriber sub-type for Health or Care Professionals', 1, 0),
    ('E00095', 'manage Alternate Authorising Prescribers for Health or Care Professionals', 1, 0),
    ('E00096', 'manage prescribed Acute and Repeat medication for Patients', 1, 0),
    ('E00097', 'create Repeatable Batch Issue of Repeat medication for Patients', 1, 0),
    ('E00098', 'manage prescribed Instalment medication for Patients', 1, 0),
    ('E00099', 'record medication personally administered or dispensed by the Healthcare Organisation', 1, 0),
    ('E00100', 'warnings at the point of prescribing', 1, 0),
    ('E00101', 'Electronic Prescription Service (EPS) - Prescribing', 1, 0),
    ('E00102', 'record medication not issued by the Healthcare Organisation using prescribable item', 1, 0),
    ('E00103', 'view all medications for a Patient', 1, 0),
    ('E00104', 'manage NHS prescriptions', 1, 0),
    ('E00105', 'create Delayed prescriptions', 1, 0),
    ('E00106', 'early re-issue of medication warning at the point of prescribing', 1, 0),
    ('E00107', 'manage Patient medication regimen reviews', 1, 0),
    ('E00108', 'manage Repeat medication requests', 1, 0),
    ('E00109', 'submit Management Information (MI) data to NHS Digital', 1, 0),
    ('E00110', 'manage custom Formularies', 1, 0),
    ('E00111', 'share custom Formularies between organisations using the same Solution', 1, 0),
    ('E00112', 'set default custom Formulary for Health or Care Professionals', 1, 0),
    ('E00113', 'restrict use of custom Formulary to specific Health or Care Professionals', 1, 0),
    ('E00114', 'age-related default dosage for medication within a custom Formulary', 1, 0),
    ('E00115', 'record medication not issued by the Healthcare Organisation using free-text item', 1, 0),
    ('E00116', 'record medication from Schedule 1 of the NHS regulations', 1, 0),
    ('E00117', 'configure view of all medications for a Patient', 1, 0),
    ('E00118', 'reprint prescriptions', 1, 0),
    ('E00119', 'manage prescribed Private medication', 1, 0),
    ('E00120', 'manage Private prescriptions', 1, 0),
    ('E00121', 'manage Private controlled drug prescriptions', 1, 0),
    ('E00122', 'Repeat issue limitations warning at the point of prescribing', 1, 0),
    ('E00123', 'convert proprietary medication into generic medication at the point of prescribing', 1, 0),
    ('E00124', 'convert generic medication into a proprietary medication at the point of prescribing', 1, 0),
    ('E00125', 'separation of ''as needed'' medication items on NHS Repeatable prescriptions', 1, 0),
    ('E00126', 'manage Patient''s Preferred Pharmacy', 1, 0),
    ('E00127', 'view prescribed medication reports', 1, 0),
    ('E00128', 'view Repeat medication review report', 1, 0),
    ('E00129', 'view medication regimen review report', 1, 0),
    ('E00130', 'manage Acute medication requests', 1, 0),
    ('E00131', 'view not requested Repeat medication report', 1, 0),
    ('E00132', 'view EPS Nominated Pharmacy changes', 1, 0),
    ('E00133', 'configurable warnings for prescribable items', 1, 0),
    ('E00134', 'link medication to a diagnosis', 1, 0),
    ('E00135', 'record information for Consultations', 1, 0),
    ('E00136', 'extract Female Genital Mutilation (FGM) data', 1, 0),
    ('E00137', 'Electronic Yellow Card Reporting', 1, 0),
    ('E00138', 'eMED3 (Fit Notes)', 1, 0),
    ('E00139', 'manage Consultation form templates', 1, 0),
    ('E00140', 'record Consultation using published form template', 1, 0),
    ('E00141', 'Consultation form template version roll back', 1, 0),
    ('E00142', 'upload attachments for Consultations', 1, 0),
    ('E00143', 'share Consultation form templates between organisations using the same Solution', 1, 0),
    ('E00144', 'use Supplier implemented national Consultation form templates', 1, 0),
    ('E00145', 'data recording for Call and Recall processes', 1, 0),
    ('E00146', 'view Calls and Recalls report', 1, 0),
    ('E00147', 'view report of Consultations', 1, 0),
    ('E00148', 'create Referral information for Patients', 1, 0),
    ('E00149', 'e-Referrals Service (e-RS)', 1, 0),
    ('E00150', 'manage e-Referrals', 1, 0),
    ('E00151', 'manage manual Referrals', 1, 0),
    ('E00152', 'Referral reports', 1, 0),
    ('E00153', 'manage my Health or Care Organisation site information', 1, 0),
    ('E00154', 'manage information about Staff Members at my Health or Care Organisation', 1, 0),
    ('E00155', 'manage Staff Member Groups', 1, 0),
    ('E00156', 'manage Staff Members unavailability periods', 1, 0),
    ('E00157', 'manage my Health or Care Organisation''s non-working days and times', 1, 0),
    ('E00158', 'manage Staff Members non-working days and times at my Health or Care Organisation', 1, 0),
    ('E00159', 'manage information about Related Organisations', 1, 0),
    ('E00160', 'manage information about Staff Members at Related Organisations', 1, 0),
    ('E00161', 'manage Non-human Resources', 1, 0),
    ('E00162', 'manage Non-human Resource associations', 1, 0),
    ('E00163', 'manage Patient Registrations', 1, 0),
    ('E00164', 'manage Patient Demographic Information', 1, 0),
    ('E00165', 'manage Electronic Patient Records (EPRs)', 1, 0),
    ('E00166', 'manage Citizen Service Access for a Patient', 1, 0),
    ('E00167', 'manage Identity Verifications for a Patient', 1, 0),
    ('E00168', 'manage Communicate with Practice Citizen Service access for a Patient and the Practice', 1, 0),
    ('E00169', 'manage Appointments Management Citizen Service access for a Patient and the Practice', 1, 0),
    ('E00170', 'manage Prescription Ordering Citizen Service access for a Patient and the Practice', 1, 0),
    ('E00171', 'manage View Record Citizen Service Access for the Practice', 1, 0),
    ('E00172', 'manage View Record Citizen Service Access for a Patient', 1, 0),
    ('E00173', 'manage Communicate With Practice Citizen Service communications', 1, 0),
    ('E00174', 'configure access to elements of the Electronic Patient Record (EPR) in the View Record Citizen Service', 1, 0),
    ('E00175', 'NHAIS HA/GP Links', 1, 0),
    ('E00176', 'Personal Demographics Service (PDS)', 1, 0),
    ('E00177', 'transfer Electronic Patient Records (EPRs) between GP Practices', 1, 0),
    ('E00178', 'Summary Care Record (SCR)', 1, 0),
    ('E00179', 'GP Connect - Access Record', 1, 0),
    ('E00180', 'GP Connect Messaging - Send Document', 1, 0),
    ('E00181', 'NHS 111 CDA messages', 1, 0),
    ('E00182', 'General Practice Extraction Service (GPES)', 1, 0),
    ('E00183', 'Digital Medicines and Pharmacy FHIR Messages', 1, 0),
    ('E00184', 'GP Data for Planning and Research', 1, 0),
    ('E00185', 'submit Primary Care Clinical Terminology Usage data to NHS Digital', 1, 0),
    ('E00186', 'search for Electronic Patient Records (EPRs)', 1, 0),
    ('E00187', 'search content of an Electronic Patient Record (EPR)', 1, 0),
    ('E00188', 'search the contents of documents attached to an Electronic Patient Record (EPR)', 1, 0),
    ('E00189', 'print the Electronic Patient Record (EPR)', 1, 0),
    ('E00190', 'export the Electronic Patient Record (EPR)', 1, 0),
    ('E00191', 'manage a personalised display of Electronic Patient Records (EPRs)', 1, 0),
    ('E00192', 'notifications for outstanding actions on an Electronic Patient Record (EPR)', 1, 0),
    ('E00193', 'manage Patient Related Persons', 1, 0),
    ('E00194', 'manage Patient Problem lists', 1, 0),
    ('E00195', 'manage manual Patient Alerts for Electronic Patient Records (EPRs)', 1, 0),
    ('E00196', 'manage automatic Patient Alerts for Electronic Patient Records (EPRs)', 1, 0),
    ('E00197', 'support Registrations for Private Patients', 1, 0),
    ('E00198', 'support Registrations for Other Services', 1, 0),
    ('E00199', 'manage Subject Access Request (SAR) requests', 1, 0),
    ('E00200', 'view Subject Access Request reports', 1, 0),
    ('E00201', 'verify Patient Contact Details', 1, 0),
    ('E00202', 'configure Welcome Message for Citizen Services', 1, 0),
    ('E00203', 'configure a Disclaimer for Citizens using a Communicate with Practice Citizen Service', 1, 0),
    ('E00204', 'view Citizen Service Usage', 1, 0),
    ('E00205', 'manage Acute Prescription Request Citizen Service for a Patient and the Practice', 1, 0),
    ('E00206', 'manage Citizen Service Access for a Proxy', 1, 0),
    ('E00207', 'manage Identity Verifications for a Proxy', 1, 0),
    ('E00208', 'manage Communicate with Practice Citizen Service access for a Proxy', 1, 0),
    ('E00209', 'manage Appointments Management Citizen Service access for a Proxy', 1, 0),
    ('E00210', 'manage Prescription Ordering Citizen Service access for a Proxy', 1, 0),
    ('E00211', 'manage Acute Prescription Request Citizen Service for a Proxy', 1, 0),
    ('E00212', 'manage View Record Citizen Service Access for a Proxy', 1, 0),
    ('E00213', 'identify Patients outside of Catchment Area', 1, 0),
    ('E00214', 'view Patient Reports', 1, 0),
    ('E00215', 'manage Patient Cohorts from Search Results', 1, 0),
    ('E00216', 'Patient-based report on Capitation', 1, 0),
    ('E00217', 'Patient-based report on childhood vaccinations and immunisations', 1, 0),
    ('E00218', 'Patient-based report on Carers', 1, 0),
    ('E00219', 'manage Appointments for Patients', 1, 0),
    ('E00220', 'manage Sessions', 1, 0),
    ('E00221', 'manage Appointment Slots within a Session', 1, 0),
    ('E00222', 'record National Slot Type Category for Appointment Slots', 1, 0),
    ('E00223', 'record Appointment status', 1, 0),
    ('E00224', 'display a combined view of a Patient''s future and historical Appointments', 1, 0),
    ('E00225', 'display Patient Alerts', 1, 0),
    ('E00226', 'GP Connect - Appointments Management', 1, 0),
    ('E00227', 'General Practice Appointments Data Reporting', 1, 0),
    ('E00229', 'search available Appointment Slots', 1, 0),
    ('E00230', 'view a summary of a Patient''s Appointment history', 1, 0),
    ('E00231', 'record Non-human resources for an Appointment', 1, 0),
    ('E00232', 'book Appointments not linked to a Patient', 1, 0),
    ('E00233', 'record other attendees for an Appointment', 1, 0),
    ('E00234', 're-schedule Appointments', 1, 0),
    ('E00235', 'record non-time bound Appointments', 1, 0),
    ('E00236', 'automatically record status of Did Not Attend (DNA) for Appointments', 1, 0),
    ('E00237', 'configure delayed release Appointment Slots', 1, 0),
    ('E00238', 'filter the combined view of a Patient''s Appointments', 1, 0),
    ('E00239', 'display a combined view of available Appointment Slots and booked Appointments for the Healthcare Organisation', 1, 0),
    ('E00240', 'view Appointment dashboard for the Healthcare Organisation', 1, 0),
    ('E00241', 'configure the Appointment dashboard for the Healthcare Organisation', 1, 0),
    ('E00242', 'export the Appointment dashboard for the Healthcare Organisation', 1, 0),
    ('E00243', 'view Appointment reports', 1, 0),
    ('E00244', 'generate automatic Appointment reminder communications to Patients', 1, 0),
    ('E00245', 'manually generate Appointment communications', 1, 0),
    ('E00246', 'generate automatic Appointment communications when Appointment status is updated', 1, 0),
    ('E00247', 'alerts for Health or Care Professional unavailability periods when creating Appointment slots', 1, 0),
    ('E00248', 'alerts for Health or Care Professional non-working days and times when creating Appointment slots', 1, 0),
    ('E00249', 'alerts for the Healthcare Organisation''s closed periods when creating Appointment slots', 1, 0),
    ('E00250', 'display Appointment alerts when managing Appointments', 1, 0),
    ('E00251', 'manage Session templates', 1, 0),
    ('E00252', 'export Sessions', 1, 0),
    ('E00253', 'share Session templates with other Health or Care Organisations using the same Solution', 1, 0),
    ('E00255', 'display Patient Alerts for Electronic Patient Records (EPRs)', 1, 0),
    ('E00267', 'manage Tasks', 1, 0),
    ('E00268', 'manually create ad-hoc Tasks', 1, 0),
    ('E00269', 'assign Tasks to Health or Care Professionals', 1, 0),
    ('E00270', 'flag overdue Tasks', 1, 0),
    ('E00271', 'manage Task templates', 1, 0),
    ('E00272', 'manage a personalised display for Tasks', 1, 0),
    ('E00273', 'configure Task rules', 1, 0),
    ('E00274', 'view Task reports', 1, 0),
    ('E00277', 'Transfer of Care FHIR Payload', 1, 0),
    ('E00285', 'National Data Opt-Out', 1, 0),
    ('E00305', 'Patient managing Appointments', 1, 0),
    ('E00306', 'Patient viewing future and historical Appointments', 1, 0),
    ('E00307', 'Proxy managing a Patient''s Appointments', 1, 0),
    ('E00308', 'Proxy viewing a Patient''s future and historical Appointments', 1, 0),
    ('E00309', 'add free text when booking an Appointment', 1, 0),
    ('E00310', 'display a Healthcare Organisation configured Welcome Message in Citizen Services', 1, 0),
    ('E00311', 'view the Healthcare Organisation''s details', 1, 0),
    ('E00312', 'view my Citizen Services configuration', 1, 0),
    ('E00313', 'request to amend my Citizen Service Access', 1, 0),
    ('E00314', 'Patient viewing Demographic Information held by the Healthcare Organisation', 1, 0),
    ('E00315', 'Proxy viewing Patient''s Demographic Information held by the Healthcare Organisation', 1, 0),
    ('E00316', 'Patient request to update Demographic Information', 1, 0),
    ('E00317', 'Proxy request to update Patient''s Demographic Information', 1, 0),
    ('E00318', 'Citizen viewing Citizen Services usage information', 1, 0),
    ('E00319', 'Verified User Account (VUA) credentials reminder', 1, 0),
    ('E00320', 'request Verified User Account (VUA) Linkage Key reset', 1, 0),
    ('E00321', 'Patient managing communications', 1, 0),
    ('E00322', 'Proxy managing communications', 1, 0),
    ('E00323', 'include attachments with communications', 1, 0),
    ('E00324', 'organising communications', 1, 0),
    ('E00325', 'notification of received communications', 1, 0),
    ('E00326', 'display a Healthcare Organisation configured Disclaimer when creating communications', 1, 0),
    ('E00327', 'Patient viewing Repeat medications', 1, 0),
    ('E00328', 'Patient managing requests for a Repeat medication to be issued', 1, 0),
    ('E00329', 'Proxy viewing a Patient''s Repeat medications', 1, 0),
    ('E00330', 'Proxy managing requests for a Patient''s Repeat medication to be issued', 1, 0),
    ('E00331', 'add free text information to a request for a Repeat medication to be issued', 1, 0),
    ('E00332', 'add free text information when cancelling a request for a Repeat medication to be issued', 1, 0),
    ('E00333', 'Patient viewing Acute medications', 1, 0),
    ('E00334', 'Patient managing requests for an Acute medication to be issued', 1, 0),
    ('E00335', 'Proxy viewing Patient''s Acute medications', 1, 0),
    ('E00336', 'Proxy managing requests for a Patient''s Acute medication to be issued', 1, 0),
    ('E00337', 'add free text information to a request for an Acute medication to be issued', 1, 0),
    ('E00338', 'add free text information when cancelling a request for an Acute medication to be issued', 1, 0),
    ('E00339', 'view additional medication information', 1, 0),
    ('E00340', 'Patient managing Electronic Prescription Service (EPS) Nominated Pharmacy', 1, 0),
    ('E00341', 'Proxy managing Patient''s Electronic Prescription Service (EPS) Nominated Pharmacy', 1, 0),
    ('E00342', 'Patient managing Preferred Pharmacy', 1, 0),
    ('E00343', 'Proxy managing Patient''s Preferred Pharmacy', 1, 0),
    ('E00344', 'report on a range of data items', 1, 0),
    ('E00345', 'include Synthetic Patients/Service Users in reports', 1, 0),
    ('E00346', 'configure display of report results', 1, 0),
    ('E00347', 'schedule the automated running of reports', 1, 0),
    ('E00348', 'save locally defined reports', 1, 0),
    ('E00349', 'save report results', 1, 0),
    ('E00350', 'export report results', 1, 0),
    ('E00351', 'print report results', 1, 0),
    ('E00352', 'share access to a report within my Health or Care Organisation', 1, 0),
    ('E00353', 'share report with another Health or Care Organisation', 1, 0),
    ('E00354', 'identify the reason a Patient/Service User meets each report criterion for a report', 1, 0),
    ('E00355', 'identify which report criteria a Patient/Service User does not meet', 1, 0),
    ('E00356', 'perform analytics', 1, 0),
    ('E00357', 'Patient viewing Summary Information Record', 1, 0),
    ('E00358', 'Patient viewing Detailed Coded Record', 1, 0),
    ('E00359', 'Patient viewing documents from Electronic Patient Record (EPR)', 1, 0),
    ('E00360', 'Patient viewing Full Patient Record', 1, 0),
    ('E00361', 'Proxy viewing Patient''s Summary Information Record', 1, 0),
    ('E00362', 'Proxy viewing Patient''s Detailed Coded Record', 1, 0),
    ('E00363', 'Proxy viewing documents from Electronic Patient Record (EPR)', 1, 0),
    ('E00364', 'Proxy viewing Patient''s Full Patient Record', 1, 0),
    ('E00365', 'print the available Electronic Patient Record (EPR) content', 1, 0),
    ('E00366', 'export the available Electronic Patient Record (EPR) content', 1, 0),
    ('E00367', 'create communications', 1, 0),
    ('E00368', 'manage communications to Patients/Service Users', 1, 0),
    ('E00369', 'create communications to Health or Care Professionals', 1, 0),
    ('E00370', 'create communications to external organisations', 1, 0),
    ('E00371', 'create communications to a group of recipients', 1, 0),
    ('E00372', 'record communication preferences for Patients/Service Users', 1, 0),
    ('E00373', 'email communications', 1, 0),
    ('E00374', 'SMS message communications', 1, 0),
    ('E00375', 'letter communications', 1, 0),
    ('E00376', 'manage communication templates', 1, 0),
    ('E00377', 'create communications to Patients/Service Users from communication templates', 1, 0),
    ('E00378', 'manage automated communications', 1, 0),
    ('E00379', 'manage incoming communications', 1, 0),
    ('E00380', 'view communication reports', 1, 0),
    ('E00397', 'manage custom Workflows', 1, 0),
    ('E00398', 'configure Workflow rules', 1, 0),
    ('E00399', 'manage Workflow templates', 1, 0),
    ('E00400', 'manually create Workflows from templates', 1, 0),
    ('E00401', 'automatically create Workflows from templates', 1, 0),
    ('E00402', 'create a personalised display for Workflows', 1, 0),
    ('E00403', 'share personalised display for Workflows', 1, 0),
    ('E00404', 'flag overdue Workflows', 1, 0),
    ('E00405', 'view Workflow reports', 1, 0),
    ('E00406', 'scan documents', 1, 0),
    ('E00407', 'document image enhancement', 1, 0),
    ('E00409', 'manually manage documents', 1, 0),
    ('E00410', 'manually manage document and Patient/Service User matching', 1, 0),
    ('E00411', 'manage document information', 1, 0),
    ('E00412', 'manage coded information associated with documents', 1, 0),
    ('E00413', 'manage document annotations', 1, 0),
    ('E00414', 'search for documents based on document information', 1, 0),
    ('E00415', 'search for documents based on document coded information', 1, 0),
    ('E00416', 'search for documents based on document content', 1, 0),
    ('E00417', 'search document content', 1, 0),
    ('E00418', 'visually compare multiple documents', 1, 0),
    ('E00419', 'view all previous versions of documents', 1, 0),
    ('E00420', 'automatically add electronic documents', 1, 0),
    ('E00421', 'manage rules for automatically suggesting document information', 1, 0),
    ('E00422', 'manage automatic suggestions for document information', 1, 0),
    ('E00423', 'manage rules for automatically suggesting document coded information', 1, 0),
    ('E00424', 'manage automatic suggestions for document coded information', 1, 0),
    ('E00425', 'manage automatic suggestions for matching documents and Patients/Service Users', 1, 0),
    ('E00426', 'document Workflows', 1, 0),
    ('E00427', 'print documents', 1, 0),
    ('E00428', 'export documents to new formats', 1, 0),
    ('E00429', 'view document reports', 1, 0),
    ('E00430', 'view national GPES payment extract reports', 1, 0),
    ('E00431', 'view national GPES non-payment extract reports', 1, 0),
    ('E00432', 'manage Requests for Investigations information for Patients', 1, 0),
    ('E00433', 'Digital Diagnostics & Pathology Messaging', 1, 0),
    ('E00434', 'reconcile Investigation Results with Requests for Investigations information', 1, 0),
    ('E00435', 'view Requests for Investigations information reports', 1, 0),
    ('E00436', 'create Request for Investigations information for multiple Patients in a single action', 1, 0),
    ('E00437', 'receive external Requests for Investigations information for Patients', 1, 0),
    ('E00438', 'manage Appointments for Patients/Service Users at other Health or Care Organisations', 1, 0),
    ('E00439', 'view Cross-Organisation Appointment Booking reports', 1, 0),
    ('S00001', '1-Way Video Triage Consultations', 1, 1),
    ('S00002', 'Alert practice if expected response times are exceeded', 1, 1),
    ('S00003', 'Allow practice to set target response times for patient requests', 1, 1),
    ('S00004', 'Allow to track whether a patient has read the practice response.', 1, 1),
    ('S00005', 'Appless Video', 1, 1),
    ('S00006', 'Auto Transcription', 1, 1),
    ('S00007', 'Automated  multiple reminders via multiple channels', 1, 1),
    ('S00008', 'Automatic follow-up reminders', 1, 1),
    ('S00009', 'Capacity & Demand Modelling', 1, 1),
    ('S00010', 'Consultation Countdown', 1, 1),
    ('S00011', 'DIRECT Automatic searching, matching, linking and opening of patient records in TPP SystmOne, Emis Web, Cegedim Vision - Lan & Aeros', 1, 1),
    ('S00012', 'Display a warning to a patient when they are uploading an image', 1, 1),
    ('S00013', 'Event Log', 1, 1),
    ('S00014', 'Facility for clinician to take notes during video consultation', 1, 1),
    ('S00015', 'Filtering, sorting and searching all incoming messages', 1, 1),
    ('S00016', 'In call video tools - video blurring', 1, 1),
    ('S00017', 'In call video tools - zoom', 1, 1),
    ('S00018', 'Instant Forwarding', 1, 1),
    ('S00019', 'Instant Location', 1, 1),
    ('S00020', 'Instant Vital Signs', 1, 1),
    ('S00021', 'Out of  Hours (OOH) Requests', 1, 1),
    ('S00022', 'Pop Out Floating Video', 1, 1),
    ('S00023', 'Preset Messages', 1, 1),
    ('S00024', 'Remote Monitoring', 1, 1),
    ('S00025', 'Reporting', 1, 1),
    ('S00026', 'Reporting Dashboards', 1, 1),
    ('S00027', 'Search online consultations', 1, 1),
    ('S00028', 'Take screen captures during video consultation', 1, 1),
    ('S00029', 'To determine when a patient has read a practice response, if at all.', 1, 1),
    ('S00030', 'Vidtu video consultation solution', 1, 1),
    ('S00031', 'Viewing messages sent to patients', 1, 1),
    ('S020X01E01', 'Supplier-defined epic 1', 1, 1),
    ('S020X01E02', 'Supplier-defined epic 2', 1, 1),
    ('S020X01E03', 'Supplier-defined epic 3', 1, 1),
    ('S020X01E04', 'Supplier-defined epic 4', 1, 1),
    ('S020X01E05', 'Supplier-defined epic 5', 1, 1),
    ('S020X01E06', 'Supplier-defined epic 6', 1, 1),
    ('S020X01E07', 'Supplier-defined epic 7', 0, 1),
    ('S020X01E08', 'Supplier-defined epic 8', 1, 1)

    INSERT INTO @capabilityEpics(EpicId, CapabilityId, CompliancyLevelId)
    VALUES
    ('C10E1', 10, 1), -- View GPES payment extract reports -> GP Extracts Verification
    ('C10E2', 10, 3), -- View national GPES non-payment extract reports -> GP Extracts Verification
    ('C11E1', 11, 1), -- Manage Referrals -> Referral Management - GP
    ('C11E2', 11, 1), -- View Referral reports -> Referral Management - GP
    ('C12E1', 12, 1), -- Manage General Practice and Branch site information -> Resource Management
    ('C12E2', 12, 1), -- Manage General Practice Staff Members -> Resource Management
    ('C12E3', 12, 1), -- Manage Staff Member inactivity periods -> Resource Management
    ('C12E4', 12, 1), -- Manage Staff Member Groups -> Resource Management
    ('C12E5', 12, 1), -- Manage Related Organisations information -> Resource Management
    ('C12E6', 12, 1), -- Manage Related Organisation Staff Members -> Resource Management
    ('C12E7', 12, 3), -- Manage Non-human Resources -> Resource Management
    ('C13E1', 13, 1), -- Manage Patients  -> Patient Information Maintenance - GP
    ('C13E10', 13, 1), -- Configure Citizen service access for the Practice -> Patient Information Maintenance - GP
    ('C13E11', 13, 1), -- Identify Patients outside of Catchment Area -> Patient Information Maintenance - GP
    ('C13E12', 13, 1), -- Manage Patient Cohorts from Search Results -> Patient Information Maintenance - GP
    ('C13E13', 13, 3), -- View Subject Access Request reports -> Patient Information Maintenance - GP
    ('C13E14', 13, 3), -- Manage Acute Prescription Request Service -> Patient Information Maintenance - GP
    ('C13E15', 13, 3), -- Notify the Patient of changes -> Patient Information Maintenance - GP
    ('C13E16', 13, 3), -- Manage Subject Access Request (SAR) requests -> Patient Information Maintenance - GP
    ('C13E17', 13, 3), -- Notify the Proxy of changes -> Patient Information Maintenance - GP
    ('C13E18', 13, 3), -- Manage Practice notifications – Proxy -> Patient Information Maintenance - GP
    ('C13E19', 13, 3), -- Configure Proxy notifications -> Patient Information Maintenance - GP
    ('C13E2', 13, 1), -- Access Patient Record -> Patient Information Maintenance - GP
    ('C13E20', 13, 3), -- Manage Proxy Communications -> Patient Information Maintenance - GP
    ('C13E21', 13, 3), -- Manage Proxys for Citizen Services -> Patient Information Maintenance - GP
    ('C13E3', 13, 1), -- Manage Patient Related Persons -> Patient Information Maintenance - GP
    ('C13E4', 13, 1), -- Manage Patients for Citizen Services -> Patient Information Maintenance - GP
    ('C13E5', 13, 1), -- Manage Patient Communications -> Patient Information Maintenance - GP
    ('C13E6', 13, 1), -- Configure Patient notifications -> Patient Information Maintenance - GP
    ('C13E7', 13, 1), -- Manage Practice notifications – Patient -> Patient Information Maintenance - GP
    ('C13E8', 13, 1), -- Search for Patient Records -> Patient Information Maintenance - GP
    ('C13E9', 13, 1), -- View Patient Reports -> Patient Information Maintenance - GP
    ('C14E1', 14, 1), -- Access prescribable items -> Prescribing
    ('C14E10', 14, 1), -- Manage Repeat Medication requests -> Prescribing
    ('C14E11', 14, 1), -- Manage Acute Medication requests -> Prescribing
    ('C14E12', 14, 1), -- Manage Authorising Prescribers -> Prescribing
    ('C14E13', 14, 1), -- Access Patient Record -> Prescribing
    ('C14E14', 14, 3), -- View EPS Nominated Pharmacy changes -> Prescribing
    ('C14E15', 14, 3), -- Configure warnings for prescribable items -> Prescribing
    ('C14E16', 14, 3), -- Medications are linked to diagnoses -> Prescribing
    ('C14E2', 14, 1), -- Manage Formularies -> Prescribing
    ('C14E3', 14, 1), -- Manage shared Formularies -> Prescribing
    ('C14E4', 14, 1), -- Set default Formulary for Practice Users -> Prescribing
    ('C14E5', 14, 1), -- Manage prescribed medication -> Prescribing
    ('C14E6', 14, 1), -- Manage prescriptions -> Prescribing
    ('C14E7', 14, 1), -- Manage Patient's Preferred Pharmacy -> Prescribing
    ('C14E8', 14, 1), -- Manage Patient medication reviews -> Prescribing
    ('C14E9', 14, 1), -- View prescribed medication reports -> Prescribing
    ('C15E1', 15, 1), -- Record Consultation information -> Recording Consultations - GP
    ('C15E2', 15, 1), -- View report on calls and recalls -> Recording Consultations - GP
    ('C15E3', 15, 1), -- View report of Consultations -> Recording Consultations - GP
    ('C15E4', 15, 1), -- Access Patient Record -> Recording Consultations - GP
    ('C15E5', 15, 1), -- Manage Consultation form templates -> Recording Consultations - GP
    ('C15E6', 15, 1), -- Share Consultation form templates -> Recording Consultations - GP
    ('C15E7', 15, 1), -- Use supplier implemented national Consultation form templates -> Recording Consultations - GP
    ('C15E8', 15, 1), -- Extract Female Genital Mutilation data -> Recording Consultations - GP
    ('C16E1', 16, 1), -- Report data from other Capabilities -> Reporting
    ('C17E1', 17, 1), -- Scan documents -> Scanning
    ('C17E2', 17, 3), -- Image enhancement -> Scanning
    ('C18E1', 18, 1), -- share monitoring data with my General Practice -> Telehealth
    ('C18E2', 18, 1), -- configure Telehealth for the Practice -> Telehealth
    ('C18E3', 18, 1), -- configure Telehealth for the Patient -> Telehealth
    ('C18E4', 18, 1), -- manage incoming Telehealth data -> Telehealth
    ('C19E1', 19, 1), -- Document classification -> Unstructured Data Extraction
    ('C19E2', 19, 1), -- Manage Document Classification rules -> Unstructured Data Extraction
    ('C19E3', 19, 1), -- Document and Patient matching -> Unstructured Data Extraction
    ('C1E1', 1, 1), -- Manage Appointments -> Appointments Management - Citizen
    ('C1E2', 1, 3), -- Manage Appointments by Proxy -> Appointments Management - Citizen
    ('C20E1', 20, 1), -- Manage Task templates -> Custom Workflows
    ('C20E10', 20, 1), -- View Workflow reports -> Custom Workflows
    ('C20E11', 20, 1), -- Access Patient Record -> Custom Workflows
    ('C20E12', 20, 3), -- Share Task List configuration -> Custom Workflows
    ('C20E13', 20, 3), -- Share Workflow List configuration -> Custom Workflows
    ('C20E2', 20, 1), -- Manage Workflow templates -> Custom Workflows
    ('C20E3', 20, 1), -- Configure Task rules -> Custom Workflows
    ('C20E4', 20, 1), -- Configure Workflow rules -> Custom Workflows
    ('C20E5', 20, 1), -- Manage Tasks -> Custom Workflows
    ('C20E6', 20, 1), -- Manage Workflows -> Custom Workflows
    ('C20E7', 20, 1), -- Manage Task List configurations -> Custom Workflows
    ('C20E8', 20, 1), -- Manage Workflows List configurations -> Custom Workflows
    ('C20E9', 20, 1), -- View Task reports -> Custom Workflows
    ('C21E1', 21, 1), -- maintain Resident's Care Home Record -> Care Homes
    ('C21E2', 21, 3), -- maintain Resident Proxy preferences -> Care Homes
    ('C21E3', 21, 3), -- view and maintain End of Life Care Plans -> Care Homes
    ('C21E4', 21, 3), -- record incident and adverse events -> Care Homes
    ('C21E5', 21, 3), -- maintain Staff Records -> Care Homes
    ('C21E6', 21, 3), -- maintain Staff Task schedules -> Care Homes
    ('C21E7', 21, 3), -- manage Tasks -> Care Homes
    ('C21E8', 21, 3), -- reporting -> Care Homes
    ('C22E1', 22, 1), -- manage Cases -> Caseload Management
    ('C22E2', 22, 1), -- maintain Caseloads -> Caseload Management
    ('C22E3', 22, 1), -- generate and manage contact schedules -> Caseload Management
    ('C22E4', 22, 1), -- update Case details -> Caseload Management
    ('C22E5', 22, 3), -- review and comment on Caseload -> Caseload Management
    ('C22E6', 22, 3), -- review and comment on contact schedule -> Caseload Management
    ('C22E7', 22, 3), -- view and update Patient/Service User's Health or Care Record -> Caseload Management
    ('C22E8', 22, 3), -- reporting -> Caseload Management
    ('C22E9', 22, 3), -- care Pathway templates -> Caseload Management
    ('C23E1', 23, 1), -- Make Appointments available to external organisations -> Cross-organisation Appointment Booking
    ('C23E2', 23, 1), -- Search externally bookable Appointment slots -> Cross-organisation Appointment Booking
    ('C23E3', 23, 1), -- Book externally bookable Appointment slots -> Cross-organisation Appointment Booking
    ('C23E4', 23, 1), -- Maintain Appointments -> Cross-organisation Appointment Booking
    ('C23E5', 23, 3), -- Notifications -> Cross-organisation Appointment Booking
    ('C23E6', 23, 3), -- Manage Appointment Requests -> Cross-organisation Appointment Booking
    ('C23E7', 23, 3), -- Booking approval -> Cross-organisation Appointment Booking
    ('C23E8', 23, 3), -- Report on usage of Cross-Organisation Appointments -> Cross-organisation Appointment Booking
    ('C23E9', 23, 3), -- Manage Cross-Organisation Appointment Booking templates -> Cross-organisation Appointment Booking
    ('C24E1', 24, 1), -- use Workflow to run a Cross-organisational Process -> Cross-organisation Workflow Tools
    ('C24E2', 24, 1), -- maintain cross-organisational workflows -> Cross-organisation Workflow Tools
    ('C24E3', 24, 3), -- maintain cross-organisational workflow templates -> Cross-organisation Workflow Tools
    ('C24E4', 24, 3), -- share workflow templates -> Cross-organisation Workflow Tools
    ('C24E5', 24, 3), -- manage automated notifications and reminders -> Cross-organisation Workflow Tools
    ('C24E6', 24, 3), -- manage ad-hoc notifications -> Cross-organisation Workflow Tools
    ('C24E7', 24, 3), -- report on Cross-organisational Workflows -> Cross-organisation Workflow Tools
    ('C25E1', 25, 1), -- maintain service schedule -> Cross-organisation Workforce Management
    ('C25E2', 25, 1), -- share service schedule -> Cross-organisation Workforce Management
    ('C25E3', 25, 3), -- workforce management reporting -> Cross-organisation Workforce Management
    ('C26E1', 26, 1), -- analyse data across multiple organisations within the Integrated/Federated Care Setting (Federation) -> Data Analytics for Integrated and Federated Care
    ('C26E10', 26, 3), -- enable reporting at different levels -> Data Analytics for Integrated and Federated Care
    ('C26E2', 26, 1), -- analyse data across different datasets -> Data Analytics for Integrated and Federated Care
    ('C26E3', 26, 1), -- create new or update existing reports  -> Data Analytics for Integrated and Federated Care
    ('C26E4', 26, 1), -- run existing reports -> Data Analytics for Integrated and Federated Care
    ('C26E5', 26, 1), -- present output -> Data Analytics for Integrated and Federated Care
    ('C26E6', 26, 1), -- define selection rules on reports -> Data Analytics for Integrated and Federated Care
    ('C26E7', 26, 3), -- create and run performance-based reports -> Data Analytics for Integrated and Federated Care
    ('C26E8', 26, 3), -- drill down to detailed information -> Data Analytics for Integrated and Federated Care
    ('C26E9', 26, 3), -- forecasting -> Data Analytics for Integrated and Federated Care
    ('C27E1', 27, 1), -- maintain Domiciliary Care schedules -> Domiciliary Care
    ('C27E2', 27, 1), -- share Domiciliary Care schedules -> Domiciliary Care
    ('C27E3', 27, 1), -- manage Appointments -> Domiciliary Care
    ('C27E4', 27, 3), -- Service User manages their schedule for Domiciliary Care -> Domiciliary Care
    ('C27E5', 27, 3), -- manage Care Plans for Service Users -> Domiciliary Care
    ('C27E6', 27, 3), -- remote access to Domiciliary Care schedule -> Domiciliary Care
    ('C27E7', 27, 3), -- receive notifications relating to Service User -> Domiciliary Care
    ('C27E8', 27, 3), -- reports -> Domiciliary Care
    ('C27E9', 27, 3), -- nominated individuals to view Domiciliary Care schedule  -> Domiciliary Care
    ('C29E1', 29, 1), -- Health or Care Professional requests support -> e-Consultations (Professional to Professional)
    ('C29E2', 29, 1), -- respond to request for support from another Health or Care Professional -> e-Consultations (Professional to Professional)
    ('C29E3', 29, 3), -- link additional information to a request for support -> e-Consultations (Professional to Professional)
    ('C29E4', 29, 3), -- live Consultation: Health and Care Professionals -> e-Consultations (Professional to Professional)
    ('C29E5', 29, 3), -- link Consultation to Patient/Service User's Record -> e-Consultations (Professional to Professional)
    ('C29E6', 29, 3), -- reports -> e-Consultations (Professional to Professional)
    ('C2E1', 2, 1), -- Manage communications – Patient -> Communicate With Practice - Citizen
    ('C2E2', 2, 3), -- Manage communications – Proxy -> Communicate With Practice - Citizen
    ('C30E1', 30, 1), -- single unified medication view -> Medicines Optimisation
    ('C30E10', 30, 3), -- access national or local Medicines Optimisation guidance -> Medicines Optimisation
    ('C30E11', 30, 3), -- prescribing decision support -> Medicines Optimisation
    ('C30E12', 30, 3), -- Medicines Optimisation reporting -> Medicines Optimisation
    ('C30E13', 30, 3), -- configure notifications for required Medicines Reviews -> Medicines Optimisation
    ('C30E14', 30, 3), -- receive notification for required medicines reviews -> Medicines Optimisation
    ('C30E2', 30, 1), -- request medication changes -> Medicines Optimisation
    ('C30E3', 30, 3), -- identify Patients requiring medicines review -> Medicines Optimisation
    ('C30E4', 30, 3), -- maintain medicines review -> Medicines Optimisation
    ('C30E5', 30, 3), -- notify Patient and Proxies of medication changes -> Medicines Optimisation
    ('C30E6', 30, 3), -- notify other interested parties of medication changes -> Medicines Optimisation
    ('C30E7', 30, 3), -- configure medication substitutions -> Medicines Optimisation
    ('C30E8', 30, 3), -- use pre-configured medication substitutions -> Medicines Optimisation
    ('C30E9', 30, 3), -- maintain prescribed medication -> Medicines Optimisation
    ('C32E1', 32, 1), -- manage Personal Health Budget -> Personal Health Budget
    ('C32E10', 32, 3), -- manage multiple budgets -> Personal Health Budget
    ('C32E11', 32, 3), -- link to Patient Record -> Personal Health Budget
    ('C32E12', 32, 3), -- link to Workflow -> Personal Health Budget
    ('C32E13', 32, 3), -- provider view -> Personal Health Budget
    ('C32E14', 32, 3), -- Management Information -> Personal Health Budget
    ('C32E15', 32, 3), -- identify candidates for Personal Health Budgets -> Personal Health Budget
    ('C32E2', 32, 1), -- record Personal Health Budget purchases -> Personal Health Budget
    ('C32E3', 32, 1), -- assess Personal Health Budgets -> Personal Health Budget
    ('C32E4', 32, 3), -- link Personal Health Budget with care plan -> Personal Health Budget
    ('C32E5', 32, 3), -- support different models for management of Personal Health Budgets -> Personal Health Budget
    ('C32E6', 32, 3), -- apply criteria for the use of Personal Health Budgets -> Personal Health Budget
    ('C32E7', 32, 3), -- payments under Personal Health Budgets -> Personal Health Budget
    ('C32E8', 32, 3), -- maintain directory of equipment, treatments and services -> Personal Health Budget
    ('C32E9', 32, 3), -- search a directory of equipment, treatments and services -> Personal Health Budget
    ('C33E1', 33, 1), -- maintain Personal Health Record content -> Personal Health Record
    ('C33E2', 33, 3), -- organise Personal Health Record  -> Personal Health Record
    ('C33E3', 33, 3), -- manage access to Personal Health Record -> Personal Health Record
    ('C33E4', 33, 3), -- manage data coming into Personal Health Record -> Personal Health Record
    ('C34E1', 34, 1), -- access healthcare data -> Population Health Management
    ('C34E2', 34, 1), -- maintain cohorts -> Population Health Management
    ('C34E3', 34, 1), -- stratify population by risk -> Population Health Management
    ('C34E4', 34, 1), -- data analysis and reporting -> Population Health Management
    ('C34E5', 34, 1), -- outcomes -> Population Health Management
    ('C34E6', 34, 3), -- dashboard -> Population Health Management
    ('C35E1', 35, 1), -- run Risk Stratification algorithms -> Risk Stratification
    ('C36E1', 36, 1), -- create Shared Care Plan -> Shared Care Plans
    ('C36E10', 36, 3), -- reports -> Shared Care Plans
    ('C36E11', 36, 3), -- manage Shared Care Plan templates -> Shared Care Plans
    ('C36E12', 36, 3), -- manage care schedules -> Shared Care Plans
    ('C36E2', 36, 1), -- view Shared Care Plan -> Shared Care Plans
    ('C36E3', 36, 1), -- amend Shared Care Plan -> Shared Care Plans
    ('C36E4', 36, 1), -- close Shared Care Plan  -> Shared Care Plans
    ('C36E5', 36, 3), -- assign Shared Care Plan actions -> Shared Care Plans
    ('C36E6', 36, 3), -- access Shared Care Plans remotely -> Shared Care Plans
    ('C36E7', 36, 3), -- aearch and view Shared Care Plans -> Shared Care Plans
    ('C36E8', 36, 3), -- real-time access to Shared Care Plans -> Shared Care Plans
    ('C36E9', 36, 3), -- notifications -> Shared Care Plans
    ('C37E1', 37, 1), -- assess wellness or well-being of the Patient or Service User -> Social Prescribing
    ('C37E10', 37, 3), -- Patient self-referral -> Social Prescribing
    ('C37E11', 37, 3), -- integrate Social Prescribing Referral Record with Clinical Record -> Social Prescribing
    ('C37E12', 37, 3), -- receive notification of an Appointment -> Social Prescribing
    ('C37E13', 37, 3), -- remind Patients/Service Users of Appointments -> Social Prescribing
    ('C37E14', 37, 3), -- provide service feedback -> Social Prescribing
    ('C37E15', 37, 3), -- view service feedback -> Social Prescribing
    ('C37E16', 37, 3), -- Obtain Management Information (MI) on Social Prescribing -> Social Prescribing
    ('C37E2', 37, 1), -- search the directory -> Social Prescribing
    ('C37E3', 37, 1), -- refer Patient/Service User to service(s) -> Social Prescribing
    ('C37E4', 37, 1), -- maintain referral record -> Social Prescribing
    ('C37E5', 37, 1), -- link to national or local directory of services -> Social Prescribing
    ('C37E6', 37, 1), -- maintain directory of services -> Social Prescribing
    ('C37E7', 37, 1), -- maintain service criteria -> Social Prescribing
    ('C37E8', 37, 3), -- refer Patient/Service User to Link Worker -> Social Prescribing
    ('C37E9', 37, 3), -- capture Patient/Service User consent -> Social Prescribing
    ('C38E1', 38, 1), -- define response to event -> Telecare
    ('C38E10', 38, 3), -- manual testing of Telecare device -> Telecare
    ('C38E11', 38, 3), -- sustainability of Telecare device -> Telecare
    ('C38E2', 38, 1), -- monitor and alert -> Telecare
    ('C38E3', 38, 1), -- receive alerts -> Telecare
    ('C38E4', 38, 1), -- meet availability targets -> Telecare
    ('C38E5', 38, 3), -- ease of use -> Telecare
    ('C38E6', 38, 3), -- Patient/Service Users with sensory impairment(s) -> Telecare
    ('C38E7', 38, 3), -- obtain Management Information (MI) on Telecare -> Telecare
    ('C38E8', 38, 3), -- enable 2-way communication with Patient/Service User -> Telecare
    ('C38E9', 38, 3), -- remote testing of Telecare device -> Telecare
    ('C39E1', 39, 1), -- view Unified Care Record -> Unified Care Record
    ('C39E2', 39, 3), -- Patient/Service User views the Unified Care Record -> Unified Care Record
    ('C39E3', 39, 3), -- default Views -> Unified Care Record
    ('C3E1', 3, 1), -- Manage Repeat Medications – Patient -> Prescription Ordering - Citizen
    ('C3E10', 3, 3), -- View medication information as a Proxy -> Prescription Ordering - Citizen
    ('C3E2', 3, 1), -- Manage my nominated EPS pharmacy -> Prescription Ordering - Citizen
    ('C3E3', 3, 3), -- Manage my Preferred PharmacyAs a Patient -> Prescription Ordering - Citizen
    ('C3E4', 3, 3), -- Manage Acute Medications -> Prescription Ordering - Citizen
    ('C3E5', 3, 3), -- View medication information -> Prescription Ordering - Citizen
    ('C3E6', 3, 3), -- Manage Repeat Medications as a Proxy -> Prescription Ordering - Citizen
    ('C3E7', 3, 3), -- Manage nominated EPS pharmacy as a Proxy -> Prescription Ordering - Citizen
    ('C3E8', 3, 3), -- Manage Preferred Pharmacy as a Proxy -> Prescription Ordering - Citizen
    ('C3E9', 3, 3), -- Manage Acute Medications as a Proxy -> Prescription Ordering - Citizen
    ('C40E1', 40, 1), -- Verify Medicinal Product Unique Identifiers -> Medicines Verification
    ('C40E2', 40, 1), -- Decommission Medicinal Products -> Medicines Verification
    ('C40E3', 40, 3), -- Record the integrity of Anti-tampering Devices -> Medicines Verification
    ('C42E1', 42, 1), -- manage Stock in a Dispensary -> Dispensing
    ('C42E2', 42, 1), -- manage Stock Orders -> Dispensing
    ('C42E3', 42, 1), -- manage Dispensing tasks for a Dispensary -> Dispensing
    ('C42E4', 42, 1), -- dispense Medication -> Dispensing
    ('C42E5', 42, 1), -- manage Dispensaries -> Dispensing
    ('C42E6', 42, 1), -- manage Endorsements -> Dispensing
    ('C42E7', 42, 1), -- manage Supplier Accounts -> Dispensing
    ('C42E8', 42, 1), -- view Stock reports -> Dispensing
    ('C45E1', 45, 1), -- identify COVID-19 vaccination cohorts -> Cohort Identification
    ('C45E2', 45, 1), -- verify Patient information using Personal Demographics Service (PDS) -> Cohort Identification
    ('C45E3', 45, 1), -- import or consume COVID-19 vaccination data for Patients -> Cohort Identification
    ('C45E4', 45, 3), -- extract COVID-19 vaccination cohorts data in .CSV file format -> Cohort Identification
    ('C45E5', 45, 3), -- bulk send SMS messages for COVID-19 vaccination invite communications -> Cohort Identification
    ('C45E6', 45, 3), -- bulk create letters for COVID-19 vaccination invite communications -> Cohort Identification
    ('C45E7', 45, 3), -- bulk send email for COVID-19 vaccination invite communications -> Cohort Identification
    ('C45E8', 45, 3), -- automatically record which Patients have had COVID-19 vaccination invites created -> Cohort Identification
    ('C45E9', 45, 3), -- view whether Patients have had a COVID-19 vaccination invite communication created -> Cohort Identification
    ('C46E1', 46, 1), -- define appointment availability for a vaccination site -> PCN Appointments Management - Vaccinations
    ('C46E10', 46, 3), -- automatically record which Patients have had COVID-19 vaccination invites created -> PCN Appointments Management - Vaccinations
    ('C46E11', 46, 3), -- view whether Patients have had a COVID-19 vaccination invite communication created -> PCN Appointments Management - Vaccinations
    ('C46E12', 46, 3), -- automatically bulk send booking reminders to Patients via SMS messages for COVID-19 vaccination invites -> PCN Appointments Management - Vaccinations
    ('C46E13', 46, 3), -- automatically bulk create booking reminders to Patients as letters for COVID-19 vaccination invites -> PCN Appointments Management - Vaccinations
    ('C46E14', 46, 3), -- automatically bulk send booking reminders to Patients via email for COVID-19 vaccination invites -> PCN Appointments Management - Vaccinations
    ('C46E15', 46, 3), -- book Appointments across Solutions using GP Connect Appointments Management -> PCN Appointments Management - Vaccinations
    ('C46E16', 46, 3), -- Patients can book their own COVID-19 vaccination appointments -> PCN Appointments Management - Vaccinations
    ('C46E17', 46, 3), -- Patients can re-schedule their own future COVID-19 vaccination appointment -> PCN Appointments Management - Vaccinations
    ('C46E18', 46, 3), -- Patients can cancel their own future COVID-19 vaccination appointment -> PCN Appointments Management - Vaccinations
    ('C46E19', 46, 3), -- automatically send booking notifications to Patients via SMS messages for COVID-19 vaccination appointments -> PCN Appointments Management - Vaccinations
    ('C46E2', 46, 1), -- book vaccination appointments for eligible Patients registered across different GP Practices -> PCN Appointments Management - Vaccinations
    ('C46E20', 46, 3), -- automatically create booking notifications to Patients as letters for vaccination appointments -> PCN Appointments Management - Vaccinations
    ('C46E21', 46, 3), -- automatically send booking notifications to Patients via email for vaccination appointments -> PCN Appointments Management - Vaccinations
    ('C46E22', 46, 3), -- create ad-hoc booking notifications to Patients for vaccination appointments -> PCN Appointments Management - Vaccinations
    ('C46E23', 46, 3), -- automatically bulk send appointment reminders to Patients via SMS messages for vaccination appointments -> PCN Appointments Management - Vaccinations
    ('C46E24', 46, 3), -- automatically bulk create booking reminders to Patients as letters for vaccination appointments -> PCN Appointments Management - Vaccinations
    ('C46E25', 46, 3), -- automatically bulk send appointment reminders to Patients via email for vaccination appointments -> PCN Appointments Management - Vaccinations
    ('C46E26', 46, 3), -- send ad-hoc appointment reminders to Patients for vaccination appointments -> PCN Appointments Management - Vaccinations
    ('C46E27', 46, 3), -- view all booked vaccination appointments for a specified time period -> PCN Appointments Management - Vaccinations
    ('C46E28', 46, 3), -- export all booked vaccination appointments for a specified time period -> PCN Appointments Management - Vaccinations
    ('C46E29', 46, 3), -- cancel booked vaccination appointments for Patients -> PCN Appointments Management - Vaccinations
    ('C46E3', 46, 1), -- record that a vaccination appointment for a Patient has been completed -> PCN Appointments Management - Vaccinations
    ('C46E30', 46, 3), -- re-schedule booked vaccination appointments for Patients -> PCN Appointments Management - Vaccinations
    ('C46E31', 46, 3), -- automatically send appointment cancellation notifications to Patients via SMS messages for appointments -> PCN Appointments Management - Vaccinations
    ('C46E32', 46, 3), -- automatically create appointment cancellation notifications to Patients as letters for appointments -> PCN Appointments Management - Vaccinations
    ('C46E33', 46, 3), -- automatically send appointment cancellation notifications to Patients via email for appointments -> PCN Appointments Management - Vaccinations
    ('C46E4', 46, 1), -- extract vaccination appointments data for NHS Digital -> PCN Appointments Management - Vaccinations
    ('C46E5', 46, 3), -- import vaccination Patient cohorts data via .CSV file -> PCN Appointments Management - Vaccinations
    ('C46E6', 46, 3), -- verify Patient information using Personal Demographics Service (PDS) -> PCN Appointments Management - Vaccinations
    ('C46E7', 46, 3), -- bulk send SMS messages for vaccination invite communications -> PCN Appointments Management - Vaccinations
    ('C46E8', 46, 3), -- bulk create letters for vaccination invite communications -> PCN Appointments Management - Vaccinations
    ('C46E9', 46, 3), -- bulk send email for vaccination invite communications -> PCN Appointments Management - Vaccinations
    ('C47E1', 47, 1), -- record structured vaccination data at the point of care for Patients registered at different GP Practices -> Vaccination and Adverse Reaction Recording
    ('C47E10', 47, 3), -- view Summary Care Record (SCR) for a Patient -> Vaccination and Adverse Reaction Recording
    ('C47E11', 47, 3), -- scanning of a GS1 barcode when recording vaccination data -> Vaccination and Adverse Reaction Recording
    ('C47E12', 47, 3), -- record structured vaccination data at the point of care directly into GP Patient Record -> Vaccination and Adverse Reaction Recording
    ('C47E13', 47, 3), -- record structured adverse reaction data at the point of care directly into GP Patient Record -> Vaccination and Adverse Reaction Recording
    ('C47E14', 47, 1), -- verify Patient information using Personal Demographics Service (PDS) -> Vaccination and Adverse Reaction Recording
    ('C47E15', 47, 1), -- latest Clinical Screening Questions at the point of care for Patients registered at different GP Practices -> Vaccination and Adverse Reaction Recording
    ('C47E16', 47, 1), -- record structured vaccination data at the point of care for Patients using pre-configured vaccine batches -> Vaccination and Adverse Reaction Recording
    ('C47E17', 47, 1), -- view vaccination information for a Patient held by the National Immunisation Management Service (NIMS) at point of care -> Vaccination and Adverse Reaction Recording
    ('C47E18', 47, 1), -- update previously recorded structured vaccination and adverse reaction data for Patients -> Vaccination and Adverse Reaction Recording
    ('C47E19', 47, 1), -- extract COVID-19 Extended Attributes data for NHS Digital Extended Attributes Extract -> Vaccination and Adverse Reaction Recording
    ('C47E2', 47, 1), -- record structured adverse reaction data at the point of care for Patients registered at different GP Practices -> Vaccination and Adverse Reaction Recording
    ('C47E20', 47, 1), -- view reports on vaccination data -> Vaccination and Adverse Reaction Recording
    ('C47E3', 47, 1), -- extract vaccination data for NHS Digital Daily Clinical Vaccination Extract -> Vaccination and Adverse Reaction Recording
    ('C47E4', 47, 1), -- extract adverse reaction data for NHS Digital Daily Clinical Adverse Reaction Extract -> Vaccination and Adverse Reaction Recording
    ('C47E5', 47, 1), -- automatically send vaccination data to Patient's registered GP Practice Foundation Solution using Digital Medicines FHIR messages -> Vaccination and Adverse Reaction Recording
    ('C47E6', 47, 1), -- automatically send adverse reaction data to Patient's registered GP Practice Foundation Solution using Digital Medicines FHIR messages -> Vaccination and Adverse Reaction Recording
    ('C47E7', 47, 1), -- automatically send vaccination data to the NHS Business Services Authority (NHSBSA) -> Vaccination and Adverse Reaction Recording
    ('C47E8', 47, 3), -- view information from the GP Patient Record using GP Connect Access Record HTML -> Vaccination and Adverse Reaction Recording
    ('C47E9', 47, 3), -- view information from the GP Patient Record held by the same Solution -> Vaccination and Adverse Reaction Recording
    ('C4E1', 4, 1), -- View Patient Record – Patient -> View Record - Citizen
    ('C4E2', 4, 3), -- View Patient Record – Proxy -> View Record - Citizen
    ('C5E1', 5, 1), -- Manage Session templates -> Appointments Management - GP
    ('C5E2', 5, 1), -- Manage Sessions -> Appointments Management - GP
    ('C5E3', 5, 1), -- Configure Appointments -> Appointments Management - GP
    ('C5E4', 5, 1), -- Practice configuration -> Appointments Management - GP
    ('C5E5', 5, 1), -- Manage Appointments -> Appointments Management - GP
    ('C5E6', 5, 1), -- View Appointment reports -> Appointments Management - GP
    ('C5E7', 5, 1), -- Access Patient Record -> Appointments Management - GP
    ('C6E1', 6, 1), -- access to Clinical Decision Support -> Clinical Decision Support
    ('C6E2', 6, 1), -- local configuration for Clinical Decision Support triggering -> Clinical Decision Support
    ('C6E3', 6, 1), -- view Clinical Decision Support reports -> Clinical Decision Support
    ('C6E4', 6, 3), -- configuration for custom Clinical Decision Support -> Clinical Decision Support
    ('C7E1', 7, 1), -- Manage communication consents for a Patient -> Communication Management
    ('C7E10', 7, 3), -- Manage incoming communications -> Communication Management
    ('C7E2', 7, 1), -- Manage communication preferences for a Patient -> Communication Management
    ('C7E3', 7, 1), -- Manage communication templates -> Communication Management
    ('C7E4', 7, 1), -- Create communications -> Communication Management
    ('C7E5', 7, 1), -- Manage automated communications -> Communication Management
    ('C7E6', 7, 1), -- View communication reports -> Communication Management
    ('C7E7', 7, 1), -- Access Patient Record -> Communication Management
    ('C7E8', 7, 3), -- Manage communication consents for a Proxy -> Communication Management
    ('C7E9', 7, 3), -- Manage communication preferences for a Proxy -> Communication Management
    ('C8E1', 8, 1), -- Manage Requests for Investigations -> Digital Diagnostics
    ('C8E2', 8, 1), -- View Requests for Investigations reports -> Digital Diagnostics
    ('C8E3', 8, 3), -- Create a Request for Investigation for multiple Patients -> Digital Diagnostics
    ('C8E4', 8, 3), -- Receive external Request for Investigation information -> Digital Diagnostics
    ('C9E1', 9, 1), -- Manage document classifications -> Document Management
    ('C9E10', 9, 1), -- Visually compare multiple documents -> Document Management
    ('C9E11', 9, 1), -- View any version of a document -> Document Management
    ('C9E12', 9, 1), -- Print documents -> Document Management
    ('C9E13', 9, 1), -- Export documents to new formats -> Document Management
    ('C9E14', 9, 1), -- Document reports -> Document Management
    ('C9E15', 9, 1), -- Receipt of electronic documents -> Document Management
    ('C9E16', 9, 1), -- Access Patient Record -> Document Management
    ('C9E17', 9, 3), -- Search for documents using document content -> Document Management
    ('C9E2', 9, 1), -- Manage document properties -> Document Management
    ('C9E3', 9, 1), -- Manage document attributes -> Document Management
    ('C9E4', 9, 1), -- Manage document coded entries -> Document Management
    ('C9E5', 9, 1), -- Document workflows -> Document Management
    ('C9E6', 9, 1), -- Manage document annotation -> Document Management
    ('C9E7', 9, 1), -- Search for documents -> Document Management
    ('C9E8', 9, 1), -- Search document content -> Document Management
    ('C9E9', 9, 1), -- Document and Patient matching -> Document Management
    ('E00001', 43, 1), -- Online Consultation -> Online Consultation
    ('E00002', 43, 3), -- Online Consultation with a Proxy -> Online Consultation
    ('E00003', 43, 3), -- Patient/Service User requests for Online Consultation support and provides information -> Online Consultation
    ('E00004', 43, 3), -- Proxy requests for Online Consultation support and provides information -> Online Consultation
    ('E00005', 43, 3), -- respond to Online Consultation requests for support from Patients/Service Users -> Online Consultation
    ('E00006', 43, 3), -- respond to Online Consultation requests for support from Proxies -> Online Consultation
    ('E00007', 43, 3), -- include attachments in Online Consultation requests -> Online Consultation
    ('E00008', 43, 3), -- include attachments in Online Consultation requests from a Proxy -> Online Consultation
    ('E00009', 43, 3), -- automated response to Online Consultation requests for support from Patients/Service Users -> Online Consultation
    ('E00010', 43, 3), -- automated response to Online Consultation requests for support from Proxies -> Online Consultation
    ('E00011', 43, 3), -- Patient/Service User makes an administrative request -> Online Consultation
    ('E00012', 43, 3), -- Proxy makes an administrative request -> Online Consultation
    ('E00013', 43, 3), -- respond to administrative requests for support from Patients/Service Users -> Online Consultation
    ('E00014', 43, 3), -- respond to administrative requests for support from Proxies -> Online Consultation
    ('E00015', 43, 3), -- automated responses to administrative requests from Patients/Service Users -> Online Consultation
    ('E00016', 43, 3), -- automated responses to administrative requests from Proxies -> Online Consultation
    ('E00017', 43, 3), -- link Online Consultation requests for support and responses -> Online Consultation
    ('E00018', 43, 3), -- link Online Consultation requests for support from a Proxy and responses -> Online Consultation
    ('E00019', 43, 3), -- self-help and signposting -> Online Consultation
    ('E00020', 43, 3), -- Proxy supporting self-help and signposting -> Online Consultation
    ('E00021', 43, 3), -- symptom checking -> Online Consultation
    ('E00022', 43, 3), -- symptom checking by a Proxy -> Online Consultation
    ('E00023', 43, 3), -- Direct Messaging -> Online Consultation
    ('E00024', 43, 3), -- Direct Messaging by a Proxy -> Online Consultation
    ('E00025', 43, 3), -- view the Patient Record during Online Consultation -> Online Consultation
    ('E00026', 43, 3), -- electronically share files during Direct Messaging -> Online Consultation
    ('E00027', 43, 3), -- electronically share files during Direct Messaging with a Proxy -> Online Consultation
    ('E00028', 43, 3), -- customisation of report -> Online Consultation
    ('E00029', 43, 3), -- report on utilisation of Online Consultation requests for support -> Online Consultation
    ('E00030', 43, 3), -- report on outcomes or dispositions provided to the Patient/Service User -> Online Consultation
    ('E000304', 13, 1), -- Patient-based report on lipid management -> Patient Information Maintenance - GP
    ('E00031', 43, 3), -- report on the status of Online Consultations -> Online Consultation
    ('E00032', 43, 3), -- report on Patient demographics using Online Consultation -> Online Consultation
    ('E00033', 43, 3), -- manually prioritise Online Consultation requests for support -> Online Consultation
    ('E00034', 43, 3), -- assign Online Consultation requests to a Health or Care Professional manually -> Online Consultation
    ('E00035', 43, 3), -- categorise outcome of Online Consultation requests -> Online Consultation
    ('E00037', 43, 3), -- automatically prioritise Online Consultation requests -> Online Consultation
    ('E00038', 43, 3), -- assign Online Consultation requests to Health or Care Professional automatically -> Online Consultation
    ('E00039', 44, 1), -- conduct Video Consultation -> Video Consultation
    ('E000396', 13, 1), -- firearms warnings -> Patient Information Maintenance - GP
    ('E00040', 44, 3), -- conduct Video Consultation with a Proxy -> Video Consultation
    ('E00041', 44, 3), -- conduct a Video Consultation with the Patient/Service User without registration -> Video Consultation
    ('E00042', 44, 3), -- conduct Video Consultation with a Proxy without registration -> Video Consultation
    ('E00043', 44, 3), -- end Video Consultation with a Patient/Service User -> Video Consultation
    ('E00045', 44, 3), -- Direct Messaging during a Video Consultation -> Video Consultation
    ('E00047', 44, 3), -- view the Patient Record during Video Consultation -> Video Consultation
    ('E00048', 44, 3), -- conduct group Video Consultations -> Video Consultation
    ('E00051', 44, 3), -- electronically share files during a Video Consultation -> Video Consultation
    ('E00053', 44, 3), -- Health or Care Professional can share their screen during a Video Consultation -> Video Consultation
    ('E00055', 44, 3), -- record Video Consultation outcome to the Patient record -> Video Consultation
    ('E00056', 43, 3), -- disable and enable Direct Messaging for a Healthcare Organisation -> Online Consultation
    ('E00057', 43, 3), -- disable and enable Direct Messaging for a Patient/Service User -> Online Consultation
    ('E00058', 43, 3), -- disable and enable electronic file sharing during Direct Messaging for a Healthcare Organisation -> Online Consultation
    ('E00059', 44, 3), -- Health or Care Professional can record a Video Consultation -> Video Consultation
    ('E00060', 44, 3), -- Patient/Service User can record a Video Consultation -> Video Consultation
    ('E00061', 44, 3), -- accessibility options for Video Consultation -> Video Consultation
    ('E00062', 44, 3), -- waiting room -> Video Consultation
    ('E00063', 44, 3), -- disable and enable Direct Messaging during a Video Consultation for the Patient/Service User -> Video Consultation
    ('E00064', 44, 3), -- record Direct Messages to the Patient Record -> Video Consultation
    ('E00065', 44, 3), -- Patient/Service User name is not automatically visible in a group Video Consultation -> Video Consultation
    ('E00066', 44, 3), -- invite new participants to an existing Video Consultation with a Patient/Service User -> Video Consultation
    ('E00067', 44, 3), -- disable and enable electronic file sharing during a Video Consultation -> Video Consultation
    ('E00068', 44, 3), -- disable and enable screen sharing during a Video Consultation -> Video Consultation
    ('E00069', 44, 3), -- Patient/Service User feedback on Video Consultations -> Video Consultation
    ('E00070', 44, 3), -- test the Video Consultation settings -> Video Consultation
    ('E00071', 44, 3), -- consecutive consultations with multiple Patients/Service Users via a single Video Consultation -> Video Consultation
    ('E00072', 44, 3), -- reminder of upcoming or scheduled Video Consultation -> Video Consultation
    ('E00073', 44, 3), -- disable and enable audio during a Video Consultation -> Video Consultation
    ('E00074', 44, 3), -- disable and enable video during a Video Consultation -> Video Consultation
    ('E00075', 43, 3), -- Patient/Service User feedback for Online Consultation -> Online Consultation
    ('E00076', 43, 3), -- record Online Consultation outcome to the Patient Record -> Online Consultation
    ('E00077', 43, 3), -- retain attachments (file and images) in the Patient Record -> Online Consultation
    ('E00078', 43, 3), -- Verify Patient/Service User details against Personal Demographics Service (PDS) -> Online Consultation
    ('E00079', 43, 3), -- SNOMED code Online Consultation -> Online Consultation
    ('E00080', 43, 3), -- customisation of the question sets for Patients/Service Users requesting Online Consultation support -> Online Consultation
    ('E00081', 43, 3), -- accessibility options for Online Consultation -> Online Consultation
    ('E00082', 43, 3), -- notification to Patients/Service Users -> Online Consultation
    ('E00083', 43, 3), -- customisation of instructions to Patients/Service Users using Online Consultation Solution -> Online Consultation
    ('E00084', 43, 3), -- categorise administration requests -> Online Consultation
    ('E00085', 43, 3), -- disable and enable Direct Messaging for an Online Consultation request for support -> Online Consultation
    ('E00086', 43, 3), -- configuration of the triage process -> Online Consultation
    ('E00087', 44, 3), -- retain attachments (file and images) received during Video Consultation in the Patient Record -> Video Consultation
    ('E00088', 44, 3), -- SNOMED code Video Consultation -> Video Consultation
    ('E00089', 43, 3), -- save the complete record of an Online Consultation to the Patient Record -> Online Consultation
    ('E00090', 43, 3), -- Health or Care Professional initiates an Online Consultations request -> Online Consultation
    ('E00091', 43, 3), -- Proxy Verification -> Online Consultation
    ('E00092', 14, 1), -- access pre-configured prescribable items and related information at the point of prescribing -> Prescribing
    ('E00093', 14, 1), -- manage Prescriber Type for Health or Care Professionals -> Prescribing
    ('E00094', 14, 1), -- manage prescriber sub-type for Health or Care Professionals -> Prescribing
    ('E00095', 14, 1), -- manage Alternate Authorising Prescribers for Health or Care Professionals -> Prescribing
    ('E00096', 14, 1), -- manage prescribed Acute and Repeat medication for Patients -> Prescribing
    ('E00097', 14, 1), -- create Repeatable Batch Issue of Repeat medication for Patients -> Prescribing
    ('E00098', 14, 1), -- manage prescribed Instalment medication for Patients -> Prescribing
    ('E00099', 14, 1), -- record medication personally administered or dispensed by the Healthcare Organisation -> Prescribing
    ('E00100', 14, 1), -- warnings at the point of prescribing -> Prescribing
    ('E00101', 14, 1), -- Electronic Prescription Service (EPS) - Prescribing -> Prescribing
    ('E00102', 14, 1), -- record medication not issued by the Healthcare Organisation using prescribable item -> Prescribing
    ('E00103', 14, 1), -- view all medications for a Patient -> Prescribing
    ('E00104', 14, 1), -- manage NHS prescriptions -> Prescribing
    ('E00105', 14, 1), -- create Delayed prescriptions -> Prescribing
    ('E00106', 14, 1), -- early re-issue of medication warning at the point of prescribing -> Prescribing
    ('E00107', 14, 1), -- manage Patient medication regimen reviews -> Prescribing
    ('E00108', 14, 1), -- manage Repeat medication requests -> Prescribing
    ('E00109', 1, 1), -- submit Management Information (MI) data to NHS Digital -> Appointments Management - Citizen
    ('E00109', 2, 3), -- submit Management Information (MI) data to NHS Digital -> Communicate With Practice - Citizen
    ('E00109', 3, 1), -- submit Management Information (MI) data to NHS Digital -> Prescription Ordering - Citizen
    ('E00109', 4, 1), -- submit Management Information (MI) data to NHS Digital -> View Record - Citizen
    ('E00109', 5, 1), -- submit Management Information (MI) data to NHS Digital -> Appointments Management - GP
    ('E00109', 9, 3), -- submit Management Information (MI) data to NHS Digital -> Document Management
    ('E00109', 13, 1), -- submit Management Information (MI) data to NHS Digital -> Patient Information Maintenance - GP
    ('E00109', 14, 3), -- submit Management Information (MI) data to NHS Digital -> Prescribing
    ('E00110', 14, 3), -- manage custom Formularies -> Prescribing
    ('E00111', 14, 3), -- share custom Formularies between organisations using the same Solution -> Prescribing
    ('E00112', 14, 3), -- set default custom Formulary for Health or Care Professionals -> Prescribing
    ('E00113', 14, 3), -- restrict use of custom Formulary to specific Health or Care Professionals -> Prescribing
    ('E00114', 14, 3), -- age-related default dosage for medication within a custom Formulary -> Prescribing
    ('E00115', 14, 3), -- record medication not issued by the Healthcare Organisation using free-text item -> Prescribing
    ('E00116', 14, 3), -- record medication from Schedule 1 of the NHS regulations -> Prescribing
    ('E00117', 14, 3), -- configure view of all medications for a Patient -> Prescribing
    ('E00118', 14, 3), -- reprint prescriptions -> Prescribing
    ('E00119', 14, 3), -- manage prescribed Private medication -> Prescribing
    ('E00120', 14, 3), -- manage Private prescriptions -> Prescribing
    ('E00121', 14, 3), -- manage Private controlled drug prescriptions -> Prescribing
    ('E00122', 14, 3), -- Repeat issue limitations warning at the point of prescribing -> Prescribing
    ('E00123', 14, 3), -- convert proprietary medication into generic medication at the point of prescribing -> Prescribing
    ('E00124', 14, 3), -- convert generic medication into a proprietary medication at the point of prescribing -> Prescribing
    ('E00125', 14, 3), -- separation of 'as needed' medication items on NHS Repeatable prescriptions -> Prescribing
    ('E00126', 14, 3), -- manage Patient's Preferred Pharmacy -> Prescribing
    ('E00127', 14, 3), -- view prescribed medication reports -> Prescribing
    ('E00128', 14, 3), -- view Repeat medication review report -> Prescribing
    ('E00129', 14, 3), -- view medication regimen review report -> Prescribing
    ('E00130', 14, 3), -- manage Acute medication requests -> Prescribing
    ('E00131', 14, 3), -- view not requested Repeat medication report -> Prescribing
    ('E00132', 14, 3), -- view EPS Nominated Pharmacy changes -> Prescribing
    ('E00133', 14, 3), -- configurable warnings for prescribable items -> Prescribing
    ('E00134', 14, 3), -- link medication to a diagnosis -> Prescribing
    ('E00135', 15, 1), -- record information for Consultations -> Recording Consultations - GP
    ('E00136', 15, 1), -- extract Female Genital Mutilation (FGM) data -> Recording Consultations - GP
    ('E00137', 15, 1), -- Electronic Yellow Card Reporting -> Recording Consultations - GP
    ('E00138', 15, 1), -- eMED3 (Fit Notes) -> Recording Consultations - GP
    ('E00139', 15, 3), -- manage Consultation form templates -> Recording Consultations - GP
    ('E00140', 15, 3), -- record Consultation using published form template -> Recording Consultations - GP
    ('E00141', 15, 3), -- Consultation form template version roll back -> Recording Consultations - GP
    ('E00142', 15, 3), -- upload attachments for Consultations -> Recording Consultations - GP
    ('E00143', 15, 3), -- share Consultation form templates between organisations using the same Solution -> Recording Consultations - GP
    ('E00144', 15, 3), -- use Supplier implemented national Consultation form templates -> Recording Consultations - GP
    ('E00145', 15, 3), -- data recording for Call and Recall processes -> Recording Consultations - GP
    ('E00146', 15, 3), -- view Calls and Recalls report -> Recording Consultations - GP
    ('E00147', 15, 3), -- view report of Consultations -> Recording Consultations - GP
    ('E00148', 11, 1), -- create Referral information for Patients -> Referral Management - GP
    ('E00149', 11, 1), -- e-Referrals Service (e-RS) -> Referral Management - GP
    ('E00150', 11, 3), -- manage e-Referrals -> Referral Management - GP
    ('E00151', 11, 3), -- manage manual Referrals -> Referral Management - GP
    ('E00152', 11, 3), -- Referral reports -> Referral Management - GP
    ('E00153', 12, 1), -- manage my Health or Care Organisation site information -> Resource Management
    ('E00154', 12, 1), -- manage information about Staff Members at my Health or Care Organisation -> Resource Management
    ('E00155', 12, 3), -- manage Staff Member Groups -> Resource Management
    ('E00156', 12, 3), -- manage Staff Members unavailability periods -> Resource Management
    ('E00157', 12, 3), -- manage my Health or Care Organisation's non-working days and times -> Resource Management
    ('E00158', 12, 3), -- manage Staff Members non-working days and times at my Health or Care Organisation -> Resource Management
    ('E00159', 12, 3), -- manage information about Related Organisations -> Resource Management
    ('E00160', 12, 3), -- manage information about Staff Members at Related Organisations -> Resource Management
    ('E00161', 12, 3), -- manage Non-human Resources -> Resource Management
    ('E00162', 12, 3), -- manage Non-human Resource associations -> Resource Management
    ('E00163', 13, 1), -- manage Patient Registrations -> Patient Information Maintenance - GP
    ('E00164', 13, 1), -- manage Patient Demographic Information -> Patient Information Maintenance - GP
    ('E00165', 13, 1), -- manage Electronic Patient Records (EPRs) -> Patient Information Maintenance - GP
    ('E00166', 13, 1), -- manage Citizen Service Access for a Patient -> Patient Information Maintenance - GP
    ('E00167', 13, 1), -- manage Identity Verifications for a Patient -> Patient Information Maintenance - GP
    ('E00168', 13, 1), -- manage Communicate with Practice Citizen Service access for a Patient and the Practice -> Patient Information Maintenance - GP
    ('E00169', 13, 1), -- manage Appointments Management Citizen Service access for a Patient and the Practice -> Patient Information Maintenance - GP
    ('E00170', 13, 1), -- manage Prescription Ordering Citizen Service access for a Patient and the Practice -> Patient Information Maintenance - GP
    ('E00171', 13, 1), -- manage View Record Citizen Service Access for the Practice -> Patient Information Maintenance - GP
    ('E00172', 13, 1), -- manage View Record Citizen Service Access for a Patient -> Patient Information Maintenance - GP
    ('E00173', 13, 1), -- manage Communicate With Practice Citizen Service communications -> Patient Information Maintenance - GP
    ('E00174', 13, 1), -- configure access to elements of the Electronic Patient Record (EPR) in the View Record Citizen Service -> Patient Information Maintenance - GP
    ('E00175', 13, 1), -- NHAIS HA/GP Links -> Patient Information Maintenance - GP
    ('E00176', 13, 1), -- Personal Demographics Service (PDS) -> Patient Information Maintenance - GP
    ('E00177', 13, 1), -- transfer Electronic Patient Records (EPRs) between GP Practices -> Patient Information Maintenance - GP
    ('E00178', 13, 1), -- Summary Care Record (SCR) -> Patient Information Maintenance - GP
    ('E00179', 13, 1), -- GP Connect - Access Record -> Patient Information Maintenance - GP
    ('E00180', 13, 1), -- GP Connect Messaging - Send Document -> Patient Information Maintenance - GP
    ('E00181', 13, 1), -- NHS 111 CDA messages -> Patient Information Maintenance - GP
    ('E00182', 13, 1), -- General Practice Extraction Service (GPES) -> Patient Information Maintenance - GP
    ('E00183', 13, 1), -- Digital Medicines and Pharmacy FHIR Messages -> Patient Information Maintenance - GP
    ('E00184', 5, 1), -- GP Data for Planning and Research -> Appointments Management - GP
    ('E00184', 13, 1), -- GP Data for Planning and Research -> Patient Information Maintenance - GP
    ('E00185', 13, 1), -- submit Primary Care Clinical Terminology Usage data to NHS Digital -> Patient Information Maintenance - GP
    ('E00186', 13, 3), -- search for Electronic Patient Records (EPRs) -> Patient Information Maintenance - GP
    ('E00187', 13, 3), -- search content of an Electronic Patient Record (EPR) -> Patient Information Maintenance - GP
    ('E00188', 13, 3), -- search the contents of documents attached to an Electronic Patient Record (EPR) -> Patient Information Maintenance - GP
    ('E00189', 13, 3), -- print the Electronic Patient Record (EPR) -> Patient Information Maintenance - GP
    ('E00190', 13, 3), -- export the Electronic Patient Record (EPR) -> Patient Information Maintenance - GP
    ('E00191', 13, 3), -- manage a personalised display of Electronic Patient Records (EPRs) -> Patient Information Maintenance - GP
    ('E00192', 13, 3), -- notifications for outstanding actions on an Electronic Patient Record (EPR) -> Patient Information Maintenance - GP
    ('E00193', 13, 3), -- manage Patient Related Persons -> Patient Information Maintenance - GP
    ('E00194', 13, 3), -- manage Patient Problem lists -> Patient Information Maintenance - GP
    ('E00195', 13, 3), -- manage manual Patient Alerts for Electronic Patient Records (EPRs) -> Patient Information Maintenance - GP
    ('E00196', 13, 3), -- manage automatic Patient Alerts for Electronic Patient Records (EPRs) -> Patient Information Maintenance - GP
    ('E00197', 13, 3), -- support Registrations for Private Patients -> Patient Information Maintenance - GP
    ('E00198', 13, 3), -- support Registrations for Other Services -> Patient Information Maintenance - GP
    ('E00199', 13, 3), -- manage Subject Access Request (SAR) requests -> Patient Information Maintenance - GP
    ('E00200', 13, 3), -- view Subject Access Request reports -> Patient Information Maintenance - GP
    ('E00201', 13, 3), -- verify Patient Contact Details -> Patient Information Maintenance - GP
    ('E00202', 13, 3), -- configure Welcome Message for Citizen Services -> Patient Information Maintenance - GP
    ('E00203', 13, 3), -- configure a Disclaimer for Citizens using a Communicate with Practice Citizen Service -> Patient Information Maintenance - GP
    ('E00204', 13, 3), -- view Citizen Service Usage -> Patient Information Maintenance - GP
    ('E00205', 13, 3), -- manage Acute Prescription Request Citizen Service for a Patient and the Practice -> Patient Information Maintenance - GP
    ('E00206', 13, 3), -- manage Citizen Service Access for a Proxy -> Patient Information Maintenance - GP
    ('E00207', 13, 3), -- manage Identity Verifications for a Proxy -> Patient Information Maintenance - GP
    ('E00208', 13, 3), -- manage Communicate with Practice Citizen Service access for a Proxy -> Patient Information Maintenance - GP
    ('E00209', 13, 3), -- manage Appointments Management Citizen Service access for a Proxy -> Patient Information Maintenance - GP
    ('E00210', 13, 3), -- manage Prescription Ordering Citizen Service access for a Proxy -> Patient Information Maintenance - GP
    ('E00211', 13, 3), -- manage Acute Prescription Request Citizen Service for a Proxy -> Patient Information Maintenance - GP
    ('E00212', 13, 3), -- manage View Record Citizen Service Access for a Proxy -> Patient Information Maintenance - GP
    ('E00213', 13, 3), -- identify Patients outside of Catchment Area -> Patient Information Maintenance - GP
    ('E00214', 13, 3), -- view Patient Reports -> Patient Information Maintenance - GP
    ('E00215', 13, 3), -- manage Patient Cohorts from Search Results -> Patient Information Maintenance - GP
    ('E00216', 13, 3), -- Patient-based report on Capitation -> Patient Information Maintenance - GP
    ('E00217', 13, 3), -- Patient-based report on childhood vaccinations and immunisations -> Patient Information Maintenance - GP
    ('E00218', 13, 3), -- Patient-based report on Carers -> Patient Information Maintenance - GP
    ('E00219', 5, 1), -- manage Appointments for Patients -> Appointments Management - GP
    ('E00220', 5, 1), -- manage Sessions -> Appointments Management - GP
    ('E00221', 5, 1), -- manage Appointment Slots within a Session -> Appointments Management - GP
    ('E00222', 5, 1), -- record National Slot Type Category for Appointment Slots -> Appointments Management - GP
    ('E00223', 5, 1), -- record Appointment status -> Appointments Management - GP
    ('E00224', 5, 1), -- display a combined view of a Patient's future and historical Appointments -> Appointments Management - GP
    ('E00225', 5, 1), -- display Patient Alerts -> Appointments Management - GP
    ('E00226', 5, 1), -- GP Connect - Appointments Management -> Appointments Management - GP
    ('E00227', 5, 1), -- General Practice Appointments Data Reporting -> Appointments Management - GP
    ('E00229', 5, 3), -- search available Appointment Slots -> Appointments Management - GP
    ('E00230', 5, 3), -- view a summary of a Patient's Appointment history -> Appointments Management - GP
    ('E00231', 5, 3), -- record Non-human resources for an Appointment -> Appointments Management - GP
    ('E00232', 5, 3), -- book Appointments not linked to a Patient -> Appointments Management - GP
    ('E00233', 5, 3), -- record other attendees for an Appointment -> Appointments Management - GP
    ('E00234', 5, 3), -- re-schedule Appointments -> Appointments Management - GP
    ('E00235', 5, 3), -- record non-time bound Appointments -> Appointments Management - GP
    ('E00236', 5, 3), -- automatically record status of Did Not Attend (DNA) for Appointments -> Appointments Management - GP
    ('E00237', 5, 3), -- configure delayed release Appointment Slots -> Appointments Management - GP
    ('E00238', 5, 3), -- filter the combined view of a Patient's Appointments -> Appointments Management - GP
    ('E00239', 5, 3), -- display a combined view of available Appointment Slots and booked Appointments for the Healthcare Organisation -> Appointments Management - GP
    ('E00240', 5, 3), -- view Appointment dashboard for the Healthcare Organisation -> Appointments Management - GP
    ('E00241', 5, 3), -- configure the Appointment dashboard for the Healthcare Organisation -> Appointments Management - GP
    ('E00242', 5, 3), -- export the Appointment dashboard for the Healthcare Organisation -> Appointments Management - GP
    ('E00243', 5, 3), -- view Appointment reports -> Appointments Management - GP
    ('E00244', 5, 3), -- generate automatic Appointment reminder communications to Patients -> Appointments Management - GP
    ('E00245', 5, 3), -- manually generate Appointment communications -> Appointments Management - GP
    ('E00246', 5, 3), -- generate automatic Appointment communications when Appointment status is updated -> Appointments Management - GP
    ('E00247', 5, 3), -- alerts for Health or Care Professional unavailability periods when creating Appointment slots -> Appointments Management - GP
    ('E00248', 5, 3), -- alerts for Health or Care Professional non-working days and times when creating Appointment slots -> Appointments Management - GP
    ('E00249', 5, 3), -- alerts for the Healthcare Organisation's closed periods when creating Appointment slots -> Appointments Management - GP
    ('E00250', 5, 3), -- display Appointment alerts when managing Appointments -> Appointments Management - GP
    ('E00251', 5, 3), -- manage Session templates -> Appointments Management - GP
    ('E00252', 5, 3), -- export Sessions -> Appointments Management - GP
    ('E00253', 5, 3), -- share Session templates with other Health or Care Organisations using the same Solution -> Appointments Management - GP
    ('E00255', 13, 1), -- display Patient Alerts for Electronic Patient Records (EPRs) -> Patient Information Maintenance - GP
    ('E00267', 50, 1), -- manage Tasks -> Task Management
    ('E00268', 50, 3), -- manually create ad-hoc Tasks -> Task Management
    ('E00269', 50, 3), -- assign Tasks to Health or Care Professionals -> Task Management
    ('E00270', 50, 3), -- flag overdue Tasks -> Task Management
    ('E00271', 50, 3), -- manage Task templates -> Task Management
    ('E00272', 50, 3), -- manage a personalised display for Tasks -> Task Management
    ('E00273', 50, 3), -- configure Task rules -> Task Management
    ('E00274', 50, 3), -- view Task reports -> Task Management
    ('E00277', 13, 1), -- Transfer of Care FHIR Payload -> Patient Information Maintenance - GP
    ('E00285', 13, 1), -- National Data Opt-Out -> Patient Information Maintenance - GP
    ('E00305', 1, 1), -- Patient managing Appointments -> Appointments Management - Citizen
    ('E00306', 1, 1), -- Patient viewing future and historical Appointments -> Appointments Management - Citizen
    ('E00307', 1, 3), -- Proxy managing a Patient's Appointments -> Appointments Management - Citizen
    ('E00308', 1, 3), -- Proxy viewing a Patient's future and historical Appointments -> Appointments Management - Citizen
    ('E00309', 1, 3), -- add free text when booking an Appointment -> Appointments Management - Citizen
    ('E00310', 1, 3), -- display a Healthcare Organisation configured Welcome Message in Citizen Services -> Appointments Management - Citizen
    ('E00310', 2, 3), -- display a Healthcare Organisation configured Welcome Message in Citizen Services -> Communicate With Practice - Citizen
    ('E00310', 3, 3), -- display a Healthcare Organisation configured Welcome Message in Citizen Services -> Prescription Ordering - Citizen
    ('E00310', 4, 3), -- display a Healthcare Organisation configured Welcome Message in Citizen Services -> View Record - Citizen
    ('E00311', 1, 3), -- view the Healthcare Organisation's details -> Appointments Management - Citizen
    ('E00311', 2, 3), -- view the Healthcare Organisation's details -> Communicate With Practice - Citizen
    ('E00311', 3, 3), -- view the Healthcare Organisation's details -> Prescription Ordering - Citizen
    ('E00311', 4, 3), -- view the Healthcare Organisation's details -> View Record - Citizen
    ('E00312', 1, 3), -- view my Citizen Services configuration -> Appointments Management - Citizen
    ('E00312', 2, 3), -- view my Citizen Services configuration -> Communicate With Practice - Citizen
    ('E00312', 3, 3), -- view my Citizen Services configuration -> Prescription Ordering - Citizen
    ('E00312', 4, 3), -- view my Citizen Services configuration -> View Record - Citizen
    ('E00313', 1, 3), -- request to amend my Citizen Service Access -> Appointments Management - Citizen
    ('E00313', 2, 3), -- request to amend my Citizen Service Access -> Communicate With Practice - Citizen
    ('E00313', 3, 3), -- request to amend my Citizen Service Access -> Prescription Ordering - Citizen
    ('E00313', 4, 3), -- request to amend my Citizen Service Access -> View Record - Citizen
    ('E00314', 1, 3), -- Patient viewing Demographic Information held by the Healthcare Organisation -> Appointments Management - Citizen
    ('E00314', 2, 3), -- Patient viewing Demographic Information held by the Healthcare Organisation -> Communicate With Practice - Citizen
    ('E00314', 3, 3), -- Patient viewing Demographic Information held by the Healthcare Organisation -> Prescription Ordering - Citizen
    ('E00314', 4, 3), -- Patient viewing Demographic Information held by the Healthcare Organisation -> View Record - Citizen
    ('E00315', 1, 3), -- Proxy viewing Patient's Demographic Information held by the Healthcare Organisation -> Appointments Management - Citizen
    ('E00315', 2, 3), -- Proxy viewing Patient's Demographic Information held by the Healthcare Organisation -> Communicate With Practice - Citizen
    ('E00315', 3, 3), -- Proxy viewing Patient's Demographic Information held by the Healthcare Organisation -> Prescription Ordering - Citizen
    ('E00315', 4, 3), -- Proxy viewing Patient's Demographic Information held by the Healthcare Organisation -> View Record - Citizen
    ('E00316', 1, 3), -- Patient request to update Demographic Information -> Appointments Management - Citizen
    ('E00316', 2, 3), -- Patient request to update Demographic Information -> Communicate With Practice - Citizen
    ('E00316', 3, 3), -- Patient request to update Demographic Information -> Prescription Ordering - Citizen
    ('E00316', 4, 3), -- Patient request to update Demographic Information -> View Record - Citizen
    ('E00317', 1, 3), -- Proxy request to update Patient's Demographic Information -> Appointments Management - Citizen
    ('E00317', 2, 3), -- Proxy request to update Patient's Demographic Information -> Communicate With Practice - Citizen
    ('E00317', 3, 3), -- Proxy request to update Patient's Demographic Information -> Prescription Ordering - Citizen
    ('E00317', 4, 3), -- Proxy request to update Patient's Demographic Information -> View Record - Citizen
    ('E00318', 1, 3), -- Citizen viewing Citizen Services usage information -> Appointments Management - Citizen
    ('E00318', 2, 3), -- Citizen viewing Citizen Services usage information -> Communicate With Practice - Citizen
    ('E00318', 3, 3), -- Citizen viewing Citizen Services usage information -> Prescription Ordering - Citizen
    ('E00318', 4, 3), -- Citizen viewing Citizen Services usage information -> View Record - Citizen
    ('E00319', 1, 3), -- Verified User Account (VUA) credentials reminder -> Appointments Management - Citizen
    ('E00319', 2, 3), -- Verified User Account (VUA) credentials reminder -> Communicate With Practice - Citizen
    ('E00319', 3, 3), -- Verified User Account (VUA) credentials reminder -> Prescription Ordering - Citizen
    ('E00319', 4, 3), -- Verified User Account (VUA) credentials reminder -> View Record - Citizen
    ('E00320', 1, 3), -- request Verified User Account (VUA) Linkage Key reset -> Appointments Management - Citizen
    ('E00320', 2, 3), -- request Verified User Account (VUA) Linkage Key reset -> Communicate With Practice - Citizen
    ('E00320', 3, 3), -- request Verified User Account (VUA) Linkage Key reset -> Prescription Ordering - Citizen
    ('E00320', 4, 3), -- request Verified User Account (VUA) Linkage Key reset -> View Record - Citizen
    ('E00321', 2, 1), -- Patient managing communications -> Communicate With Practice - Citizen
    ('E00322', 2, 3), -- Proxy managing communications -> Communicate With Practice - Citizen
    ('E00323', 2, 3), -- include attachments with communications -> Communicate With Practice - Citizen
    ('E00324', 2, 3), -- organising communications -> Communicate With Practice - Citizen
    ('E00325', 2, 3), -- notification of received communications -> Communicate With Practice - Citizen
    ('E00326', 2, 3), -- display a Healthcare Organisation configured Disclaimer when creating communications -> Communicate With Practice - Citizen
    ('E00327', 3, 1), -- Patient viewing Repeat medications -> Prescription Ordering - Citizen
    ('E00328', 3, 1), -- Patient managing requests for a Repeat medication to be issued -> Prescription Ordering - Citizen
    ('E00329', 3, 3), -- Proxy viewing a Patient's Repeat medications -> Prescription Ordering - Citizen
    ('E00330', 3, 3), -- Proxy managing requests for a Patient's Repeat medication to be issued -> Prescription Ordering - Citizen
    ('E00331', 3, 3), -- add free text information to a request for a Repeat medication to be issued -> Prescription Ordering - Citizen
    ('E00332', 3, 3), -- add free text information when cancelling a request for a Repeat medication to be issued -> Prescription Ordering - Citizen
    ('E00333', 3, 3), -- Patient viewing Acute medications -> Prescription Ordering - Citizen
    ('E00334', 3, 3), -- Patient managing requests for an Acute medication to be issued -> Prescription Ordering - Citizen
    ('E00335', 3, 3), -- Proxy viewing Patient's Acute medications -> Prescription Ordering - Citizen
    ('E00336', 3, 3), -- Proxy managing requests for a Patient's Acute medication to be issued -> Prescription Ordering - Citizen
    ('E00337', 3, 3), -- add free text information to a request for an Acute medication to be issued -> Prescription Ordering - Citizen
    ('E00338', 3, 3), -- add free text information when cancelling a request for an Acute medication to be issued -> Prescription Ordering - Citizen
    ('E00339', 3, 3), -- view additional medication information -> Prescription Ordering - Citizen
    ('E00340', 3, 3), -- Patient managing Electronic Prescription Service (EPS) Nominated Pharmacy -> Prescription Ordering - Citizen
    ('E00341', 3, 3), -- Proxy managing Patient's Electronic Prescription Service (EPS) Nominated Pharmacy -> Prescription Ordering - Citizen
    ('E00342', 3, 3), -- Patient managing Preferred Pharmacy -> Prescription Ordering - Citizen
    ('E00343', 3, 3), -- Proxy managing Patient's Preferred Pharmacy -> Prescription Ordering - Citizen
    ('E00344', 16, 1), -- report on a range of data items -> Reporting
    ('E00345', 16, 3), -- include Synthetic Patients/Service Users in reports -> Reporting
    ('E00346', 16, 3), -- configure display of report results -> Reporting
    ('E00347', 16, 3), -- schedule the automated running of reports -> Reporting
    ('E00348', 16, 3), -- save locally defined reports -> Reporting
    ('E00349', 16, 3), -- save report results -> Reporting
    ('E00350', 16, 3), -- export report results -> Reporting
    ('E00351', 16, 3), -- print report results -> Reporting
    ('E00352', 16, 3), -- share access to a report within my Health or Care Organisation -> Reporting
    ('E00353', 16, 3), -- share report with another Health or Care Organisation -> Reporting
    ('E00354', 16, 3), -- identify the reason a Patient/Service User meets each report criterion for a report -> Reporting
    ('E00355', 16, 3), -- identify which report criteria a Patient/Service User does not meet -> Reporting
    ('E00356', 16, 3), -- perform analytics -> Reporting
    ('E00357', 4, 1), -- Patient viewing Summary Information Record -> View Record - Citizen
    ('E00358', 4, 1), -- Patient viewing Detailed Coded Record -> View Record - Citizen
    ('E00359', 4, 1), -- Patient viewing documents from Electronic Patient Record (EPR) -> View Record - Citizen
    ('E00360', 4, 1), -- Patient viewing Full Patient Record -> View Record - Citizen
    ('E00361', 4, 3), -- Proxy viewing Patient's Summary Information Record -> View Record - Citizen
    ('E00362', 4, 3), -- Proxy viewing Patient's Detailed Coded Record -> View Record - Citizen
    ('E00363', 4, 3), -- Proxy viewing documents from Electronic Patient Record (EPR) -> View Record - Citizen
    ('E00364', 4, 3), -- Proxy viewing Patient's Full Patient Record -> View Record - Citizen
    ('E00365', 4, 3), -- print the available Electronic Patient Record (EPR) content -> View Record - Citizen
    ('E00366', 4, 3), -- export the available Electronic Patient Record (EPR) content -> View Record - Citizen
    ('E00367', 7, 1), -- create communications -> Communication Management
    ('E00368', 7, 3), -- manage communications to Patients/Service Users -> Communication Management
    ('E00369', 7, 3), -- create communications to Health or Care Professionals -> Communication Management
    ('E00370', 7, 3), -- create communications to external organisations -> Communication Management
    ('E00371', 7, 3), -- create communications to a group of recipients -> Communication Management
    ('E00372', 7, 3), -- record communication preferences for Patients/Service Users -> Communication Management
    ('E00373', 7, 3), -- email communications -> Communication Management
    ('E00374', 7, 3), -- SMS message communications -> Communication Management
    ('E00375', 7, 3), -- letter communications -> Communication Management
    ('E00376', 7, 3), -- manage communication templates -> Communication Management
    ('E00377', 7, 3), -- create communications to Patients/Service Users from communication templates -> Communication Management
    ('E00378', 7, 3), -- manage automated communications -> Communication Management
    ('E00379', 7, 3), -- manage incoming communications -> Communication Management
    ('E00380', 7, 3), -- view communication reports -> Communication Management
    ('E00397', 20, 1), -- manage custom Workflows -> Custom Workflows
    ('E00398', 20, 3), -- configure Workflow rules -> Custom Workflows
    ('E00399', 20, 3), -- manage Workflow templates -> Custom Workflows
    ('E00400', 20, 3), -- manually create Workflows from templates -> Custom Workflows
    ('E00401', 20, 3), -- automatically create Workflows from templates -> Custom Workflows
    ('E00402', 20, 3), -- create a personalised display for Workflows -> Custom Workflows
    ('E00403', 20, 3), -- share personalised display for Workflows -> Custom Workflows
    ('E00404', 20, 3), -- flag overdue Workflows -> Custom Workflows
    ('E00405', 20, 3), -- view Workflow reports -> Custom Workflows
    ('E00406', 17, 1), -- scan documents -> Scanning
    ('E00407', 17, 3), -- document image enhancement -> Scanning
    ('E00409', 9, 1), -- manually manage documents -> Document Management
    ('E00410', 9, 1), -- manually manage document and Patient/Service User matching -> Document Management
    ('E00411', 9, 3), -- manage document information -> Document Management
    ('E00412', 9, 3), -- manage coded information associated with documents -> Document Management
    ('E00413', 9, 3), -- manage document annotations -> Document Management
    ('E00414', 9, 3), -- search for documents based on document information -> Document Management
    ('E00415', 9, 3), -- search for documents based on document coded information -> Document Management
    ('E00416', 9, 3), -- search for documents based on document content -> Document Management
    ('E00417', 9, 3), -- search document content -> Document Management
    ('E00418', 9, 3), -- visually compare multiple documents -> Document Management
    ('E00419', 9, 3), -- view all previous versions of documents -> Document Management
    ('E00420', 9, 3), -- automatically add electronic documents -> Document Management
    ('E00421', 9, 3), -- manage rules for automatically suggesting document information -> Document Management
    ('E00422', 9, 3), -- manage automatic suggestions for document information -> Document Management
    ('E00423', 9, 3), -- manage rules for automatically suggesting document coded information -> Document Management
    ('E00424', 9, 3), -- manage automatic suggestions for document coded information -> Document Management
    ('E00425', 9, 3), -- manage automatic suggestions for matching documents and Patients/Service Users -> Document Management
    ('E00426', 9, 3), -- document Workflows -> Document Management
    ('E00427', 9, 3), -- print documents -> Document Management
    ('E00428', 9, 3), -- export documents to new formats -> Document Management
    ('E00429', 9, 3), -- view document reports -> Document Management
    ('E00430', 10, 1), -- view national GPES payment extract reports -> GP Extracts Verification
    ('E00431', 10, 3), -- view national GPES non-payment extract reports -> GP Extracts Verification
    ('E00432', 8, 1), -- manage Requests for Investigations information for Patients -> Digital Diagnostics
    ('E00433', 8, 1), -- Digital Diagnostics & Pathology Messaging -> Digital Diagnostics
    ('E00434', 8, 3), -- reconcile Investigation Results with Requests for Investigations information -> Digital Diagnostics
    ('E00435', 8, 3), -- view Requests for Investigations information reports -> Digital Diagnostics
    ('E00436', 8, 3), -- create Request for Investigations information for multiple Patients in a single action -> Digital Diagnostics
    ('E00437', 8, 3), -- receive external Requests for Investigations information for Patients -> Digital Diagnostics
    ('E00438', 23, 1), -- manage Appointments for Patients/Service Users at other Health or Care Organisations -> Cross-organisation Appointment Booking
    ('E00439', 23, 3), -- view Cross-Organisation Appointment Booking reports -> Cross-organisation Appointment Booking
    ('S00001', 44, 3), -- 1-Way Video Triage Consultations -> Video Consultation
    ('S00002', 43, 3), -- Alert practice if expected response times are exceeded -> Online Consultation
    ('S00003', 43, 3), -- Allow practice to set target response times for patient requests -> Online Consultation
    ('S00004', 43, 3), -- Allow to track whether a patient has read the practice response. -> Online Consultation
    ('S00005', 44, 3), -- Appless Video -> Video Consultation
    ('S00006', 44, 3), -- Auto Transcription -> Video Consultation
    ('S00007', 44, 3), -- Automated  multiple reminders via multiple channels -> Video Consultation
    ('S00008', 43, 3), -- Automatic follow-up reminders -> Online Consultation
    ('S00009', 43, 3), -- Capacity & Demand Modelling -> Online Consultation
    ('S00010', 44, 3), -- Consultation Countdown -> Video Consultation
    ('S00011', 43, 3), -- DIRECT Automatic searching, matching, linking and opening of patient records in TPP SystmOne, Emis Web, Cegedim Vision - Lan & Aeros -> Online Consultation
    ('S00012', 43, 3), -- Display a warning to a patient when they are uploading an image -> Online Consultation
    ('S00013', 43, 3), -- Event Log -> Online Consultation
    ('S00014', 44, 3), -- Facility for clinician to take notes during video consultation -> Video Consultation
    ('S00015', 43, 3), -- Filtering, sorting and searching all incoming messages -> Online Consultation
    ('S00016', 44, 3), -- In call video tools - video blurring -> Video Consultation
    ('S00017', 44, 3), -- In call video tools - zoom -> Video Consultation
    ('S00018', 44, 3), -- Instant Forwarding -> Video Consultation
    ('S00019', 44, 3), -- Instant Location -> Video Consultation
    ('S00020', 44, 3), -- Instant Vital Signs -> Video Consultation
    ('S00021', 43, 3), -- Out of  Hours (OOH) Requests -> Online Consultation
    ('S00022', 44, 3), -- Pop Out Floating Video -> Video Consultation
    ('S00023', 43, 3), -- Preset Messages -> Online Consultation
    ('S00024', 43, 3), -- Remote Monitoring -> Online Consultation
    ('S00025', 44, 3), -- Reporting -> Video Consultation
    ('S00026', 44, 3), -- Reporting Dashboards -> Video Consultation
    ('S00027', 43, 3), -- Search online consultations -> Online Consultation
    ('S00028', 44, 3), -- Take screen captures during video consultation -> Video Consultation
    ('S00029', 43, 3), -- To determine when a patient has read a practice response, if at all. -> Online Consultation
    ('S00030', 44, 3), -- Vidtu video consultation solution -> Video Consultation
    ('S00031', 43, 3), -- Viewing messages sent to patients -> Online Consultation
    ('S020X01E01', 43, 3), -- Supplier-defined epic 1 -> Online Consultation
    ('S020X01E02', 43, 3), -- Supplier-defined epic 2 -> Online Consultation
    ('S020X01E03', 44, 3), -- Supplier-defined epic 3 -> Video Consultation
    ('S020X01E04', 44, 3), -- Supplier-defined epic 4 -> Video Consultation
    ('S020X01E05', 1, 3), -- Supplier-defined epic 5 -> Appointments Management - Citizen
    ('S020X01E06', 1, 3), -- Supplier-defined epic 6 -> Appointments Management - Citizen
    ('S020X01E07', 1, 3), -- Supplier-defined epic 7 -> Appointments Management - Citizen
    ('S020X01E08', 1, 3) -- Supplier-defined epic 8 -> Appointments Management - Citizen

    MERGE INTO catalogue.Epics AS TARGET
    USING @epics AS SOURCE
      ON TARGET.Id = SOURCE.Id
    WHEN MATCHED THEN
        UPDATE SET TARGET.[Name] = SOURCE.[Name],
                   TARGET.SupplierDefined = SOURCE.SupplierDefined,
                   TARGET.IsActive = SOURCE.IsActive
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Id, [Name], IsActive, SupplierDefined)
        VALUES (SOURCE.Id, SOURCE.[Name], SOURCE.IsActive, SOURCE.SupplierDefined);

    MERGE INTO catalogue.CapabilityEpics AS TARGET
    USING @capabilityEpics AS SOURCE
      ON TARGET.CapabilityId = SOURCE.CapabilityId 
        AND TARGET.EpicId = SOURCE.EpicID 
        AND TARGET.CompliancyLevelId = SOURCE.CompliancyLevelId
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (EpicId, CapabilityId, CompliancyLevelId)
        VALUES (SOURCE.EpicId, SOURCE.CapabilityId, SOURCE.CompliancyLevelId)
    WHEN NOT MATCHED BY SOURCE THEN
        DELETE;

END 
