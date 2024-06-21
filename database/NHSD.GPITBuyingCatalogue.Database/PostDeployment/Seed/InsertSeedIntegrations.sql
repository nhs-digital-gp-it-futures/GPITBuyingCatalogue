IF NOT EXISTS(SELECT 1 FROM catalogue.Integrations)
BEGIN
    INSERT INTO catalogue.Integrations([Id], [Name]) VALUES (0, 'IM1'), (1, 'GP Connect'), (2, 'NHS App');
END;

IF NOT EXISTS(SELECT 1 FROM catalogue.IntegrationTypes)
BEGIN
    INSERT INTO catalogue.IntegrationTypes([Name], [Description], [IntegrationId]) VALUES
    -- IM1
    ('Bulk', NULL, 1),
    ('Transactional', NULL, 1),
    ('Patient Facing', NULL, 1),

    -- GP Connect
    ('Access Record HTML', NULL, 2),
    ('Appointment Management', NULL, 2),
    ('Access Record Structured', NULL, 2),
    ('Send Document', NULL, 2),
    ('Update Record', NULL, 2),

    -- NHS App
    ('Online Consultations', 'Local integrations that allow NHS App users to submit medical or admin queries to their GP surgery through an online questionnaire.', 3),
    ('Personal Health Records', 'Local integrations for primary and secondary care that allow NHS App users to manage their own health and care by accessing messages, letters, test results and allowing users to upload their own information.', 3),
    ('Primary Care Notifications', 'An integration that allows this solution to use the NHS App messaging service to send notifications and messages to users.', 3),
    ('Secondary Care Notifications', 'An integration that allows this solution to use the NHS App messaging service to send secondary care appointment related messages to users.', 3);
END;

