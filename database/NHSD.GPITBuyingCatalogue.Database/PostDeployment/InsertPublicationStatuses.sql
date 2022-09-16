MERGE INTO catalogue.PublicationStatus AS TARGET
USING (
VALUES 
(1, 'Draft'),
(2, 'Unpublished'),
(3, 'Published'),
(4, 'Suspended'),
(5, 'InRemediation')
)
AS SOURCE ([Id], [Name])
ON TARGET.[Id] = SOURCE.[Id]

WHEN MATCHED 
THEN UPDATE SET
TARGET.[Name] = SOURCE.[Name]

    
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Name])
VALUES (SOURCE.[Id], SOURCE.[Name]);
GO
