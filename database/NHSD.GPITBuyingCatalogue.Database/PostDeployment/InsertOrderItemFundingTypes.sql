IF NOT EXISTS (SELECT * FROM ordering.OrderItemFundingTypes)
    INSERT INTO ordering.OrderItemFundingTypes(Id, [Name])
    VALUES
    (0, 'None'),
    (1, 'Central'),
    (2, 'Local'),
    (3, 'Mixed'),
    (4, 'No Funding Required'),
    (5, 'Local Funding Only');
GO
