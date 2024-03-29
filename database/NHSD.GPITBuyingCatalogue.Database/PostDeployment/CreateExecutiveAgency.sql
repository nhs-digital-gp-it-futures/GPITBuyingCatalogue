﻿DECLARE @executiveAgencyRoleId AS nchar(5) = 'RO116';

SET IDENTITY_INSERT organisations.Organisations ON;

IF UPPER('$(SEED_ORGANISATIONS)') = 'TRUE' AND NOT EXISTS (SELECT * FROM organisations.Organisations WHERE PrimaryRoleId = @executiveAgencyRoleId)
    INSERT INTO organisations.Organisations (Id, [Name], [Address], OdsCode, PrimaryRoleId, ExternalIdentifier, InternalIdentifier, OrganisationTypeId)
    VALUES
    (1, 'NHS Digital', '{"line1":"1 TREVELYAN SQUARE","town":"LEEDS","postcode":"LS1 6AE","country":"ENGLAND"}', 'X26', @executiveAgencyRoleId, 'X26', 'EA-X26', 2);

SET IDENTITY_INSERT organisations.Organisations OFF;
GO
