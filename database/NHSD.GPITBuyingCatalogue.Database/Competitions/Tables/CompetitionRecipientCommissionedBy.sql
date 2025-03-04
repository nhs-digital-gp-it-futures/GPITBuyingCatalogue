CREATE TABLE [competitions].[CompetitionRecipientCommissionedBy]
(
    [CompetitionId] int NOT NULL,
    [RecipientOdsCode] nvarchar(10) NOT NULL,
    [CommissionedByOdsCode] nvarchar(10) NOT NULL,
    CONSTRAINT PK_CompetitionRecipientCommissionedBy PRIMARY KEY ([CompetitionId],[RecipientOdsCode]),
    CONSTRAINT FK_CompetitionRecipientCommissionedBy_Competitions FOREIGN KEY ([CompetitionId]) REFERENCES [competitions].[Competitions] ([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_CompetitionRecipientsComissionedBy_CompetitionRecipients FOREIGN KEY ([CompetitionId], [RecipientOdsCode]) REFERENCES [competitions].[CompetitionRecipients] ([CompetitionId],[OdsCode]) ON DELETE CASCADE,
    CONSTRAINT FK_CompetitionRecipientCommissionedBy_OdsOrganisations_Recipient FOREIGN KEY ([RecipientOdsCode]) REFERENCES [ods_organisations].[OdsOrganisations]([Id]),
    CONSTRAINT FK_CompetitionRecipientCommissionedBy_CompetitionSublocations FOREIGN KEY ([CompetitionId], [CommissionedByOdsCode]) REFERENCES [competitions].[CompetitionSublocations]([CompetitionId],[SublocationOdsCode])
)
