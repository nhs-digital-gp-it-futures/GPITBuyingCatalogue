CREATE TABLE ordering.Contracts
(
    Id int IDENTITY(1, 1) NOT NULL,
    OrderId int NOT NULL, 
    ImplementationPlanId int NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_Contracts PRIMARY KEY (Id),
    CONSTRAINT FK_Contracts_Order FOREIGN KEY (OrderId) REFERENCES ordering.Orders(Id),
    CONSTRAINT FK_Contracts_ImplementationPlan FOREIGN KEY (ImplementationPlanId) REFERENCES ordering.ImplementationPlans(Id)
);
