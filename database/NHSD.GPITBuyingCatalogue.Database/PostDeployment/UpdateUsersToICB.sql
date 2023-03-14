update u
SET u.PrimaryOrganisationId = [ods_organisations].GetParentICB(u.PrimaryOrganisationId)
FROM 
[GPITBuyingCatalogue].[users].[AspNetUsers] u
inner join [GPITBuyingCatalogue].[organisations].[Organisations] o
on u.PrimaryOrganisationId = o.Id
WHERE 
o.PrimaryRoleId = 'RO98'
