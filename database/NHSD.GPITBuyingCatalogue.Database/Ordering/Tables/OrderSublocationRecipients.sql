CREATE TABLE [ordering].[OrderRecipients]
(
    [OrderId] INT NOT NULL,
    [RecipientOdsCode] NVARCHAR(10) NOT NULL,
    [ParentSublocationOdsCode] NVARCHAR(10) NOT NULL,
    CONSTRAINT PK_OrderSublocationRecipients PRIMARY KEY ([OrderId], [RecipientOdsCode]),
    CONSTRAINT FK_OrderSublocationRecipients_Order FOREIGN KEY ([OrderId]) REFERENCES [ordering].[Orders]([Id]),
    CONSTRAINT FK_OrderSublocationRecipients_ParentSublocation FOREIGN KEY ([OrderId], [ParentSublocationOdsCode]) REFERENCES [ordering].[OrderSublocations]([OrderId], [SublocationOdsCode]) ON DELETE CASCADE,
    CONSTRAINT FK_OrderSublocationRecipients_OdsOrganisations_Sublocation FOREIGN KEY ([ParentSublocationOdsCode]) REFERENCES [ods_organisations].[OdsOrganisations]([Id]),
    CONSTRAINT FK_OrderSublocationRecipients_OdsOrganisations_Recipient FOREIGN KEY ([RecipientOdsCode]) REFERENCES [ods_organisations].[OdsOrganisations]([Id])
)
