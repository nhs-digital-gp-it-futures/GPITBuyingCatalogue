CREATE ROLE Importer;
GO

ALTER ROLE db_datareader
ADD MEMBER Importer;
GO

GRANT INSERT, UPDATE ON dbo.FrameworkSolutions TO Importer;
GO

GRANT INSERT ON dbo.CatalogueItem TO Importer;
GO

GRANT INSERT ON dbo.Solution TO Importer;
GO

GRANT DELETE, INSERT ON dbo.SolutionCapability TO Importer;
GO

GRANT EXEC ON SCHEMA::import TO Importer;
GO
