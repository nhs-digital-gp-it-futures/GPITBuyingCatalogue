CREATE TABLE [competitions].[CompetitionRecipients]
(
    [CompetitionId] INT NOT NULL,
    [OdsCode] NVARCHAR(10) NOT NULL,
    CONSTRAINT PK_CompetitionRecipients PRIMARY KEY ([CompetitionId], [OdsCode]),
    CONSTRAINT FK_CompetitionRecipients_Competition FOREIGN KEY ([CompetitionId]) REFERENCES [competitions].[Competitions] (Id),
    CONSTRAINT FK_CompetitionRecipients_ServiceRecipient FOREIGN KEY ([OdsCode]) REFERENCES [ods_organisations].[OdsOrganisations] (Id)
);
