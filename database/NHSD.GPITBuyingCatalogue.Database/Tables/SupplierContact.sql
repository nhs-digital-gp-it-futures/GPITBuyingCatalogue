CREATE TABLE dbo.SupplierContact
(
     Id uniqueidentifier NOT NULL,
     SupplierId nvarchar(6) NOT NULL,
     FirstName nvarchar(35) NOT NULL,
     LastName nvarchar(35) NOT NULL,
     Email nvarchar(255) NOT NULL,
     PhoneNumber nvarchar(35) NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy uniqueidentifier NOT NULL,
     CONSTRAINT PK_SupplierContact PRIMARY KEY NONCLUSTERED (Id),
     CONSTRAINT FK_SupplierContact_Supplier FOREIGN KEY (SupplierId) REFERENCES dbo.Supplier(Id) ON DELETE CASCADE,
     INDEX IX_SupplierContactSupplierId CLUSTERED (SupplierId)
);
