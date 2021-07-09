CREATE PROCEDURE test.ClearData AS
    SET NOCOUNT ON;

    TRUNCATE TABLE catalogue.FrameworkCapabilities;
    
    DELETE FROM catalogue.CatalogueItems;
    DELETE FROM catalogue.Epics;
    DELETE FROM catalogue.Capabilities;
    DELETE FROM catalogue.Suppliers;

    ALTER ROLE Api
    ADD MEMBER [NHSD-BAPI];
GO
