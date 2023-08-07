CREATE TABLE [competitions].[RecipientQuantities]
(
    [CompetitionId] INT NOT NULL,
    [OdsCode] NVARCHAR(10) NOT NULL,
    [CatalogueItemId] NVARCHAR(14) NOT NULL,
    [Quantity] INT NOT NULL,
    CONSTRAINT PK_RecipientQuantities PRIMARY KEY ([CompetitionId], [OdsCode], [CatalogueItemId]),
    CONSTRAINT FK_RecipientQuantities_Competition FOREIGN KEY ([CompetitionId]) REFERENCES [competitions].[Competitions] (Id),
    CONSTRAINT FK_RecipientQuantities_ServiceRecipient FOREIGN KEY ([OdsCode]) REFERENCES [ods_organisations].[OdsOrganisations] (Id) ON DELETE CASCADE
);
