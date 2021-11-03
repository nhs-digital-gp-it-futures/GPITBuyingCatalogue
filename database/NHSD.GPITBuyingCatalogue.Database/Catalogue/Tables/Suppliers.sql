CREATE TABLE catalogue.Suppliers
(
     Id int NOT NULL,
     [Name] nvarchar(255) NOT NULL,
     LegalName nvarchar(255) NOT NULL,
     Summary nvarchar(1100) NULL,
     SupplierUrl nvarchar(1000) NULL,
     [Address] nvarchar(500) NULL,
     Deleted bit CONSTRAINT DF_Supplier_Deleted DEFAULT 0 NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NOT NULL,
     IsActive bit CONSTRAINT DF_Suppliers_IsActive DEFAULT 0 NOT NULL, 
     CONSTRAINT PK_Suppliers PRIMARY KEY (Id),
     CONSTRAINT FK_Suppliers_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
     INDEX IX_Suppliers_Name ([Name]),
);
