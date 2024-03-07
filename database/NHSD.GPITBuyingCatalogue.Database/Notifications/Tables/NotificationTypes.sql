CREATE TABLE [notifications].[NotificationType]
(
	[Id] INT NOT NULL,
    [Name] NVARCHAR(20) NOT NULL,
    CONSTRAINT PK_NotificationTypes PRIMARY KEY (Id),
)

--        BuyerOrderCompleted = 1,
--        FinanceOrderCompleted = 2,
--        OrderExpiryDueFirstThreshold = 3,
