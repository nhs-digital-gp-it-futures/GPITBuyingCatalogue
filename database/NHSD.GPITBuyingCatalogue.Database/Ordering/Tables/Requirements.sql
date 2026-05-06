CREATE TABLE ordering.Requirements
(
    Id int IDENTITY(1, 1) NOT NULL,
    OrderItemId INT NULL,
    ContractBillingId int NOT NULL, 
    OrderId int NULL,
    CatalogueItemId nvarchar(14) NULL,
    Details nvarchar(1000) NOT NULL,
    RequiresExplanation BIT DEFAULT(0),
    CONSTRAINT PK_Requirements PRIMARY KEY (Id),
    CONSTRAINT FK_Requirements_ContractBilling FOREIGN KEY (ContractBillingId) REFERENCES ordering.ContractBilling(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Requirements_OrderItem FOREIGN KEY (OrderId, CatalogueItemId) REFERENCES ordering.OrderItems(OrderId, CatalogueItemId),
    CONSTRAINT FK_Requirements_OrderItemV2 FOREIGN KEY (OrderItemId) REFERENCES ordering.OrderItemsV2(Id)
);
