IF NOT EXISTS (SELECT * FROM dbo.PublicationStatus)
    INSERT INTO dbo.PublicationStatus(Id, [Name])
    VALUES
    (1, 'Draft'),
    (2, 'Unpublished'),
    (3, 'Published'),
    (4, 'Withdrawn');
GO
