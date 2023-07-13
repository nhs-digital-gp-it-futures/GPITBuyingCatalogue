CREATE TABLE ordering.ContractBillingItems
(
    Id int IDENTITY(1, 1) NOT NULL,
    ContractBillingId int NOT NULL, 
    OrderId int NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    Quantity int NOT NULL,
    CONSTRAINT PK_ContractBillingItems PRIMARY KEY (Id),
    CONSTRAINT FK_ContractBillingItems_ContractBilling FOREIGN KEY (ContractBillingId) REFERENCES ordering.ContractBilling(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ContractBillingItems_OrderItem FOREIGN KEY (OrderId, CatalogueItemId) REFERENCES ordering.OrderItems(OrderId, CatalogueItemId),
);
