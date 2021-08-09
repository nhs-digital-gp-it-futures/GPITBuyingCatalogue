DECLARE @executiveAgencyRoleId AS nchar(5) = 'RO116';

IF NOT EXISTS (SELECT * FROM organisations.Organisations WHERE PrimaryRoleId = @executiveAgencyRoleId)
	INSERT INTO organisations.Organisations (Id, [Name], [Address], OdsCode, PrimaryRoleId)
	VALUES
	('C7A94E85-025B-403F-B984-20EE5F9EC333', 'NHS Digital', '{"line1":"1 TREVELYAN SQUARE","town":"LEEDS","postcode":"LS1 6AE","country":"ENGLAND"}', 'X26', @executiveAgencyRoleId);
GO
