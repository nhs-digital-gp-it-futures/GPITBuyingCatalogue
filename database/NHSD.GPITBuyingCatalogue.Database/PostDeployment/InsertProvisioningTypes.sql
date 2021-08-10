IF NOT EXISTS (SELECT * FROM catalogue.ProvisioningTypes)
    INSERT INTO catalogue.ProvisioningTypes(Id, [Name])
    VALUES
    (1, 'Patient'),
    (2, 'Declarative'),
    (3, 'OnDemand');
GO
