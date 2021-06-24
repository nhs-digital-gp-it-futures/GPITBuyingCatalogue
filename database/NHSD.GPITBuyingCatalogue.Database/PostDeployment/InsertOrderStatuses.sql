IF NOT EXISTS (SELECT * FROM ordering.OrderStatus)
    INSERT INTO ordering.OrderStatus(Id, [Name])
    VALUES
    (1, 'Complete'),
    (2, 'Incomplete');
GO
