CREATE TABLE ordering.ContractBillingItems
(
    Id int IDENTITY(1, 1) NOT NULL,
    ContractBillingId int NOT NULL, 
    OrderId int NULL,
    CatalogueItemId nvarchar(14) NULL,
    OrderItemId INT NULL,
    Quantity int NULL,
    CONSTRAINT PK_ContractBillingItems PRIMARY KEY (Id),
    CONSTRAINT FK_ContractBillingItems_ContractBilling FOREIGN KEY (ContractBillingId) REFERENCES ordering.ContractBilling(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ContractBillingItems_OrderItem FOREIGN KEY (OrderId, CatalogueItemId) REFERENCES ordering.OrderItems(OrderId, CatalogueItemId),
    CONSTRAINT FK_ContractBillingItems_OrderItemV2 FOREIGN KEY (OrderItemId) REFERENCES ordering.OrderItemsV2(Id)
);
