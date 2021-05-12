IF NOT EXISTS (SELECT * FROM dbo.SolutionEpicStatus)
    INSERT INTO dbo.SolutionEpicStatus(Id, [Name], IsMet)
    VALUES
    (1, 'Passed', 1),
    (2, 'Not Evidenced', 0),
    (3, 'Failed', 0);
GO
