CREATE TABLE catalogue.CatalogueItemContacts_History
(
    CatalogueItemId nvarchar(14) NOT NULL,
    SupplierContactId int NOT NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_CatalogueItemContacts_History
ON catalogue.CatalogueItemContacts_History;
GO

CREATE NONCLUSTERED INDEX IX_CatalogueItemContacts_History_CatalogueItemId_Period_Columns
ON catalogue.CatalogueItemContacts_History (SysEndTime, SysStartTime, CatalogueItemId);
GO
