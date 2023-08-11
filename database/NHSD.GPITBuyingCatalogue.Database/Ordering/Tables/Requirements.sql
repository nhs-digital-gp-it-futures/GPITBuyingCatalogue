CREATE TABLE ordering.Requirements
(
    Id int IDENTITY(1, 1) NOT NULL,
    ContractBillingId int NOT NULL, 
    OrderId int NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    Details nvarchar(1000) NOT NULL,
    CONSTRAINT PK_Requirements PRIMARY KEY (Id),
    CONSTRAINT FK_Requirements_ContractBilling FOREIGN KEY (ContractBillingId) REFERENCES ordering.ContractBilling(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Requirements_OrderItem FOREIGN KEY (OrderId, CatalogueItemId) REFERENCES ordering.OrderItems(OrderId, CatalogueItemId)
);
