﻿CREATE TABLE ordering.Orders
(
    Id int IDENTITY(10001, 1) NOT NULL,
    CallOffId AS CONCAT('C', FORMAT(Id, '000000'), '-01'),
    [Description] nvarchar(100) NOT NULL,
    OrderingPartyId int NOT NULL,
    OrderingPartyContactId int NULL,
    SupplierId int NULL,
    SupplierContactId int NULL,
    CommencementDate date NULL,
    FundingSourceOnlyGMS bit NULL,
    Created datetime2 CONSTRAINT DF_Order_Created DEFAULT GETUTCDATE() NOT NULL,
    LastUpdated datetime2 CONSTRAINT DF_Order_LastUpdated DEFAULT GETUTCDATE() NOT NULL CONSTRAINT Order_LastUpdatedNotBeforeCreated CHECK (LastUpdated >= Created),
    LastUpdatedBy int NULL,
    Completed datetime2 NULL CONSTRAINT Order_CompletedNotBeforeCreated CHECK (Completed >= Created),
    OrderStatusId int NOT NULL,
    IsDeleted bit CONSTRAINT DF_Order_IsDeleted DEFAULT 0 NOT NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
	SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_Orders PRIMARY KEY (Id),
    CONSTRAINT FK_Orders_OrderingParty FOREIGN KEY (OrderingPartyId) REFERENCES organisations.Organisations (Id),
    CONSTRAINT FK_Orders_OrderingPartyContact FOREIGN KEY (OrderingPartyContactId) REFERENCES ordering.Contacts (Id),
    CONSTRAINT FK_Orders_Supplier FOREIGN KEY (SupplierId) REFERENCES catalogue.Suppliers (Id),
    CONSTRAINT FK_Orders_SupplierContact FOREIGN KEY (SupplierContactId) REFERENCES ordering.Contacts (Id),
    CONSTRAINT FK_Orders_OrderStatus FOREIGN KEY (OrderStatusId) REFERENCES ordering.OrderStatus (Id),
    CONSTRAINT FK_Orders_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
    INDEX IX_Orders_IsDeleted (IsDeleted)        
);
