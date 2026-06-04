CREATE TABLE [users].[AccountRequests]
(
    [Id]                     UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [FirstName]              NVARCHAR(100)    NOT NULL,
    [LastName]               NVARCHAR(100)    NOT NULL,
    [Email]                  NVARCHAR(256)    NOT NULL,
    [OdsCode]                NVARCHAR(10)     NOT NULL,
    [Status]                 INT              NOT NULL DEFAULT 1,
    [RequestJustification]   NVARCHAR(1500)   NOT NULL,
    [HasOptedInUserResearch] BIT              NOT NULL DEFAULT 0,
    [DecisionJustification]  NVARCHAR(1500)   NULL,
    [RequestedOn]            DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    [DecidedOn]              DATETIME2        NULL,
    [UserId]                 INT              NULL,
    [DecidedBy]              INT              NULL,
    [IsConfirmed]            BIT              NOT NULL DEFAULT 0,
    CONSTRAINT FK_AccountRequests_User FOREIGN KEY (UserId) REFERENCES [users].[AspNetUsers] (Id),
    CONSTRAINT FK_AccountRequests_DecidedBy FOREIGN KEY (DecidedBy) REFERENCES [users].[AspNetUsers] (Id),
    CONSTRAINT FK_AccountRequests_Status FOREIGN KEY (Status) REFERENCES [users].[AccountRequestStatuses] (Id),
    CONSTRAINT FK_AccountRequests_Organisation FOREIGN KEY (OdsCode) REFERENCES [ods_organisations].[OdsOrganisations] ([Id])
)
