DELETE FROM catalogue.StandardsCapabilities;

INSERT INTO catalogue.StandardsCapabilities(StandardId, CapabilityId)
VALUES
('S33', 13), -- 111 -> Patient Information Maintainance
('S34', 8),  -- Digital Diagnostics & Pathology Messaging -> Digital Diagnostics
('S77', 13), -- Digital Medicines and Pharmacy FHIR Payload -> Patient Information Maintainance
('S35', 11), -- e-Referrals Service (e-RS) -> Referral Management
('S36', 14), -- Electronic Prescription Service (EPS) -> Prescribing
('S37', 15), -- Electronic Yellow Card Reporting -> Recording Consultations
('S39', 15), -- eMED3 (Fit Notes) -> Recording Consultations
('S43', 13), -- GP2GP -> Patient Information Maintainance
('S44', 13), -- GP Connect -> Patient Information Maintainance
('S44', 5),  -- GP Connect -> Appointments Management - GP
('S44', 23), -- GP Connect -> Cross-organisation Appointment Booking
('S44', 39), -- GP Connect -> Unified Care Record
('S44', 30), -- GP Connect -> Medicines Optimisation
('S78', 13), -- GP Data for Planning and Research -> Patient Information Maintainance
('S78', 5),  -- GP Data for Planning and Research -> Appointments Management - GP
('S46', 13), -- GPES Data Extraction -> Patient Information Maintainance
('S47', 13), -- IM1 - Interface Mechanism -> Patient Information Maintainance
('S47', 15), -- IM1 - Interface Mechanism -> Recording Consultations
('S47', 5),  -- IM1 - Interface Mechanism -> Appointments Management - GP
('S47', 14), -- IM1 - Interface Mechanism -> Prescribing
('S47', 11), -- IM1 - Interface Mechanism -> Referral Management
('S47', 12), -- IM1 - Interface Mechanism -> Resource Management
('S47', 1),  -- IM1 - Interface Mechanism -> Appointments Management - Citizen
('S47', 3),  -- IM1 - Interface Mechanism -> Prescription Ordering - Citizen
('S47', 4),  -- IM1 - Interface Mechanism -> View Record - Citizen
('S47', 2),  -- IM1 - Interface Mechanism -> Communicate With Practice - Citizen
('S53', 13), -- NHAIS HA/GP Links -> Patient Information Maintainance
('S56', 13), -- Personal Demographics Service (PDS) -> Patient Information Maintainance
('S58', 8),  -- Screening messaging -> Digital Diagnostics
('S60', 13); -- Summary Care Record -> Patient Information Maintainance
