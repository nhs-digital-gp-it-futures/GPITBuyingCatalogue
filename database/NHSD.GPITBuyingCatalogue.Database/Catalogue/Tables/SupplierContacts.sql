CREATE TABLE catalogue.SupplierContacts
(
     Id int IDENTITY(1, 1) NOT NULL,
     SupplierId int NOT NULL,
     FirstName nvarchar(35) NOT NULL,
     LastName nvarchar(35) NOT NULL,
     Email nvarchar(255) NOT NULL,
     PhoneNumber nvarchar(35) NULL,
     Department nvarchar(50) NULL,
     LastUpdated datetime2(7) CONSTRAINT DF_SupplierContacts_LastUpdated DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_SupplierContacts PRIMARY KEY (Id),
     CONSTRAINT FK_SupplierContacts_Supplier FOREIGN KEY (SupplierId) REFERENCES catalogue.Suppliers(Id) ON DELETE CASCADE,
     CONSTRAINT FK_SupplierContacts_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
     INDEX IX_SupplierContacts_SupplierId (SupplierId),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.SupplierContacts_History));
