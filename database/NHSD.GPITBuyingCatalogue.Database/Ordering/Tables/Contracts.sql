﻿CREATE TABLE ordering.Contracts
(
    Id int IDENTITY(1, 1) NOT NULL,
    OrderId int NOT NULL,
    CONSTRAINT PK_Contracts PRIMARY KEY (Id),
    CONSTRAINT FK_Contracts_Order FOREIGN KEY (OrderId) REFERENCES ordering.Orders(Id)
);
