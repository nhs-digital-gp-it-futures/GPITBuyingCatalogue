IF NOT EXISTS (SELECT * FROM ordering.OrderTriageValues)
    INSERT INTO ordering.OrderTriageValues(Id, [Name])
    VALUES
        (0, 'Under40K'),
        (1, 'Between40KTo250K'),
        (2, 'Over250K');
GO
