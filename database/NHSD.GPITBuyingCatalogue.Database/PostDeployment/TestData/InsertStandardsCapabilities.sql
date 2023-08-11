IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
BEGIN
    DELETE FROM catalogue.StandardsCapabilities;

    INSERT INTO catalogue.StandardsCapabilities(StandardId, CapabilityId)
    VALUES
    ('S24', 1), -- Business Continuity & Disaster Recovery -> Appointments Management - Citizen
    ('S24', 2), -- Business Continuity & Disaster Recovery -> Communicate With Practice - Citizen
    ('S24', 3), -- Business Continuity & Disaster Recovery -> Prescription Ordering - Citizen
    ('S24', 4), -- Business Continuity & Disaster Recovery -> View Record - Citizen
    ('S24', 5), -- Business Continuity & Disaster Recovery -> Appointments Management - GP
    ('S24', 6), -- Business Continuity & Disaster Recovery -> Clinical Decision Support
    ('S24', 7), -- Business Continuity & Disaster Recovery -> Communication Management
    ('S24', 8), -- Business Continuity & Disaster Recovery -> Digital Diagnostics
    ('S24', 9), -- Business Continuity & Disaster Recovery -> Document Management
    ('S24', 10), -- Business Continuity & Disaster Recovery -> GP Extracts Verification
    ('S24', 11), -- Business Continuity & Disaster Recovery -> Referral Management - GP
    ('S24', 12), -- Business Continuity & Disaster Recovery -> Resource Management
    ('S24', 13), -- Business Continuity & Disaster Recovery -> Patient Information Maintenance - GP
    ('S24', 14), -- Business Continuity & Disaster Recovery -> Prescribing
    ('S24', 15), -- Business Continuity & Disaster Recovery -> Recording Consultations - GP
    ('S24', 16), -- Business Continuity & Disaster Recovery -> Reporting
    ('S24', 17), -- Business Continuity & Disaster Recovery -> Scanning
    ('S24', 18), -- Business Continuity & Disaster Recovery -> Telehealth
    ('S24', 20), -- Business Continuity & Disaster Recovery -> Custom Workflows
    ('S24', 21), -- Business Continuity & Disaster Recovery -> Care Homes
    ('S24', 22), -- Business Continuity & Disaster Recovery -> Caseload Management
    ('S24', 23), -- Business Continuity & Disaster Recovery -> Cross-organisation Appointment Booking
    ('S24', 24), -- Business Continuity & Disaster Recovery -> Cross-organisation Workflow Tools
    ('S24', 25), -- Business Continuity & Disaster Recovery -> Cross-organisation Workforce Management
    ('S24', 26), -- Business Continuity & Disaster Recovery -> Data Analytics for Integrated and Federated Care
    ('S24', 27), -- Business Continuity & Disaster Recovery -> Domiciliary Care
    ('S24', 29), -- Business Continuity & Disaster Recovery -> e-Consultations (Professional to Professional)
    ('S24', 30), -- Business Continuity & Disaster Recovery -> Medicines Optimisation
    ('S24', 32), -- Business Continuity & Disaster Recovery -> Personal Health Budget
    ('S24', 33), -- Business Continuity & Disaster Recovery -> Personal Health Record
    ('S24', 34), -- Business Continuity & Disaster Recovery -> Population Health Management
    ('S24', 35), -- Business Continuity & Disaster Recovery -> Risk Stratification
    ('S24', 36), -- Business Continuity & Disaster Recovery -> Shared Care Plans
    ('S24', 37), -- Business Continuity & Disaster Recovery -> Social Prescribing
    ('S24', 38), -- Business Continuity & Disaster Recovery -> Telecare
    ('S24', 39), -- Business Continuity & Disaster Recovery -> Unified Care Record
    ('S24', 41), -- Business Continuity & Disaster Recovery -> Productivity
    ('S24', 42), -- Business Continuity & Disaster Recovery -> Dispensing
    ('S24', 43), -- Business Continuity & Disaster Recovery -> Online Consultation
    ('S24', 44), -- Business Continuity & Disaster Recovery -> Video Consultation
    ('S24', 45), -- Business Continuity & Disaster Recovery -> Cohort Identification
    ('S24', 46), -- Business Continuity & Disaster Recovery -> PCN Appointments Management - Vaccinations
    ('S24', 47), -- Business Continuity & Disaster Recovery -> Vaccination and Adverse Reaction Recording
    ('S24', 50), -- Business Continuity & Disaster Recovery -> Task Management
    ('S25', 1), -- Clinical Safety -> Appointments Management - Citizen
    ('S25', 2), -- Clinical Safety -> Communicate With Practice - Citizen
    ('S25', 3), -- Clinical Safety -> Prescription Ordering - Citizen
    ('S25', 4), -- Clinical Safety -> View Record - Citizen
    ('S25', 5), -- Clinical Safety -> Appointments Management - GP
    ('S25', 6), -- Clinical Safety -> Clinical Decision Support
    ('S25', 7), -- Clinical Safety -> Communication Management
    ('S25', 8), -- Clinical Safety -> Digital Diagnostics
    ('S25', 9), -- Clinical Safety -> Document Management
    ('S25', 10), -- Clinical Safety -> GP Extracts Verification
    ('S25', 11), -- Clinical Safety -> Referral Management - GP
    ('S25', 12), -- Clinical Safety -> Resource Management
    ('S25', 13), -- Clinical Safety -> Patient Information Maintenance - GP
    ('S25', 14), -- Clinical Safety -> Prescribing
    ('S25', 15), -- Clinical Safety -> Recording Consultations - GP
    ('S25', 16), -- Clinical Safety -> Reporting
    ('S25', 17), -- Clinical Safety -> Scanning
    ('S25', 18), -- Clinical Safety -> Telehealth
    ('S25', 20), -- Clinical Safety -> Custom Workflows
    ('S25', 21), -- Clinical Safety -> Care Homes
    ('S25', 22), -- Clinical Safety -> Caseload Management
    ('S25', 23), -- Clinical Safety -> Cross-organisation Appointment Booking
    ('S25', 24), -- Clinical Safety -> Cross-organisation Workflow Tools
    ('S25', 25), -- Clinical Safety -> Cross-organisation Workforce Management
    ('S25', 26), -- Clinical Safety -> Data Analytics for Integrated and Federated Care
    ('S25', 27), -- Clinical Safety -> Domiciliary Care
    ('S25', 29), -- Clinical Safety -> e-Consultations (Professional to Professional)
    ('S25', 30), -- Clinical Safety -> Medicines Optimisation
    ('S25', 32), -- Clinical Safety -> Personal Health Budget
    ('S25', 33), -- Clinical Safety -> Personal Health Record
    ('S25', 34), -- Clinical Safety -> Population Health Management
    ('S25', 35), -- Clinical Safety -> Risk Stratification
    ('S25', 36), -- Clinical Safety -> Shared Care Plans
    ('S25', 37), -- Clinical Safety -> Social Prescribing
    ('S25', 38), -- Clinical Safety -> Telecare
    ('S25', 39), -- Clinical Safety -> Unified Care Record
    ('S25', 41), -- Clinical Safety -> Productivity
    ('S25', 42), -- Clinical Safety -> Dispensing
    ('S25', 43), -- Clinical Safety -> Online Consultation
    ('S25', 44), -- Clinical Safety -> Video Consultation
    ('S25', 45), -- Clinical Safety -> Cohort Identification
    ('S25', 46), -- Clinical Safety -> PCN Appointments Management - Vaccinations
    ('S25', 47), -- Clinical Safety -> Vaccination and Adverse Reaction Recording
    ('S25', 50), -- Clinical Safety -> Task Management
    ('S26', 1), -- Data Migration -> Appointments Management - Citizen
    ('S26', 2), -- Data Migration -> Communicate With Practice - Citizen
    ('S26', 3), -- Data Migration -> Prescription Ordering - Citizen
    ('S26', 4), -- Data Migration -> View Record - Citizen
    ('S26', 5), -- Data Migration -> Appointments Management - GP
    ('S26', 6), -- Data Migration -> Clinical Decision Support
    ('S26', 7), -- Data Migration -> Communication Management
    ('S26', 8), -- Data Migration -> Digital Diagnostics
    ('S26', 9), -- Data Migration -> Document Management
    ('S26', 10), -- Data Migration -> GP Extracts Verification
    ('S26', 11), -- Data Migration -> Referral Management - GP
    ('S26', 12), -- Data Migration -> Resource Management
    ('S26', 13), -- Data Migration -> Patient Information Maintenance - GP
    ('S26', 14), -- Data Migration -> Prescribing
    ('S26', 15), -- Data Migration -> Recording Consultations - GP
    ('S26', 16), -- Data Migration -> Reporting
    ('S26', 17), -- Data Migration -> Scanning
    ('S26', 18), -- Data Migration -> Telehealth
    ('S26', 20), -- Data Migration -> Custom Workflows
    ('S26', 21), -- Data Migration -> Care Homes
    ('S26', 22), -- Data Migration -> Caseload Management
    ('S26', 23), -- Data Migration -> Cross-organisation Appointment Booking
    ('S26', 24), -- Data Migration -> Cross-organisation Workflow Tools
    ('S26', 25), -- Data Migration -> Cross-organisation Workforce Management
    ('S26', 26), -- Data Migration -> Data Analytics for Integrated and Federated Care
    ('S26', 27), -- Data Migration -> Domiciliary Care
    ('S26', 29), -- Data Migration -> e-Consultations (Professional to Professional)
    ('S26', 30), -- Data Migration -> Medicines Optimisation
    ('S26', 32), -- Data Migration -> Personal Health Budget
    ('S26', 33), -- Data Migration -> Personal Health Record
    ('S26', 34), -- Data Migration -> Population Health Management
    ('S26', 35), -- Data Migration -> Risk Stratification
    ('S26', 36), -- Data Migration -> Shared Care Plans
    ('S26', 37), -- Data Migration -> Social Prescribing
    ('S26', 38), -- Data Migration -> Telecare
    ('S26', 39), -- Data Migration -> Unified Care Record
    ('S26', 41), -- Data Migration -> Productivity
    ('S26', 42), -- Data Migration -> Dispensing
    ('S26', 43), -- Data Migration -> Online Consultation
    ('S26', 44), -- Data Migration -> Video Consultation
    ('S26', 45), -- Data Migration -> Cohort Identification
    ('S26', 46), -- Data Migration -> PCN Appointments Management - Vaccinations
    ('S26', 47), -- Data Migration -> Vaccination and Adverse Reaction Recording
    ('S26', 50), -- Data Migration -> Task Management
    ('S27', 1), -- Data Standards -> Appointments Management - Citizen
    ('S27', 2), -- Data Standards -> Communicate With Practice - Citizen
    ('S27', 3), -- Data Standards -> Prescription Ordering - Citizen
    ('S27', 4), -- Data Standards -> View Record - Citizen
    ('S27', 5), -- Data Standards -> Appointments Management - GP
    ('S27', 6), -- Data Standards -> Clinical Decision Support
    ('S27', 7), -- Data Standards -> Communication Management
    ('S27', 8), -- Data Standards -> Digital Diagnostics
    ('S27', 9), -- Data Standards -> Document Management
    ('S27', 10), -- Data Standards -> GP Extracts Verification
    ('S27', 11), -- Data Standards -> Referral Management - GP
    ('S27', 12), -- Data Standards -> Resource Management
    ('S27', 13), -- Data Standards -> Patient Information Maintenance - GP
    ('S27', 14), -- Data Standards -> Prescribing
    ('S27', 15), -- Data Standards -> Recording Consultations - GP
    ('S27', 16), -- Data Standards -> Reporting
    ('S27', 17), -- Data Standards -> Scanning
    ('S27', 18), -- Data Standards -> Telehealth
    ('S27', 20), -- Data Standards -> Custom Workflows
    ('S27', 21), -- Data Standards -> Care Homes
    ('S27', 22), -- Data Standards -> Caseload Management
    ('S27', 23), -- Data Standards -> Cross-organisation Appointment Booking
    ('S27', 24), -- Data Standards -> Cross-organisation Workflow Tools
    ('S27', 25), -- Data Standards -> Cross-organisation Workforce Management
    ('S27', 26), -- Data Standards -> Data Analytics for Integrated and Federated Care
    ('S27', 27), -- Data Standards -> Domiciliary Care
    ('S27', 29), -- Data Standards -> e-Consultations (Professional to Professional)
    ('S27', 30), -- Data Standards -> Medicines Optimisation
    ('S27', 32), -- Data Standards -> Personal Health Budget
    ('S27', 33), -- Data Standards -> Personal Health Record
    ('S27', 34), -- Data Standards -> Population Health Management
    ('S27', 35), -- Data Standards -> Risk Stratification
    ('S27', 36), -- Data Standards -> Shared Care Plans
    ('S27', 37), -- Data Standards -> Social Prescribing
    ('S27', 38), -- Data Standards -> Telecare
    ('S27', 39), -- Data Standards -> Unified Care Record
    ('S27', 41), -- Data Standards -> Productivity
    ('S27', 42), -- Data Standards -> Dispensing
    ('S27', 43), -- Data Standards -> Online Consultation
    ('S27', 44), -- Data Standards -> Video Consultation
    ('S27', 45), -- Data Standards -> Cohort Identification
    ('S27', 46), -- Data Standards -> PCN Appointments Management - Vaccinations
    ('S27', 47), -- Data Standards -> Vaccination and Adverse Reaction Recording
    ('S27', 50), -- Data Standards -> Task Management
    ('S28', 1), -- Training -> Appointments Management - Citizen
    ('S28', 2), -- Training -> Communicate With Practice - Citizen
    ('S28', 3), -- Training -> Prescription Ordering - Citizen
    ('S28', 4), -- Training -> View Record - Citizen
    ('S28', 5), -- Training -> Appointments Management - GP
    ('S28', 6), -- Training -> Clinical Decision Support
    ('S28', 7), -- Training -> Communication Management
    ('S28', 8), -- Training -> Digital Diagnostics
    ('S28', 9), -- Training -> Document Management
    ('S28', 10), -- Training -> GP Extracts Verification
    ('S28', 11), -- Training -> Referral Management - GP
    ('S28', 12), -- Training -> Resource Management
    ('S28', 13), -- Training -> Patient Information Maintenance - GP
    ('S28', 14), -- Training -> Prescribing
    ('S28', 15), -- Training -> Recording Consultations - GP
    ('S28', 16), -- Training -> Reporting
    ('S28', 17), -- Training -> Scanning
    ('S28', 18), -- Training -> Telehealth
    ('S28', 20), -- Training -> Custom Workflows
    ('S28', 21), -- Training -> Care Homes
    ('S28', 22), -- Training -> Caseload Management
    ('S28', 23), -- Training -> Cross-organisation Appointment Booking
    ('S28', 24), -- Training -> Cross-organisation Workflow Tools
    ('S28', 25), -- Training -> Cross-organisation Workforce Management
    ('S28', 26), -- Training -> Data Analytics for Integrated and Federated Care
    ('S28', 27), -- Training -> Domiciliary Care
    ('S28', 29), -- Training -> e-Consultations (Professional to Professional)
    ('S28', 30), -- Training -> Medicines Optimisation
    ('S28', 32), -- Training -> Personal Health Budget
    ('S28', 33), -- Training -> Personal Health Record
    ('S28', 34), -- Training -> Population Health Management
    ('S28', 35), -- Training -> Risk Stratification
    ('S28', 36), -- Training -> Shared Care Plans
    ('S28', 37), -- Training -> Social Prescribing
    ('S28', 38), -- Training -> Telecare
    ('S28', 39), -- Training -> Unified Care Record
    ('S28', 41), -- Training -> Productivity
    ('S28', 42), -- Training -> Dispensing
    ('S28', 43), -- Training -> Online Consultation
    ('S28', 44), -- Training -> Video Consultation
    ('S28', 45), -- Training -> Cohort Identification
    ('S28', 46), -- Training -> PCN Appointments Management - Vaccinations
    ('S28', 47), -- Training -> Vaccination and Adverse Reaction Recording
    ('S28', 50), -- Training -> Task Management
    ('S29', 1), -- Hosting & Infrastructure -> Appointments Management - Citizen
    ('S29', 2), -- Hosting & Infrastructure -> Communicate With Practice - Citizen
    ('S29', 3), -- Hosting & Infrastructure -> Prescription Ordering - Citizen
    ('S29', 4), -- Hosting & Infrastructure -> View Record - Citizen
    ('S29', 5), -- Hosting & Infrastructure -> Appointments Management - GP
    ('S29', 6), -- Hosting & Infrastructure -> Clinical Decision Support
    ('S29', 7), -- Hosting & Infrastructure -> Communication Management
    ('S29', 8), -- Hosting & Infrastructure -> Digital Diagnostics
    ('S29', 9), -- Hosting & Infrastructure -> Document Management
    ('S29', 10), -- Hosting & Infrastructure -> GP Extracts Verification
    ('S29', 11), -- Hosting & Infrastructure -> Referral Management - GP
    ('S29', 12), -- Hosting & Infrastructure -> Resource Management
    ('S29', 13), -- Hosting & Infrastructure -> Patient Information Maintenance - GP
    ('S29', 14), -- Hosting & Infrastructure -> Prescribing
    ('S29', 15), -- Hosting & Infrastructure -> Recording Consultations - GP
    ('S29', 16), -- Hosting & Infrastructure -> Reporting
    ('S29', 17), -- Hosting & Infrastructure -> Scanning
    ('S29', 18), -- Hosting & Infrastructure -> Telehealth
    ('S29', 20), -- Hosting & Infrastructure -> Custom Workflows
    ('S29', 21), -- Hosting & Infrastructure -> Care Homes
    ('S29', 22), -- Hosting & Infrastructure -> Caseload Management
    ('S29', 23), -- Hosting & Infrastructure -> Cross-organisation Appointment Booking
    ('S29', 24), -- Hosting & Infrastructure -> Cross-organisation Workflow Tools
    ('S29', 25), -- Hosting & Infrastructure -> Cross-organisation Workforce Management
    ('S29', 26), -- Hosting & Infrastructure -> Data Analytics for Integrated and Federated Care
    ('S29', 27), -- Hosting & Infrastructure -> Domiciliary Care
    ('S29', 29), -- Hosting & Infrastructure -> e-Consultations (Professional to Professional)
    ('S29', 30), -- Hosting & Infrastructure -> Medicines Optimisation
    ('S29', 32), -- Hosting & Infrastructure -> Personal Health Budget
    ('S29', 33), -- Hosting & Infrastructure -> Personal Health Record
    ('S29', 34), -- Hosting & Infrastructure -> Population Health Management
    ('S29', 35), -- Hosting & Infrastructure -> Risk Stratification
    ('S29', 36), -- Hosting & Infrastructure -> Shared Care Plans
    ('S29', 37), -- Hosting & Infrastructure -> Social Prescribing
    ('S29', 38), -- Hosting & Infrastructure -> Telecare
    ('S29', 39), -- Hosting & Infrastructure -> Unified Care Record
    ('S29', 41), -- Hosting & Infrastructure -> Productivity
    ('S29', 42), -- Hosting & Infrastructure -> Dispensing
    ('S29', 43), -- Hosting & Infrastructure -> Online Consultation
    ('S29', 44), -- Hosting & Infrastructure -> Video Consultation
    ('S29', 45), -- Hosting & Infrastructure -> Cohort Identification
    ('S29', 46), -- Hosting & Infrastructure -> PCN Appointments Management - Vaccinations
    ('S29', 47), -- Hosting & Infrastructure -> Vaccination and Adverse Reaction Recording
    ('S29', 50), -- Hosting & Infrastructure -> Task Management
    ('S30', 1), -- Information Governance -> Appointments Management - Citizen
    ('S30', 2), -- Information Governance -> Communicate With Practice - Citizen
    ('S30', 3), -- Information Governance -> Prescription Ordering - Citizen
    ('S30', 4), -- Information Governance -> View Record - Citizen
    ('S30', 5), -- Information Governance -> Appointments Management - GP
    ('S30', 6), -- Information Governance -> Clinical Decision Support
    ('S30', 7), -- Information Governance -> Communication Management
    ('S30', 8), -- Information Governance -> Digital Diagnostics
    ('S30', 9), -- Information Governance -> Document Management
    ('S30', 10), -- Information Governance -> GP Extracts Verification
    ('S30', 11), -- Information Governance -> Referral Management - GP
    ('S30', 12), -- Information Governance -> Resource Management
    ('S30', 13), -- Information Governance -> Patient Information Maintenance - GP
    ('S30', 14), -- Information Governance -> Prescribing
    ('S30', 15), -- Information Governance -> Recording Consultations - GP
    ('S30', 16), -- Information Governance -> Reporting
    ('S30', 17), -- Information Governance -> Scanning
    ('S30', 18), -- Information Governance -> Telehealth
    ('S30', 20), -- Information Governance -> Custom Workflows
    ('S30', 21), -- Information Governance -> Care Homes
    ('S30', 22), -- Information Governance -> Caseload Management
    ('S30', 23), -- Information Governance -> Cross-organisation Appointment Booking
    ('S30', 24), -- Information Governance -> Cross-organisation Workflow Tools
    ('S30', 25), -- Information Governance -> Cross-organisation Workforce Management
    ('S30', 26), -- Information Governance -> Data Analytics for Integrated and Federated Care
    ('S30', 27), -- Information Governance -> Domiciliary Care
    ('S30', 29), -- Information Governance -> e-Consultations (Professional to Professional)
    ('S30', 30), -- Information Governance -> Medicines Optimisation
    ('S30', 32), -- Information Governance -> Personal Health Budget
    ('S30', 33), -- Information Governance -> Personal Health Record
    ('S30', 34), -- Information Governance -> Population Health Management
    ('S30', 35), -- Information Governance -> Risk Stratification
    ('S30', 36), -- Information Governance -> Shared Care Plans
    ('S30', 37), -- Information Governance -> Social Prescribing
    ('S30', 38), -- Information Governance -> Telecare
    ('S30', 39), -- Information Governance -> Unified Care Record
    ('S30', 41), -- Information Governance -> Productivity
    ('S30', 42), -- Information Governance -> Dispensing
    ('S30', 43), -- Information Governance -> Online Consultation
    ('S30', 44), -- Information Governance -> Video Consultation
    ('S30', 45), -- Information Governance -> Cohort Identification
    ('S30', 46), -- Information Governance -> PCN Appointments Management - Vaccinations
    ('S30', 47), -- Information Governance -> Vaccination and Adverse Reaction Recording
    ('S30', 50), -- Information Governance -> Task Management
    ('S31', 1), -- Commercial Standard -> Appointments Management - Citizen
    ('S31', 2), -- Commercial Standard -> Communicate With Practice - Citizen
    ('S31', 3), -- Commercial Standard -> Prescription Ordering - Citizen
    ('S31', 4), -- Commercial Standard -> View Record - Citizen
    ('S31', 5), -- Commercial Standard -> Appointments Management - GP
    ('S31', 6), -- Commercial Standard -> Clinical Decision Support
    ('S31', 7), -- Commercial Standard -> Communication Management
    ('S31', 8), -- Commercial Standard -> Digital Diagnostics
    ('S31', 9), -- Commercial Standard -> Document Management
    ('S31', 10), -- Commercial Standard -> GP Extracts Verification
    ('S31', 11), -- Commercial Standard -> Referral Management - GP
    ('S31', 12), -- Commercial Standard -> Resource Management
    ('S31', 13), -- Commercial Standard -> Patient Information Maintenance - GP
    ('S31', 14), -- Commercial Standard -> Prescribing
    ('S31', 15), -- Commercial Standard -> Recording Consultations - GP
    ('S31', 16), -- Commercial Standard -> Reporting
    ('S31', 17), -- Commercial Standard -> Scanning
    ('S31', 18), -- Commercial Standard -> Telehealth
    ('S31', 20), -- Commercial Standard -> Custom Workflows
    ('S31', 21), -- Commercial Standard -> Care Homes
    ('S31', 22), -- Commercial Standard -> Caseload Management
    ('S31', 23), -- Commercial Standard -> Cross-organisation Appointment Booking
    ('S31', 24), -- Commercial Standard -> Cross-organisation Workflow Tools
    ('S31', 25), -- Commercial Standard -> Cross-organisation Workforce Management
    ('S31', 26), -- Commercial Standard -> Data Analytics for Integrated and Federated Care
    ('S31', 27), -- Commercial Standard -> Domiciliary Care
    ('S31', 29), -- Commercial Standard -> e-Consultations (Professional to Professional)
    ('S31', 30), -- Commercial Standard -> Medicines Optimisation
    ('S31', 32), -- Commercial Standard -> Personal Health Budget
    ('S31', 33), -- Commercial Standard -> Personal Health Record
    ('S31', 34), -- Commercial Standard -> Population Health Management
    ('S31', 35), -- Commercial Standard -> Risk Stratification
    ('S31', 36), -- Commercial Standard -> Shared Care Plans
    ('S31', 37), -- Commercial Standard -> Social Prescribing
    ('S31', 38), -- Commercial Standard -> Telecare
    ('S31', 39), -- Commercial Standard -> Unified Care Record
    ('S31', 41), -- Commercial Standard -> Productivity
    ('S31', 42), -- Commercial Standard -> Dispensing
    ('S31', 43), -- Commercial Standard -> Online Consultation
    ('S31', 44), -- Commercial Standard -> Video Consultation
    ('S31', 45), -- Commercial Standard -> Cohort Identification
    ('S31', 46), -- Commercial Standard -> PCN Appointments Management - Vaccinations
    ('S31', 47), -- Commercial Standard -> Vaccination and Adverse Reaction Recording
    ('S31', 50), -- Commercial Standard -> Task Management
    ('S32', 1), -- Interoperability Standard -> Appointments Management - Citizen
    ('S32', 2), -- Interoperability Standard -> Communicate With Practice - Citizen
    ('S32', 3), -- Interoperability Standard -> Prescription Ordering - Citizen
    ('S32', 4), -- Interoperability Standard -> View Record - Citizen
    ('S32', 5), -- Interoperability Standard -> Appointments Management - GP
    ('S32', 6), -- Interoperability Standard -> Clinical Decision Support
    ('S32', 7), -- Interoperability Standard -> Communication Management
    ('S32', 8), -- Interoperability Standard -> Digital Diagnostics
    ('S32', 9), -- Interoperability Standard -> Document Management
    ('S32', 10), -- Interoperability Standard -> GP Extracts Verification
    ('S32', 11), -- Interoperability Standard -> Referral Management - GP
    ('S32', 12), -- Interoperability Standard -> Resource Management
    ('S32', 13), -- Interoperability Standard -> Patient Information Maintenance - GP
    ('S32', 14), -- Interoperability Standard -> Prescribing
    ('S32', 15), -- Interoperability Standard -> Recording Consultations - GP
    ('S32', 16), -- Interoperability Standard -> Reporting
    ('S32', 17), -- Interoperability Standard -> Scanning
    ('S32', 18), -- Interoperability Standard -> Telehealth
    ('S32', 20), -- Interoperability Standard -> Custom Workflows
    ('S32', 21), -- Interoperability Standard -> Care Homes
    ('S32', 22), -- Interoperability Standard -> Caseload Management
    ('S32', 23), -- Interoperability Standard -> Cross-organisation Appointment Booking
    ('S32', 24), -- Interoperability Standard -> Cross-organisation Workflow Tools
    ('S32', 25), -- Interoperability Standard -> Cross-organisation Workforce Management
    ('S32', 26), -- Interoperability Standard -> Data Analytics for Integrated and Federated Care
    ('S32', 27), -- Interoperability Standard -> Domiciliary Care
    ('S32', 29), -- Interoperability Standard -> e-Consultations (Professional to Professional)
    ('S32', 30), -- Interoperability Standard -> Medicines Optimisation
    ('S32', 32), -- Interoperability Standard -> Personal Health Budget
    ('S32', 33), -- Interoperability Standard -> Personal Health Record
    ('S32', 34), -- Interoperability Standard -> Population Health Management
    ('S32', 35), -- Interoperability Standard -> Risk Stratification
    ('S32', 36), -- Interoperability Standard -> Shared Care Plans
    ('S32', 37), -- Interoperability Standard -> Social Prescribing
    ('S32', 38), -- Interoperability Standard -> Telecare
    ('S32', 39), -- Interoperability Standard -> Unified Care Record
    ('S32', 41), -- Interoperability Standard -> Productivity
    ('S32', 42), -- Interoperability Standard -> Dispensing
    ('S32', 45), -- Interoperability Standard -> Cohort Identification
    ('S32', 46), -- Interoperability Standard -> PCN Appointments Management - Vaccinations
    ('S32', 47), -- Interoperability Standard -> Vaccination and Adverse Reaction Recording
    ('S32', 50), -- Interoperability Standard -> Task Management
    ('S33', 13), -- 111 -> Patient Information Maintenance - GP
    ('S34', 8), -- Digital Diagnostics & Pathology Messaging -> Digital Diagnostics
    ('S35', 11), -- e-Referrals Service (eRS) -> Referral Management - GP
    ('S36', 14), -- Electronic Prescription Service (EPS) - Prescribing -> Prescribing
    ('S37', 15), -- Electronic Yellow Card Reporting -> Recording Consultations - GP
    ('S38', 7), -- Email -> Communication Management
    ('S39', 15), -- eMED3 (Fit Notes) -> Recording Consultations - GP
    ('S40', 11), -- External Interface Specification (EIS) -> Referral Management - GP
    ('S40', 13), -- External Interface Specification (EIS) -> Patient Information Maintenance - GP
    ('S40', 14), -- External Interface Specification (EIS) -> Prescribing
    ('S43', 13), -- GP2GP -> Patient Information Maintenance - GP
    ('S44', 5), -- GP Connect -> Appointments Management - GP
    ('S44', 13), -- GP Connect -> Patient Information Maintenance - GP
    ('S44', 23), -- GP Connect -> Cross-organisation Appointment Booking
    ('S44', 30), -- GP Connect -> Medicines Optimisation
    ('S44', 39), -- GP Connect -> Unified Care Record
    ('S46', 13), -- GPES Data Extraction -> Patient Information Maintenance - GP
    ('S47', 1), -- IM1 - Interface Mechanism -> Appointments Management - Citizen
    ('S47', 2), -- IM1 - Interface Mechanism -> Communicate With Practice - Citizen
    ('S47', 3), -- IM1 - Interface Mechanism -> Prescription Ordering - Citizen
    ('S47', 4), -- IM1 - Interface Mechanism -> View Record - Citizen
    ('S47', 5), -- IM1 - Interface Mechanism -> Appointments Management - GP
    ('S47', 11), -- IM1 - Interface Mechanism -> Referral Management - GP
    ('S47', 12), -- IM1 - Interface Mechanism -> Resource Management
    ('S47', 13), -- IM1 - Interface Mechanism -> Patient Information Maintenance - GP
    ('S47', 14), -- IM1 - Interface Mechanism -> Prescribing
    ('S47', 15), -- IM1 - Interface Mechanism -> Recording Consultations - GP
    ('S48', 13), -- Interoperability Toolkit (ITK) -> Patient Information Maintenance - GP
    ('S49', 1), -- Management Information (MI) Reporting -> Appointments Management - Citizen
    ('S49', 2), -- Management Information (MI) Reporting -> Communicate With Practice - Citizen
    ('S49', 3), -- Management Information (MI) Reporting -> Prescription Ordering - Citizen
    ('S49', 4), -- Management Information (MI) Reporting -> View Record - Citizen
    ('S49', 5), -- Management Information (MI) Reporting -> Appointments Management - GP
    ('S49', 9), -- Management Information (MI) Reporting -> Document Management
    ('S49', 13), -- Management Information (MI) Reporting -> Patient Information Maintenance - GP
    ('S49', 14), -- Management Information (MI) Reporting -> Prescribing
    ('S50', 5), -- Messaging Exchange for Social Care and Health (MESH) -> Appointments Management - GP
    ('S50', 8), -- Messaging Exchange for Social Care and Health (MESH) -> Digital Diagnostics
    ('S50', 13), -- Messaging Exchange for Social Care and Health (MESH) -> Patient Information Maintenance - GP
    ('S50', 15), -- Messaging Exchange for Social Care and Health (MESH) -> Recording Consultations - GP
    ('S53', 13), -- NHAIS HA/GP Links -> Patient Information Maintenance - GP
    ('S54', 13), -- Authentication and Access -> Patient Information Maintenance - GP
    ('S55', 11), -- NHS Messaging Implementation Manual (MIM) -> Referral Management - GP
    ('S55', 13), -- NHS Messaging Implementation Manual (MIM) -> Patient Information Maintenance - GP
    ('S55', 14), -- NHS Messaging Implementation Manual (MIM) -> Prescribing
    ('S56', 5), -- Personal Demographics Service (PDS) -> Appointments Management - GP
    ('S56', 11), -- Personal Demographics Service (PDS) -> Referral Management - GP
    ('S56', 13), -- Personal Demographics Service (PDS) -> Patient Information Maintenance - GP
    ('S56', 14), -- Personal Demographics Service (PDS) -> Prescribing
    ('S56', 23), -- Personal Demographics Service (PDS) -> Cross-organisation Appointment Booking
    ('S56', 39), -- Personal Demographics Service (PDS) -> Unified Care Record
    ('S58', 8), -- Screening Messaging -> Digital Diagnostics
    ('S59', 5), -- Secure Electronic File Transfer (SEFT) -> Appointments Management - GP
    ('S59', 13), -- Secure Electronic File Transfer (SEFT) -> Patient Information Maintenance - GP
    ('S6', 6), -- Clinical Decision Support - Standard -> Clinical Decision Support
    ('S60', 13), -- Summary Care Record (SCR) -> Patient Information Maintenance - GP
    ('S62', 5), -- General Practice Appointments Data Reporting -> Appointments Management - GP
    ('S63', 1), -- Non-Functional Questions -> Appointments Management - Citizen
    ('S63', 2), -- Non-Functional Questions -> Communicate With Practice - Citizen
    ('S63', 3), -- Non-Functional Questions -> Prescription Ordering - Citizen
    ('S63', 4), -- Non-Functional Questions -> View Record - Citizen
    ('S63', 5), -- Non-Functional Questions -> Appointments Management - GP
    ('S63', 6), -- Non-Functional Questions -> Clinical Decision Support
    ('S63', 7), -- Non-Functional Questions -> Communication Management
    ('S63', 8), -- Non-Functional Questions -> Digital Diagnostics
    ('S63', 9), -- Non-Functional Questions -> Document Management
    ('S63', 10), -- Non-Functional Questions -> GP Extracts Verification
    ('S63', 11), -- Non-Functional Questions -> Referral Management - GP
    ('S63', 12), -- Non-Functional Questions -> Resource Management
    ('S63', 13), -- Non-Functional Questions -> Patient Information Maintenance - GP
    ('S63', 14), -- Non-Functional Questions -> Prescribing
    ('S63', 15), -- Non-Functional Questions -> Recording Consultations - GP
    ('S63', 16), -- Non-Functional Questions -> Reporting
    ('S63', 17), -- Non-Functional Questions -> Scanning
    ('S63', 18), -- Non-Functional Questions -> Telehealth
    ('S63', 20), -- Non-Functional Questions -> Custom Workflows
    ('S63', 21), -- Non-Functional Questions -> Care Homes
    ('S63', 22), -- Non-Functional Questions -> Caseload Management
    ('S63', 23), -- Non-Functional Questions -> Cross-organisation Appointment Booking
    ('S63', 24), -- Non-Functional Questions -> Cross-organisation Workflow Tools
    ('S63', 25), -- Non-Functional Questions -> Cross-organisation Workforce Management
    ('S63', 26), -- Non-Functional Questions -> Data Analytics for Integrated and Federated Care
    ('S63', 27), -- Non-Functional Questions -> Domiciliary Care
    ('S63', 29), -- Non-Functional Questions -> e-Consultations (Professional to Professional)
    ('S63', 30), -- Non-Functional Questions -> Medicines Optimisation
    ('S63', 32), -- Non-Functional Questions -> Personal Health Budget
    ('S63', 33), -- Non-Functional Questions -> Personal Health Record
    ('S63', 34), -- Non-Functional Questions -> Population Health Management
    ('S63', 35), -- Non-Functional Questions -> Risk Stratification
    ('S63', 36), -- Non-Functional Questions -> Shared Care Plans
    ('S63', 37), -- Non-Functional Questions -> Social Prescribing
    ('S63', 38), -- Non-Functional Questions -> Telecare
    ('S63', 39), -- Non-Functional Questions -> Unified Care Record
    ('S63', 41), -- Non-Functional Questions -> Productivity
    ('S63', 42), -- Non-Functional Questions -> Dispensing
    ('S63', 43), -- Non-Functional Questions -> Online Consultation
    ('S63', 44), -- Non-Functional Questions -> Video Consultation
    ('S63', 45), -- Non-Functional Questions -> Cohort Identification
    ('S63', 46), -- Non-Functional Questions -> PCN Appointments Management - Vaccinations
    ('S63', 47), -- Non-Functional Questions -> Vaccination and Adverse Reaction Recording
    ('S63', 50), -- Non-Functional Questions -> Task Management
    ('S64', 13), -- Clinical Document Architecture (CDA) -> Patient Information Maintenance - GP
    ('S65', 1), -- Service Management -> Appointments Management - Citizen
    ('S65', 2), -- Service Management -> Communicate With Practice - Citizen
    ('S65', 3), -- Service Management -> Prescription Ordering - Citizen
    ('S65', 4), -- Service Management -> View Record - Citizen
    ('S65', 5), -- Service Management -> Appointments Management - GP
    ('S65', 6), -- Service Management -> Clinical Decision Support
    ('S65', 7), -- Service Management -> Communication Management
    ('S65', 8), -- Service Management -> Digital Diagnostics
    ('S65', 9), -- Service Management -> Document Management
    ('S65', 10), -- Service Management -> GP Extracts Verification
    ('S65', 11), -- Service Management -> Referral Management - GP
    ('S65', 12), -- Service Management -> Resource Management
    ('S65', 13), -- Service Management -> Patient Information Maintenance - GP
    ('S65', 14), -- Service Management -> Prescribing
    ('S65', 15), -- Service Management -> Recording Consultations - GP
    ('S65', 16), -- Service Management -> Reporting
    ('S65', 17), -- Service Management -> Scanning
    ('S65', 18), -- Service Management -> Telehealth
    ('S65', 20), -- Service Management -> Custom Workflows
    ('S65', 21), -- Service Management -> Care Homes
    ('S65', 22), -- Service Management -> Caseload Management
    ('S65', 23), -- Service Management -> Cross-organisation Appointment Booking
    ('S65', 24), -- Service Management -> Cross-organisation Workflow Tools
    ('S65', 25), -- Service Management -> Cross-organisation Workforce Management
    ('S65', 26), -- Service Management -> Data Analytics for Integrated and Federated Care
    ('S65', 27), -- Service Management -> Domiciliary Care
    ('S65', 29), -- Service Management -> e-Consultations (Professional to Professional)
    ('S65', 30), -- Service Management -> Medicines Optimisation
    ('S65', 32), -- Service Management -> Personal Health Budget
    ('S65', 33), -- Service Management -> Personal Health Record
    ('S65', 34), -- Service Management -> Population Health Management
    ('S65', 35), -- Service Management -> Risk Stratification
    ('S65', 36), -- Service Management -> Shared Care Plans
    ('S65', 37), -- Service Management -> Social Prescribing
    ('S65', 38), -- Service Management -> Telecare
    ('S65', 39), -- Service Management -> Unified Care Record
    ('S65', 41), -- Service Management -> Productivity
    ('S65', 42), -- Service Management -> Dispensing
    ('S65', 43), -- Service Management -> Online Consultation
    ('S65', 44), -- Service Management -> Video Consultation
    ('S65', 45), -- Service Management -> Cohort Identification
    ('S65', 46), -- Service Management -> PCN Appointments Management - Vaccinations
    ('S65', 47), -- Service Management -> Vaccination and Adverse Reaction Recording
    ('S65', 50), -- Service Management -> Task Management
    ('S66', 5), -- Spine Mini Services -> Appointments Management - GP
    ('S66', 13), -- Spine Mini Services -> Patient Information Maintenance - GP
    ('S66', 23), -- Spine Mini Services -> Cross-organisation Appointment Booking
    ('S66', 39), -- Spine Mini Services -> Unified Care Record
    ('S68', 1), -- NHS Login Service -> Appointments Management - Citizen
    ('S68', 2), -- NHS Login Service -> Communicate With Practice - Citizen
    ('S68', 3), -- NHS Login Service -> Prescription Ordering - Citizen
    ('S68', 4), -- NHS Login Service -> View Record - Citizen
    ('S69', 1), -- Testing -> Appointments Management - Citizen
    ('S69', 2), -- Testing -> Communicate With Practice - Citizen
    ('S69', 3), -- Testing -> Prescription Ordering - Citizen
    ('S69', 4), -- Testing -> View Record - Citizen
    ('S69', 5), -- Testing -> Appointments Management - GP
    ('S69', 6), -- Testing -> Clinical Decision Support
    ('S69', 7), -- Testing -> Communication Management
    ('S69', 8), -- Testing -> Digital Diagnostics
    ('S69', 9), -- Testing -> Document Management
    ('S69', 10), -- Testing -> GP Extracts Verification
    ('S69', 11), -- Testing -> Referral Management - GP
    ('S69', 12), -- Testing -> Resource Management
    ('S69', 13), -- Testing -> Patient Information Maintenance - GP
    ('S69', 14), -- Testing -> Prescribing
    ('S69', 15), -- Testing -> Recording Consultations - GP
    ('S69', 16), -- Testing -> Reporting
    ('S69', 17), -- Testing -> Scanning
    ('S69', 18), -- Testing -> Telehealth
    ('S69', 20), -- Testing -> Custom Workflows
    ('S69', 21), -- Testing -> Care Homes
    ('S69', 22), -- Testing -> Caseload Management
    ('S69', 23), -- Testing -> Cross-organisation Appointment Booking
    ('S69', 24), -- Testing -> Cross-organisation Workflow Tools
    ('S69', 25), -- Testing -> Cross-organisation Workforce Management
    ('S69', 26), -- Testing -> Data Analytics for Integrated and Federated Care
    ('S69', 27), -- Testing -> Domiciliary Care
    ('S69', 29), -- Testing -> e-Consultations (Professional to Professional)
    ('S69', 30), -- Testing -> Medicines Optimisation
    ('S69', 32), -- Testing -> Personal Health Budget
    ('S69', 33), -- Testing -> Personal Health Record
    ('S69', 34), -- Testing -> Population Health Management
    ('S69', 35), -- Testing -> Risk Stratification
    ('S69', 36), -- Testing -> Shared Care Plans
    ('S69', 37), -- Testing -> Social Prescribing
    ('S69', 38), -- Testing -> Telecare
    ('S69', 39), -- Testing -> Unified Care Record
    ('S69', 41), -- Testing -> Productivity
    ('S69', 42), -- Testing -> Dispensing
    ('S69', 43), -- Testing -> Online Consultation
    ('S69', 44), -- Testing -> Video Consultation
    ('S69', 45), -- Testing -> Cohort Identification
    ('S69', 46), -- Testing -> PCN Appointments Management - Vaccinations
    ('S69', 47), -- Testing -> Vaccination and Adverse Reaction Recording
    ('S69', 50), -- Testing -> Task Management
    ('S70', 13), -- Primary Care Clinical Terminology Usage Report -> Patient Information Maintenance - GP
    ('S73', 13), -- National Data Opt-Out -> Patient Information Maintenance - GP
    ('S74', 43), -- Interoperability Standard (DFOCVC) -> Online Consultation
    ('S74', 44), -- Interoperability Standard (DFOCVC) -> Video Consultation
    ('S77', 13), -- Digital Medicines and Pharmacy FHIR Payload -> Patient Information Maintenance - GP
    ('S78', 5), -- GP Data for Planning and Research -> Appointments Management - GP
    ('S78', 13), -- GP Data for Planning and Research -> Patient Information Maintenance - GP
    ('S79', 13); -- Transfer of Care FHIR Payload -> Patient Information Maintenance - GP

END
