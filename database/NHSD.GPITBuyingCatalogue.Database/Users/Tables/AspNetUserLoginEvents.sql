CREATE TABLE [users].[AspNetUserLoginEvents]
(
    [Id] INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
    [UserId] INT NOT NULL,
    [Date] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_AspNetUsers_User FOREIGN KEY ([UserId]) REFERENCES [users].[AspNetUsers] ([Id]),
);

GO
CREATE NONCLUSTERED INDEX [IX_AspNetUserLoginEvents_UserId_Date] ON [users].[AspNetUserLoginEvents] ([UserId], [Date]);
GO
CREATE NONCLUSTERED INDEX [IX_AspNetUserLoginEvents_Date] ON [users].[AspNetUserLoginEvents] ([Date]);
GO
