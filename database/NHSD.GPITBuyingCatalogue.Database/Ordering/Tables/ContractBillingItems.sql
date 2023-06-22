CREATE TABLE ordering.ContractBillingItems
(
    Id int IDENTITY(1, 1) NOT NULL,
    ContractBillingId int NOT NULL, 
    OrderId int NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    ImplementationPlanMilestoneId int NOT NULL,
    Quantity int NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_ContractBillingItems PRIMARY KEY (Id),
    CONSTRAINT FK_ContractBillingItems_ContractBilling FOREIGN KEY (ContractBillingId) REFERENCES ordering.ContractBilling(Id),
    CONSTRAINT FK_ContractBillingItems_OrderItem FOREIGN KEY (OrderId, CatalogueItemId) REFERENCES ordering.OrderItems(OrderId, CatalogueItemId),
    CONSTRAINT FK_ContractBillingItems_Milestone FOREIGN KEY (ImplementationPlanMilestoneId) REFERENCES ordering.ImplementationPlanMilestones(Id),
    CONSTRAINT FK_ContractBillingItems_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ordering.ContractBillingItems_History));
