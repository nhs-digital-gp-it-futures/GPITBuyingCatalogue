CREATE TABLE [ordering].[OrderRecipients]
(
    [OrderId] INT NOT NULL,
    [OdsCode] NVARCHAR(10) NOT NULL,
    CONSTRAINT PK_OrderRecipients PRIMARY KEY ([OrderId], [OdsCode]),
    CONSTRAINT FK_OrderRecipients_Orders FOREIGN KEY ([OrderId]) REFERENCES [ordering].[Orders] (Id),
    CONSTRAINT FK_OrderRecipients_ServiceRecipient FOREIGN KEY ([OdsCode]) REFERENCES [ods_organisations].[OdsOrganisations] (Id)
)
