BEGIN TRANSACTION

DECLARE @OrderSublocationIsCommissionedBy VARCHAR(3)
DECLARE @OrderSublocationIsLocatedInTheGeographyOf VARCHAR(3)
DECLARE @IcbSublocationRole VARCHAR(10)
SET @OrderSublocationIsCommissionedBy = 'RE4'
SET @OrderSublocationIsLocatedInTheGeographyOf = 'RE5'
SET @IcbSublocationRole = 'RO319'

INSERT INTO [GPITBuyingCatalogue].[ordering].[OrderSublocations]
    ([OrderId], [SublocationOdsCode], [OwnerOdsCode])
SELECT DISTINCT [or].[OrderId], [rel].[OwnerOrganisationId] AS [SublocationOdsCode], [rel2].[OwnerOrganisationId] AS [OwnerOdsCode]
FROM [GPITBuyingCatalogue].[ordering].[OrderRecipients] [or]
    JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] [rel]
    ON [or].[OdsCode] = [rel].[TargetOrganisationId]
    JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] [rel2]
    ON [rel].[OwnerOrganisationId] = [rel2].[TargetOrganisationId]
WHERE [rel].[RelationshipTypeId] = @OrderSublocationIsCommissionedBy
    AND [rel2].[RelationshipTypeId] = @OrderSublocationIsLocatedInTheGeographyOf
    AND [rel].[IsActive] = 1
    AND [rel2].[IsActive] = 1;

INSERT INTO [GPITBuyingCatalogue].[ordering].[OrderSublocations]
    ([OrderId], [SublocationOdsCode], [OwnerOdsCode])
SELECT DISTINCT
    [or].[OrderId],
    [or].[OdsCode] AS [SublocationOdsCode],
    [rel2].[OwnerOrganisationId] AS [OwnerOdsCode]
FROM
    [GPITBuyingCatalogue].[ordering].[OrderRecipients] [or]
    JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRoles] [role]
    ON [or].[OdsCode] = [role].[OrganisationId]
    JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] [rel2]
    ON [or].[OdsCode] = [rel2].[TargetOrganisationId]
WHERE [role].[RoleId] = @IcbSublocationRole
    AND [rel2].[RelationshipTypeId] = @OrderSublocationIsLocatedInTheGeographyOf
    AND [role].[IsActive] = 1
    AND [rel2].[IsActive] = 1
    AND NOT EXISTS (
 SELECT 1
    FROM [GPITBuyingCatalogue].[ordering].[OrderSublocations] [os]
    WHERE [os].[OrderId] = [or].[OrderId]
        AND [os].[SublocationOdsCode] = [or].[OdsCode]
 );

INSERT INTO [GPITBuyingCatalogue].[ordering].[OrderSublocationRecipients]
    ([OrderId], [ParentSublocationOdsCode], [RecipientOdsCode])
SELECT [or].[OrderId], [rel].[OwnerOrganisationId] AS [ParentSublocationOdsCode], [or].[OdsCode] AS [RecipientOdsCode]
FROM [GPITBuyingCatalogue].[ordering].[OrderRecipients] [or]
    JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] [rel]
    ON [or].[OdsCode] = [rel].[TargetOrganisationId]
WHERE [rel].[RelationshipTypeId] = @OrderSublocationIsCommissionedBy AND [rel].[IsActive] = 1;

INSERT INTO [ordering].[OrderItemSublocationRecipients]
    ([OrderId], [CatalogueItemId], [ParentSublocationOdsCode], [RecipientOdsCode], [Quantity], [DeliveryDate], [LastUpdated], [LastUpdatedBy])
SELECT [oir].[OrderId], [oir].[CatalogueItemId], [osr].[ParentSublocationOdsCode], [osr].[RecipientOdsCode], [oir].[Quantity], [oir].[DeliveryDate], [oir].[LastUpdated], [oir].[LastUpdatedBy]
FROM [ordering].[OrderItemRecipients] [oir]
    JOIN [GPITBuyingCatalogue].[ordering].[OrderSublocationRecipients] [osr]
    ON [oir].[OrderId] = [osr].[OrderId] AND [oir].[OdsCode] = [osr].[RecipientOdsCode];

COMMIT TRANSACTION;