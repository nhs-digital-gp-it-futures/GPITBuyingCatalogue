IF NOT EXISTS(SELECT 1 FROM catalogue.Integrations)
BEGIN
    INSERT INTO catalogue.Integrations([Name]) VALUES ('IM1'), ('GP Connect'), ('NHS App');
END;

IF NOT EXISTS(SELECT 1 FROM catalogue.IntegrationTypes)
BEGIN
    INSERT INTO catalogue.IntegrationTypes([Name], [IntegrationId]) VALUES
    ('Bulk', 1), ('Transactional', 1), ('Patient Facing', 1), -- IM1
    ('Access Record HTML', 2), ('Appointment Management', 2), ('Access Record Structured', 2), ('Send Document', 2), ('Update Record', 2), -- GP Connect
    ('Online Consultations', 3), ('Personal Health Records', 3), ('Primary Care Notifications', 3), ('Secondary Care Notifications', 3); -- NHS App
END;

