CREATE TABLE catalogue.SupplierContacts
(
     Id int IDENTITY(1, 1) NOT NULL,
     SupplierId int NOT NULL,
     FirstName nvarchar(35) NOT NULL,
     LastName nvarchar(35) NOT NULL,
     Email nvarchar(255) NOT NULL,
     PhoneNumber nvarchar(35) NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy uniqueidentifier NOT NULL,
     CONSTRAINT PK_SupplierContact PRIMARY KEY (Id),
     CONSTRAINT FK_SupplierContact_Supplier FOREIGN KEY (SupplierId) REFERENCES catalogue.Suppliers(Id) ON DELETE CASCADE,
     INDEX IX_SupplierContactSupplierId (SupplierId),
);
