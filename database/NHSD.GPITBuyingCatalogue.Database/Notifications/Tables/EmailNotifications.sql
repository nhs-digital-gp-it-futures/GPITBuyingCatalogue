CREATE TABLE notifications.EmailNotifications
(
    [Id] UNIQUEIDENTIFIER DEFAULT (newsequentialid()) NOT NULL,
    [To] nvarchar(256) NOT NULL,
    [EmailNotificationTypeId] INT NOT NULL,
    [Json] nvarchar(max) NULL,
    [ReceiptId] nvarchar(50) NULL,
    [Created] DATETIME2 CONSTRAINT DF_EmailNotification_Created DEFAULT GETUTCDATE() NOT NULL,
    SysStartTime DATETIME2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime DATETIME2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_EmailNotifications PRIMARY KEY (Id),
    CONSTRAINT FK_EmailNotifications_EmailNotificationType FOREIGN KEY (EmailNotificationTypeId) REFERENCES notifications.EmailNotificationTypes (Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = notifications.EmailNotifications_History));
