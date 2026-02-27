CREATE TABLE [competitions].[CompetitionItemQuantities]
(
    [Id]                       INT IDENTITY (1,1) PRIMARY KEY,
    [CompetitionId]            INT          NOT NULL,
    [ParentSublocationOdsCode] NVARCHAR(10) NOT NULL,
    [RecipientOdsCode]         NVARCHAR(10) NOT NULL,
    [CompetitionItemId]        INT          NOT NULL,
    [Quantity]                 INT          NULL,
    CONSTRAINT FK_CompetitionItemQuantities_Competition FOREIGN KEY ([CompetitionId]) REFERENCES competitions.Competitions ([Id]),
    CONSTRAINT FK_CompetitionItemQuantities_CompetitionItem FOREIGN KEY ([CompetitionItemId]) REFERENCES competitions.CompetitionCatalogueItems ([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_CompetitionItemQuantities_Recipient FOREIGN KEY ([CompetitionId], [ParentSublocationOdsCode], [RecipientOdsCode]) REFERENCES competitions.CompetitionSublocationRecipients ([CompetitionId], [ParentSublocationOdsCode], [RecipientOdsCode]) ON DELETE CASCADE
);
