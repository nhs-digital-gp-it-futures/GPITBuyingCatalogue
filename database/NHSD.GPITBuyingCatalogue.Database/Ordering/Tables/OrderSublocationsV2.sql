CREATE TABLE [ordering].[OrderSublocationsV2]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [OrderId] INT NOT NULL,
    [SublocationOdsCode] NVARCHAR(10) NOT NULL,
    [OwnerOdsCode] NVARCHAR(10) NOT NULL,
    CONSTRAINT PK_OrderSublocationsV2 PRIMARY KEY ([Id]),
    CONSTRAINT UQ_OrderSublocationsV2_OrderId_SublocationOdsCode UNIQUE ([OrderId], [SublocationOdsCode]),
    CONSTRAINT FK_OrderSublocationsV2_Order FOREIGN KEY ([OrderId]) REFERENCES [ordering].[Orders]([Id]),
    CONSTRAINT FK_OrderSublocationsV2_OdsOrganisations_Owner FOREIGN KEY ([OwnerOdsCode]) REFERENCES [ods_organisations].[OdsOrganisations]([Id]),
    CONSTRAINT FK_OrderSublocationsV2_OdsOrganisations_Sublocation FOREIGN KEY ([SublocationOdsCode]) REFERENCES [ods_organisations].[OdsOrganisations]([Id])
)