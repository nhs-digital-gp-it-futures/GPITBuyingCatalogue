CREATE TABLE catalogue.Suppliers
(
     Id int NOT NULL,
     [Name] nvarchar(255) NOT NULL,
     LegalName nvarchar(255) NOT NULL,
     Summary nvarchar(1100) NULL,
     SupplierUrl nvarchar(1000) NULL,
     [Address] nvarchar(500) NULL,
     Deleted bit CONSTRAINT DF_Supplier_Deleted DEFAULT 0 NOT NULL,
     LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     IsActive bit CONSTRAINT DF_Suppliers_IsActive DEFAULT 0 NOT NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),     
     CONSTRAINT PK_Suppliers PRIMARY KEY (Id),
     CONSTRAINT FK_Suppliers_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
     INDEX IX_Suppliers_Name ([Name]),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.Suppliers_History));
