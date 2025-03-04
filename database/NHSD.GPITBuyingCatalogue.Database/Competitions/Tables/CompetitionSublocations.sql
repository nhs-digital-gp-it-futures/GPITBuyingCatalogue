CREATE TABLE [competitions].[CompetitionSublocationOwners]
(
    [CompetitionId] INT NOT NULL,
    [SublocationOdsCode] nvarchar(10) NOT NULL,
    [OwnerOdsCode] nvarchar(10) NOT NULL,
    CONSTRAINT PK_CompetitionSublocationOwners PRIMARY KEY ([CompetitionId],[SublocationOdsCode]),
    CONSTRAINT FK_CompetitionSublocationOwners_Competitions FOREIGN KEY ([CompetitionId]) REFERENCES [competitions].[Competitions]([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_CompetitionSublocationOwners_OdsOrganisations_Sublocation FOREIGN KEY ([SublocationOdsCode]) REFERENCES [ods_organisations].[OdsOrganisations]([Id]),
    CONSTRAINT FK_CompetitionSublocationOwners_OdsOrganisations_Owner FOREIGN KEY ([OwnerOdsCode]) REFERENCES [ods_organisations].[OdsOrganisations]([Id])
)
