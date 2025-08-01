CREATE TABLE [ordering].[OrderSublocations]
(
    [OrderId] INT NOT NULL,
    [SublocationOdsCode] NVARCHAR(10) NOT NULL,
    [OwnerOdsCode] NVARCHAR(10) NOT NULL,
    CONSTRAINT PK_OrderSublocations PRIMARY KEY ([OrderId], [SublocationOdsCode]),
    CONSTRAINT FK_OrderSublocations_Order FOREIGN KEY ([OrderId]) REFERENCES [ordering].[Orders]([Id]),
    CONSTRAINT FK_OrderSublocations_OdsOrganisations_Owner FOREIGN KEY ([OwnerOdsCode]) REFERENCES [ods_organisations].[OdsOrganisations]([Id]),
    CONSTRAINT FK_OrderSublocations_OdsOrganisations_Sublocation FOREIGN KEY ([SublocationOdsCode]) REFERENCES [ods_organisations].[OdsOrganisations]([Id])
)