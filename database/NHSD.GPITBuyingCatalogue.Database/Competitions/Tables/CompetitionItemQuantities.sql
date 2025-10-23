CREATE TABLE [competitions].[CompetitionItemQuantities]
(
    [Id]                INT          NOT NULL,
    [CompetitionId]     INT          NOT NULL,
    [CompetitionItemId] INT          NOT NULL,
    [ParentOdsCode]     NVARCHAR(10) NOT NULL,
    [OdsCode]           NVARCHAR(10) NOT NULL,
    [Quantity]          INT          NOT NULL,
    CONSTRAINT PK_CompetitionItemQuantities PRIMARY KEY ([Id]),
    CONSTRAINT FK_CompetitionItemQuantities_Competition FOREIGN KEY ([CompetitionId]) REFERENCES competitions.Competitions ([Id]),
    CONSTRAINT FK_CompetitionItemQuantities_CompetitionItem FOREIGN KEY ([CompetitionItemId]) REFERENCES competitions.CompetitionCatalogueItems ([Id]),
    CONSTRAINT FK_CompetitionItemQuantities_Recipient FOREIGN KEY ([CompetitionId], [ParentOdsCode], [OdsCode]) REFERENCES competitions.CompetitionSublocationRecipients ([CompetitionId], [ParentSublocationOdsCode], [RecipientOdsCode])
)
