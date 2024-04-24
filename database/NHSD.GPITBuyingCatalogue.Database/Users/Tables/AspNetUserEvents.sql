CREATE TABLE [users].[AspNetUserEvents]
(
    Id INT IDENTITY(1, 1) NOT NULL,
    UserId INT NOT NULL,
    EventTypeId INT NOT NULL,
    [Created] DATETIME2 CONSTRAINT DF_AspNetUserEvents_Created DEFAULT GETUTCDATE() NOT NULL,
    CONSTRAINT PK_AspNetUserEvents PRIMARY KEY (Id),
    CONSTRAINT FK_AspNetUserEvents_Order FOREIGN KEY (UserId) REFERENCES users.AspNetUsers (Id) ON DELETE CASCADE,
    CONSTRAINT FK_AspNetUserEvents_EventType FOREIGN KEY (EventTypeId) REFERENCES notifications.EventTypes (Id),
);
