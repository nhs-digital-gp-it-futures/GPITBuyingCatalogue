CREATE TABLE catalogue.CatalogueItemContacts_History
(
    CatalogueItemId nvarchar(14) NOT NULL,
    SupplierContactId int NOT NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
