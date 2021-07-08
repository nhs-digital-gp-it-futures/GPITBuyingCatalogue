IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItemEpicStatus)
    INSERT INTO catalogue.CatalogueItemEpicStatus(Id, [Name], IsMet)
    VALUES
    (1, 'Passed', 1),
    (2, 'Not Evidenced', 0),
    (3, 'Failed', 0);
GO
