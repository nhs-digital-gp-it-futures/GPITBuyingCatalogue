CREATE TABLE [competitions].[CompetitionRecipients]
(
    [CompetitionId] INT NOT NULL,
    [OdsCode] NVARCHAR(10) NOT NULL,
    CONSTRAINT PK_CompetitionRecipients PRIMARY KEY ([CompetitionId], [OdsCode]),
);
