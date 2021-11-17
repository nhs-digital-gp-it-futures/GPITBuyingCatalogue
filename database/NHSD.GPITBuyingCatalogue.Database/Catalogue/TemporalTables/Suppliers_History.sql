CREATE TABLE catalogue.Suppliers_History
(
     Id int NOT NULL,
     [Name] nvarchar(255) NOT NULL,
     LegalName nvarchar(255) NOT NULL,
     Summary nvarchar(1100) NULL,
     SupplierUrl nvarchar(1000) NULL,
     [Address] nvarchar(500) NULL,
     Deleted bit  NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     IsActive bit NOT NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_Suppliers_History
ON catalogue.Suppliers_History;
GO
