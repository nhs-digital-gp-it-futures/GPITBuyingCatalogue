CREATE TABLE [ordering].[OrderSublocationRecipientsV2]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [OrderSublocationId] INT NOT NULL,
    [RecipientOdsCode] NVARCHAR(10) NOT NULL,
    CONSTRAINT PK_OrderSublocationRecipientsV2 PRIMARY KEY ([Id]),
    CONSTRAINT UQ_OrderSublocationRecipientsV2 UNIQUE ([OrderSublocationId], [RecipientOdsCode]),
    CONSTRAINT FK_OrderSublocationRecipientsV2_ParentSublocation FOREIGN KEY ([OrderSublocationId]) REFERENCES [ordering].[OrderSublocationsV2]([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_OrderSublocationRecipientsV2_OdsOrganisations_Recipient FOREIGN KEY ([RecipientOdsCode]) REFERENCES [ods_organisations].[OdsOrganisations]([Id])
)
