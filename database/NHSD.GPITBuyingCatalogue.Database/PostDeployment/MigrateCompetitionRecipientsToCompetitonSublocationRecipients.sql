BEGIN TRANSACTION

INSERT INTO GPITBuyingCatalogue.competitions.CompetitionSublocations
    (CompetitionId, SublocationOdsCode, OwnerOdsCode, IsActive)
SELECT DISTINCT cr.CompetitionId, rel.OwnerOrganisationId AS SublocationOdsCode, rel2.OwnerOrganisationId AS OwnerOdsCode, 1 AS IsActive
FROM GPITBuyingCatalogue.competitions.CompetitionRecipients cr
    JOIN GPITBuyingCatalogue.ods_organisations.OrganisationRelationships rel
    ON cr.OdsCode = rel.TargetOrganisationId
    JOIN GPITBuyingCatalogue.ods_organisations.OrganisationRelationships rel2
    ON rel.OwnerOrganisationId  = rel2.TargetOrganisationId
WHERE rel.RelationshipTypeId = 'RE4'
    AND rel2.RelationshipTypeId = 'RE5';

INSERT INTO GPITBuyingCatalogue.competitions.CompetitionSublocationRecipients
    (CompetitionId, RecipientOdsCode, ParentSublocationOdsCode)
SELECT cr.CompetitionId, cr.OdsCode AS RecipientOdsCode, rel.OwnerOrganisationId AS ParentSublocationOdsCode
FROM GPITBuyingCatalogue.competitions.CompetitionRecipients cr
    JOIN GPITBuyingCatalogue.ods_organisations.OrganisationRelationships rel
    ON cr.OdsCode = rel.TargetOrganisationId
WHERE rel.RelationshipTypeId = 'RE4';

COMMIT TRANSACTION;