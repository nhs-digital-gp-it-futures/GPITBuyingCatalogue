﻿CREATE TABLE catalogue.CatalogueItems
(
    Id nvarchar(14) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    CatalogueItemTypeId int NOT NULL,
    SupplierId int NOT NULL,
    PublishedStatusId int CONSTRAINT DF_CatalogueItem_PublishedStatus DEFAULT 1 NOT NULL,
    Created datetime2(7) CONSTRAINT DF_CatalogueItem_Created DEFAULT GETUTCDATE() NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_CatalogueItems PRIMARY KEY (Id),
    CONSTRAINT FK_CatalogueItems_CatalogueItemType FOREIGN KEY (CatalogueItemTypeId) REFERENCES catalogue.CatalogueItemTypes(Id),
    CONSTRAINT FK_CatalogueItems_Supplier FOREIGN KEY (SupplierId) REFERENCES catalogue.Suppliers(Id),
    CONSTRAINT FK_CatalogueItems_PublicationStatus FOREIGN KEY (PublishedStatusId) REFERENCES catalogue.PublicationStatus(Id),
    CONSTRAINT AK_CatalogueItems_Supplier_Name UNIQUE (SupplierId, [Name]),
    CONSTRAINT FK_CatalogueItems_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
);
