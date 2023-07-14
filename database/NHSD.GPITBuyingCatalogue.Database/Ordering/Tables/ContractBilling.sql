CREATE TABLE ordering.ContractBilling
(
    Id int IDENTITY(1, 1) NOT NULL,
    ContractId int NOT NULL,
    HasConfirmedRequirements bit NULL,
    CONSTRAINT PK_ContractBilling PRIMARY KEY (Id),
    CONSTRAINT FK_ContractBilling_Contract FOREIGN KEY (ContractId) REFERENCES ordering.Contracts(Id) ON DELETE CASCADE
);
