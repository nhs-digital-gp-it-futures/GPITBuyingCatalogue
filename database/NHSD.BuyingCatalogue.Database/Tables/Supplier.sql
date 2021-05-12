CREATE TABLE dbo.Supplier
(
     Id nvarchar(6) NOT NULL,
     [Name] nvarchar(255) NOT NULL,
     LegalName nvarchar(255) NOT NULL,
     Summary nvarchar(1100) NULL,
     SupplierUrl nvarchar(1000) NULL,
     [Address] nvarchar(500) NULL,
     OdsCode nvarchar(8) NULL,
     CrmRef uniqueidentifier NULL,
     Deleted bit CONSTRAINT DF_Supplier_Deleted DEFAULT 0 NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy uniqueidentifier NOT NULL,
     CONSTRAINT PK_Supplier PRIMARY KEY CLUSTERED (Id),
     INDEX IX_SupplierName NONCLUSTERED ([Name])
);
