MERGE INTO catalogue.Integrations AS TARGET
USING (
    VALUES (0, 'IM1'), (1, 'GP Connect'), (2, 'NHS App')
)
AS SOURCE ([Id], [Name])
ON TARGET.[Id] = SOURCE.[Id]

WHEN MATCHED AND TARGET.[Name] <> SOURCE.[Name]
THEN UPDATE SET TARGET.[Name] = SOURCE.[Name]

WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Name])
VALUES (SOURCE.[Id], SOURCE.[Name]);

GO

IF NOT EXISTS(SELECT 1 FROM catalogue.IntegrationTypes)
BEGIN
    INSERT INTO catalogue.IntegrationTypes([Name], [Description], [IntegrationId]) VALUES
    -- IM1
    ('Bulk', NULL, 0),
    ('Transactional', NULL, 0),
    ('Patient Facing', NULL, 0),

    -- GP Connect
    ('Access Record HTML', NULL, 1),
    ('Appointment Management', NULL, 1),
    ('Access Record Structured', NULL, 1),
    ('Send Document', NULL, 1),
    ('Update Record', NULL, 1),

    -- NHS App
    ('Online Consultations', 'Local integrations that allow NHS App users to submit medical or admin queries to their GP surgery through an online questionnaire.', 2),
    ('Personal Health Records', 'Local integrations for primary and secondary care that allow NHS App users to manage their own health and care by accessing messages, letters, test results and allowing users to upload their own information.', 2),
    ('Primary Care Notifications', 'An integration that allows this solution to use the NHS App messaging service to send notifications and messages to users.', 2),
    ('Secondary Care Notifications', 'An integration that allows this solution to use the NHS App messaging service to send secondary care appointment related messages to users.', 2);
END;

