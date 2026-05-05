CREATE TABLE [ordering].[OrderItemFundingV2_History]
(
    Id INT NOT NULL,
    OrderItemId INT NOT NULL,
    OrderItemFundingType INT NOT NULL,
    LastUpdated DATETIME2(7) NOT NULL,
    LastUpdatedBy INT NULL,
    SysStartTime DATETIME2(0) NOT NULL,
    SysEndTime DATETIME2(0) NOT NULL,
)
