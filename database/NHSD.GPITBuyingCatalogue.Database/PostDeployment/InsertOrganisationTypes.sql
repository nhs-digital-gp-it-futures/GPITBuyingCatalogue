IF NOT EXISTS (SELECT * FROM organisations.OrganisationTypes)
INSERT INTO organisations.OrganisationTypes (Id, Name, Identifier)
VALUES
(1, 'Clinical Commissioning Group', 'CG');
GO
