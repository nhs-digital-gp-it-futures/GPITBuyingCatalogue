IF NOT EXISTS (SELECT * FROM catalogue.PublicationStatus)
    INSERT INTO catalogue.PublicationStatus(Id, [Name])
    VALUES
    (1, 'Draft'),
    (2, 'Unpublished'),
    (3, 'Published'),
    (4, 'Suspended'),
    (5, 'InRemediation');
GO
