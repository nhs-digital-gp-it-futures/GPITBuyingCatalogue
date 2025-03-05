CREATE TABLE [competitions].[CompetitionSublocationRecipients]
(
    [CompetitionId] int NOT NULL,
    [RecipientOdsCode] nvarchar(10) NOT NULL,
    [ParentSublocationOdsCode] nvarchar(10) NOT NULL,
    CONSTRAINT PK_CompetitionSublocationRecipients PRIMARY KEY ([CompetitionId],[RecipientOdsCode]),
    CONSTRAINT FK_CompetitionSublocationRecipients_Competitions FOREIGN KEY ([CompetitionId]) REFERENCES [competitions].[Competitions]([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_CompetitionSublocationRecipients_CompetitionSublocations FOREIGN KEY ([CompetitionId], [ParentSublocationOdsCode]) REFERENCES [competitions].[CompetitionSublocations]([CompetitionId],[SublocationOdsCode]),
    CONSTRAINT FK_CompetitionSublocationRecipients_OdsOrganisations_Recipient FOREIGN KEY ([RecipientOdsCode]) REFERENCES [ods_organisations].[OdsOrganisations]([Id]),
    CONSTRAINT FK_CompetitionSublocationRecipients_OdsOrganisations_ParentSublocation FOREIGN KEY ([ParentSublocationOdsCode]) REFERENCES [ods_organisations].[OdsOrganisations]([Id])
)
