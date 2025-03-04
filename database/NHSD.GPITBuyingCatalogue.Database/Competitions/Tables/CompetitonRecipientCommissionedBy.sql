CREATE TABLE [competitions].[CompetitonRecipientCommissionedBy]
(
    [CompetitionId] int NOT NULL,
    [RecipientOdsCode] nvarchar(10) NOT NULL,
    [CommissionedByOdsCode] nvarchar(10) NOT NULL,
    CONSTRAINT PK_CompetitonRecipientCommissionedBy PRIMARY KEY ([CompetitionId],[RecipientOdsCode]),
    CONSTRAINT FK_CompetitonRecipientCommissionedBy_Competitions FOREIGN KEY ([CompetitionId]) REFERENCES [competitions].[Competitions] ([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_CompetitonRecipientCommissionedBy_OdsOrganisations_Recipient FOREIGN KEY ([RecipientOdsCode]) REFERENCES [ods_organisations].[OdsOrganisations]([Id]),
    CONSTRAINT FK_CompetitonRecipientCommissionedBy_CompetitionSublocationOwners FOREIGN KEY ([CommissionedByOdsCode]) REFERENCES [competitions].[CompetitionSublocationOwners]([SublocationOdsCode])
)
