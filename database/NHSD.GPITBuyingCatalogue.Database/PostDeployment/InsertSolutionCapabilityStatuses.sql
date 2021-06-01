IF NOT EXISTS (SELECT * FROM dbo.SolutionCapabilityStatus)
    INSERT INTO dbo.SolutionCapabilityStatus(Id, [Name], Pass)
    VALUES
    (1, 'Passed – Full', 1),
    (2, 'Passed – Partial', 1),
    (3, 'Failed', 0);
GO
