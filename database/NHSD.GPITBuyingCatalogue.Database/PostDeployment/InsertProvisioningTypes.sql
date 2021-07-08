IF NOT EXISTS (SELECT * FROM catalogue.ProvisioningTypes)
    INSERT INTO catalogue.ProvisioningTypes(ProvisioningTypeId, [Name])
    VALUES
    (1, 'Patient'),
    (2, 'Declarative'),
    (3, 'OnDemand');
GO
