CREATE TABLE catalogue.CatalogueItemContacts
(
    CatalogueItemId nvarchar(14) NOT NULL,
    SupplierContactId int NOT NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_CatalogueItemContacts PRIMARY KEY (CatalogueItemId, SupplierContactId),
    CONSTRAINT FK_CatalogueItemContacts_CatalogueItemId FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CatalogueItemContacts_SupplierContactId FOREIGN KEY (SupplierContactId) REFERENCES catalogue.SupplierContacts(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.CatalogueItemContacts_History));
