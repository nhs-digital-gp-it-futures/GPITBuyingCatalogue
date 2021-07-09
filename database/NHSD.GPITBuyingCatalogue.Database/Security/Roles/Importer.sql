CREATE ROLE Importer;
GO

ALTER ROLE db_datareader
ADD MEMBER Importer;
GO

GRANT INSERT, UPDATE ON catalogue.FrameworkSolutions TO Importer;
GO

GRANT INSERT ON catalogue.CatalogueItems TO Importer;
GO

GRANT INSERT ON catalogue.Solutions TO Importer;
GO

GRANT DELETE, INSERT ON catalogue.CatalogueItemCapabilities TO Importer;
GO

GRANT EXEC ON SCHEMA::import TO Importer;
GO
