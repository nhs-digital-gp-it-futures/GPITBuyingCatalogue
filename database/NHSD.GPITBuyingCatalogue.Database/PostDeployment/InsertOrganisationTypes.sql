IF NOT EXISTS (SELECT * FROM organisations.OrganisationTypes WHERE Identifier = 'CG')
INSERT INTO organisations.OrganisationTypes (Id, Name, Identifier)
VALUES
(1, 'Clinical Commissioning Group', 'CG');
GO

IF NOT EXISTS (SELECT * FROM organisations.OrganisationTypes WHERE Identifier = 'EA')
INSERT INTO organisations.OrganisationTypes (Id, Name, Identifier)
VALUES
(2, 'Executive Agency', 'EA');
GO
