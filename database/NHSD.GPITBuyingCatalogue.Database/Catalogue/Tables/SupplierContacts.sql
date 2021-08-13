CREATE TABLE catalogue.SupplierContacts
(
     Id int IDENTITY(1, 1) NOT NULL,
     SupplierId int NOT NULL,
     FirstName nvarchar(35) NOT NULL,
     LastName nvarchar(35) NOT NULL,
     Email nvarchar(255) NOT NULL,
     PhoneNumber nvarchar(35) NULL,
     Department nvarchar(50) NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     CONSTRAINT PK_SupplierContacts PRIMARY KEY (Id),
     CONSTRAINT FK_SupplierContacts_Supplier FOREIGN KEY (SupplierId) REFERENCES catalogue.Suppliers(Id) ON DELETE CASCADE,
     CONSTRAINT FK_SupplierContacts_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
     INDEX IX_SupplierContactSupplierId (SupplierId),
);
