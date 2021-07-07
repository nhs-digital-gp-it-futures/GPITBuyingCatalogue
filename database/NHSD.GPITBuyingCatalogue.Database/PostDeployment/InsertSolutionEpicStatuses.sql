IF NOT EXISTS (SELECT * FROM dbo.CatalogueItemEpicStatus)
    INSERT INTO dbo.CatalogueItemEpicStatus(Id, [Name], IsMet)
    VALUES
    (1, 'Passed', 1),
    (2, 'Not Evidenced', 0),
    (3, 'Failed', 0);
GO
