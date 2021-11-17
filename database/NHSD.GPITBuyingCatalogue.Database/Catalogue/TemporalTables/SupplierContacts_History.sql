CREATE TABLE catalogue.SupplierContacts_History
(
     Id int NOT NULL,
     SupplierId int NOT NULL,
     FirstName nvarchar(35) NOT NULL,
     LastName nvarchar(35) NOT NULL,
     Email nvarchar(255) NOT NULL,
     PhoneNumber nvarchar(35) NULL,
     Department nvarchar(50) NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_SupplierContacts_History
ON catalogue.SupplierContacts_History;
GO

CREATE NONCLUSTERED INDEX IX_SupplierContacts_History_Id_Period_Columns
ON catalogue.SupplierContacts_History (SysEndTime, SysStartTime, Id);
GO
