﻿CREATE ROLE Publisher;
GO

ALTER ROLE db_datareader
ADD MEMBER Publisher;
GO

GRANT UPDATE ON catalogue.CatalogueItems TO Publisher;
GO

GRANT EXEC ON SCHEMA::publish TO Publisher;
GO
