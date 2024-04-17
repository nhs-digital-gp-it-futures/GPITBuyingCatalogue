CREATE TABLE notifications.EmailNotifications_History
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [To] nvarchar(256) NOT NULL,
    [EmailNotificationTypeId] INT NOT NULL,
    [Json] nvarchar(max) NULL,
    [ReceiptId] nvarchar(50) NULL,
    [Created] DATETIME2 NOT NULL,
    SysStartTime DATETIME2(0) NOT NULL,
    SysEndTime DATETIME2(0) NOT NULL,
);
