IF NOT EXISTS (SELECT * FROM catalogue.TimeUnits)
    INSERT INTO catalogue.TimeUnits(Id, [Name], [Description])
    VALUES
    (1, 'month', 'per month'),
    (2, 'year', 'per year');
GO
