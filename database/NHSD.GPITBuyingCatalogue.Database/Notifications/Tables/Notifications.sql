CREATE TABLE notifications.Notifications
(
    [Id] UNIQUEIDENTIFIER DEFAULT (newsequentialid()) NOT NULL,
    [To] nvarchar(256) NOT NULL,
    [NotificationTypeId] INT NOT NULL,
    [Json] nvarchar(max) NULL,
    [ReceiptId] nvarchar(50) NULL,
    [Created] DATETIME2 CONSTRAINT DF_Notification_Created DEFAULT GETUTCDATE() NOT NULL,
    CONSTRAINT PK_Notifications PRIMARY KEY (Id),
);
