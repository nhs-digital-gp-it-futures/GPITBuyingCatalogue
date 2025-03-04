CREATE TABLE [competitions].[CompetitionSublocations]
(
    [CompetitionId] INT NOT NULL,
    [SublocationOdsCode] nvarchar(10) NOT NULL,
    [OwnerOdsCode] nvarchar(10) NOT NULL,
    [IsActive] bit DEFAULT 0 NOT NULL,
    CONSTRAINT PK_CompetitionSublocations PRIMARY KEY ([CompetitionId],[SublocationOdsCode]),
    CONSTRAINT FK_CompetitionSublocations_Competitions FOREIGN KEY ([CompetitionId]) REFERENCES [competitions].[Competitions]([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_CompetitionSublocations_OdsOrganisations_Sublocation FOREIGN KEY ([SublocationOdsCode]) REFERENCES [ods_organisations].[OdsOrganisations]([Id]),
    CONSTRAINT FK_CompetitionSublocations_OdsOrganisations_Owner FOREIGN KEY ([OwnerOdsCode]) REFERENCES [ods_organisations].[OdsOrganisations]([Id])
)
